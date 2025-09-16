using Microsoft.Maui.Controls.Shapes;

namespace Rently;

public partial class Dashboard : ContentPage
{
    int exercicioCount = 0;

    public Dashboard()
    {
        InitializeComponent();
        AdicionarExercicio();

        // Escuta quando o tema muda (claro/escuro)
        Application.Current.RequestedThemeChanged += (s, e) =>
        {
            AtualizarCores();
        };
    }

    // Cor do Label "Exercício X" (laranja no claro, branco no escuro)
    private Color GetExercicioLabelColor()
    {
        return Application.Current.RequestedTheme == AppTheme.Dark
            ? Colors.White
            : Color.FromArgb("#FF6B35");
    }

    // Cor neutra fixa para Entry e bordas
    private Color GetNeutralColor()
    {
        return Color.FromArgb("#919191");
    }

    private void OnAdicionarExercicioClicked(object sender, EventArgs e)
    {
        AdicionarExercicio();
    }

    private void AdicionarExercicio()
    {
        exercicioCount++;

        var border = new Border
        {
            Stroke = GetNeutralColor(),
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            BackgroundColor = Colors.Transparent,
            Padding = 10,
            Margin = new Thickness(0, 0, 0, 10),

            Content = new VerticalStackLayout
            {
                Spacing = 7,
                Children =
                {
                    // Label "Exercício X"
                    new Label
                    {
                        Text = $"Exercício {exercicioCount}",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 16,
                        TextColor = GetExercicioLabelColor()
                    },

                    // Nome do exercício
                    new Border
                    {
                        Stroke = GetNeutralColor(),
                        StrokeThickness = 1,
                        StrokeShape = new RoundRectangle { CornerRadius = 6 },
                        BackgroundColor = Colors.Transparent,
                        Padding = 5,
                        Content = new Entry
                        {
                            Placeholder = "Nome do exercício",
                            PlaceholderColor = GetNeutralColor(),
                            TextColor = GetNeutralColor(),
                            BackgroundColor = Colors.Transparent,
                            HeightRequest = 40
                        }
                    },

                    // Séries e Repetições lado a lado
                    new HorizontalStackLayout
                    {
                        Spacing = 15,
                        Children =
                        {
                            // Séries
                            new Border
                            {
                                Stroke = GetNeutralColor(),
                                StrokeThickness = 1,
                                StrokeShape = new RoundRectangle { CornerRadius = 6 },
                                BackgroundColor = Colors.Transparent,
                                Padding = 5,
                                Content = new Entry
                                {
                                    Placeholder = "Séries",
                                    PlaceholderColor = GetNeutralColor(),
                                    TextColor = GetNeutralColor(),
                                    Keyboard = Keyboard.Numeric,
                                    BackgroundColor = Colors.Transparent,
                                    HeightRequest = 40
                                }
                            },

                            // Repetições
                            new Border
                            {
                                Stroke = GetNeutralColor(),
                                StrokeThickness = 1,
                                StrokeShape = new RoundRectangle { CornerRadius = 6 },
                                BackgroundColor = Colors.Transparent,
                                Padding = 5,
                               
                                Content = new Entry
                                {
                                    Placeholder = "Repetições ex: 12-15",
                                    PlaceholderColor = GetNeutralColor(),
                                    TextColor = GetNeutralColor(),
                                    Keyboard = Keyboard.Numeric,
                                    BackgroundColor = Colors.Transparent,
                                    HeightRequest = 40,
                                    
                                }
                            }
                        }
                    }
                }
            }
        };

        ExerciciosStack.Children.Add(border);
    }

    // Atualiza as cores quando o tema muda
    private void AtualizarCores()
    {
        foreach (var child in ExerciciosStack.Children)
        {
            if (child is Border border && border.Content is Layout layout)
            {
                foreach (var item in layout.Children)
                {
                    // Apenas atualiza o Label "Exercício X"
                    if (item is Label lbl)
                        lbl.TextColor = GetExercicioLabelColor();
                }
            }
        }
    }
}
