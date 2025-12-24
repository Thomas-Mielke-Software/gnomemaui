using System.Reflection;

namespace GnomeMaui.Tools;

static partial class Extensions
{
	static bool Verbose = false;

	internal static void ParseArgs(this string[] args)
	{
		if (args.Any(a => a is "--verbose"))
		{
			Verbose = true;
		}

		if (args.Length == 0 || args.Any(a => a is "-?" or "-h" or "--help"))
		{
			Program.Help();
			return;
		}

		if (args.Any(a => a is "--version" or "-v"))
		{
			Console.Out.WriteLine($"{Program.Description} v{Program.ManifestVersion}");
			return;
		}

		if (args.Any(a => a is "-i" or "--install"))
		{
			Install();
			return;
		}

		if (args.Any(a => a is "-u" or "--uninstall"))
		{
			Uninstall();
			return;
		}

		throw new InvalidOperationException($"Invalid arguments: {string.Join(" ", args)}. Use --help to see usage.");
	}
}
