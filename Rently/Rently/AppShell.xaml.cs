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
        }
    }
}
