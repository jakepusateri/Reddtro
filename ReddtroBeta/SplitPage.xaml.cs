using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Callisto.Controls;
using ReddtroBeta.Data;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace ReddtroBeta
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for the
    /// currently selected item.
    /// </summary>
    public sealed partial class SplitPage : ReddtroBeta.Common.LayoutAwarePage
    {
        public SplitPage()
        {
            this.InitializeComponent();
            RegisterForShare();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {            
            base.OnNavigatedTo(e);
                           
        }
        
        #region Page state management

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
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var group = SampleDataSource.GetGroup((String)navigationParameter);
            this.DefaultViewModel["Group"] = group;            
            this.DefaultViewModel["Items"] = group.Items;

            if (pageState == null)
            {
                // When this is a new page, select the first item automatically unless logical page
                // navigation is being used (see the logical page navigation #region below.)
                if (!this.UsingLogicalPageNavigation() && this.itemsViewSource.View != null)
                {
                    this.itemsViewSource.View.MoveCurrentToFirst();
                }
            }
            else
            {
                // Restore the previously saved state associated with this page
                if (pageState.ContainsKey("SelectedItem") && this.itemsViewSource.View != null)
                {
                    var selectedItem = SampleDataSource.GetItem((String)pageState["SelectedItem"]);
                    this.itemsViewSource.View.MoveCurrentTo(selectedItem);
                }
            }
            
        }
        //private TypedEventHandler<DataTransferManager, DataRequestedEventArgs> shareHandler = new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(ShareTextHandler);                        
        private void RegisterForShare()
        {            
                DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
                dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(ShareTextHandler); ;
        }
        private void UnregisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(ShareTextHandler);                        
        }

        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            if (itemListView.SelectedItem != null)
            {                
                SampleDataItem sditem = itemListView.SelectedItem as SampleDataItem;
                DataRequest request = e.Request;
                request.Data.Properties.Description = sditem.Title;
                request.Data.Properties.Title = "Share this link:";
                request.Data.SetUri(new Uri(sditem.Url));
                //request.Data.SetText("Hello World!");
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            if (this.itemsViewSource.View != null)
            {
                var selectedItem = (SampleDataItem)this.itemsViewSource.View.CurrentItem;
                if (selectedItem != null) pageState["SelectedItem"] = selectedItem.UniqueId;
            }
        }

        #endregion

        #region Logical page navigation

        // Visual state management typically reflects the four application view states directly
        // (full screen landscape and portrait plus snapped and filled views.)  The split page is
        // designed so that the snapped and portrait view states each have two distinct sub-states:
        // either the item list or the details are displayed, but not both at the same time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        /// <summary>
        /// Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed, or null
        /// for the current view state.  This parameter is optional with null as the default
        /// value.</param>
        /// <returns>True when the view state in question is portrait or snapped, false
        /// otherwise.</returns>
        private bool UsingLogicalPageNavigation(ApplicationViewState? viewState = null)
        {
            if (viewState == null) viewState = ApplicationView.Value;
            return viewState == ApplicationViewState.FullScreenPortrait ||
                viewState == ApplicationViewState.Snapped;
        }
        
        /// <summary>
        /// Invoked when an item within the list is selected.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is Snapped)
        /// displaying the selected item.</param>
        /// <param name="e">Event data that describes how the selection was changed.</param>
        /// 

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);
            if (e.OriginalSource != null && e.OriginalSource.GetType().Equals(typeof(Button)))
            {
                SampleDataItem s = itemListView.SelectedItem as SampleDataItem;
                string temp = s.Content;
                s.Content = "";
                itemListView.SelectedIndex = -1;
                itemListView.SelectedItem = s;
                s.Content = temp;            
            }
            
        }
        async void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Invalidate the view state when logical page navigation is in effect, as a change
            // in selection may cause a corresponding change in the current logical page.  When
            // an item is selected this has the effect of changing from displaying the item list
            // to showing the selected item's details.  When the selection is cleared this has the
            // opposite effect.
            
            if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();

            //scroll to top, not inherting a previous comments scroll depth
            itemDetail.ScrollToVerticalOffset(0); 
            
            // If there's a selected item (in AddedItems)
            // show it in the WebView.
            if (e.AddedItems.Count > 0)
            {
                SampleDataItem feedItem = e.AddedItems[0] as SampleDataItem;
                if (feedItem != null && feedItem.Url != "")
                {
                    
                    // Navigate the WebView to the blog post content HTML string.
                    //contentView.NavigateToString("");
                    contentView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    contentViewGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    WebViewGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    //if (e.OriginalSource != null && e.OriginalSource.GetType().Equals(new Button().GetType())) ;
                        
                    if (feedItem.Content == "") //it's a link, load webview
                    {                                                
                        contentView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        contentViewGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        WebViewGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        
                        contentViewRect.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        CommentListView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

                        contentView.Navigate(new Uri(feedItem.Url));
                        itemListView.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    }
                    else
                    {
                        WebViewGrid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;                        
                        //commentLoadingRing.IsActive = true;
                        //commentLoadingStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;

                        CommentListView.Visibility = Windows.UI.Xaml.Visibility.Visible;

                        //Task.Run(() => loadComments(commentLoadingStackPanel, commentLoadingRing, feedItem)).RunSynchronously();
                        await loadComments(commentLoadingStackPanel, commentLoadingRing, feedItem).ConfigureAwait(continueOnCapturedContext:false);
                        
                        //itemListView.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    }
                    
                }
            }
            else
            {
                // If the item was de-selected, clear the WebView.
                contentView.NavigateToString("");                
            }            
        }
        private async Task loadComments(StackPanel sp, ProgressRing pr, SampleDataItem feedItem)
        {

            if (feedItem.aComments.Count == 0)
            {
                sp.Visibility = Windows.UI.Xaml.Visibility.Visible;
                pr.IsActive = true;

                await SampleDataSource.ParseCommentsAsync(feedItem);
                //await SampleDataSource.ParseComments(feedItem);

                sp.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                pr.IsActive = false;

                
                //feedItem.aComments.Clear();
                if (feedItem.aComments.Count == 0)
                {
                    await SampleDataSource.AddChildren(feedItem, feedItem.Comments);
                    //feedItem.Comments.Clear();
                }

            }
        }
        public T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject v = (DependencyObject)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                    child = GetVisualChild<T>(v);
                if (child != null)
                    break;
            }
            return child;
        }

        /// <summary>
        /// Invoked when the page's back button is pressed.
        /// </summary>
        /// <param name="sender">The back button instance.</param>
        /// <param name="e">Event data that describes how the back button was clicked.</param>
        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
            {
                // When logical page navigation is in effect and there's a selected item that
                // item's details are currently displayed.  Clearing the selection will return
                // to the item list.  From the user's point of view this is a logical backward
                // navigation.
                this.itemListView.SelectedItem = null;
            }
            else
            {
                // When logical page navigation is not in effect, or when there is no selected
                // item, use the default back button behavior.


                (this.itemListView.DataContext as SampleDataGroup).ClearComments();
                base.GoBack(sender, e);
            }
            UnregisterForShare();
        }

        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// view state.
        /// </summary>
        /// <param name="viewState">The view state for which the question is being posed.</param>
        /// <returns>The name of the desired visual state.  This is the same as the name of the
        /// view state except when there is a selected item in portrait and snapped views where
        /// this additional logical page is represented by adding a suffix of _Detail.</returns>
        protected override string DetermineVisualState(ApplicationViewState viewState)
        {
            // Update the back button's enabled state when the view state changes
            var logicalPageBack = this.UsingLogicalPageNavigation(viewState) && this.itemListView.SelectedItem != null;
            var physicalPageBack = this.Frame != null && this.Frame.CanGoBack;
            this.DefaultViewModel["CanGoBack"] = logicalPageBack || physicalPageBack;

            // Determine visual states for landscape layouts based not on the view state, but
            // on the width of the window.  This page has one layout that is appropriate for
            // 1366 virtual pixels or wider, and another for narrower displays or when a snapped
            // application reduces the horizontal space available to less than 1366.
            if (viewState == ApplicationViewState.Filled ||
                viewState == ApplicationViewState.FullScreenLandscape)
            {
                var windowWidth = Window.Current.Bounds.Width;
                if (windowWidth >= 1366) return "FullScreenLandscapeOrWide";
                return "FilledOrNarrow";
            }

            // When in portrait or snapped start with the default visual state name, then add a
            // suffix when viewing details instead of the list
            var defaultStateName = base.DetermineVisualState(viewState);
            return logicalPageBack ? defaultStateName + "_Detail" : defaultStateName;
        }

        #endregion
        
        private void AppBar_Opened(object sender, object e)
        {
            if (WebViewGrid.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                WebViewBrush wvb = new WebViewBrush();
                wvb.SourceName = "contentView";
                contentViewRect.Visibility = Windows.UI.Xaml.Visibility.Visible;
                contentViewRect.Fill = wvb;
                wvb.Redraw();
                contentView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void AppBar_Closed(object sender, object e)
        {
            if (itemListView.SelectedItem != null)
            {
                if ((itemListView.SelectedItem as SampleDataItem).Content == "")
                    contentView.Visibility = Windows.UI.Xaml.Visibility.Visible;

                contentViewRect.Fill = new SolidColorBrush(Windows.UI.Colors.Transparent);
                //CommentListView.Focus(Windows.UI.Xaml.FocusState.Programmatic);    
                //itemListView.Focus(Windows.UI.Xaml.FocusState.Programmatic);
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
                    await Windows.System.Launcher.LaunchUriAsync(new Uri((itemListView.SelectedItem as SampleDataItem).Url));
                };

            MenuItem mi2 = new MenuItem();
            mi2.Text = "Comments";

            mi2.Tapped += async (s, a) =>
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.reddit.com" + (itemListView.SelectedItem as SampleDataItem).Permalink));           
                };

            m.Items.Add(mi1);
            m.Items.Add(new MenuItemSeparator());
            m.Items.Add(mi2);

            f.Content = m;

            f.IsOpen = true;
            
            
        }    
        
        private void Comment_Button_Click(object sender, RoutedEventArgs e)
        {
            SampleDataItem feedItem = null;
            if (itemListView.SelectedItem != null)
            {
                feedItem = itemListView.SelectedItem as SampleDataItem;
                
                var tempswap = feedItem.Content;
                if(tempswap == String.Empty)
                    feedItem.Content = " ";
                itemListView.SelectedItem = null;
                itemListView.SelectedItem = feedItem;
                feedItem.Content = tempswap;
                
            }
            bottomAppBar.IsOpen = false;
            //CommentListView.ScrollIntoView(CommentListView.Items[0]);

        }

        private void commentselectionchanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {

                var com = e.AddedItems[0] as SampleDataSource.Comment;
                int z = CommentListView.SelectedIndex;
                int x = z+1;
                if (com.IsVisible)
                {
                    //com.IsVisible = !com.IsVisible;
                    
                    int y = (CommentListView.SelectedItem as SampleDataSource.Comment).Depth;
                    ObservableCollection<SampleDataSource.Comment> rem = new ObservableCollection<SampleDataSource.Comment>();
                    
                    while (x < CommentListView.Items.Count && (CommentListView.Items[x] as SampleDataSource.Comment).Depth > y)
                    {                                                
                        rem.Add(CommentListView.Items[x] as SampleDataSource.Comment);
                        (itemDetailGrid.DataContext as SampleDataItem).aComments.RemoveAt(x);                        
                    }
                    (CommentListView.SelectedItem as SampleDataSource.Comment).HiddenChildren = rem;
                    //CommentListView.MakeVisible(CommentListView.SelectedItem as SemanticZoomLocation);
                    CommentListView.SelectedItem = null;
                }
                else
                {                    
                    ObservableCollection<SampleDataSource.Comment> rem = (CommentListView.SelectedItem as SampleDataSource.Comment).HiddenChildren;
                    
                    if (rem != null)
                    {
                                        
                        foreach (var item in rem.Reverse<SampleDataSource.Comment>())
                        {
                            (itemDetailGrid.DataContext as SampleDataItem).aComments.Insert(x,item);
                        }                
                    }                
                }                
                (CommentListView.Items[z] as SampleDataSource.Comment).IsVisible = !(CommentListView.Items[z] as SampleDataSource.Comment).IsVisible;
                CommentListView.SelectedItem = null;
                
                
                
                //pageRoot.Focus(Windows.UI.Xaml.FocusState.Pointer);
            }
        }

        private void Content_Button_Click(object sender, RoutedEventArgs e)
        {
            SampleDataItem feedItem = null;
            if (itemListView.SelectedItem != null)
            {
                feedItem = itemListView.SelectedItem as SampleDataItem;
                var swaptemp = feedItem.Content;
                feedItem.Content = "";
                itemListView.SelectedItem = null;
                itemListView.SelectedItem = feedItem;
                feedItem.Content = swaptemp;

            }
            bottomAppBar.IsOpen = false;

        }

        private void Refresh_Button_Click(object sender, RoutedEventArgs e)
        {
            SampleDataItem feedItem = null;
            if (itemListView.SelectedItem != null)
            {
                feedItem = itemListView.SelectedItem as SampleDataItem;
                
                itemListView.SelectedItem = null;
                itemListView.SelectedItem = feedItem;                
            }
            bottomAppBar.IsOpen = false;
        }

        private void HeaderImageClick(object sender, TappedRoutedEventArgs e)
        {
            var groupId = ((e.OriginalSource as Image).DataContext as SampleDataItem).Group.UniqueId;
            var itemId = ((e.OriginalSource as Image).DataContext as SampleDataItem).UniqueId;
            var comb = groupId + "," + itemId;
            UnregisterForShare();
            this.Frame.Navigate(typeof(ImageFlipViewPage), comb);
            
        }


        

        private void RichSelfText_Loaded(object sender, RoutedEventArgs e)
        {
    
            
        }        

        private void pageTitle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var groupId= ((e.OriginalSource as TextBlock).DataContext as SampleDataGroup).UniqueId;
            var itemId= ((e.OriginalSource as TextBlock).DataContext as SampleDataGroup).Items[0].UniqueId;
            var comb = groupId + "," + itemId;
            UnregisterForShare();
            this.Frame.Navigate(typeof(ImageFlipViewPage), comb);
            
            //this.Frame.Navigate(typeof(ImageFlipViewPage), groupId);
        }

        private void commentButtonpress(object sender, TappedRoutedEventArgs e)
        {
            sender.ToString();
        }        
        

        private async void More_Button_Click(object sender, RoutedEventArgs e)
        {
            if (itemListView.SelectedItem != null)
            {
                var feedItem = itemListView.SelectedItem as SampleDataItem;
                await Data.SampleDataSource.ExtendPageAsync(feedItem.Group);

            }
        }

        private async void loadMore(object sender, object e)
        {
            //ScrollViewer sv = GetVisualChild<ScrollViewer>(itemListView);
            //if (sv != null)
            //{
            //    if (sv.VerticalOffset == sv.ScrollableHeight)
            //    {
            //        //await Data.SampleDataSource.ExtendPageAsync(itemListView.DataContext as SampleDataGroup);
            //        //loading indicators?
            //    }
            //}
        }

        

        

        
        

    }
}
