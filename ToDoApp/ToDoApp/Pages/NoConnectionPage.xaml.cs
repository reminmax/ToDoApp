using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDoApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NoConnectionPage : ContentPage
    {
        public NoConnectionPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            // Can't navigate back by press device button
            return true;
        }
    }
}