using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236


namespace ReddtroBeta
{
    public sealed partial class SettingsFlyout : UserControl
    {
        public SettingsFlyout()
        {
            this.InitializeComponent();
            SettingsPane.GetForCurrentView().CommandsRequested += CommandsRequested;
        }                

        
        private void CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(new SettingsCommand("a", "About", (p) => { cfoAbout.IsOpen = true; hideWebView(); }));
            args.Request.ApplicationCommands.Add(new SettingsCommand("s", "Settings", (p) => { cfoSettings.IsOpen = true; AddSubredditList(); hideWebView(); }));
            args.Request.ApplicationCommands.Add(new SettingsCommand("s", "Privacy Policy", (p) => { Windows.System.Launcher.LaunchUriAsync(new Uri("http://mypage.iu.edu/~jjpusate/privacypolicy.html")); }));
            //webview trickery goes here; since used twice, abstract into function?

        }
        
        private void AddSubredditList()
        {           
            srmanagerlist.ItemsSource = Data.SampleDataSource._sampleDataSource.AllGroups;                        
        }

        void hideWebView()
        {
            //how to hide?
        }
        //void InitHandlers()
        //{
        //    Windows.Storage.ApplicationData.Current.DataChanged += new TypedEventHandler<ApplicationData, object>(DataChangeHandler);
            
        //}

        //void DataChangeHandler(Windows.Storage.ApplicationData appData, object o)
        //{
        //    // TODO: Refresh your data
        //}

        private async void OnMailTo(object sender, RoutedEventArgs args)
        {
            var hyperlinkButton = sender as HyperlinkButton;
            if (hyperlinkButton != null)
            {
                var uri = new Uri("http://www.reddit.com/r/reddtro");
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
        }

        private void SRMSave(object sender, RoutedEventArgs e)
        {
            Data.SubredditManager._subredditManager.WriteSubreddits();
        }

        private void SRMEdit(object sender, RoutedEventArgs e)
        {
            
        }

       
        private void SRMDelete(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.RemovedItems.Count == 0)
            {
                Data.SampleDataSource._sampleDataSource.AllGroups.RemoveAt(srmanagerlist.SelectedIndex);
                Data.SubredditManager._subredditManager.WriteSubreddits();
            }
        }

        private void SRMAddGotFocus(object sender, RoutedEventArgs e)
        {
            SRMAddTextBox.Text = "";
        }

        private async void SRMEntryCheck(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && (sender as TextBox).Text != "")
            
            {
                Add_Tapped_1(null, null);
                
            }

        }

        

        private void changeTheme(object sender, TappedRoutedEventArgs e)
        {
            var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;

            string currentTheme = null;
            if (settings.Values.ContainsKey("currentTheme"))
            {
                currentTheme = settings.Values["currentTheme"] as string;
                settings.Values.Remove("currentTheme");
            }
            
            settings.Values.Add("currentTheme", currentTheme == "light" ? "dark" : "light");
            var dia = new Windows.UI.Popups.MessageDialog("The change will be effective upon restart of Reddtro.");
            dia.ShowAsync();
        }

        private async void Add_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            if (SRMAddTextBox.Text == "Home Page")
            {
                await Data.SampleDataSource.AddHomePage();
                SRMAddTextBox.Text = "";
                Data.SubredditManager._subredditManager.WriteSubreddits();

            }
            else
            {
                try
                {
                    await Data.SampleDataSource.AddPage("http://www.reddit.com/r/" + SRMAddTextBox.Text + ".json");
                    SRMAddTextBox.Text = "";
                    Data.SubredditManager._subredditManager.WriteSubreddits();
                }
                catch (UnauthorizedAccessException ex)
                {
                    var dia = new Windows.UI.Popups.MessageDialog("Sorry, the subreddit you entered doesn't appear to exist or is private.  Please check for typos and try again");
                    dia.ShowAsync();
                }
            }
            
        }        

    }
}
