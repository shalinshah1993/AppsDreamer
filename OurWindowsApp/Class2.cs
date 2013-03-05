using System;

namespace OurWindowsApp
{
    public class Class2
    {
        public String featureName { get; set; }

        public String featureDistance { get; set; }

        public Class2(String name, string distance)
        {
            this.featureName = name;
            this.featureDistance = distance;
        }
    }
}