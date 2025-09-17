using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System.Globalization;

namespace Rently
{
    public partial class Produtos : ContentPage
    {
        private bool isAluguelSelected = true; // Aluguel é selecionado por padrão

        public Produtos()
        {
            InitializeComponent();
        }

        private void OnAluguelClicked(object sender, EventArgs e)
        {
            isAluguelSelected = true;

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

            // Estilo para Venda selecionado
            VendaBorder.BackgroundColor = Colors.DarkGray;
            VendaButton.TextColor = Colors.White;

            // Estilo para Aluguel não selecionado
            AluguelBorder.BackgroundColor = Colors.Transparent;
            AluguelButton.TextColor = Colors.Black;
        }

        private async void OnCadastrarClicked(object sender, EventArgs e)
        {
            if (ValidarCampos())
            {
                // Se todos os campos estão preenchidos, continue com o cadastro
                await DisplayAlert("Sucesso", "Produto cadastrado com sucesso!", "OK");

                // Aqui você pode adicionar a lógica para salvar o produto
                // Por exemplo: salvar no banco de dados, chamar API, etc.

                // Opcional: limpar os campos após o cadastro
                LimparCampos();
            }
        }

        private bool ValidarCampos()
        {
            var camposObrigatorios = new List<(Entry entry, string nomeCampo)>
            {
                (EntryNomeProduto, "Nome do Produto"),
                (EntryPreco, "Preço"),
                (EntryCategoria, "Categoria"),
                (EntryRua, "Rua"),
                (EntryNumero, "Número"),
                (EntryBairro, "Bairro"),
                (EntryCep, "CEP"),
                (EntryUf, "UF"),
                (EntryCidade, "Cidade"),
                (EntryTelefone, "Telefone")
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

            if (!ValidarTelefone(EntryTelefone.Text))
            {
                DisplayAlert("Erro", "Por favor, insira um telefone válido no formato (00) 00000-0000.", "OK");
                DestacarCampoVazio(EntryTelefone);
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

        private bool ValidarTelefone(string telefone)
        {
            // Verifica se o telefone está no formato correto: (00) 00000-0000
            return !string.IsNullOrWhiteSpace(telefone) &&
                   telefone.Length == 15 &&
                   telefone.StartsWith("(") &&
                   telefone.Contains(")") &&
                   telefone.Contains("-");
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

        private void LimparCampos()
        {
            EntryNomeProduto.Text = string.Empty;
            EntryPreco.Text = string.Empty;
            EntryCategoria.Text = string.Empty;
            EntryRua.Text = string.Empty;
            EntryNumero.Text = string.Empty;
            EntryBairro.Text = string.Empty;
            EntryCep.Text = string.Empty;
            EntryUf.Text = string.Empty;
            EntryCidade.Text = string.Empty;
            EntryTelefone.Text = string.Empty;

            // Reseta o toggle para Aluguel
            OnAluguelClicked(null, null);

            // Remove destaques de erro
            var entries = new[] { EntryNomeProduto, EntryPreco, EntryCategoria, EntryRua, EntryNumero, EntryBairro, EntryCep, EntryUf, EntryCidade, EntryTelefone };
            foreach (var entry in entries)
            {
                RemoverDestaqueCampo(entry);
            }
        }

        private bool _isUpdating = false;

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