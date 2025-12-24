using System.Reflection;

namespace GnomeMaui.Tools;

static partial class Extensions
{
	internal static void Uninstall()
	{
		if (!Directory.Exists(Program.TargetDir))
		{
			return;
		}
		"WorkloadDependencies.json".Delete();
		"WorkloadManifest.json".Delete();
		"WorkloadManifest.targets".Delete();
		Directory.Delete(Program.TargetDir);
		Directory.Delete(Program.WorkloadDir);
		if (Verbose)
		{
			Console.Out.WriteLine($"[gnomemaui] Deleted directory: {Program.TargetDir}");
			Console.Out.WriteLine($"[gnomemaui] Deleted directory: {Program.WorkloadDir}");
		}
	}
}
