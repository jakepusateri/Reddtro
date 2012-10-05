using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;


//using System.Windows.Data;


// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace ReddtroBeta.Data
{
    
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : ReddtroBeta.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group, String permalink, String url, int score, string subreddit)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
            this._permalink = permalink;
            this._url = url;
            this._score = score;
            this._subreddit = subreddit;
            
        }
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group, String permalink, String url, int score, string subreddit, DateTime date, int numcomments)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
            this._permalink = permalink;
            this._url = url;
            this._score = score;
            this._subreddit = subreddit;
            this._created = date;
            this._numcomments = numcomments;

        }
        private ObservableCollection<SampleDataSource.Comment> comments;
        public ObservableCollection<SampleDataSource.Comment> Comments
        {
            get { return this.comments; }
            set { this.SetProperty(ref this.comments, value); }
        }
        private ObservableCollection<SampleDataSource.Comment> acomments = new ObservableCollection<SampleDataSource.Comment>();
        public ObservableCollection<SampleDataSource.Comment> aComments
        {
            get { return this.acomments; }
            set { this.SetProperty(ref this.acomments, value); }
        }
        private string _permalink;
        public string Permalink
        {
            get { return this._permalink; }
            set { this.SetProperty(ref this._permalink, value); }
        }
        private string _url = string.Empty;
        public string Url
        {
            get { return this._url; }
            set { this.SetProperty(ref this._url, value); }
        }
        private bool _needImg = true;
        public bool NeedImg
        {
            get { return this._needImg; }
            set { this.SetProperty(ref this._needImg, value); }
        }
        public string gUrl(object o)
        {
            return this._url;
        }
        private int _score = 0;
        public int Score
        {
            get { return this._score; }
            set { this.SetProperty(ref this._score, value); }
        }
        private int _numcomments= 0;
        public int Numcomments
        {
            get { return this._numcomments; }
            set { this.SetProperty(ref this._numcomments, value); }
        }
        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }
        private DateTime _created;
        public DateTime Created
        {
            get { return this._created; }
            set { this.SetProperty(ref this._created, value); }
        }
        
        
        private string _subreddit;
        public string Subreddit
        {
            get { return this._subreddit; }
            set { this.SetProperty(ref this._subreddit, value); }
        }

        private ImageSource fv_image = null;
        private String fv_imagePath = null;
        public ImageSource FvImage
        {
            get
            {
                
                if (this.fv_image == null && this.Url != null)
                {
                    if (isFVImageSource(Url))
                    {
                        if (Url.Contains("imgur.com") && !Url.Contains(".jpg") && !Url.Contains("imgur.com/a/"))
                            Url += ".jpg";
                        this.fv_image = new BitmapImage(new Uri(Url));
                    }
                }
                return this.fv_image;
            }

            set
            {
                this.fv_imagePath = null;
                this.SetProperty(ref this.fv_image, value);
            }
        }
        public static bool isFVImageSource(string url)
        {
            bool ans = false;
            string extension = url.Substring(url.LastIndexOf('.'));
            //List<string> validextensions = new List<string>();
            //validextensions.Add("jpg");
            //validextensions.Add("png");                            
            if (extension == ".png" || extension == ".jpg")
                ans = true;
            if (url.Contains("imgur.com") && !url.Contains(".jpg") && !url.Contains("imgur.com/a/"))
                ans = true;
            return ans;
        }
        public void fvSetImage(String path)
        {
            this.fv_image = null;
            this.fv_imagePath = path;
            this.OnPropertyChanged("Image");
        }

        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

  

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            _items = new ObservableCollection<SampleDataItem>();
        }

        private ObservableCollection<SampleDataItem> _items;
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }
        
        public IEnumerable<SampleDataItem> TopItems
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed
            get { return this._items.Take(12); }
        }
        private ObservableCollection<SampleDataItem> _Images = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Images { get { return this._Images; } }

        public IEnumerable<SampleDataItem> GetImages 
        {
            get { return Items.Where(w => Data.SampleDataItem.isFVImageSource(w.Url) && w.FvImage != null);}
            //(ObservableCollection<SampleDataItem>)
        }

        public void ClearComments()
        {
            foreach (var x in _items)
            {
                x.aComments.Clear();
                if(x.Comments != null)
                    x.Comments.Clear();
            }
        }
      

    }
    public class SubredditManager
    {
        public static SubredditManager _subredditManager = new SubredditManager();
        private Windows.Storage.StorageFolder roamingFolder;
        //private Windows.Storage.StorageFile subredditlistfile;
        
        //public ObservableCollection<SampleDataItem> SubscribedSubreddits { get; set; }
        
        public SubredditManager()
        {
            Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            this.roamingFolder = Windows.Storage.ApplicationData.Current.RoamingFolder;
            
            //this.SubscribedSubreddits = new ObservableCollection<SampleDataItem>();
        }
        public async Task ReadSRFromFileAsync(ProgressRing pr, StackPanel sp)
        {
            //subredditlistfile = await roamingFolder.CreateFileAsync("subreddits.txt", Windows.Storage.CreationCollisionOption.OpenIfExists);
            var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;    
            string subs = string.Empty; 

            if (settings.Values.ContainsKey("localSubreddits"))
            {
                //settings.Values["localSubreddits"] = "";
                //settings.Values.Remove("localSubreddits");
                subs = (string)settings.Values["localSubreddits"];
            }
            else
            {
                settings.Values.Add("localSubreddits", "http://www.reddit.com/.json,http://www.reddit.com/r/Reddtro.json,http://www.reddit.com/r/wallpapers.json,http://www.reddit.com/r/Iama+askreddit.json,http://www.reddit.com/r/all.json");
                //subs = "reddtro,wallpapers,windows8";
                subs = "http://www.reddit.com/.json,http://www.reddit.com/r/Reddtro.json,http://www.reddit.com/r/wallpapers.json,http://www.reddit.com/r/Iama+askreddit.json,http://www.reddit.com/r/all.json";
                
            }
                
            string[] srs = subs.Split(',');
            

            foreach (var item in srs)
            {
                //await SampleDataSource.AddPage("http://www.reddit.com/r/"+item +".json");
                if (item == "http://www.reddit.com/.json")
                    await SampleDataSource.AddHomePage();
                else
                    await SampleDataSource.AddPage(item);
            }

            //login and fetch/add account-specific subreddits 

            //await Data.SampleDataSource.accountManager.Login("juanpabl0", "onfire69");

            //JsonObject ps = await SampleDataSource.accountManager.makeJsonPageRequest("http://www.reddit.com/reddits/mine.json");
            //JsonArray ch = ps.GetNamedObject("data").GetNamedArray("children");
            //foreach (JsonObject item in ch)
            //{
            //    string baseUrl = "http://www.reddit.com";
            //    string sr = item.GetNamedObject("data").GetNamedString("url");
            //    string url = baseUrl + sr;
            //    await SampleDataSource.AddPage(url);
            //}

            pr.IsActive = false;
            sp.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            
        }
        public void WriteSubreddits()
        {
            List<string> srnames = new List<string>();
            foreach (var x in Data.SampleDataSource._sampleDataSource.AllGroups)
            {
                srnames.Add(x.UniqueId);
            }
            //srnames.Add("nsfw");
            var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (settings.Values.ContainsKey("localSubreddits"))
            {
                settings.Values["localSubreddits"] = string.Join(",", srnames); 
            }
            else
            {
                settings.Values.Add("localSubreddits", "reddtro,windows8,all,buildapc");//not reachable?
            }

            //await Windows.Storage.FileIO.WriteLinesAsync(subredditlistfile, srnames);
        }
        //write methods to write list to file
        //method to add/remove
    }
    public class Subreddit
    {
        public Subreddit(string name)
        {
            this._name = name;
        }
        private string _name;
        public string Name { get { return this._name; } }
        public string toString()
        {
            return this._name;
            
        }
        
    }
    


    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// </summary>
    public sealed class SampleDataSource
    {
        public static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public class Comment : INotifyPropertyChanged
        {
            public static bool showTitle = true;
            public Comment()
            {

            }

            
            public Comment(ObservableCollection<Comment> children, string body, string author, int score, string parent, string id, int depth, DateTime date, string op)
            {
                this.children = children;
                this.body = body;
                this.author = author;
                this.score = score;
                this.parent = parent;
                this.id = id;
                this.depth = depth;
                this.isVisible = true;
                this.created = date;
                if (this.author == op)
                    Op = true;
                else                
                    Op = false;
                

                //this.HiddenChildren = new ObservableCollection<Comment>();
            }

            public ObservableCollection<Comment> children;
            private string body;
            public string Body
            {
                get
                {
                    return this.body;
                }
            }
            private string author;
            public string Author
            {
                get
                {
                    return this.author;
                }
            }
            private int score;
            public int Score
            {
                get { return this.score; }
            }
            private string parent;
            public string Parent { get { return this.parent; } }
            private DateTime created;
            public DateTime Created { get { return this.created; } }
            private string id;
            public string Id
            {
                get { return this.id; }
            }
            private int depth;
            public int Depth { get { return this.depth; } }
            private bool isVisible;
            public bool IsVisible
            {
                get { return this.isVisible; }
                set 
                { 
                    this.isVisible = value;
                    NotifyPropertyChanged("IsVisible");
                }
            }
            //private ObservableCollection<Comment> hiddenChildren;
            public ObservableCollection<Comment> HiddenChildren
            {
                get;
                set;
            }
            public Boolean Op { get; set; }

            public string toString()
            {
                return this.body;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(String info)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(info));
                }
            }


        }
        

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }
        public static SampleDataItem GetImage(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Images).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        //public static AccountManager accountManager = new AccountManager();
        public SampleDataSource()
        {                                
        }

        public class AccountManager
        {
            public class Account
            {
                public Account()
                {

                }
                public Account(string username, string cookie, string modhash)
                {
                    this._username = username;
                    this._cookie = cookie;
                    this._modhash = modhash;
                }
                private string _username;
                private string _modhash;
                private string _cookie;
            }
            public Account defaultAccount;
            public AccountManager()
            {
                _client = new HttpClient();
                _client.DefaultRequestHeaders.Add("user-agent", "Reddtro 1.0.1");                          
            }
            public async Task Login(string username, string password)
            {
                
                HttpResponseMessage login = await _client.PostAsync("http://www.reddit.com/api/login/" + username + "?user="+username + "&passwd=" + password + "&api_type=json" ,null);                
                
                string loginresponse = await login.Content.ReadAsStringAsync();
                JsonObject jo;
                JsonObject.TryParse(loginresponse, out jo);
                jo = jo.GetNamedObject("json");
                if (jo.GetNamedArray("errors").Count == 0)
                {
                    JsonObject data = jo.GetNamedObject("data");
                    string modhash = data.GetNamedString("modhash");
                    string cookie = data.GetNamedString("cookie");
                    defaultAccount = new Account(username, cookie, modhash);

                    

                    cookie = WebUtility.UrlEncode(cookie);
                    var cC = new CookieContainer();
                    cC.Add(new Uri("http://www.reddit.com"), new Cookie("userCookie", cookie));
                    cC.Add(new Uri("http://api.reddit.com"), new Cookie("apiCookie", cookie));
                    var handler = new HttpClientHandler() { CookieContainer = cC };
                    _client = new HttpClient(handler);
                    
                    
                }
                //get subreddits for logged-in user
                //http://www.reddit.com/reddits/mine.json
                
            }
            public List<Account> Accounts { get { return this._accounts; } }
            private List<Account> _accounts;
            private Account _currentAccount;
            public HttpClient Client { get { return this._client; } }
            private HttpClient _client;

            public async Task<JsonObject> makeJsonPageRequest(string url)
            {
                
                HttpResponseMessage response = await _client.GetAsync(url);
                string responseText = await response.Content.ReadAsStringAsync();
                JsonObject json = null;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    JsonObject.TryParse(responseText, out json);
                }
                else if (response.RequestMessage.RequestUri.AbsoluteUri == url && response.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    var dia = new Windows.UI.Popups.MessageDialog("It appears reddit.com is under heavy load. Please try again later.");
                    await dia.ShowAsync();
                }
                else if (response.ReasonPhrase == "Moved Temporarily" || response.StatusCode == HttpStatusCode.NotFound)
                {
                    var dia = new Windows.UI.Popups.MessageDialog("Sorry, the subreddit you entered doesn't appear to exist or is private.  Please check for typos and try again");
                    dia.ShowAsync();
                }

                return json;
            }

            public async Task<JsonArray> makeJsonCommentRequest(string url)
            {
                HttpClient client = SampleDataSource.accountManager.Client;
                HttpResponseMessage response = await client.GetAsync(url);
                string responseText = await response.Content.ReadAsStringAsync();
                //deal with 504
                JsonArray json = null;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {                    
                    JsonArray.TryParse(responseText, out json);
                }
                return json;
            }
            

        }

        public static AccountManager accountManager = new AccountManager();

        public static async Task AddHomePage()
        {

            string url = "http://www.reddit.com/.json";

            JsonObject json = await accountManager.makeJsonPageRequest(url);

            JsonObject pageinfo = json.GetNamedObject("data");
            JsonArray links = pageinfo.GetNamedArray("children");

            string sr = "Home Page";

            string srimg = "Assets/reddit_logo.png";
            SampleDataGroup LinkPage = new SampleDataGroup(url, sr, "", srimg, "/r/all");
            //add http request to grab subreddit link
            for (uint i = 0; i < links.Count; i++)
            {
                JsonObject Data = links.GetObjectAt(i).GetNamedObject("data");
                string title = Data.GetNamedString("title");
                string id = Data.GetNamedString("id");
                id = links.GetObjectAt(i).GetNamedString("kind") + "_" + id;
                string author = Data.GetNamedString("author");
                string thumbnail = Data.GetNamedString("thumbnail");
                string permalink = Data.GetNamedString("permalink");
                string selftext = Data.GetNamedString("selftext");
                string subreddit = Data.GetNamedString("subreddit");
                int numcomments = (int)Data.GetNamedNumber("num_comments");
                DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                double sec = Data.GetNamedNumber("created_utc");
                date = date.AddSeconds(sec);


                string linkurl = Data.GetNamedString("url");
                //string selftext = links.GetObjectAt(i).GetNamedObject("data").GetNamedString("permalink");
                int score = int.Parse(Data.GetNamedNumber("score").ToString());

                SampleDataItem ans = new SampleDataItem(id, title, author, thumbnail, permalink, selftext, LinkPage, permalink, linkurl, score, subreddit, date, numcomments);
                if (thumbnail == "" || selftext != "")
                {
                    ans.NeedImg = false;
                }

                // await ParseComments(ans);

                LinkPage.Items.Add(ans);
                if (SampleDataItem.isFVImageSource(ans.Url) && ans.FvImage != null)
                    LinkPage.Images.Add(ans);
            }
            SampleDataSource._sampleDataSource.AllGroups.Add(LinkPage);
        }


        public static async Task AddPage(string url)
        {

            JsonObject json = await accountManager.makeJsonPageRequest(url);
            if (json != null)
            {
                JsonObject pageinfo = json.GetNamedObject("data");
                JsonArray links = pageinfo.GetNamedArray("children");

                //string srt = links.GetObjectAt(0).GetNamedObject("data").GetNamedString("subreddit");
                string sr = url.Substring(24, url.Length - 29);
                //srimg = "http://thumbs.reddit.com/" + srt + ".png";

                string srimg = "Assets/reddit_logo.png";
                SampleDataGroup LinkPage = new SampleDataGroup(url, sr, "", srimg, "/r/all");
                //add http request to grab subreddit link
                for (uint i = 0; i < links.Count; i++)
                {
                    JsonObject Data = links.GetObjectAt(i).GetNamedObject("data");
                    string title = Data.GetNamedString("title");
                    string id = Data.GetNamedString("id");
                    id = links.GetObjectAt(i).GetNamedString("kind") + "_" + id;
                    string author = Data.GetNamedString("author");
                    string thumbnail = Data.GetNamedString("thumbnail");
                    string permalink = Data.GetNamedString("permalink");
                    string selftext = Data.GetNamedString("selftext");
                    string subreddit = Data.GetNamedString("subreddit");
                    int numcomments = (int)Data.GetNamedNumber("num_comments");
                    DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    double sec = Data.GetNamedNumber("created_utc");
                    date = date.AddSeconds(sec);


                    string linkurl = Data.GetNamedString("url");
                    //string selftext = links.GetObjectAt(i).GetNamedObject("data").GetNamedString("permalink");
                    int score = int.Parse(Data.GetNamedNumber("score").ToString());

                    SampleDataItem ans = new SampleDataItem(id, title, author, thumbnail, permalink, selftext, LinkPage, permalink, linkurl, score, subreddit, date, numcomments);
                    if (thumbnail == "" || selftext != "")
                    {
                        ans.NeedImg = false;
                    }

                    // await ParseComments(ans);

                    LinkPage.Items.Add(ans);
                    if (SampleDataItem.isFVImageSource(ans.Url) && ans.FvImage != null)
                        LinkPage.Images.Add(ans);
                }

                SampleDataSource._sampleDataSource.AllGroups.Add(LinkPage);
            }
            else
            {
                //show the user that the page wasn't valid
            }


        }
                                                
            //String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group
            //String uniqueId, String title, String subtitle, String imagePath, String description

        public static async Task ExtendPageAsync(SampleDataGroup sg)
        {
            string url = sg.Items.Last().UniqueId;
            url = "http://www.reddit.com/r/" + sg.Title + "/.json?count=25&after=" + url;


            JsonObject json = await accountManager.makeJsonPageRequest(url);

            JsonObject pageinfo = json.GetNamedObject("data");
            JsonArray links = pageinfo.GetNamedArray("children");

            //add http request to grab subreddit link
            for (uint i = 0; i < links.Count; i++)
            {
                JsonObject Data = links.GetObjectAt(i).GetNamedObject("data");
                string title = Data.GetNamedString("title");
                string id = Data.GetNamedString("id");
                id = links.GetObjectAt(i).GetNamedString("kind") + "_" + id;
                string author = Data.GetNamedString("author");
                string thumbnail = Data.GetNamedString("thumbnail");
                string permalink = Data.GetNamedString("permalink");
                string selftext = Data.GetNamedString("selftext");
                string subreddit = Data.GetNamedString("subreddit");
                int numcomments = (int)Data.GetNamedNumber("num_comments");
                DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                double sec = Data.GetNamedNumber("created_utc");
                date = date.AddSeconds(sec);


                string linkurl = Data.GetNamedString("url");
                //string selftext = links.GetObjectAt(i).GetNamedObject("data").GetNamedString("permalink");
                int score = int.Parse(Data.GetNamedNumber("score").ToString());

                SampleDataItem ans = new SampleDataItem(id, title, author, thumbnail, permalink, selftext, sg, permalink, linkurl, score, subreddit, date, numcomments);
                if (thumbnail == "" || selftext != "")
                {
                    ans.NeedImg = false;
                }
                sg.Items.Add(ans);
                if (SampleDataItem.isFVImageSource(ans.Url) && ans.FvImage != null)
                    sg.Images.Add(ans);

            }
        }



        public static async Task ParseCommentsAsync(SampleDataItem sditem)
        {
            string comments = "http://www.reddit.com" + sditem.Permalink + ".json";

            JsonArray json = await accountManager.makeJsonCommentRequest(comments);

            JsonArray commentlist = json.GetObjectAt(1).GetNamedObject("data").GetNamedArray("children");
            string op = json.GetObjectAt(0).GetNamedObject("data").GetNamedArray("children").GetObjectAt(0).GetNamedObject("data").GetNamedString("author");
            sditem.Comments = xJArraytoCommentList(commentlist, -2, op);

        }

        private static ObservableCollection<Comment> xJArraytoCommentList(JsonArray replies, int depth, string op)
        {
            if (replies != null)
            {
                ObservableCollection<Comment> ans = new ObservableCollection<Comment>();
                for (uint i = 0; i < replies.Count; i++)
                {
                    Comment ncheck = xJsonToComment(replies.GetObjectAt(i), depth, op);
                    if (ncheck != null)
                        ans.Add(ncheck);
                }
                return ans;
            }
            else
                return null;

        }
        private static Comment xJsonToComment(JsonObject comm, int depth, string op)
        {
            if (comm != null)
            {
                string type = comm.GetNamedString("kind");
                JsonObject data = comm.GetNamedObject("data");
                depth = depth + 3;

                try
                {
                    ObservableCollection<Comment> children = new ObservableCollection<Comment>();
                    try
                    {                        
                        JsonValue jtype = data.GetNamedValue("replies");
                        if (jtype.ValueType == JsonValueType.Object)
                        {                                                        
                            children = xJArraytoCommentList(jtype.GetObject().GetNamedObject("data").GetNamedArray("children"), depth, op);
                        }                        
                    }
                    catch (System.NullReferenceException e)
                    {
                        children = null;
                    }
                    
                    //string body = comm.GetNamedObject("data").GetNamedString("body_html");
                    string body = data.GetNamedString("body");
                    string parentid = data.GetNamedString("parent_id");
                    string id = data.GetNamedString("id");
                    id = type + "_" + id;
                    DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    
                    double sec = data.GetNamedNumber("created_utc");
                    date = date.AddSeconds(sec);

                    //body = Windows.Data.Html.HtmlUtilities.ConvertToText(body);
                    //body = WebUtility.HtmlDecode(body);

                    string author = data.GetNamedString("author");
                    int score = int.Parse((data.GetNamedNumber("ups") - data.GetNamedNumber("downs")).ToString());
                    //int score = 0;
                    return new Comment(children, body, author, score, parentid, id, depth * 5, date, op);
                }

                catch
                {
                    return null;
                }
            }
            else
                return null;
            //List<Comment> children, string body, string author, int score
        }
        
        public static async Task AddChildren(SampleDataItem sditem, ObservableCollection<Comment> list)
        {            
            foreach (var item in list)
            {
                sditem.aComments.Add(item);
                if (item.children != null && item.children.Count > 0)
                {
                    await SampleDataSource.AddChildren(sditem, item.children);
                }

            }
            //sditem.Comments.Clear();
        }
    }
}
