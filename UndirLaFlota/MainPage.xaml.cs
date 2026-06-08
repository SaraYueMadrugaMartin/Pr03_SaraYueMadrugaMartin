using UndirLaFlota.Juego;

namespace UndirLaFlota;

public partial class MainPage : ContentPage
{
    #region Variables
    private GameManager gameManager;
    private IAPlayer enemigoIA = new IAPlayer();

    private int sunkenBoatsPlayer01 = 0;
    private int sunkenBoatsPlayer02 = 0;
    private int lostBoatsPlayer01 = 5;
    private int lostBoatsPlayer02 = 5;
    #endregion

    #region Constructor MainPage
    public MainPage()
    {
        InitializeComponent();

        gameManager = new GameManager();
        CounterBoatsPlayer01.Text = "0";
        CounterBoatsPlayer02.Text = "0";

        CounterLostBoatsPlayer01.Text = "5";
        CounterLostBoatsPlayer02.Text = "5";

        InitializeBoard(TableroGridPlayer01, gameManager.BoardPlayer01, "Jugador 1");
        InitializeBoard(TableroGridPlayer02, gameManager.BoardPlayer02, "Jugador 2");
    }
    #endregion

    #region Métodos Propios
    /// <summary>
    /// Ejecuta la acción del botón de confirmación de barcos.
    /// </summary>
    private async void OnAceptarClicked(object sender, EventArgs e)
    {
        if (gameManager.BoardPlayer01.CheckBoatsPositions(out List<int> detectados))
        {
            gameManager.BoardPlayer01.IsPlacingBoats = false;
            gameManager.IsPlayerTurn = true;
            ((Button)sender).IsEnabled = false;
            await DisplayAlert("¡Empieza la partida!", "Ya has colocado todos tus barcos", "Empezar");
        }
        else
        {
            string detectadosStr = detectados.Count == 0 ? "Ninguno" : string.Join(", ", detectados);
            await DisplayAlert("No se puede comenzar", $"Debes tener barcos de tamaños: 6, 5, 3, 3, 2.\n" + $"Tus barcos actuales miden: {detectadosStr}", "Revisar");
        }
    }

    /// <summary>
    /// Método para crear el tablero con las configuraciones pertinentes.
    /// </summary>
    private void InitializeBoard(Grid grid, Tablero board, string nombreBoard)
    {
        ConfigureGrid(grid, board.Dim);
        AddHeaders(grid, board.Dim);
        BuildBoard(board, grid);

        board.ImprimirTablero(nombreBoard);
    }

    /// <summary>
    /// Método para definir las dimensiones del tablero.
    /// </summary>
    private void ConfigureGrid(Grid grid, int dim)
    {
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();

        for (int i = 0; i <= dim; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }
    }

