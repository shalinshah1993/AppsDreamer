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

namespace OurWindowsApp
{
    public partial class MainPage : PhoneApplicationPage
    {
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

    }
}
