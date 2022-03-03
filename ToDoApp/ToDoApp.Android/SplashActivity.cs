using Android.App;
using Android.OS;
using ToDoApp.Droid;

namespace FitnessTools.Droid
{
    [Activity(Label = "ToDoApp", Theme = "@style/Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            StartActivity(typeof(MainActivity));
        }

        public override void OnBackPressed()
        {
        }
    }
}