    /// <summary>
    /// Método para añadir letras y números para indicar cada columna y fila.
    /// </summary>
    private void AddHeaders(Grid grid, int dim)
    {
        // Añadir fila indicando con números.
        for (int i = 1; i <= dim; i++)
            grid.Add(new Label { Text = i.ToString(), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, i, 0);

        // Añadir columna indicando con letras.
        for (int i = 1; i <= dim; i++)
            grid.Add(new Label { Text = ((char)('A' + i - 1)).ToString(), HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center }, 0, i);
    }

    /// <summary>
    /// Construye visualmente los botones del tablero.
    /// </summary>
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
                    BackgroundColor = Colors.Aqua,
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

    /// <summary>
    /// Gestiona las consecuencias de disparar a una casilla.
    /// </summary>
    private async void Seleccion(Tablero board, Button btn, int row, int column)
    {
        if (gameManager.BoardPlayer01.IsPlacingBoats)
        {
            if (board == gameManager.BoardPlayer01)
            {
                if (board.TableroList[row][column] == 0)
                {
                    if (board.CountBoats() >= 19)
                    {
                        await DisplayAlert("Límite", "Ya has colocado las 19 casillas.", "OK");
                        return;
                    }
                    board.TableroList[row][column] = 2;
                    btn.Text = "2";
                    btn.BackgroundColor = Colors.Grey;
                }
                else if (board.TableroList[row][column] == 2)
                {
                    board.TableroList[row][column] = 0;
                    btn.Text = "0";
                    btn.BackgroundColor = Colors.Aqua;
                }
            }
            else
            {
                await DisplayAlert("Atención", "Coloca tus barcos primero en tu tablero.", "OK");
            }
            return;
        }

        if (!gameManager.IsPlayerTurn) return;

        if (board == gameManager.BoardPlayer01)
        {
            await DisplayAlert("Atención", "¡No te dispares a ti mismo!", "OK");
            return;
        }

        string? result = board.Jugada(row, column);
        if (result == null) return;

        switch (result)
        {
            case "Agua":
                gameManager.IsPlayerTurn = false;
                btn.Text = "1";
                btn.BackgroundColor = Colors.Blue;
                await Task.Delay(1000);
                EjecutarTurnoIA();
                break;
            case "Tocado":
                btn.Text = "3";
                btn.BackgroundColor = Colors.Red;
                break;
            case "Hundido":
                SinkBoatPlayer(board);
                await DisplayAlert("¡Buen disparo!", "¡Has hundido un barco enemigo!", "OK");
                break;
            case "Partida finalizada":
                SinkBoatPlayer(board);
                await Navigation.PushAsync(new FinalPage());
                //await DisplayAlert("Partida", "¡Partida finalizada!", "OK");
                break;
        }
    }

    /// <summary>
    /// Gestiona de forma automática el disparo de la Inteligencia Artificial.
    /// </summary>
    private async void EjecutarTurnoIA()
    {
        if (gameManager.BoardPlayer01.IsPlacingBoats || gameManager.IsPlayerTurn) return;

        var coordenada = enemigoIA.IAShoot(gameManager.BoardPlayer01);
        int fila = coordenada.Item1;
        int columna = coordenada.Item2;

        string? result = gameManager.BoardPlayer01.Jugada(fila, columna);
        if (result == null) return;

        enemigoIA.SaveResult(result, fila, columna);

        Button? botonImpactado = BuscarBotonEnGrid(TableroGridPlayer01, fila, columna);
        if (botonImpactado != null)
        {
            if (result == "Agua")
            {
                botonImpactado.Text = "1";
                botonImpactado.BackgroundColor = Colors.Blue;
            }
            else if (result == "Tocado" || result == "Hundido" || result == "Partida finalizada")
            {
                botonImpactado.Text = "3";
                botonImpactado.BackgroundColor = Colors.Red;
            }

            if (result == "Hundido")
            {
                SinkBoatIA();
                await DisplayAlert("¡Malas noticias!", "La IA ha hundido uno de tus barcos.", "OK");
                await Task.Delay(1500);
                EjecutarTurnoIA();
                return;
            }

            if (result == "Partida finalizada")
            {
                SinkBoatIA();
                await DisplayAlert("Derrota", "La IA ha hundido tu flota.", "OK");
                return;
            }

            if (result == "Tocado")
            {
                await Task.Delay(1500);
                EjecutarTurnoIA();
            }
            else if (result == "Agua")
                gameManager.IsPlayerTurn = true;
        }
    }

    /// <summary>
    /// Busca un objeto tipo button dentro de la cuadrícula mediante sus coordenadas.
    /// </summary>
    private Button? BuscarBotonEnGrid(Grid grid, int fila, int columna)
    {
        foreach (var child in grid.Children)
        {
            if (child is Button button && button.CommandParameter is Tuple<int, int> pos)
            {
                if (pos.Item1 == fila && pos.Item2 == columna)
                    return button;
            }
        }
        return null;
    }

    private void SinkBoatPlayer(Tablero board)
    {
        var casillasHundidasIA = board.LastShipSunk;

        foreach (var coordenada in casillasHundidasIA)
        {
            Button? botonBarco = BuscarBotonEnGrid(TableroGridPlayer02, coordenada.Item1, coordenada.Item2);
            if (botonBarco != null)
            {
                botonBarco.Text = "3";
                botonBarco.BackgroundColor = Colors.LightPink;
            }
        }

        sunkenBoatsPlayer01++;
        lostBoatsPlayer02--;
        CounterBoatsPlayer01.Text = sunkenBoatsPlayer01.ToString();
        CounterLostBoatsPlayer02.Text = lostBoatsPlayer02.ToString();
    }

    private void SinkBoatIA()
    {
        var casillasHundidasJugador = gameManager.BoardPlayer01.LastShipSunk;

        foreach (var pos in casillasHundidasJugador)
        {
            Button? botonBarco = BuscarBotonEnGrid(TableroGridPlayer01, pos.Item1, pos.Item2);
            if (botonBarco != null)
            {
                botonBarco.Text = "3";
                botonBarco.BackgroundColor = Colors.LightPink;
            }
        }

        sunkenBoatsPlayer02++;
        lostBoatsPlayer01--;
        CounterBoatsPlayer02.Text = sunkenBoatsPlayer02.ToString();
        CounterLostBoatsPlayer01.Text = lostBoatsPlayer01.ToString();
    }
    #endregion
}