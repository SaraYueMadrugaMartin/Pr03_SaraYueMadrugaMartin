using System.Diagnostics;
using UndirLaFlota.Juego;

namespace UndirLaFlota;

public partial class MainPage : ContentPage
{
    // Referencias a la clase "Tablero".
    private Tablero boardPlayer01; // Tablero del jugador 1.
    private Tablero boardPlayer02; // Tablero del jugador 2 (IA).

    public MainPage()
    {
        InitializeComponent();

        boardPlayer01 = new Tablero(false); // Para el jugador no se le generan barcos, así puede montar él su tablero como quiera.
        boardPlayer02 = new Tablero(true); // A la IA se le genera el tablero automáticamente.

        InitializeBoard(TableroGridPlayer01, boardPlayer01);
        InitializeBoard(TableroGridPlayer02, boardPlayer02);
    }

    /// <summary>
    /// Método para crear el tablero con las configuraciones pertinentes.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="board"></param>
    private void InitializeBoard(Grid grid, Tablero board)
    {
        ConfigureGrid(grid, board.Dim);
        AddHeaders(grid, board.Dim);
        BuildBoard(board, grid);
    }

    /// <summary>
    /// Método para definir las dimensiones del tablero.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="dim"></param>
    private void ConfigureGrid(Grid grid, int dim)
    {
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();

        for (int i = 0; i <= dim; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition{Height = GridLength.Star});
            grid.ColumnDefinitions.Add(new ColumnDefinition{Width = GridLength.Star});
        }
    }

    /// <summary>
    /// Método para añadir letras y números para indicar cada columna y fila.
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="dim"></param>
    private void AddHeaders(Grid grid, int dim)
    {
        // Añadir fila indicando con números.
        for (int i = 1; i <= dim; i++)
            grid.Add(new Label { Text = i.ToString(), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, i, 0);

        // Añadir columna indicando con letras.
        for (int i = 1; i <= dim; i++)
            grid.Add(new Label { Text = ((char)('A' + i - 1)).ToString(), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 0, i);
    }

    private void BuildBoard(Tablero board, Grid playerGrid)
    {
        for (int row = 0; row < board.Dim; row++)
        {
            for (int col = 0; col < board.Dim; col++)
            {
                string txt = board.TableroList[row][col].ToString();

                // Ocultar barcos
                if (txt == "2")
                    txt = "0";

                var button = new Button
                {
                    Text = txt,
                    FontSize = 10,
                    Padding = 0,
                    Margin = 1,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    CommandParameter = new Tuple<int, int>(row, col)
                };

                button.Clicked += (sender, e) =>
                {
                    var btn = (Button)sender!;
                    var position = (Tuple<int, int>)btn.CommandParameter;

                    Seleccion(board, btn, position.Item1, position.Item2);
                };

                playerGrid.Add(button, col + 1, row + 1);
            }
        }
    }

    private async void Seleccion(Tablero board, Button btn, int row, int column)
    {
        string? result = board.Jugada(row, column);

        if (result == null)
            return;

        switch (result)
        {
            case "Agua":
                btn.Text = "1";
                break;

            case "Tocado":
                btn.Text = "3";
                break;

            case "Partida finalizada":
                await DisplayAlert(
                    "Partida",
                    "¡Partida finalizada!",
                    "OK");
                break;
        }
    }
}