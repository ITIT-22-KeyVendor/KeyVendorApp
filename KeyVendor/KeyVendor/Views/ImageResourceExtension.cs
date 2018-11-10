using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KeyVendor.Views
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return ImageSource.FromResource(Source);
        }
    }
}