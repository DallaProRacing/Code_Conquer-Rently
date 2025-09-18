using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Rently;

public partial class Anuncios : ContentPage
{
    // ====== MODELO (no mesmo arquivo) ======
    public class Produto
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int idProduto { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int idUsuario { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int idCategoria { get; set; }

        public string nomeProduto { get; set; } = "";
        public string precoProduto { get; set; } = "";
        public int idVendaAluguel { get; set; }

        // Pode vir "3", 3 ou null:
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int? idEndereco { get; set; }

        public DateTime createdAt { get; set; }
        public string descricaoProduto { get; set; } = "";
        public string unidadeDePreco { get; set; } = "un";
        public bool ativo { get; set; } = true;

        public double avaliacao { get; set; }
        public int pedidos { get; set; }
        public double receitaMes { get; set; }

        public string PrecoFormatado =>
            string.Equals(unidadeDePreco, "hora", StringComparison.OrdinalIgnoreCase)
                ? $"R$ {precoProduto}/hora"
                : $"R$ {precoProduto}/{unidadeDePreco}";
    }

        // ====== PROPRIEDADES PARA BINDING DIRETO ======
        public ObservableCollection<Produto> Ativos { get; } = new();
    public ObservableCollection<Produto> Pausados { get; } = new();

    bool _carregando;
    public bool Carregando { get => _carregando; set { _carregando = value; OnPropertyChanged(); } }

    // ====== HTTP CLIENT E BASE URL ======
    private readonly HttpClient _http = new();


    private const string BaseUrl = "http://localhost:4000"; // Windows / MacCatalyst

    public Anuncios()
    {
        InitializeComponent();
        // opcional: headers
        // _http.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarAsync();
    }

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    private async Task CarregarAsync()
    {
        try
        {
            Carregando = true;
            Ativos.Clear(); Pausados.Clear();

            var url = $"{BaseUrl}/produtos";
            var todos = await _http.GetFromJsonAsync<List<Produto>>(url, JsonOpts) ?? new();

            foreach (var p in todos.OrderByDescending(x => x.createdAt))
                if (p.ativo) Ativos.Add(p); else Pausados.Add(p);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao carregar: {ex.Message}", "OK");
        }
        finally { Carregando = false; }
    }

    private async Task AlternarPausaAsync(Produto p)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Patch, $"{BaseUrl}/produtos/{p.idProduto}/pause")
            {
                Content = new StringContent("", System.Text.Encoding.UTF8, "application/json")
            };
            var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode)
            {
                await DisplayAlert("Erro", "Não foi possível alterar o status.", "OK");
                return;
            }

            var atualizado = await resp.Content.ReadFromJsonAsync<Produto>();
            if (atualizado == null) return;

            // mover entre coleções localmente
            if (atualizado.ativo)
            {
                // saiu dos pausados
                var item = Pausados.FirstOrDefault(x => x.idProduto == atualizado.idProduto);
                if (item != null) Pausados.Remove(item);
                if (!Ativos.Any(x => x.idProduto == atualizado.idProduto))
                    Ativos.Insert(0, atualizado);
            }
            else
            {
                var item = Ativos.FirstOrDefault(x => x.idProduto == atualizado.idProduto);
                if (item != null) Ativos.Remove(item);
                if (!Pausados.Any(x => x.idProduto == atualizado.idProduto))
                    Pausados.Insert(0, atualizado);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao pausar/reativar: {ex.Message}", "OK");
        }
    }

    private async Task ExcluirAsync(Produto p)
    {
        try
        {
            var confirmar = await DisplayAlert("Confirmar", $"Excluir \"{p.nomeProduto}\"?", "Sim", "Não");
            if (!confirmar) return;

            var resp = await _http.DeleteAsync($"{BaseUrl}/produtos/{p.idProduto}");
            if (!resp.IsSuccessStatusCode)
            {
                await DisplayAlert("Erro", "Não foi possível excluir.", "OK");
                return;
            }

            var a = Ativos.FirstOrDefault(x => x.idProduto == p.idProduto);
            if (a != null) Ativos.Remove(a);
            var q = Pausados.FirstOrDefault(x => x.idProduto == p.idProduto);
            if (q != null) Pausados.Remove(q);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao excluir: {ex.Message}", "OK");
        }
    }

    // ====== HANDLERS DOS BOTÕES (sem comandos/MVVM) ======
    private async void OnPausarClicked(object sender, EventArgs e)
    {
        if (sender is Button b && b.CommandParameter is Produto p)
            await AlternarPausaAsync(p);
    }

    private async void OnExcluirClicked(object sender, EventArgs e)
    {
        if (sender is Button b && b.CommandParameter is Produto p)
            await ExcluirAsync(p);
    }

    private async void CriarProdutos(object sender, EventArgs e)
    {
        
        await Shell.Current.GoToAsync("Produtos"); 
    }

    private async void OnMoreClicked(object sender, EventArgs e)
    {
        if (sender is not Button b || b.CommandParameter is not Produto p) return;

        // opções variam conforme o status
        var opcoes = p.ativo
            ? new[] { "Editar", "Excluir" }
            : new[] { "Editar", "Excluir" };

        var escolha = await DisplayActionSheet(p.nomeProduto, "Cancelar", null, opcoes);
        switch (escolha)
        {
            case "Editar":
                // navegue para edição ou abra modal
                await DisplayAlert("Editar", $"Implementar edição de \"{p.nomeProduto}\"", "OK");
                break;

            case "Pausar":
            case "Reativar":
                await AlternarPausaAsync(p);
                break;

            case "Excluir":
                await ExcluirAsync(p);
                break;
        }
    }

}
