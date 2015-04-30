using MP3Player.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Net;
using System.Threading;


namespace MP3Player
{

   
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private CancellationTokenSource cts;
        private List<Track> trackList;

        public HubPage()
        {
            this.InitializeComponent();
            


            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            InitData();
            
            
        }


        void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(LoginPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        private async void InitData()
        {
            cts = new CancellationTokenSource();

            try
            {
                trackList = await Track.GetAllTracks();
                HubSec.Header = "\r\nTracks:"+trackList.Count;
                HubSec.DataContext = new CollectionViewSource { Source = trackList };
                
            }
            catch (OperationCanceledException)
            {
                HubSec.Header = "\r\nDownloads canceled.\r\n";
            }
            catch (Exception)
            {
                HubSec.Header = "\r\nDownloads failed.\r\n";
            }

            cts = null;
        }
        

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }


        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            //var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            //this.DefaultViewModel["Groups"] = sampleDataGroups;
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Track currentTrack = ((Track)e.ClickedItem);
            mediaPlayer.Source = new Uri(currentTrack.Mp3Uri);
            List<Track> temp = new List<Track>();
            currentTrackView.Width = double.NaN;
            temp.Add(currentTrack);
            currentTrackView.DataContext = new CollectionViewSource { Source = temp };
            PlayMedia(true);      
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            TogglePlayPause();
        }

        private void TogglePlayPause()
        {
            if ((string)PlayPauseButton.Tag == "play")
            {
                PlayMedia(false);
            }
            else
            {
                PlayMedia(true);
            } 
        }

        private void PlayMedia(bool play)
        {
            if (play)
            {
                PlayPauseButton.Icon = new SymbolIcon(Symbol.Pause);
                PlayPauseButton.Tag = "play";
                PlayPauseButton.IsEnabled = true;
                mediaPlayer.Play();
            }
            else
            {
                PlayPauseButton.Icon = new SymbolIcon(Symbol.Play);
                PlayPauseButton.Tag = "pause";
                mediaPlayer.Pause();
                
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            PlayMedia(false);
            mediaPlayer.Stop();
        }
    }
}
