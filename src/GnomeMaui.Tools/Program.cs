using System.Reflection;
using System.Text;
using GnomeMaui.Tools;

static class Program
{
	internal const string WorkloadId = "gnomemaui.net.sdk";

	internal static Assembly Assembly => typeof(Program).Assembly;

	internal static Version Version
		=> Program.Assembly.GetName().Version
		?? throw new InvalidOperationException("Could not determine assembly version.");

	internal static string Description
		=> Program.Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description
		?? throw new InvalidOperationException("Could not determine assembly description.");

	internal static string DotNetRoot()
		=> Environment.GetEnvironmentVariable("DOTNET_ROOT")
		?? throw new InvalidOperationException("DOTNET_ROOT environment variable is not set.");

	internal static string SdkManifestsDir = Path.Combine(Program.DotNetRoot(), "sdk-manifests");

	internal static Version SdkVersion = Directory.GetDirectories(Program.SdkManifestsDir)
		.Select(Path.GetFileName)
		.Select(version => Version.TryParse(version, out var parsedVersion) ? parsedVersion : null)
		.OfType<Version>()
		.OrderByDescending(version => version)
		.FirstOrDefault()
		?? throw new InvalidOperationException("Could not find .NET SDK directory.");

	internal static string SdkBand = $"{SdkVersion.Major}.{SdkVersion.Minor}.{SdkVersion.Build}";
	internal static string ManifestVersion = $"{Program.Version.Major}.{Program.Version.Minor}.{Program.Version.Build}";
	internal static string WorkloadDir = Path.Combine(Program.SdkManifestsDir, SdkBand, WorkloadId);
	internal static string TargetDir = Path.Combine(WorkloadDir, ManifestVersion);

	internal static void Help() => Console.Out.WriteLine(new StringBuilder($"""
{Program.Description} v{Program.ManifestVersion}

DOTNET_ROOT:
    {Program.DotNetRoot()}

USAGE:
    dotnet gnomemaui --install

OPTIONS:
    -i, --install    Install GNOME MAUI workload manifest files into DOTNET_ROOT
    -u, --uninstall  Uninstall GNOME MAUI workload manifest files from DOTNET_ROOT

    -?, -h, --help   Display this help screen
    -v, --version    Display version information

    --verbose        Enable verbose output
""").ToString());

	public static int Main(string[] args)
	{
		try
		{
			args.ParseArgs();
			Environment.Exit(0);
			return 0;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine(ex);
			Environment.Exit(1);
			return 1;
		}
	}
}
