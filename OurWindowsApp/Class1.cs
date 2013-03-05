using Microsoft.Phone.Controls;

namespace OurWindowsApp
{
    public static class PanoramaExtensions
    {
        public static void SlideToPage(this Panorama self, int item)
        {
            var slide_transition = new SlideTransition() { };
            slide_transition.Mode = SlideTransitionMode.SlideRightFadeOut;
            ITransition transition = slide_transition.GetTransition(self);
            transition.Completed += delegate
            {
                self.DefaultItem = self.Items[item];

                //PanoramaItem panItem = (PanoramaItem)self.Items[1];

                //self.Items.Remove(panItem);

                //self.Items.Insert(0, panItem);
                transition.Stop();
            };
            transition.Begin();
        }
    }
}