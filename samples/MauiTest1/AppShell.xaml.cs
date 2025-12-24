namespace MauiTest1;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register the SecondPage route
		Routing.RegisterRoute("home", typeof(Home));
		Routing.RegisterRoute("counter", typeof(Counter));
		Routing.RegisterRoute("christmas", typeof(Christmas));
		Routing.RegisterRoute("juliafractal", typeof(JuliaFractal));
	}
}
