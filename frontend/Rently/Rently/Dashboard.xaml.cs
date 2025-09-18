using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Rently;

public partial class Dashboard : ContentPage, INotifyPropertyChanged
{
    // ====== MODELOS (no mesmo arquivo) ======
    public class Categoria
    {
        public int id { get; set; }
        public string nomeCategoria { get; set; } = "";
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }

    public class Produto
    {
        public int idProduto { get; set; }
        public int idLoja { get; set; }
        public int idCategoria { get; set; }
        public string nomeProduto { get; set; } = "";
        public decimal precoProduto { get; set; }
        public int idVendaAluguel { get; set; }
        public string? descricaoProduto { get; set; } // Campo opcional para descrição
        public bool ativo { get; set; } = true; // Campo para controlar se está ativo
    }

    // ====== PROPRIEDADES PARA BINDING DIRETO ======
    public ObservableCollection<Categoria> Categorias { get; } = new();
    public ObservableCollection<Produto> Produtos { get; } = new();

    bool _carregando;
    public bool Carregando
    {
        get => _carregando;
        set
        {
            _carregando = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Carregando)));
        }
    }

    public bool MostrarMensagemVazia => !Carregando && !Categorias.Any();
    public bool MostrarMensagemVaziaProdutos => !Carregando && !Produtos.Any();

    // ====== INotifyPropertyChanged Implementation ======
    public event PropertyChangedEventHandler PropertyChanged;

    // ====== HTTP CLIENT E BASE URL ======
    private readonly HttpClient _http = new();
    private const string BaseUrlCategorias = "http://localhost:4000/categorias"; // URL para categorias
    private const string BaseUrlProdutos = "http://localhost:4000/produtos"; // URL para produtos

    public Dashboard()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarCategoriasAsync();
        await CarregarProdutosAsync();
    }

    private async Task CarregarCategoriasAsync()
    {
        try
        {
            Carregando = true;
            Categorias.Clear();

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            var response = await client.GetAsync(BaseUrlCategorias);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var categorias = JsonConvert.DeserializeObject<List<Categoria>>(json);

                // Executa no thread principal
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (categorias != null)
                    {
                        foreach (var categoria in categorias.OrderBy(x => x.nomeCategoria))
                            Categorias.Add(categoria);
                    }

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MostrarMensagemVazia)));
                });
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Erro", $"Falha ao carregar categorias: {response.StatusCode}", "OK");
                });
            }
        }
        catch (HttpRequestException ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Erro de Conexão", $"Erro ao conectar com a API: {ex.Message}", "OK");
            });
        }
        catch (TaskCanceledException)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Timeout", "Timeout ao carregar categorias.", "OK");
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Erro", $"Erro ao carregar categorias: {ex.Message}", "OK");
            });
        }
        finally
        {
            Carregando = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MostrarMensagemVazia)));
        }
    }

    private async Task CarregarProdutosAsync()
    {
        try
        {
            Produtos.Clear();

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            var response = await client.GetAsync(BaseUrlProdutos);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var produtos = JsonConvert.DeserializeObject<List<Produto>>(json);

                // Executa no thread principal
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (produtos != null)
                    {
                        foreach (var produto in produtos.Where(p => p.ativo))
                        {
                            Produtos.Add(produto);
                        }
                    }

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MostrarMensagemVaziaProdutos)));
                });
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Erro", $"Falha ao carregar produtos: {response.StatusCode}", "OK");
                });
            }
        }
        catch (HttpRequestException ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Erro de Conexão", $"Erro ao conectar com a API de produtos: {ex.Message}", "OK");
            });
        }
        catch (TaskCanceledException)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Timeout", "Timeout ao carregar produtos.", "OK");
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Erro", $"Erro ao carregar produtos: {ex.Message}", "OK");
            });
        }
    }

    // ====== HANDLERS DOS EVENTOS ======
    private void OnCategoriaTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Categoria categoria)
        {
            // Implementar navegação ou filtro por categoria
            System.Diagnostics.Debug.WriteLine($"Categoria selecionada: {categoria.nomeCategoria}");

            // Exemplo: filtrar na barra de busca (buscar o SearchBar pelo nome)
            var searchBar = this.FindByName<SearchBar>("SearchBarServicos");
            if (searchBar != null)
            {
                searchBar.Text = categoria.nomeCategoria;
            }

            // Ou navegar para uma página específica
            // await Shell.Current.GoToAsync($"categoria-detalhes?categoriaId={categoria.id}");
        }
    }

    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        var searchBar = sender as SearchBar;
        if (!string.IsNullOrWhiteSpace(searchBar?.Text))
        {
            await BuscarServicos(searchBar.Text);
        }
    }

    private async void OnAlugarClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Produto produto)
        {
            try
            {
                string tipoTransacao = produto.idVendaAluguel == 1 ? "Venda" : "Aluguel";

                var resultado = await DisplayAlert(
                    $"Confirmar {tipoTransacao}",
                    $"Deseja realizar {tipoTransacao.ToLower()} de '{produto.nomeProduto}' por R$ {produto.precoProduto:F2}?",
                    "Sim",
                    "Cancelar"
                );

                if (resultado)
                {
                    // Implementar lógica de aluguel/venda aqui
                    System.Diagnostics.Debug.WriteLine($"Produto ID: {produto.idProduto} - {tipoTransacao}: {produto.nomeProduto}");

                    await DisplayAlert("Sucesso", $"'{produto.nomeProduto}' foi processado com sucesso!", "OK");

                    // Aqui você pode navegar para uma tela de detalhes
                    // await Shell.Current.GoToAsync($"detalhes-transacao?produtoId={produto.idProduto}&tipo={produto.idVendaAluguel}");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao processar transação: {ex.Message}", "OK");
            }
        }
    }

    private async Task BuscarServicos(string termoBusca)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Buscando serviços com termo: {termoBusca}");

            // Implementar busca de serviços aqui
            // var servicosEncontrados = await _http.GetFromJsonAsync<List<Servico>>($"{BaseUrlProdutos}/buscar?termo={termoBusca}");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao buscar serviços: {ex.Message}", "OK");
        }
    }

    // ====== FUNCÇÕES ORIGINAIS MANTIDAS ======
    private bool menuAberto = false;

    private async void VerDetalhes(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("Detalhes");
    }

    private void navegacaoMenu(String rota)
    {
        Console.WriteLine("navegacao rotas==> " + rota);
    }

    private System.Text.Json.Nodes.JsonArray ListarServicos()
    {
        System.Text.Json.Nodes.JsonArray servico = new System.Text.Json.Nodes.JsonArray();
        return servico;
    }

    private string alugarServico(System.Text.Json.Nodes.JsonArray servico)
    {
        Console.WriteLine("Serviço==> " + servico);
        return "Serviço alugado com sucesso!";
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private System.Text.Json.Nodes.JsonArray buscarServico(String valor)
    {
        Console.WriteLine("Valor de busca==> " + valor);
        System.Text.Json.Nodes.JsonArray servico = new System.Text.Json.Nodes.JsonArray();
        return servico;
    }
}