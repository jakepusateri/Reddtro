using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReddtroBeta.Data;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace ReddtroBeta
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class ItemsPage : ReddtroBeta.Common.LayoutAwarePage
    {
        public ItemsPage()
        {
            this.InitializeComponent();
            
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            
            //var sds = new SampleDataSource();
            if (pageState == null)
            {
                mainpageLoadingRing.IsActive = true;
                mainpageLoadingPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    
                //if (SampleDataSource._sampleDataSource.AllGroups.Count == 0)
                //{
                    try
                    {
                        Data.SubredditManager._subredditManager.ReadSRFromFileAsync(mainpageLoadingRing,mainpageLoadingPanel); 
                        //this is on purpose. subreddit should pop up as they are created.
                        //the method is passed the elements to disable upon its' completed
                    }
                    catch (System.Net.Http.HttpRequestException e)
                    {
                        var dia = new Windows.UI.Popups.MessageDialog("Your computer and Reddit are having connection issues. Check internet connectivity and try again.");
                        dia.ShowAsync();
                    }
                //}                                        
            }

            var sampleDataGroups = SampleDataSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Items"] = sampleDataGroups;
        }

        /// <summary>
        /// Invoked when an item is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(SplitPage), groupId);
        }

        private async void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            mainpageLoadingRing.IsActive = true;
            mainpageLoadingPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            SampleDataSource._sampleDataSource.AllGroups.Clear();
            if (SampleDataSource._sampleDataSource.AllGroups.Count == 0)
            {
                try
                {
                    Data.SubredditManager._subredditManager.ReadSRFromFileAsync(mainpageLoadingRing, mainpageLoadingPanel);
                    //this is on purpose. subreddit should pop up as they are created.
                    //the method is passed the elements to disable upon its' completed
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    var dia = new Windows.UI.Popups.MessageDialog("Your computer and Reddit are having connection issues. Check internet connectivity and try again.");
                    dia.ShowAsync();
                }
            }
            this.bottomAppBar.IsOpen = false;
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
        //    if (e.AddedItems != null && e.RemovedItems.Count == 0)
        //    {
        //        Data.SampleDataSource._sampleDataSource.AllGroups.RemoveAt(srmanagerlist.SelectedIndex);
        //        Data.SubredditManager._subredditManager.WriteSubreddits();
        //    }
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
