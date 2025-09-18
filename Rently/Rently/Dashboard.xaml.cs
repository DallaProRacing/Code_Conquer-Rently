using Microsoft.Maui.Controls.Shapes;
using System.Text.Json.Nodes;

namespace Rently;

public partial class Dashboard : ContentPage
{
    private bool menuAberto = false;

    public Dashboard()
    {
        InitializeComponent();

    }
   
    // Função chamada quando o usuário clica no overlay (fundo escuro)


   

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