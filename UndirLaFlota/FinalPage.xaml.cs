namespace UndirLaFlota;

public partial class FinalPage : ContentPage
{
    private readonly string _humanPlayerName;

    public FinalPage(string winnerName, int shots, string humanPlayerName)
    {
        InitializeComponent();

        _humanPlayerName = humanPlayerName;

        LabelGanador.Text = winnerName;
        LabelDisparos.Text = shots.ToString();

        if (winnerName == "Jugador 2")
            LabelGanador.TextColor = Colors.Red;
        else
            LabelGanador.TextColor = Colors.DarkGreen;
    }

    /// <summary>
    /// Método para empezar una nueva partida.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnNewGame(object sender, EventArgs e)
    {
        if (sender is Button btn) btn.IsEnabled = false;

        var nuevaPartida = new MainPage(_humanPlayerName);

        Application.Current.MainPage = new NavigationPage(nuevaPartida);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Método para 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnExit(object sender, EventArgs e)
    {
        // Cierra la ventana actual de la aplicación, provocando el cierre en Windows, Android, etc.
        if (Application.Current != null)
        {
            Application.Current.CloseWindow(Application.Current.MainPage.Window);
        }
    }
}