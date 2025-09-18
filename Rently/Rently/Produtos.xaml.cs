using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Globalization;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Rently
{
    public partial class Produtos : ContentPage
    {
        private string apiUrlProdutos = "http://localhost:4000/produtos";
        private string apiUrlCategorias = "http://localhost:4000/categorias";
        private int idUsuario = 10;     // Tempor�rio - pegar do login/sess�o depois
        private int idCategoria = 1;   // Ser� din�mico baseado na categoria escolhida
        private int? idEndereco = null; // Pode ser null se n�o tiver endere�o cadastrado
        private string unidadeDePreco = "dia"; // "dia", "m�s", "unidade", etc.

        private bool isAluguelSelected = true; // Aluguel � selecionado por padr�o
        private bool _isUpdating = false;

        // Lista para armazenar as categorias vindas da API
        private List<Categoria> categorias = new List<Categoria>();

        public Produtos()
        {
            InitializeComponent();
            // Carrega categorias de forma mais segura
            Loaded += OnPageLoaded;
        }

        // Evento quando a p�gina � carregada
        private async void OnPageLoaded(object sender, EventArgs e)
        {
            await CarregarCategoriasAsync();
        }

        // Classe para mapear o retorno da API de categorias
        public class Categoria
        {
            public int idCategoria { get; set; }
            public string nomeCategoria { get; set; }
        }

        private async Task CarregarCategoriasAsync()
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                var response = await client.GetAsync(apiUrlCategorias);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var categoriasApi = JsonConvert.DeserializeObject<List<Categoria>>(json);

                    // Executa no thread principal
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        // Limpa as categorias atuais
                        categorias.Clear();
                        PickerCategoria.Items.Clear();

                        // Adiciona item padr�o
                        PickerCategoria.Items.Add("Selecione uma categoria");
                        categorias.Add(new Categoria { idCategoria = 0, nomeCategoria = "Selecione uma categoria" });

                        // Adiciona categorias da API
                        if (categoriasApi != null)
                        {
                            foreach (var categoria in categoriasApi)
                            {
                                PickerCategoria.Items.Add(categoria.nomeCategoria);
                                categorias.Add(categoria);
                            }
                        }

                        // Define o primeiro item como selecionado por padr�o
                        PickerCategoria.SelectedIndex = 0;
                    });
                }
                else
                {
                    // Fallback: se n�o conseguir carregar, adiciona categorias b�sicas
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await DisplayAlert("Aviso", "N�o foi poss�vel carregar as categorias. Usando categorias padr�o.", "OK");
                        CarregarCategoriasPadrao();
                    });
                }
            }
            catch (HttpRequestException ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Erro de Conex�o", $"Erro ao conectar com a API: {ex.Message}", "OK");
                    CarregarCategoriasPadrao();
                });
            }
            catch (TaskCanceledException)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Timeout", "Timeout ao carregar categorias. Usando categorias padr�o.", "OK");
                    CarregarCategoriasPadrao();
                });
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Erro", $"Erro ao carregar categorias: {ex.Message}", "OK");
                    CarregarCategoriasPadrao();
                });
            }
        }

        private void CarregarCategoriasPadrao()
        {
            try
            {
                // Categorias padr�o caso a API falhe
                categorias.Clear();
                PickerCategoria.Items.Clear();

                var categoriasDefault = new List<Categoria>
                {
                    new Categoria { idCategoria = 0, nomeCategoria = "Selecione" },
                    new Categoria { idCategoria = 1, nomeCategoria = "Outros" }
                };

                foreach (var categoria in categoriasDefault)
                {
                    PickerCategoria.Items.Add(categoria.nomeCategoria);
                    categorias.Add(categoria);
                }

                PickerCategoria.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // Log do erro mas n�o trava a aplica��o
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar categorias padr�o: {ex.Message}");
            }
        }

        private async void OnRefreshCategoriasClicked(object sender, EventArgs e)
        {
            await CarregarCategoriasAsync();
        }

        private void OnAluguelClicked(object sender, EventArgs e)
        {
            try
            {
                isAluguelSelected = true;
                unidadeDePreco = "dia"; // Para aluguel, normalmente � por dia/m�s

                // Estilo para Aluguel selecionado
                AluguelBorder.BackgroundColor = Colors.DarkGray;
                AluguelButton.TextColor = Colors.White;

                // Estilo para Venda n�o selecionado
                VendaBorder.BackgroundColor = Colors.Transparent;
                VendaButton.TextColor = Colors.Black;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao selecionar aluguel: {ex.Message}");
            }
        }

        private void OnVendaClicked(object sender, EventArgs e)
        {
            try
            {
                isAluguelSelected = false;
                unidadeDePreco = "unidade"; // Para venda, normalmente � por unidade

                // Estilo para Venda selecionado
                VendaBorder.BackgroundColor = Colors.DarkGray;
                VendaButton.TextColor = Colors.White;

                // Estilo para Aluguel n�o selecionado
                AluguelBorder.BackgroundColor = Colors.Transparent;
                AluguelButton.TextColor = Colors.Black;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao selecionar venda: {ex.Message}");
            }
        }

        private async void OnCadastrarClicked(object sender, EventArgs e)
        {
            try
            {
                // Validar campos obrigat�rios primeiro
                if (!ValidarCampos())
                    return;

                // Extrair pre�o corretamente
                var preco = ExtrairPreco(EntryPreco.Text);
                if (preco <= 0)
                {
                    await DisplayAlert("Erro", "Pre�o deve ser maior que zero", "OK");
                    return;
                }

                // Obter ID da categoria selecionada
                var indiceSelecionado = PickerCategoria.SelectedIndex;
                if (indiceSelecionado <= 0 || indiceSelecionado >= categorias.Count)
                {
                    await DisplayAlert("Erro", "Selecione uma categoria v�lida", "OK");
                    return;
                }

                var categoriaSelecionada = categorias[indiceSelecionado];
                idCategoria = categoriaSelecionada.idCategoria;

                // Criar objeto do produto conforme esperado pela API
                var produto = new
                {
                    idUsuario = idUsuario,
                    idCategoria = idCategoria,
                    nome = EntryNomeProduto.Text?.Trim() ?? "",
                    preco = preco,
                    idVendaAluguel = isAluguelSelected ? 1 : 2, // 1 = Aluguel, 2 = Venda
                    idEndereco = idEndereco, // null por enquanto
                    descricaoProduto = EntryDescricaoProduto.Text?.Trim() ?? "",
                    unidadeDePreco = unidadeDePreco,
                    // Endere�o separado para facilitar o tratamento na API
                    endereco = new
                    {
                        rua = EntryRua.Text?.Trim() ?? "",
                        numero = EntryNumero.Text?.Trim() ?? "",
                        bairro = EntryBairro.Text?.Trim() ?? "",
                        cidade = EntryCidade.Text?.Trim() ?? "",
                        uf = EntryUf.Text?.Trim().ToUpper() ?? "",
                        cep = EntryCep.Text?.Trim() ?? ""
                    }
                };

                var json = JsonConvert.SerializeObject(produto, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    Formatting = Formatting.None
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                // Headers adicionais que podem ser necess�rios
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = await client.PostAsync(apiUrlProdutos, content);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Sucesso", "Produto cadastrado com sucesso!", "OK");

                    // Limpar formul�rio ap�s sucesso
                    LimparCampos();
                }
                else
                {
                    string errorContent = "";
                    try
                    {
                        errorContent = await response.Content.ReadAsStringAsync();
                    }
                    catch
                    {
                        errorContent = "N�o foi poss�vel ler o erro do servidor";
                    }

                    await DisplayAlert("Erro", $"Falha ao cadastrar produto:\nStatus: {response.StatusCode}\nDetalhes: {errorContent}", "OK");
                }
            }
            catch (HttpRequestException ex)
            {
                await DisplayAlert("Erro de Conex�o", $"N�o foi poss�vel conectar ao servidor:\n{ex.Message}", "OK");
            }
            catch (TaskCanceledException)
            {
                await DisplayAlert("Timeout", "A requisi��o demorou muito para responder. Verifique sua conex�o.", "OK");
            }
            catch (JsonException ex)
            {
                await DisplayAlert("Erro de Dados", $"Erro ao processar dados:\n{ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro inesperado:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "OK");
            }
        }

        private decimal ExtrairPreco(string precoTexto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(precoTexto))
                    return 0;

                // Remove "R$", espa�os e s�mbolos, mantendo apenas n�meros e v�rgula/ponto
                var precoLimpo = precoTexto
                    .Replace("R$", "")
                    .Replace(" ", "")
                    .Replace(".", "");

                // Converte v�rgula para ponto para parsing
                precoLimpo = precoLimpo.Replace(",", ".");

                if (decimal.TryParse(precoLimpo, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal preco))
                    return preco;

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private bool ValidarCampos()
        {
            try
            {
                var camposObrigatorios = new List<(Entry entry, string nomeCampo)>
                {
                    (EntryNomeProduto, "Nome do Produto"),
                    (EntryPreco, "Pre�o"),
                    (EntryRua, "Rua"),
                    (EntryDescricaoProduto, "Descri��o"),
                    (EntryNumero, "N�mero"),
                    (EntryBairro, "Bairro"),
                    (EntryCep, "CEP"),
                    (EntryUf, "UF"),
                    (EntryCidade, "Cidade"),
                };

                var camposVazios = new List<string>();

                foreach (var (entry, nomeCampo) in camposObrigatorios)
                {
                    if (entry == null || string.IsNullOrWhiteSpace(entry.Text))
                    {
                        camposVazios.Add(nomeCampo);
                        // Destaca o campo vazio com borda vermelha
                        if (entry != null)
                            DestacarCampoVazio(entry);
                    }
                    else
                    {
                        // Remove o destaque se o campo foi preenchido
                        RemoverDestaqueCampo(entry);
                    }
                }

                // Validar categoria separadamente
                if (PickerCategoria.SelectedIndex <= 0 || PickerCategoria.SelectedItem == null)
                {
                    camposVazios.Add("Categoria");
                    DestacarCampoPicker(PickerCategoria);
                }
                else
                {
                    RemoverDestaqueCampoPicker(PickerCategoria);
                }

                if (camposVazios.Any())
                {
                    string mensagem = $"Os seguintes campos s�o obrigat�rios:\n� {string.Join("\n� ", camposVazios)}";
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Campos Obrigat�rios", mensagem, "OK");
                    });
                    return false;
                }

                // Valida��es adicionais
                if (!ValidarPreco(EntryPreco.Text))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Erro", "Por favor, insira um pre�o v�lido.", "OK");
                        DestacarCampoVazio(EntryPreco);
                    });
                    return false;
                }

                if (!ValidarCep(EntryCep.Text))
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Erro", "Por favor, insira um CEP v�lido no formato 00000-000.", "OK");
                        DestacarCampoVazio(EntryCep);
                    });
                    return false;
                }

                if (EntryUf.Text?.Length != 2)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Erro", "UF deve conter exatamente 2 caracteres.", "OK");
                        DestacarCampoVazio(EntryUf);
                    });
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro na valida��o: {ex.Message}");
                return false;
            }
        }

        private bool ValidarPreco(string preco)
        {
            try
            {
                // Remove caracteres n�o num�ricos exceto v�rgula e ponto
                string precoLimpo = preco?.Replace("R$", "").Replace(" ", "").Replace(".", "").Replace(",", ".");

                return decimal.TryParse(precoLimpo, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor) && valor > 0;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidarCep(string cep)
        {
            try
            {
                // Verifica se o CEP est� no formato correto: 00000-000
                return !string.IsNullOrWhiteSpace(cep) &&
                       cep.Length == 9 &&
                       cep.Contains('-') &&
                       cep.Replace("-", "").All(char.IsDigit);
            }
            catch
            {
                return false;
            }
        }

        private void DestacarCampoVazio(Entry entry)
        {
            try
            {
                // Encontra o Border pai do Entry e muda a cor da borda
                if (entry?.Parent is Border border)
                {
                    border.Stroke = Colors.Red;
                    border.StrokeThickness = 2;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao destacar campo: {ex.Message}");
            }
        }

        private void RemoverDestaqueCampo(Entry entry)
        {
            try
            {
                // Restaura a cor original da borda
                if (entry?.Parent is Border border)
                {
                    border.Stroke = Colors.Gray;
                    border.StrokeThickness = 1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao remover destaque: {ex.Message}");
            }
        }

        private void DestacarCampoPicker(Picker picker)
        {
            try
            {
                // Encontra o Border pai do Picker e muda a cor da borda
                if (picker?.Parent is Border border)
                {
                    border.Stroke = Colors.Red;
                    border.StrokeThickness = 2;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao destacar picker: {ex.Message}");
            }
        }

        private void RemoverDestaqueCampoPicker(Picker picker)
        {
            try
            {
                // Restaura a cor original da borda
                if (picker?.Parent is Border border)
                {
                    border.Stroke = Colors.Gray;
                    border.StrokeThickness = 1;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao remover destaque do picker: {ex.Message}");
            }
        }

        private void LimparCampos()
        {
            try
            {
                EntryNomeProduto.Text = string.Empty;
                EntryPreco.Text = string.Empty;
                PickerCategoria.SelectedIndex = 0; // Volta para "Selecione uma categoria"
                EntryDescricaoProduto.Text = string.Empty;
                EntryRua.Text = string.Empty;
                EntryNumero.Text = string.Empty;
                EntryBairro.Text = string.Empty;
                EntryCep.Text = string.Empty;
                EntryUf.Text = string.Empty;
                EntryCidade.Text = string.Empty;

                // Reseta o toggle para Aluguel
                OnAluguelClicked(null, null);

                // Remove destaques de erro
                var entries = new[] { EntryNomeProduto, EntryPreco, EntryDescricaoProduto, EntryRua, EntryNumero, EntryBairro, EntryCep, EntryUf, EntryCidade };
                foreach (var entry in entries)
                {
                    if (entry != null)
                        RemoverDestaqueCampo(entry);
                }

                // Remove destaque do Picker
                RemoverDestaqueCampoPicker(PickerCategoria);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao limpar campos: {ex.Message}");
            }
        }

        private void EntryPreco_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (_isUpdating) return;

                var entry = sender as Entry;
                var newText = e.NewTextValue;

                if (string.IsNullOrEmpty(newText))
                    return;

                _isUpdating = true;

                // Remove caracteres n�o num�ricos
                string numericText = new string(newText.Where(char.IsDigit).ToArray());

                if (string.IsNullOrEmpty(numericText))
                {
                    entry.Text = "";
                    _isUpdating = false;
                    return;
                }

                // Converte para decimal e formata
                if (decimal.TryParse(numericText, out decimal value))
                {
                    decimal currency = value / 100;

                    // Usa ToString com formato C2 (2 casas decimais)
                    var cultureBR = new CultureInfo("pt-BR");
                    entry.Text = currency.ToString("C2", cultureBR);
                    entry.CursorPosition = entry.Text.Length;
                }

                _isUpdating = false;
            }
            catch (Exception ex)
            {
                _isUpdating = false;
                System.Diagnostics.Debug.WriteLine($"Erro ao formatar pre�o: {ex.Message}");
            }
        }
    }
}