namespace LoginApp.Maui.UserControls;

public partial class FlyoutHeaderControl : ContentView
{
	public FlyoutHeaderControl()
	{
		InitializeComponent();
		if(App.user != null)
		{
			lblText.Text = "Login as: ";
			lblUsername.Text = App.user.Username;
		}
	}
}