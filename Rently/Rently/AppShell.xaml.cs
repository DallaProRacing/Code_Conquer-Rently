namespace Rently
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute("Cadastro", typeof(Cadastro));
            Routing.RegisterRoute("Senha", typeof(Senha));
            Routing.RegisterRoute("Dashboard", typeof(Dashboard));
            Routing.RegisterRoute("Perfil", typeof(Perfil));
            Routing.RegisterRoute("Produtos", typeof(Produtos));
            Routing.RegisterRoute("Anuncios", typeof(Anuncios));
        }

        private async void Inicio_Clicked_1(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Dashboard");
        }

        private async void Anuncios_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Anuncios");
        }

        private async void Perfil_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Perfil");
        }
    }
}
