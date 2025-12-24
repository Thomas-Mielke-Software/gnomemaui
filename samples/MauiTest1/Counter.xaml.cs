namespace MauiTest1;

public partial class Counter : ContentPage
{
	public Counter()
	{
		InitializeComponent();
	}

	int _counter = 0;
	void OnCounterClicked(object sender, EventArgs e)
	{
		_counter++;
		CounterLabel.Text = $"Counter: {_counter}";
	}
}
