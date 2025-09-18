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
            Routing.RegisterRoute("Detalhes", typeof(Detalhesxaml));
        }
    }
}
