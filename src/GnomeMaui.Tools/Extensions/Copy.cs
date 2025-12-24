using System.Reflection;

namespace GnomeMaui.Tools;

static partial class Extensions
{
	internal static void Copy(this string resourceName)
	{
		using var stream
			= Program.Assembly.GetManifestResourceStream(resourceName)
			?? throw new InvalidOperationException($"Embedded resource not found: {resourceName}");

		var targetPath = Path.Combine(Program.TargetDir, resourceName);
		using var file = File.Create(targetPath);
		stream.CopyTo(file);
		if (Verbose)
		{
			Console.Out.WriteLine($"[gnomemaui] Copied resource {resourceName} to {targetPath}");
		}
	}
}