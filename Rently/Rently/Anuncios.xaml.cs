namespace Rently;

public partial class Anuncios : ContentPage
{
    public Anuncios()
    {
        InitializeComponent();

        // Conectar eventos
        BotaoVoltar.Clicked += OnVoltarClicked;
     
    }

    // Função para voltar à tela anterior
    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
   
}