using ReddtroBeta.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI.Xaml.Documents;

// The Split App template is documented at http://go.microsoft.com/fwlink/?LinkId=234228

namespace ReddtroBeta
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;

            if (!settings.Values.ContainsKey("currentTheme"))
                RequestedTheme = ApplicationTheme.Dark;
            else if (settings.Values.ContainsKey("currentTheme") && (string)settings.Values["currentTheme"] == "light")
                RequestedTheme = ApplicationTheme.Light;
            else
                RequestedTheme = ApplicationTheme.Dark; 
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            // Create a Frame to act as the navigation context and associate it with
            // a SuspensionManager key
            var rootFrame = new CharmFlyoutLibrary.CharmFrame { CharmContent = new SettingsFlyout() };
            SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

            //if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            //{
            //    // Restore the saved session state only when appropriate
            //    await SuspensionManager.RestoreAsync();
            //}
            //i don't want/need this

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(ItemsPage), "AllGroups"))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();

            //log in 
            
            

        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }
        
    }
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
    public sealed class DepthToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {           
            return new Thickness((int)value, 0,0,0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
    public sealed class opColoringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? "CornflowerBlue" :"ApplicationForegroundThemeBrush";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
    public sealed class depthFixer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            return new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException();
        }
    }
    public class commentListViewItemStyleSelector : StyleSelector
    {
        protected override Style SelectStyleCore(object item, DependencyObject container) //orverride?
        {
            Style st = new Style(typeof(ListViewItem));
            //st.TargetType = typeof(ListViewItem);

            Setter sttr = new Setter();
            sttr.Property = ListViewItem.MaxHeightProperty;            
            
            ListView listView = ItemsControl.ItemsControlFromItemContainer(container) as ListView;
            int index = listView.ItemContainerGenerator.IndexFromContainer(container);
            bool vis = (listView.Items[index] as Data.SampleDataSource.Comment).IsVisible;
            if (!vis)
            {
                sttr.Value = 10;
                st.Setters.Add(sttr);
            }
            return st;
         
        }
    }
    public class ListViewItemStyleSelector : StyleSelector
    {
        protected override Style SelectStyleCore(object item,
            DependencyObject container)
        {
            Style st = new Style();
            st.TargetType = typeof(ListViewItem);
            Setter backGroundSetter = new Setter();
            Setter sttr = new Setter();
            backGroundSetter.Property = ListViewItem.BackgroundProperty;
            sttr.Property = ListViewItem.MaxHeightProperty;
            ListView listView =
                ItemsControl.ItemsControlFromItemContainer(container)
                  as ListView;
            int index =
                listView.ItemContainerGenerator.IndexFromContainer(container);
            if (index % 2 == 0)
            {
                backGroundSetter.Value = "Green";
                sttr.Value = 50;
            }
            else
            {
                backGroundSetter.Value = "Red";
                sttr.Value = 25;
            }
            st.Setters.Add(backGroundSetter);
            st.Setters.Add(sttr);
            return st;
        }
    }
    public class AuthorStyleSelector : StyleSelector
    {
        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            Style st = (Style)Application.Current.Resources["TitleTextStyle"];

            Setter sttr = new Setter(TextBlock.ForegroundProperty, "CornflowerBlue");
            st.Setters.Add(sttr);
            return st;
        }
    }

    public class MarkdownParser : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //Create a new RichTextBox.
            RichTextBlock MyRTB = new RichTextBlock();
            
            // Create a Run of plain text and hyperlink.
            Run myRun = new Run();
            myRun.Text = " are enabled in a read-only RichTextBox.";
            InlineUIContainer mylinkcont = new InlineUIContainer();
            HyperlinkButton MyLink = new HyperlinkButton();
            MyLink.Content = "Hyperlink";
            MyLink.NavigateUri = new Uri("http://www.msdn.com");
            
            mylinkcont.Child = MyLink;
            // Create a paragraph and add the Run and hyperlink to it.
            Paragraph myParagraph = new Paragraph();
            myParagraph.Inlines.Add(mylinkcont);
            myParagraph.Inlines.Add(myRun);

            // Add the paragraph to the RichTextBox.
            MyRTB.Blocks.Add(myParagraph);

            return MyRTB;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)       
        {
            return new NotImplementedException();
        }            
    }

}
