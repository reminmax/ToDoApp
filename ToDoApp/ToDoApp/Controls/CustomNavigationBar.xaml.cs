using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDoApp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomNavigationBar : ContentView
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title),
            typeof(string),
            typeof(CustomNavigationBar),
            default(string));

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
            nameof(CommandParameter),
            typeof(object),
            typeof(CustomNavigationBar),
            default);

        public static readonly BindableProperty NavigateBackCommandProperty = BindableProperty.Create(
            nameof(NavigateBackCommand),
            typeof(ICommand),
            typeof(CustomNavigationBar),
            default(ICommand));

        public CustomNavigationBar()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ICommand NavigateBackCommand
        {
            get => (ICommand)GetValue(NavigateBackCommandProperty);
            set => SetValue(NavigateBackCommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
    }
}