using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ToDoApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        private readonly int _totalSlideNumbers = 3;
        private int _slideNumber;

        public WelcomePage()
        {
            InitializeComponent();

            _slideNumber = 0;
        }

        private void GoToNextSlide(object sender, EventArgs e)
        {
            _slideNumber = _slideNumber == _totalSlideNumbers - 1 ? 0 : _slideNumber + 1;
            CarouselView.ScrollTo(_slideNumber, animate: true);
        }
    }
}