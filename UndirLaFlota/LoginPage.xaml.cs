using System.Threading.Tasks;

namespace UndirLaFlota;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private async void OnValidateUser(object sender, EventArgs e)
    {
        string playerName = NombrePlayer01.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(playerName))
        {
            await DisplayAlert("Falta Nombre", "Por favor, introduce tu nombre para jugar.", "Aceptar");
            return;
        }
        var button = (Button)sender;

		button.BackgroundColor = Colors.White;

		button.Text = "Clicado";

		await Navigation.PushAsync(new MainPage(playerName));
    }
}