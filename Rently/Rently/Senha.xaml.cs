namespace Rently;

public partial class Senha : ContentPage
{
	public Senha()
	{
		InitializeComponent();
	}

    private void ToggleSenhaButton_Clicked(object sender, EventArgs e)
    {
        SenhaEntry.IsPassword = !SenhaEntry.IsPassword;
        ToggleSenhaButton.Text = SenhaEntry.IsPassword ? "👁" : "🔒";
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Dashboard");
    }
}