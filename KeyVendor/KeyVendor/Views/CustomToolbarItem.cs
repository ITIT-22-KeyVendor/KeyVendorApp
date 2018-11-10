using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace KeyVendor.Views
{
    public class CustomToolbarItem : ToolbarItem
    {
        public CustomToolbarItem()
        {
            InitVisibility();
        }        

        public new ContentPage Parent { set; get; }

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public static BindableProperty IsVisibleProperty =
            BindableProperty.Create<CustomToolbarItem, bool>(o => o.IsVisible, false, propertyChanged: OnIsVisibleChanged);

        private async void InitVisibility()
        {
            OnIsVisibleChanged(this, false, IsVisible);
        }

        private static void OnIsVisibleChanged(BindableObject bindable, bool oldvalue, bool newvalue)
        {
            var item = bindable as CustomToolbarItem;

            if (item.Parent == null)
                return;

            var items = item.Parent.ToolbarItems;

            if (newvalue && !items.Contains(item))
            {
                items.Add(item);
            }
            else if (!newvalue && items.Contains(item))
            {
                items.Remove(item);
            }
        }
    }
}
