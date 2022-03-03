using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDoApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskPage : ContentPage
    {
        public TaskPage()
        {
            InitializeComponent();
        }
    }
}