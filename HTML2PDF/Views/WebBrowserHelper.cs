using System;
using System.Windows;
using System.Windows.Controls;

namespace HTML2PDF
{
    /// <summary>
    /// Add ability to bind Url property of WebBrowser control
    /// https://www.technical-recipes.com/2016/how-to-embed-a-web-browser-in-wpf-xaml/
    /// </summary>
    public static class WebBrowserHelper
    {

        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.RegisterAttached("Url", typeof(string), typeof(WebBrowserHelper),
                new PropertyMetadata(OnUrlChanged));

        public static string GetUrl(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(UrlProperty);
        }

        public static void SetUrl(DependencyObject dependencyObject, string body)
        {
            dependencyObject.SetValue(UrlProperty, body);
        }

        private static void OnUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is WebBrowser browser))
            {
                return;
            }

            Uri uri = null;


            try
            {
                if (e.NewValue is string s)
                {
                    var uriString = s;

                    uri = string.IsNullOrWhiteSpace(uriString) ? null : new Uri(uriString);
                }
                else if (e.NewValue is Uri uri1)
                {
                    uri = uri1;
                }
                browser.Source = uri;
            }
            catch (UriFormatException)
            {

            }
        }
    }
}
