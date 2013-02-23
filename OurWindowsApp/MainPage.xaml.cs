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

namespace OurWindowsApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        static string key = "cV2XJcXlIrGraTLDFdmL1jEWokLxziqchlyr+TR7biM=";
        static string rootUri = "https://api.datamarket.azure.com/Bing/Search";
        string query = "";
        string text;
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
            post(new Uri(rootUri + "/Web?Query='" + textBox1.Text + "'"));
            textBlock1.Text = "searching";
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
            queryRequest.Credentials = new NetworkCredential(key, key);
            queryUpdateState qState = new queryUpdateState();
            qState.AsyncRequest = queryRequest;
            queryRequest.BeginGetResponse(new AsyncCallback(HandleResponse), qState);
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
                text = xmlquery.ToString();
                int index = text.IndexOf("<d:Description m:type=");
                text = text.Substring(index + 35, 500);
                index = text.IndexOf("</d:Description>");
                text = text.Substring(0, index);
                this.Dispatcher.BeginInvoke(new Action(() => this.textBlock1.Text = text));

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
