using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace MauiBlazorApp1;

class AdwApplication : MauiAdwApplication
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public AdwApplication() : base("com.companyname.MauiTest1") { }
}
