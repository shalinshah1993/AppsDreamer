using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Threading;

namespace OurWindowsApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        static string rootUri = "http://en.wikipedia.org/w/api.php";
        static string rootUri1 = "https://maps.googleapis.com/maps/api/place/search/xml?key=AIzaSyCm5alHOA4sBeOz6J_Q60dvKyy1j2SwYbc";
        string query = "";
        string[] features = { "airport","train_station","bus_station","gas_station","hospital"};
        private string sexy1;
        private string sexy;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            panorama.DefaultItem = panorama.Items[0];
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void textBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text == "")
                textBox1.Text = "Search";
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            //Search button is clicked
            post(new Uri(rootUri + "?action=opensearch&format=xml&search=" + textBox1.Text));
            post1(new Uri(rootUri1 + "&location=23.194176,72.628384&radius=75000&sensor=false&types=" + features[0]));
            post1(new Uri(rootUri1 + "&location=23.194176,72.628384&radius=75000&sensor=false&types=" + features[1]));
            post1(new Uri(rootUri1 + "&location=23.194176,72.628384&radius=75000&sensor=false&types=" + features[2]));
            post1(new Uri(rootUri1 + "&location=23.194176,72.628384&radius=75000&sensor=false&types=" + features[3]));
            post1(new Uri(rootUri1 + "&location=23.194176,72.628384&radius=75000&sensor=false&types=" + features[4]));
            //post1(new Uri(rootUri1 + "&location=23.194176,72.628384&radius=75000&sensor=false&types=" + features[5]));
            //post1(new Uri(rootUri1 + "&location=23.194176,72.628384&radius=75000&sensor=false&types=" + features[6]));
            ProgressBar.Visibility = Visibility.Visible;
            textBlock1.Text = "Searching...";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Airport button is clicked
            panorama.SlideToPage(4);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Railways button clicked
            panorama.SlideToPage(5);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //Weather button is clicked
            panorama.SlideToPage(3);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //Wikipedia info button is clicked
            panorama.SlideToPage(2);
        }

        private void gen_query(object sender, TextChangedEventArgs e)
        {
            query = textBox1.Text;
            
        }
        void post(Uri u)
        {
            HttpWebRequest queryRequest = (HttpWebRequest)WebRequest.Create(u);
            queryRequest.Method = "POST";
            queryUpdateState qState = new queryUpdateState();
            qState.AsyncRequest = queryRequest;
            queryRequest.BeginGetResponse(new AsyncCallback(HandleResponse), qState);
        }
        void HandleResponse(IAsyncResult result)
        {
            queryUpdateState qState = (queryUpdateState)result.AsyncState;
            HttpWebRequest qRequest = (HttpWebRequest)qState.AsyncRequest;

            qState.AsyncResponse = (HttpWebResponse)qRequest.EndGetResponse(result);

            Stream streamResult;
            try
            {
                streamResult = qState.AsyncResponse.GetResponseStream();
                // load the XML
                XDocument xmlquery = XDocument.Load(streamResult);
                string x = xmlquery.ToString();
                if (x.IndexOf("<Description xml:space=\"preserve\">") != -1)
                {
                    x = x.Substring(x.IndexOf("<Description xml:space=\"preserve\">") + 34);
                    x = x.Substring(0, x.IndexOf("</Description>"));

                    this.Dispatcher.BeginInvoke(new Action(() => textBlock1.Text = x));
                }
                else
                {
                    this.Dispatcher.BeginInvoke(new Action(() => MessageBox.Show("Oy")));
                }
                this.Dispatcher.BeginInvoke(new Action(() => this.ProgressBar.Visibility = Visibility.Collapsed));

            }
            catch (FormatException)
            {
                return;
            }
        }
        void post1(Uri u)
        {
            HttpWebRequest queryRequest = (HttpWebRequest)WebRequest.Create(u);
            queryRequest.Method = "POST";
            queryUpdateState qState = new queryUpdateState();
            qState.AsyncRequest = queryRequest;
            queryRequest.BeginGetResponse(new AsyncCallback(HandleResponse1), qState);
        }
        void HandleResponse1(IAsyncResult result)
        {
            queryUpdateState qState = (queryUpdateState)result.AsyncState;
            HttpWebRequest qRequest = (HttpWebRequest)qState.AsyncRequest;
            qState.AsyncResponse = (HttpWebResponse)qRequest.EndGetResponse(result);

            Stream streamResult;
            try
            {
                streamResult = qState.AsyncResponse.GetResponseStream();
                // load the XML
                XDocument xmlquery = XDocument.Load(streamResult);
                XElement[] a = xmlquery.Descendants("result").Descendants("name").ToArray();
                string s= xmlquery.Descendants("result").Descendants("type").ToArray().First().Value;
                string i = "";
                foreach (var item in a)
                {
                    i += item.Value + System.Environment.NewLine;

                }
                //string s = 
                
                 if (s== "airport")
                        this.Dispatcher.BeginInvoke(new Action(() => Airport.Text = i));
                   else if (s== "train_station")
                       this.Dispatcher.BeginInvoke(new Action(() => Railways.Text = i));
                 else if (s == "bus_station")
                       this.Dispatcher.BeginInvoke(new Action(() => Bus.Text = i));
            }
            catch (FormatException)
            {
                return;
            }
        }
       
        
    }
    public class queryUpdateState
    {
        public HttpWebRequest AsyncRequest { get; set; }
        public HttpWebResponse AsyncResponse { get; set; }
    }
}
s;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Threading;

