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
		var button = (Button)sender;

		button.BackgroundColor = Colors.White;

		button.Text = "Clicado";

		await Navigation.PushAsync(new MainPage());
    }
}