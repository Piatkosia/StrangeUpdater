using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace StrangeUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string ImagePath = LinkParser.GetLink();
            if (!string.IsNullOrEmpty(ImagePath))
                 DownloadImage(ImagePath);
        }

        private void DownloadImage(string pth)
        {
            InitializeComponent();
            try
            {
                var c = new WebClient();
                var bytes = c.DownloadData(pth);
                var ms = new MemoryStream(bytes);

                var bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                image.Source = bi;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        ButtonState state = ButtonState.Start;
        private Updater updater = new Updater();
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (state == ButtonState.Start)
            {
                string localVerPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\VERSION";
                bool req = updater.UpdateRequired(localVerPath, Properties.Resources.ServerAddress + @"/VERSION");
                if (req)
                {
                    state = ButtonState.UpdateRequired;
                    button.Content = "AKTUALIZUJ!";
                }
                else
                {
                    state = ButtonState.End;
                    button.Content = "Aktualna. Zakończ";
                }
                return;
            }
            if (state == ButtonState.End)
            {
                Close();
            }
        }
    }

    public enum ButtonState
    {
        Start,
        UpdateRequired,
        UpdateNotRequired,
        StartUpdate,
        End,
    }
}