namespace OurWindowsApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        static string rootUri = "http://en.wikipedia.org/w/api.php";
        string query = "";
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            panorama.DefaultItem = panorama.Items[0];
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void textBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text == "")
                textBox1.Text = "Search";
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            //Search button is clicked
            post(new Uri(rootUri + "?action=opensearch&format=xml&search=" + textBox1.Text));
            ProgressBar.Visibility = Visibility.Visible;
            textBlock1.Text = "Searching...";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Airport button is clicked
            panorama.SlideToPage(4);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Railways button clicked
            panorama.SlideToPage(5);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //Weather button is clicked
            panorama.SlideToPage(3);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //Wikipedia info button is clicked
            panorama.SlideToPage(2);
        }

        private void gen_query(object sender, TextChangedEventArgs e)
        {
            query = textBox1.Text;
            
        }
        void post(Uri u)
        {
            HttpWebRequest queryRequest = (HttpWebRequest)WebRequest.Create(u);
            queryRequest.Method = "POST";
            //queryRequest.Credentials = new NetworkCredential(key, key);
            queryUpdateState qState = new queryUpdateState();
            qState.AsyncRequest = queryRequest;
            queryRequest.BeginGetResponse(new AsyncCallback(HandleResponse), qState);
          //  return 1;
        }
        void HandleResponse(IAsyncResult result)
        {
            // get the state information
            queryUpdateState qState = (queryUpdateState)result.AsyncState;
            HttpWebRequest qRequest = (HttpWebRequest)qState.AsyncRequest;

            qState.AsyncResponse = (HttpWebResponse)qRequest.EndGetResponse(result);

            Stream streamResult;
            try
            {
                streamResult = qState.AsyncResponse.GetResponseStream();
                // load the XML
                XDocument xmlquery = XDocument.Load(streamResult);
                string x = xmlquery.ToString();
                if (x.IndexOf("<Description xml:space=\"preserve\">") != -1)
                {
                    x = x.Substring(x.IndexOf("<Description xml:space=\"preserve\">") + 34);
                    x = x.Substring(0, x.IndexOf("</Description>"));

                    this.Dispatcher.BeginInvoke(new Action(() => textBlock1.Text = x));
                }
                else {
                    this.Dispatcher.BeginInvoke(new Action(() => MessageBox.Show("Oy")));
                }
                this.Dispatcher.BeginInvoke(new Action(() =>this.ProgressBar.Visibility = Visibility.Collapsed));

            }
            catch (FormatException)
            {
                return;
            }
        }
       
        
    }
    public class queryUpdateState
    {
        public HttpWebRequest AsyncRequest { get; set; }
        public HttpWebResponse AsyncResponse { get; set; }
    }
}
