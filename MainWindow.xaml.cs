using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OpenGoogleWindow_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OpenGoogleTab();
                });
            });
        }

        private void OpenGoogleTab()
        {
            var searchBox = new TextBox
            {
                Width = 200
            };

            searchBox.GotFocus += searchBox_GotFocus;
            searchBox.LostFocus += searchBox_LostFocus;

            var tabItem = new TabItem
            {
                Header = "Google",
                Content = new Grid
                {
                    Children =
                    {
                        new StackPanel
                        {
                            Orientation = Orientation.Horizontal,
                            Margin = new Thickness(10),
                            Children =
                            {
                                searchBox, // TextBox
                                new Button
                                {
                                    Content = "Cerca",
                                    Margin = new Thickness(10, 0, 0, 0)
                                }
                            }
                        },
                        new WebBrowser
                        {
                            Source = new Uri("https://www.google.com"),
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch
                        }
                    }
                }
            };

            tabControl.Items.Add(tabItem);
            tabControl.SelectedItem = tabItem;
        }

        private async Task WaitForLoadCompleted(WebBrowser webBrowser)
        {
            await Task.Run(() =>
            {
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                LoadCompletedEventHandler loadedHandler = null;
                loadedHandler = (s, e) =>
                {
                    webBrowser.LoadCompleted -= loadedHandler;
                    tcs.SetResult(true);
                };

                webBrowser.LoadCompleted += loadedHandler;
                return tcs.Task;
            });
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = (TabItem)tabControl.SelectedItem;
            tabControl.Items.Remove(tabItem);
        }

        private void searchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            watermarkText.Visibility = Visibility.Collapsed;
        }

        private void searchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(searchBox.Text))
            {
                watermarkText.Visibility = Visibility.Visible;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string input = searchBox.Text;
            if (!string.IsNullOrEmpty(input))
            {
                string url = IsURL(input) ? input : (IsURL("http://" + input) ? "http://" + input : $"https://www.google.com/search?q={Uri.EscapeDataString(input)}");
                var webBrowser = GetSelectedWebBrowser();
                webBrowser?.Navigate(url);
            }
        }

        private bool IsURL(string input)
        {
            return Uri.IsWellFormedUriString(input, UriKind.Absolute);
        }

        private WebBrowser GetSelectedWebBrowser()
        {
            var tabItem = (TabItem)tabControl.SelectedItem;
            if (tabItem != null)
            {
                return ((Grid)tabItem.Content).Children.OfType<WebBrowser>().FirstOrDefault();
            }
            return null;
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            var webBrowser = GetSelectedWebBrowser();
            if (webBrowser != null && webBrowser.CanGoBack)
            {
                webBrowser.GoBack();
            }
        }

        private void GoForward_Click(object sender, RoutedEventArgs e)
        {
            var webBrowser = GetSelectedWebBrowser();
            if (webBrowser != null && webBrowser.CanGoForward)
            {
                webBrowser.GoForward();
            }
        }

        private void GoHome_Click(object sender, RoutedEventArgs e)
        {
            var webBrowser = GetSelectedWebBrowser();
            if (webBrowser != null)
            {
                webBrowser.Source = new Uri("https://www.google.com");
            }
        }

        private void RefreshPage_Click(object sender, RoutedEventArgs e)
        {
            var webBrowser = GetSelectedWebBrowser();
            if (webBrowser != null)
            {
                webBrowser.Refresh();
            }
        }
    }
}
