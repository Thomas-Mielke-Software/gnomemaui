using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Internals;

namespace Microsoft.Maui.Controls.Platform;

partial class AlertManager
{
	private partial IAlertManagerSubscription CreateSubscription(IMauiContext mauiContext)
	{
		var platformWindow = mauiContext.GetPlatformWindow();

		return new AlertRequestHelper(Window, platformWindow);
	}

	internal sealed partial class AlertRequestHelper
	{
		Task<bool>? CurrentAlert;
		Task<string?>? CurrentPrompt;

		internal AlertRequestHelper(Window virtualView, Adw.ApplicationWindow platformView)
		{
			VirtualView = virtualView;
			PlatformView = platformView;
		}

		public Window VirtualView { get; }

		public Adw.ApplicationWindow PlatformView { get; }

		// TODO: This method is obsolete in .NET 10 and will be removed in .NET 11.
		public partial void OnPageBusy(Page sender, bool enabled)
		{
			// Not implemented for GNOME yet
		}

		public async partial void OnAlertRequested(Page sender, AlertArguments arguments)
		{
			if (!PageIsInThisWindow(sender))
			{
				return;
			}

			var currentAlert = CurrentAlert;

			while (currentAlert != null)
			{
				await currentAlert;
				currentAlert = CurrentAlert;
			}

			CurrentAlert = ShowAlert(arguments);
			arguments.SetResult(await CurrentAlert.ConfigureAwait(false));
			CurrentAlert = null;
		}

		public async partial void OnPromptRequested(Page sender, PromptArguments arguments)
		{
			if (!PageIsInThisWindow(sender))
			{
				return;
			}

			var currentPrompt = CurrentPrompt;

			while (currentPrompt != null)
			{
				await currentPrompt;
				currentPrompt = CurrentPrompt;
			}

			CurrentPrompt = ShowPrompt(arguments);
			arguments.SetResult(await CurrentPrompt.ConfigureAwait(false));
			CurrentPrompt = null;
		}

		public partial void OnActionSheetRequested(Page sender, ActionSheetArguments arguments)
		{
			if (!PageIsInThisWindow(sender))
			{
				return;
			}

			ShowActionSheet(arguments);
		}

		Task<bool> ShowAlert(AlertArguments arguments)
		{
			var tcs = new TaskCompletionSource<bool>();

			GLib.Functions.IdleAdd(0, new GLib.SourceFunc(() =>
			{
				try
				{
					var dialog = Adw.AlertDialog.New(
						arguments.Title ?? string.Empty,
						arguments.Message ?? string.Empty
					);

					// Add buttons
					if (arguments.Cancel != null)
					{
						dialog.AddResponse("cancel", arguments.Cancel);
					}

					if (arguments.Accept != null)
					{
						dialog.AddResponse("accept", arguments.Accept);
						dialog.SetDefaultResponse("accept");
						dialog.SetResponseAppearance("accept", Adw.ResponseAppearance.Suggested);
					}

					dialog.OnResponse += (sender, args) =>
					{
						bool result = args.Response == "accept";
						tcs.TrySetResult(result);
					};

					dialog.Present(PlatformView);
				}
				catch (Exception ex)
				{
					tcs.TrySetException(ex);
				}

				return GLib.Constants.SOURCE_REMOVE;
			}));

			return tcs.Task;
		}

		Task<string?> ShowPrompt(PromptArguments arguments)
		{
			var tcs = new TaskCompletionSource<string?>();

			GLib.Functions.IdleAdd(0, new GLib.SourceFunc(() =>
			{
				try
				{
					var dialog = Adw.AlertDialog.New(
						arguments.Title ?? string.Empty,
						arguments.Message ?? string.Empty
					);

					// Create entry for input
					var entry = Gtk.Entry.New();
					entry.SetText(arguments.InitialValue ?? string.Empty);
					entry.SetPlaceholderText(arguments.Placeholder ?? string.Empty);

					if (arguments.MaxLength > 0)
					{
						entry.SetMaxLength(arguments.MaxLength);
					}

					dialog.SetExtraChild(entry);

					// Add buttons
					if (arguments.Cancel != null)
					{
						dialog.AddResponse("cancel", arguments.Cancel);
					}

					if (arguments.Accept != null)
					{
						dialog.AddResponse("accept", arguments.Accept);
						dialog.SetDefaultResponse("accept");
						dialog.SetResponseAppearance("accept", Adw.ResponseAppearance.Suggested);
					}

					dialog.OnResponse += (sender, args) =>
					{
						if (args.Response == "accept")
						{
							tcs.TrySetResult(entry.GetText());
						}
						else
						{
							tcs.TrySetResult(null);
						}
					};

					dialog.Present(PlatformView);
				}
				catch (Exception ex)
				{
					tcs.TrySetException(ex);
				}

				return GLib.Constants.SOURCE_REMOVE;
			}));

			return tcs.Task;
		}

		void ShowActionSheet(ActionSheetArguments arguments)
		{
			GLib.Functions.IdleAdd(0, new GLib.SourceFunc(() =>
			{
				try
				{
					var dialog = Adw.AlertDialog.New(
						arguments.Title ?? string.Empty,
						null
					);

					// Add action buttons
					foreach (var button in arguments.Buttons)
					{
						var responseId = button.Replace(" ", "_", StringComparison.Ordinal).ToLowerInvariant();
						dialog.AddResponse(responseId, button);
					}

					// Add cancel button
					if (arguments.Cancel != null)
					{
						dialog.AddResponse("cancel", arguments.Cancel);
						dialog.SetCloseResponse("cancel");
					}

					// Add destruction button with warning appearance
					if (arguments.Destruction != null)
					{
						dialog.AddResponse("destruction", arguments.Destruction);
						dialog.SetResponseAppearance("destruction", Adw.ResponseAppearance.Destructive);
					}

					dialog.OnResponse += (sender, args) =>
					{
						if (args.Response == "cancel")
						{
							arguments.SetResult(arguments.Cancel);
						}
						else if (args.Response == "destruction")
						{
							arguments.SetResult(arguments.Destruction);
						}
						else
						{
							// Find the button text from response id
							foreach (var button in arguments.Buttons)
							{
								var responseId = button.Replace(" ", "_", StringComparison.Ordinal).ToLowerInvariant();
								if (responseId == args.Response)
								{
									arguments.SetResult(button);
									break;
								}
							}
						}
					};

					dialog.Present(PlatformView);
				}
				catch (Exception ex)
				{
					arguments.SetResult(null);
				}

				return GLib.Constants.SOURCE_REMOVE;
			}));
		}

		bool PageIsInThisWindow(Page page) =>
			page?.Window == VirtualView;
	}
}