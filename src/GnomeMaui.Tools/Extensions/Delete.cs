using System.Reflection;

namespace GnomeMaui.Tools;

static partial class Extensions
{
	internal static void Delete(this string targetPath)
	{
		var file = Path.Combine(Program.TargetDir, targetPath);
		if (File.Exists(file))
		{
			File.Delete(file);
			if (Verbose)
			{
				Console.Out.WriteLine($"[gnomemaui] Deleted file: {file}");
			}
		}
	}
}