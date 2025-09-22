using LoginApp.Maui.Models;
using LoginApp.Maui.PageModels;

namespace LoginApp.Maui.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}