namespace Rently;

public partial class Anuncios : ContentPage
{
    public Anuncios()
    {
        InitializeComponent();

        // Conectar eventos
    }

    private async void CriarProdutos(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Produtos");
    }
}