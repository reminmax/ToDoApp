using System;
using System.Collections.Generic;
using System.Windows.Input;
using FreshMvvm;
using ToDoApp.PageModels;
using ToDoApp.Services.AnalyticsService;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ToDoApp.Helpers.Extensions
{
    public class FreshBasePageModelExtended : FreshBasePageModel, IDisposable
    {
        private bool _disposedValue;
        protected IAnalyticsService AnalyticsService { get; private set; }

        public FreshBasePageModelExtended(IAnalyticsService analyticsService)
        {
            // Connectivity
            Connectivity.ConnectivityChanged += ConnectivityChangedEventHandler;
            NoInternetConnection = !Connectivity.NetworkAccess.Equals(NetworkAccess.Internet);

            AnalyticsService = analyticsService;

            // Commands
            NavigateBackCommand = new Command(async () => await CoreMethods.PopPageModel());

            MainState = LayoutState.None;
        }

        public LayoutState MainState { get; set; }

        public bool NoInternetConnection { get; set; }

        public string Title { get; set; }

        public ICommand NavigateBackCommand { get; }

        public override void Init(object initData)
        {
            base.Init(initData);

            AnalyticsService.TrackEvent("Visited page", 
                new Dictionary<string, string>
                {
                    { "Title", Title }
                });
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void ConnectivityChangedEventHandler(object sender, ConnectivityChangedEventArgs e)
        {
            NoInternetConnection = !e.NetworkAccess.Equals(NetworkAccess.Internet);

            if (NoInternetConnection)
            {
                // Navigate to corresponding page if internet connection lost
                CoreMethods.PushPageModel<NoConnectionPageModel>();
            }
            else
            {
                // If it's OK, navigate back
                CoreMethods.PopPageModel();
            }
        }

        ~FreshBasePageModelExtended() => Dispose(false);

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }
    }
}