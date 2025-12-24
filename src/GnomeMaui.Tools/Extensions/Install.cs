using System.Reflection;

namespace GnomeMaui.Tools;

static partial class Extensions
{
	internal static void Install()
	{
		if (!Directory.Exists(Program.TargetDir))
		{
			Directory.CreateDirectory(Program.TargetDir);
			if (Verbose)
			{
				Console.Out.WriteLine($"[gnomemaui] Created directory: {Program.TargetDir}");
			}
		}
		"WorkloadDependencies.json".Copy();
		"WorkloadManifest.json".Copy();
		"WorkloadManifest.targets".Copy();
	}
}
