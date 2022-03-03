using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDoApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsMenuItem : ContentView
    {
        public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon),
            typeof(string),
            typeof(SettingsMenuItem),
            default(string));

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title),
            typeof(string),
            typeof(SettingsMenuItem),
            default(string));

        public SettingsMenuItem()
        {
            InitializeComponent();
        }

        public string Icon
        {
            get => (string) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
    }
}