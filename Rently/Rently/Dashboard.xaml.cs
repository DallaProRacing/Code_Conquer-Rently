using Microsoft.Maui.Controls.Shapes;
using System.Text.Json.Nodes;

namespace Rently;

public partial class Dashboard : ContentPage
{
    private bool menuAberto = false;

    public Dashboard()
    {
        InitializeComponent();

        // Conectar eventos programaticamente
        BotaoMenu.Clicked += OnMenuButtonClicked;

        // Adicionar gesture recognizer para o overlay
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnOverlayTapped;
        MenuOverlay.GestureRecognizers.Add(tapGesture);

        // Conectar eventos dos botões do menu
        MenuInicio.Clicked += OnMenuItemClicked;
        Anuncios.Clicked += OnMenuItemClicked;
        
        MenuPerfil.Clicked += OnMenuItemClicked;
        //MenuConfiguracoes.Clicked += OnMenuItemClicked;
        MenuSair.Clicked += OnMenuItemClicked;
    }

    // ================ FUNCÇÕES MENU ================
    // Função para alternar o menu (abrir/fechar)
    private async void OnMenuButtonClicked(object sender, EventArgs e)
    {
        if (menuAberto)
        {
            await FecharMenu();
        }
        else
        {
            await AbrirMenu();
        }
    }

    // Função para abrir o menu
    private async Task AbrirMenu()
    {
        menuAberto = true;

        // Mostra o overlay e o menu
        MenuOverlay.IsVisible = true;
        MenuLateral.IsVisible = true;

        // Animações simultâneas
        var animacaoMenu = MenuLateral.TranslateTo(0, 0, 250, Easing.CubicOut);
        var animacaoOverlay = MenuOverlay.FadeTo(0.5, 250);

        await Task.WhenAll(animacaoMenu, animacaoOverlay);

        Console.WriteLine("Menu aberto");
    }

    // Função para fechar o menu
    private async Task FecharMenu()
    {
        menuAberto = false;

        // Animações simultâneas
        var animacaoMenu = MenuLateral.TranslateTo(-280, 0, 200, Easing.CubicIn);
        var animacaoOverlay = MenuOverlay.FadeTo(0, 200);

        await Task.WhenAll(animacaoMenu, animacaoOverlay);

        // Esconde os elementos após a animação
        MenuOverlay.IsVisible = false;
        MenuLateral.IsVisible = false;

        Console.WriteLine("Menu fechado");
    }

    // Função chamada quando o usuário clica no overlay (fundo escuro)
    private async void OnOverlayTapped(object sender, EventArgs e)
    {
        if (menuAberto)
        {
            await FecharMenu();
        }
    }

    // Função para lidar com os cliques nos itens do menu
    private async void OnMenuItemClicked(object sender, EventArgs e)
    {
        if (sender is Button botao)
        {
            string rota = "desconhecido";

            // Identificar qual botão foi clicado
            if (botao == MenuInicio)
                await Shell.Current.GoToAsync("Dashboard");
            else if (botao == Anuncios)
                await Shell.Current.GoToAsync("Anuncios");
            else if (botao == MenuPerfil)
                await Shell.Current.GoToAsync("Perfil");
            //  else if (botao == MenuConfiguracoes)
            //      rota = "configuracoes";
            else if (botao == MenuSair)
                rota = "sair";

            // Fecha o menu primeiro
            await FecharMenu();

            // Depois navega
            navegacaoMenu(rota);
        }
    }

    // ================ FUNCÇÕES BACKEND ================
    private void navegacaoMenu(String rota)
    {
        Console.WriteLine("navegacao rotas==> " + rota);
    }

    private JsonArray ListarServicos()
    {
        JsonArray servico = new JsonArray();
        return servico;
    }

    private string alugarServico(JsonArray servico)
    {
        Console.WriteLine("Serviço==> " + servico);
        return "Serviço alugado com sucesso!";
    }

    private JsonArray buscarServico(String valor)
    {
        Console.WriteLine("Valor de busca==> " + valor);
        JsonArray servico = new JsonArray();
        return servico;
    }
}