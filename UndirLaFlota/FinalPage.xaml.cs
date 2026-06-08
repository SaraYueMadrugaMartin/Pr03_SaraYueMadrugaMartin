namespace UndirLaFlota;

public partial class FinalPage : ContentPage
{
	public FinalPage(string winnerName)
	{
		InitializeComponent();

        LabelGanador.Text = winnerName;

        if (winnerName == "Jugador 2")
            LabelGanador.TextColor = Colors.Red;
    }
}