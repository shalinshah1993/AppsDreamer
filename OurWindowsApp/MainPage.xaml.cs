using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using Microsoft.Phone.Controls;

namespace OurWindowsApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        private static string rootUri = "http://en.wikipedia.org/w/api.php";
        private static string rootUri1 = "https://maps.googleapis.com/maps/api/place/search/xml?key=AIzaSyCm5alHOA4sBeOz6J_Q60dvKyy1j2SwYbc";
        private static string wikiuri = "http://en.wikipedia.org/w/api.php?format=xml&action=query&prop=revisions&rvprop=content&titles=";
        private string search = "";
        private GeoCoordinateWatcher watcher;

        //string accuracyText = "";
        private string[] features = { "airport", "train_station", "bus_station", "gas_station", "hospital" };

        private string clat = "23.194176";
        private string clng = "72.628384";//23.194176,72.628384
        private string slat, slng;//for search
        public List<Class2> airPlaces, busPlaces, railPlaces;

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
                StartLocationService(GeoPositionAccuracy.Default);
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
            airPlaces = new List<Class2>();
            railPlaces = new List<Class2>();
            busPlaces = new List<Class2>();
            if (!(textBox1.Text == "Search"))
            {
                search = textBox1.Text;
                postsetlocation(new Uri("http://maps.googleapis.com/maps/api/geocode/xml?sensor=false&address=" + textBox1.Text));
                /*post(new Uri(rootUri + "?action=opensearch&format=xml&search=" + textBox1.Text));
                wikipost(new Uri(wikiuri + textBox1.Text));
                post1(new Uri(rootUri1 + "&location=" + slat + "," + slng + "&radius=75000&sensor=false&types=" + features[0]));
                post1(new Uri(rootUri1 + "&location=" + slat + "," + slng + "&radius=75000&sensor=false&types=" + features[1]));
                post1(new Uri(rootUri1 + "&location=" + slat + "," + slng + "&radius=75000&sensor=false&types=" + features[2]));
                post1(new Uri(rootUri1 + "&location=" + slat + "," + slng + "&radius=75000&sensor=false&types=" + features[3]));
                post1(new Uri(rootUri1 + "&location=" + slat + "," + slng + "&radius=75000&sensor=false&types=" + features[4]));*/
                ProgressBar.Visibility = Visibility.Visible;
                textBlock1.Text = "Searching...";
            }
            else
            {
                MessageBox.Show("Please enter something to search");
            }
        }

        private void beginSearch()
        {
            post(new Uri(rootUri + "?action=opensearch&format=xml&search=" + search));
            wikipost(new Uri(wikiuri + search));
            post1(new Uri(rootUri1 + "&location=" + slat + "," + slng + "&radius=75000&sensor=false&types=" + features[0]));
            post1(new Uri(rootUri1 + "&location=" + slat + "," + slng + "&radius=75000&sensor=false&types=" + features[1]));
            post1(new Uri(rootUri1 + "&location=" + slat + "," + slng + "&radius=75000&sensor=false&types=" + features[2]));
            post1(new Uri(rootUri1 + "&location=" + slat + "," + slng + "&radius=75000&sensor=false&types=" + features[3]));
            post1(new Uri(rootUri1 + "&location=" + slat + "," + slng + "&radius=75000&sensor=false&types=" + features[4]));
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
            search = textBox1.Text;
        }

        private void post(Uri u)
        {
            HttpWebRequest queryRequest = (HttpWebRequest)WebRequest.Create(u);
            queryRequest.Method = "POST";
            queryUpdateState qState = new queryUpdateState();
            qState.AsyncRequest = queryRequest;
            queryRequest.BeginGetResponse(new AsyncCallback(HandleResponse), qState);
        }

        private void HandleResponse(IAsyncResult result)
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

        private void post1(Uri u)
        {
            HttpWebRequest queryRequest = (HttpWebRequest)WebRequest.Create(u);
            queryRequest.Method = "POST";
            queryUpdateState qState = new queryUpdateState();
            qState.AsyncRequest = queryRequest;
            queryRequest.BeginGetResponse(new AsyncCallback(HandleResponse1), qState);
        }

        private void HandleResponse1(IAsyncResult result)
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
                XElement[] geo_lat = xmlquery.Descendants("result").Descendants("geometry").Descendants("location").Descendants("lat").ToArray();
                XElement[] geo_lng = xmlquery.Descendants("result").Descendants("geometry").Descendants("location").Descendants("lng").ToArray();

                string s = xmlquery.Descendants("result").Descendants("type").ToArray().First().Value;
                double[] distance = new double[100];
                string[] names = new string[100];
                string i = "";
                for (int il = 0; il < a.Length; il++)
                {
                    double a1 = (Math.Sin(Radians(double.Parse(clat) - double.Parse(geo_lat[il].Value)) / 2) * Math.Sin(Radians(double.Parse(clat) - double.Parse(geo_lat[il].Value)) / 2)) + Math.Cos(Radians(double.Parse(geo_lat[il].Value))) * Math.Cos(Radians(double.Parse(clat))) * (Math.Sin(Radians(double.Parse(clng) - double.Parse(geo_lng[il].Value)) / 2) * Math.Sin(Radians(double.Parse(clng) - double.Parse(geo_lng[il].Value)) / 2));
                    double angle = 2 * Math.Atan2(Math.Sqrt(a1), Math.Sqrt(1 - a1));
                    double Distance = angle * 6378.16;
                    distance[il] = Math.Round(Distance, 2);
                    names[il] = a[il].Value;
                    i += Math.Round(Distance, 2) + " KM away,";
                    i += a[il].Value + System.Environment.NewLine;

                    if (s == "airport")
                    {
                        if (distance[il] != null && names[il] != null)
                            airPlaces.Add(new Class2(names[il], distance[il] + "Kms"));
                    }
                    else if (s == "train_station")
                    {
                        if (distance[il] != null && names[il] != null)
                            railPlaces.Add(new Class2(names[il], distance[il] + "Kms"));
                    }
                    else if (s == "bus_station")
                    {
                        if (distance[il] != null && names[il] != null)
                            busPlaces.Add(new Class2(names[il], distance[il] + "Kms"));
                    }
                }

                if (s == "airport")
                    this.Dispatcher.BeginInvoke(new Action(() => AirportList.ItemsSource = airPlaces));
                else if (s == "train_station")
                    this.Dispatcher.BeginInvoke(new Action(() => RailwaysList.ItemsSource = railPlaces));
                else if (s == "bus_station")
                    this.Dispatcher.BeginInvoke(new Action(() => BusList.ItemsSource = busPlaces));

                if (watcher != null)
                {
                    watcher.Stop();
                }
            }
            catch (FormatException)
            {
                return;
            }
        }

        private void postsetlocation(Uri u)
        {
            HttpWebRequest queryRequest = (HttpWebRequest)WebRequest.Create(u);
            queryRequest.Method = "POST";
            queryUpdateState qState = new queryUpdateState();
            qState.AsyncRequest = queryRequest;
            queryRequest.BeginGetResponse(new AsyncCallback(locHandleResponse), qState);
        }

        private void locHandleResponse(IAsyncResult result)
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
                slat = xmlquery.Descendants("result").Descendants("geometry").Descendants("location").Descendants("lat").First().Value;
                slng = xmlquery.Descendants("result").Descendants("geometry").Descendants("location").Descendants("lng").First().Value;
                beginSearch();
            }
            catch (FormatException)
            {
                return;
            }
        }

        private void wikipost(Uri u)
        {
            HttpWebRequest queryRequest = (HttpWebRequest)WebRequest.Create(u);
            queryRequest.Method = "POST";
            queryUpdateState qState = new queryUpdateState();
            qState.AsyncRequest = queryRequest;
            queryRequest.BeginGetResponse(new AsyncCallback(wikiHandleResponse), qState);
        }

        private void wikiHandleResponse(IAsyncResult result)
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

                //Wikipedia Data Need to be parsed
                string a = xmlquery.Descendants("query").Descendants("pages").Descendants("page").Descendants("revisions").Descendants("rev").First().Value;

                //string x = xmlquery.ToString();
                /* if (x.IndexOf("<Description xml:space=\"preserve\">") != -1)
                 {
                     x = x.Substring(x.IndexOf("<Description xml:space=\"preserve\">") + 34);
                     x = x.Substring(0, x.IndexOf("</Description>"));

                     this.Dispatcher.BeginInvoke(new Action(() => textBlock1.Text = x));
                 }
                 else
                 {
                     this.Dispatcher.BeginInvoke(new Action(() => MessageBox.Show("Oy")));
                 }
                 this.Dispatcher.BeginInvoke(new Action(() => this.ProgressBar.Visibility = Visibility.Collapsed));*/
            }
            catch (FormatException)
            {
                return;
            }
        }

        private double Radians(double p)
        {
            return p * Math.PI / 180;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            //Bus Stop Button is clicked
            panorama.SlideToPage(6);
        }

        /// <summary>
        /// Helper method to start up the location data acquisition
        /// </summary>
        /// <param name="accuracy">The accuracy level </param>
        private void StartLocationService(GeoPositionAccuracy accuracy)
        {
            // Reinitialize the GeoCoordinateWatcher
            watcher = new GeoCoordinateWatcher(accuracy);
            watcher.MovementThreshold = 20;

            // Add event handlers for StatusChanged and PositionChanged events
            watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

            // Start data acquisition
            watcher.Start();
        }

        /// <summary>
        /// Handler for the StatusChanged event. This invokes MyStatusChanged on the UI thread and
        /// passes the GeoPositionStatusChangedEventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MyStatusChanged(e));
        }

        /// <summary>
        /// Custom method called from the StatusChanged event handler
        /// </summary>
        /// <param name="e"></param>
        private void MyStatusChanged(GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:

                    // The location service is disabled or unsupported.
                    // Alert the user
                    //        StatusTextBlock.Text = "location is unsupported on this device";
                    break;

                case GeoPositionStatus.Initializing:

                    // The location service is initializing.
                    // Disable the Start Location button
                    //            StatusTextBlock.Text = "initializing location service," + accuracyText;
                    break;

                case GeoPositionStatus.NoData:

                    // The location service is working, but it cannot get location data
                    // Alert the user and enable the Stop Location button
                    //          StatusTextBlock.Text = "data unavailable," + accuracyText;
                    break;

                case GeoPositionStatus.Ready:

                    // The location service is working and is receiving location data
                    // Show the current position and enable the Stop Location button
                    //        StatusTextBlock.Text = "receiving data, " + accuracyText;
                    break;
            }
        }

        /// <summary>
        /// Handler for the PositionChanged event. This invokes MyStatusChanged on the UI thread and
        /// passes the GeoPositionStatusChangedEventArgs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MyPositionChanged(e));
        }

        /// <summary>
        /// Custom method called from the PositionChanged event handler
        /// </summary>
        /// <param name="e"></param>
        private void MyPositionChanged(GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            // Update the TextBlocks to show the current location
            clat = e.Position.Location.Latitude.ToString("0.000");
            clng = e.Position.Location.Longitude.ToString("0.000");
        }

        private void whereami_Click(object sender, RoutedEventArgs e)
        {
        }

        /*     void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
             {
                 Stream stream = e.Result;
                 XDocument xDoc = XDocument.Load(stream);

                 //pivWeather.SelectedIndex = 1;
                 GetCurrentWeather(xDoc);
                 GetForeCast(xDoc);
             }

             private void GetCurrentWeather(XDocument xDoc)
             {
                 try
                 {
                     List<WeatherReport> lstWeatherReport = new List<WeatherReport>();

                     string city = xDoc.Descendants("forecast_information").First().Element("city").Attribute("data").Value;
                     string url = "http://www.google.com" + xDoc.Descendants("current_conditions").First().Element("icon").Attribute("data").Value;

                     lstWeatherReport = (from item in xDoc.Descendants("current_conditions")
                                         select new WeatherReport()
                                         {
                                             DayOfWeek = "Today",
                                             City = city,

                                             //WeatherImage = img,
                                             Condition = item.Element("condition").Attribute("data").Value,

                                             //TempF = item.Element("temp_f").Attribute("data").Value,
                                             TempC = item.Element("temp_c").Attribute("data").Value + " C",
                                             Humidity = item.Element("humidity").Attribute("data").Value,

                                             //ImageURL = item.Element("icon").Attribute("data").Value,
                                             Wind = item.Element("wind_condition").Attribute("data").Value
                                         }).ToList();

                     // lstWeather.ItemsSource = lstWeatherReport;
                     listBox1.ItemsSource = lstWeatherReport;
                 }
                 catch
                 {
                     //lstWeather.ItemsSource = null;
                     listBox1.ItemsSource = null;
                 }
             }
             private void GetForeCast(XDocument xDoc)
             {
                 try
                 {
                     List<WeatherReport> lstWeatherReport = new List<WeatherReport>();
                     string city = xDoc.Descendants("forecast_information").First().Element("city").Attribute("data").Value;

                     lstWeatherReport = (from item in xDoc.Descendants("forecast_conditions")
                                         select new WeatherReport()
                                         {
                                             DayOfWeek = item.Element("day_of_week").Attribute("data").Value,
                                             City = city,

                                             //WeatherImage = img,
                                             Condition = item.Element("condition").Attribute("data").Value,
                                             TempF = item.Element("low").Attribute("data").Value + " F",
                                             TempC = item.Element("high").Attribute("data").Value + " F"
                                         }).ToList();

                     listBox1.ItemsSource = lstWeatherReport;
                 }
                 catch
                 {
                     listBox1.ItemsSource = null;
                 }
             }
             private void button1_Click(object sender, RoutedEventArgs e)
             {
                 if (textBox2.Text.Trim() == string.Empty)
                     return;
                 WebClient client = new WebClient();
                 client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
                 client.OpenReadAsync(new Uri("http://www.google.com/ig/api?weather=" + textBox2.Text));
             }*/
    }

    public class queryUpdateState
    {
        public HttpWebRequest AsyncRequest { get; set; }

        public HttpWebResponse AsyncResponse { get; set; }
    }
}
