using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace OurWindowsApp
{
    public class Class2
    {
        public String featureName { get; set; }
        public String featureDistance { get; set; }
        public Class2(String name,string distance) {
            this.featureName = name;
            this.featureDistance = distance;
        }
    }
}
