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
   
    // Fun��o chamada quando o usu�rio clica no overlay (fundo escuro)


   

    // ================ FUNC��ES BACKEND ================
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
        Console.WriteLine("Servi�o==> " + servico);
        return "Servi�o alugado com sucesso!";
    }

    private JsonArray buscarServico(String valor)
    {
        Console.WriteLine("Valor de busca==> " + valor);
        JsonArray servico = new JsonArray();
        return servico;
    }
}