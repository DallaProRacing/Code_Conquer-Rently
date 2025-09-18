namespace Rently;

public partial class Cadastro : ContentPage
{
	public Cadastro()
	{
        InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);
        NavigationPage.SetHasNavigationBar(this, false);
    }

    private void OnToggleSenhaButton_Clicked(object sender, EventArgs e)
    {
        SenhaEntry.IsPassword = !SenhaEntry.IsPassword;
        ToggleSenhaButton.Text = SenhaEntry.IsPassword ? "👁" : "🔒";
    }

    private void OnToggleConfirmeSenhaButton_Clicked(object sender, EventArgs e)
    {
        ConfirmeSenhaEntry.IsPassword = !ConfirmeSenhaEntry.IsPassword;
        ToggleConfirmeSenhaButton.Text = ConfirmeSenhaEntry.IsPassword ? "👁" : "🔒";
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Senha");
    }
}