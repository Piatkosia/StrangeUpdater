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
                if (e is WebException) MessageBox.Show("Napraw sobie internety i wróć:)");
                this.Close();
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
            if (state == ButtonState.UpdateRequired)
            {
                DoUpdate();
            }
        }

        private void DoUpdate()
        {
            Downloader downloader = new Downloader();
            bool up = updater.Update();
            if (up)
            {
                button.Content = "Aktualizacja w toku";
                button.IsEnabled = false;
                foreach (var VARIABLE in updater.FilesToUpdate)
                {
                    var localLink = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) +"\\"+ VARIABLE;
                    var remoteLink = Properties.Resources.ServerAddress + "/" + VARIABLE;
                    var answ = downloader.DownloadFile(remoteLink, localLink.Replace('/','\\'));
                    if (answ != State.Downloaded)
                    {
                        state = ButtonState.End;
                        button.Content = "Aktualizacja zakończona niepowodzeniem:( Zakończ";
                        button.IsEnabled = true;
                        return;
                    }
                }
                button.IsEnabled = true;
                state = ButtonState.End;
                button.Content = "Aktualna. Zakończ";
            }
            else
            {
                button.Content = "Aktualizacja zakończona niepowodzeniem:( Zakończ";
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
