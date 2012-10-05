using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Callisto.Controls;
using ReddtroBeta.Data;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace ReddtroBeta
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ImageFlipViewPage : ReddtroBeta.Common.LayoutAwarePage
    {
        
        public ImageFlipViewPage()
        {
            this.InitializeComponent();
            RegisterForShare();   
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            base.OnNavigatedTo(e);
            
        }
        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            //UnregisterForShare();           

            base.GoBack(sender, e);
            
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            UnregisterForShare();           
            base.OnNavigatingFrom(e);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            
            base.OnNavigatedFrom(e);
        }
        private void RegisterForShare()
        {
            try
            {
                DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
                dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(ShareTextHandler); ;
            }
            catch (InvalidOperationException e)
            {
                e.GetType();
            }
        }
        private void UnregisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(ShareTextHandler);
        }

        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            if (flipView.SelectedItem != null)
            {
                SampleDataItem sditem = flipView.SelectedItem as SampleDataItem;
                DataRequest request = e.Request;
                request.Data.Properties.Description = sditem.Title;
                request.Data.Properties.Title = "Share this link:";
                request.Data.SetUri(new Uri(sditem.Url));
                //request.Data.SetText("Hello World!");
            }
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
        
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            //var group = ((Data.SampleDataItem)navigationParameter).Group; //refine to only images
            //var filtereditems = group.Items.Where(w => Data.SampleDataItem.isFVImageSource(w.Url) && w.FvImage != null);
            var ids = (navigationParameter as string).Split(',');
            var group = Data.SampleDataSource.GetGroup(ids[0]);
            var item = Data.SampleDataSource.GetItem(ids[1]);

            this.DefaultViewModel["Group"] = group;
            this.DefaultViewModel["Items"] = group.Images;
            //itemsViewSource
            flipView.SelectedItem = item;

            // Allow saved page state to override the initial item to display
            if (pageState != null && pageState.ContainsKey("SelectedItem"))
            {
                navigationParameter = pageState["SelectedItem"];
            }

            // TODO: Assign a bindable group to this.DefaultViewModel["Group"]
            // TODO: Assign a collection of bindable items to this.DefaultViewModel["Items"]
            // TODO: Assign the selected item to this.flipView.SelectedItem
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            var selectedItem = this.flipView.SelectedItem;
            // TODO: Derive a serializable navigation parameter and assign it to pageState["SelectedItem"]
        }

       
        private async void FVItemLoader(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                if (e.AddedItems[0] == (sender as FlipView).Items.Last())
                {
                    var x = (sender as FlipView).Items.Count;
                    await Data.SampleDataSource.ExtendPageAsync((e.AddedItems[0] as Data.SampleDataItem).Group);

                    //this.DefaultViewModel["Items"] = ((e.AddedItems[0] as Data.SampleDataItem).Group).FvImages;
                    flipView.SelectedItem = null;
                    flipView.SelectedItem = (e.AddedItems[0]);                    
                    
                }
            }
        }

        private void imageTapped(object sender, TappedRoutedEventArgs e)
        {
            if ((((sender as Image).Parent as Grid).Children[1] as Border).Child.Visibility != Windows.UI.Xaml.Visibility.Visible)
                (((sender as Image).Parent as Grid).Children[1] as Border).Child.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else
                (((sender as Image).Parent as Grid).Children[1] as Border).Child.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //Data.SampleDataSource.Comment.showTitle = !Data.SampleDataSource.Comment.showTitle;
           
        }

        
        private void Image_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            //make flipview fullscreen
            //flipView.Margin = new Thickness(0, -100, 0, 0);

            if (titleGrid.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
            {
                pageGrid.RowDefinitions[0].Height = new GridLength(140);
                titleGrid.Visibility = Visibility.Visible;
            }
            else
            {
                pageGrid.RowDefinitions[0].Height = Windows.UI.Xaml.GridLength.Auto;
                titleGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            
            
        }

        private void titleLoad(object sender, RoutedEventArgs e)
        {
            //if (Data.SampleDataSource.Comment.showTitle == true)
                //(sender as TextBlock).Visibility = Windows.UI.Xaml.Visibility.Visible;
            //else
              //  (sender as TextBlock).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //Data.SampleDataSource.Comment.showTitle = !Data.SampleDataSource.Comment.showTitle;
            
        }        

        private async void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            
            SavingTextBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
            SaveAsImageInPicturesLibrary(flipView.SelectedItem as Data.SampleDataItem);
            
                                   
            
        }
        private async void SaveAsImageInPicturesLibrary(Data.SampleDataItem sditem)
        {
            if (sditem != null)
            {
                var client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, sditem.Url);
                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                var filename = sditem.Url.Substring(sditem.Url.LastIndexOf('/') + 1);
                Task<StorageFile> task = GetFileIfExistsAsync(KnownFolders.PicturesLibrary, filename);
                StorageFile file = await task;

                if (file == null)
                {
                    var imageFile = await KnownFolders.PicturesLibrary.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                    var fs = await imageFile.OpenAsync(FileAccessMode.ReadWrite);
                    var writer = new DataWriter(fs.GetOutputStreamAt(0));
                    writer.WriteBytes(await response.Content.ReadAsByteArrayAsync());
                    await writer.StoreAsync();
                    writer.DetachStream();
                    await fs.FlushAsync();

                    //var dialog = new MessageDialog("The image is successfully saved in \"Pictures Library\".","Saving Image");
                    //await dialog.ShowAsync();
                    SavingTextBox.Text = "Saved";
                    bottomAppBar.IsOpen = false;
                    await Task.Delay(3000);
                    //SavingStoryboard.Begin();
                }
                else
                {
                    var dialog = new MessageDialog("The image is already saved in \"Pictures Library\".",
                        "Saving Image");
                    await dialog.ShowAsync();
                }
            }
            
            SavingTextBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            SavingTextBox.Text = "Saving...";
            bottomAppBar.IsOpen = false;

        }

        public static async Task<StorageFile> GetFileIfExistsAsync(StorageFolder folder, string fileName)
        {
            try
            {
                return await folder.GetFileAsync(fileName);
            }
            catch
            {
                return null;
            }
        }

        private void Open_Button_Click(object sender, RoutedEventArgs e)
        {
            Flyout f = new Flyout();

            f.Placement = PlacementMode.Top;
            f.PlacementTarget = sender as UIElement; // this is an UI element (usually the sender)

            Menu m = new Menu();

            MenuItem mi1 = new MenuItem();
            mi1.Text = "Link";

            mi1.Tapped += async (s, a) =>
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri((flipView.SelectedItem as SampleDataItem).Url));
            };

            MenuItem mi2 = new MenuItem();
            mi2.Text = "Comments";

            mi2.Tapped += async (s, a) =>
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.reddit.com" + (flipView.SelectedItem as SampleDataItem).Permalink));
            };

            m.Items.Add(mi1);
            m.Items.Add(new MenuItemSeparator());
            m.Items.Add(mi2);

            f.Content = m;

            f.IsOpen = true;
        }

        
    }
}
