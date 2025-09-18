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
        private int idUsuario = 2;     // Temporário - pegar do login/sessão depois
        private int idCategoria = 1;   // Será dinâmico baseado na categoria escolhida
        private int? idEndereco = null; // Pode ser null se não tiver endereço cadastrado
        private string unidadeDePreco = "dia"; // "dia", "mês", "unidade", etc.
        
        private bool isAluguelSelected = true; // Aluguel é selecionado por padrão
        private bool _isUpdating = false;

        // Lista para armazenar as categorias vindas da API
        private List<Categoria> categorias = new List<Categoria>();

        public Produtos()
        {
            InitializeComponent();
            _ = CarregarCategoriasAsync(); // Chama de forma assíncrona
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
                    
                    // Limpa as categorias atuais
                    categorias.Clear();
                    PickerCategoria.Items.Clear();
                    
                    // Adiciona item padrão
                    PickerCategoria.Items.Add("Selecione uma categoria");
                    categorias.Add(new Categoria { idCategoria = 0, nomeCategoria = "Selecione uma categoria" });
                    
                    // Adiciona categorias da API
                    foreach (var categoria in categoriasApi)
                    {
                        PickerCategoria.Items.Add(categoria.nomeCategoria);
                        categorias.Add(categoria);
                    }
                    
                    // Define o primeiro item como selecionado por padrão
                    PickerCategoria.SelectedIndex = 0;
                }
                else
                {
                    // Fallback: se não conseguir carregar, adiciona categorias básicas
                    await DisplayAlert("Aviso", "Não foi possível carregar as categorias. Usando categorias padrão.", "OK");
                    CarregarCategoriasPadrao();
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Erro", "Erro ao carregar categorias. Usando categorias padrão.", "OK");
                CarregarCategoriasPadrao();
            }
        }

        private void CarregarCategoriasPadrao()
        {
            // Categorias padrão caso a API falhe
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

        private async void OnRefreshCategoriasClicked(object sender, EventArgs e)
        {
            await CarregarCategoriasAsync();
        }

        private void OnAluguelClicked(object sender, EventArgs e)
        {
            isAluguelSelected = true;
            unidadeDePreco = "dia"; // Para aluguel, normalmente é por dia/mês

            // Estilo para Aluguel selecionado
            AluguelBorder.BackgroundColor = Colors.DarkGray;
            AluguelButton.TextColor = Colors.White;

            // Estilo para Venda não selecionado
            VendaBorder.BackgroundColor = Colors.Transparent;
            VendaButton.TextColor = Colors.Black;
        }

        private void OnVendaClicked(object sender, EventArgs e)
        {
            isAluguelSelected = false;
            unidadeDePreco = "unidade"; // Para venda, normalmente é por unidade

            // Estilo para Venda selecionado
            VendaBorder.BackgroundColor = Colors.DarkGray;
            VendaButton.TextColor = Colors.White;

            // Estilo para Aluguel não selecionado
            AluguelBorder.BackgroundColor = Colors.Transparent;
            AluguelButton.TextColor = Colors.Black;
        }

        private async void OnCadastrarClicked(object sender, EventArgs e)
        {
            try
            {
                // Validar campos obrigatórios primeiro
                if (!ValidarCampos())
                    return;

                // Extrair preço corretamente
                var preco = ExtrairPreco(EntryPreco.Text);
                if (preco <= 0)
                {
                    await DisplayAlert("Erro", "Preço deve ser maior que zero", "OK");
                    return;
                }

                // Obter ID da categoria selecionada
                var indiceSelecionado = PickerCategoria.SelectedIndex;
                if (indiceSelecionado <= 0 || indiceSelecionado >= categorias.Count)
                {
                    await DisplayAlert("Erro", "Selecione uma categoria válida", "OK");
                    return;
                }
                
                var categoriaSelecionada = categorias[indiceSelecionado];
                idCategoria = categoriaSelecionada.idCategoria;

                // Montar endereço completo para descrição
                var enderecoCompleto = $"{EntryRua.Text}, {EntryNumero.Text} - {EntryBairro.Text}, {EntryCidade.Text}/{EntryUf.Text} - CEP: {EntryCep.Text}";

                // Criar objeto do produto conforme esperado pela API
                var produto = new
                {
                    idUsuario = idUsuario,
                    idCategoria = idCategoria,
                    nome = EntryNomeProduto.Text.Trim(),
                    preco = preco,
                    idVendaAluguel = isAluguelSelected ? 1 : 2, // 1 = Aluguel, 2 = Venda
                    idEndereco = idEndereco, // null por enquanto
                    descricaoProduto = EntryDescricaoProduto,
                    unidadeDePreco = unidadeDePreco
                };

                var json = JsonConvert.SerializeObject(produto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(30); // Timeout de 30 segundos

                var response = await client.PostAsync(apiUrlProdutos, content);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Sucesso", "Produto cadastrado com sucesso!", "OK");
                    
                    // Limpar formulário após sucesso
                    LimparCampos();
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    await DisplayAlert("Erro", $"Falha ao cadastrar produto:\nStatus: {response.StatusCode}\nDetalhes: {errorContent}", "OK");
                }
            }
            catch (HttpRequestException)
            {
                await DisplayAlert("Erro de Conexão", "Não foi possível conectar ao servidor. Verifique sua conexão.", "OK");
            }
            catch (TaskCanceledException)
            {
                await DisplayAlert("Timeout", "A requisição demorou muito para responder. Verifique sua conexão.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro inesperado:\n{ex.Message}", "OK");
            }
        }

        private decimal ExtrairPreco(string precoTexto)
        {
            if (string.IsNullOrWhiteSpace(precoTexto))
                return 0;

            // Remove "R$", espaços e símbolos, mantendo apenas números e vírgula/ponto
            var precoLimpo = precoTexto
                .Replace("R$", "")
                .Replace(" ", "")
                .Replace(".", "");

            // Converte vírgula para ponto para parsing
            precoLimpo = precoLimpo.Replace(",", ".");

            if (decimal.TryParse(precoLimpo, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal preco))
                return preco;

            return 0;
        }

        private bool ValidarCampos()
        {
            var camposObrigatorios = new List<(Entry entry, string nomeCampo)>
            {
                (EntryNomeProduto, "Nome do Produto"),
                (EntryPreco, "Preço"),
                (EntryRua, "Rua"),
                (EntryDescricaoProduto, "Descricao"),
                (EntryNumero, "Número"),
                (EntryBairro, "Bairro"),
                (EntryCep, "CEP"),
                (EntryUf, "UF"),
                (EntryCidade, "Cidade"),
            };

            var camposVazios = new List<string>();

            foreach (var (entry, nomeCampo) in camposObrigatorios)
            {
                if (string.IsNullOrWhiteSpace(entry.Text))
                {
                    camposVazios.Add(nomeCampo);
                    // Destaca o campo vazio com borda vermelha
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
                string mensagem = $"Os seguintes campos são obrigatórios:\n• {string.Join("\n• ", camposVazios)}";
                DisplayAlert("Campos Obrigatórios", mensagem, "OK");
                return false;
            }

            // Validações adicionais
            if (!ValidarPreco(EntryPreco.Text))
            {
                DisplayAlert("Erro", "Por favor, insira um preço válido.", "OK");
                DestacarCampoVazio(EntryPreco);
                return false;
            }

            if (!ValidarCep(EntryCep.Text))
            {
                DisplayAlert("Erro", "Por favor, insira um CEP válido no formato 00000-000.", "OK");
                DestacarCampoVazio(EntryCep);
                return false;
            }

            if (EntryUf.Text?.Length != 2)
            {
                DisplayAlert("Erro", "UF deve conter exatamente 2 caracteres.", "OK");
                DestacarCampoVazio(EntryUf);
                return false;
            }

            return true;
        }

        private bool ValidarPreco(string preco)
        {
            // Remove caracteres não numéricos exceto vírgula e ponto
            string precoLimpo = preco?.Replace("R$", "").Replace(" ", "").Replace(".", "").Replace(",", ".");

            return decimal.TryParse(precoLimpo, out decimal valor) && valor > 0;
        }

        private bool ValidarCep(string cep)
        {
            // Verifica se o CEP está no formato correto: 00000-000
            return !string.IsNullOrWhiteSpace(cep) &&
                   cep.Length == 9 &&
                   cep.Contains('-') &&
                   cep.Replace("-", "").All(char.IsDigit);
        }

        private void DestacarCampoVazio(Entry entry)
        {
            // Encontra o Border pai do Entry e muda a cor da borda
            if (entry.Parent is Border border)
            {
                border.Stroke = Colors.Red;
                border.StrokeThickness = 2;
            }
        }

        private void RemoverDestaqueCampo(Entry entry)
        {
            // Restaura a cor original da borda
            if (entry.Parent is Border border)
            {
                border.Stroke = Colors.Gray;
                border.StrokeThickness = 1;
            }
        }

        private void DestacarCampoPicker(Picker picker)
        {
            // Encontra o Border pai do Picker e muda a cor da borda
            if (picker.Parent is Border border)
            {
                border.Stroke = Colors.Red;
                border.StrokeThickness = 2;
            }
        }

        private void RemoverDestaqueCampoPicker(Picker picker)
        {
            // Restaura a cor original da borda
            if (picker.Parent is Border border)
            {
                border.Stroke = Colors.Gray;
                border.StrokeThickness = 1;
            }
        }

        private void LimparCampos()
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
                RemoverDestaqueCampo(entry);
            }
            
            // Remove destaque do Picker
            RemoverDestaqueCampoPicker(PickerCategoria);
        }

        private void EntryPreco_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return;

            var entry = sender as Entry;
            var newText = e.NewTextValue;

            if (string.IsNullOrEmpty(newText))
                return;

            _isUpdating = true;

            // Remove caracteres não numéricos
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
                entry.Text = currency.ToString("C2", new CultureInfo("pt-BR"));
                entry.CursorPosition = entry.Text.Length;
            }

            _isUpdating = false;
        }
    }
}