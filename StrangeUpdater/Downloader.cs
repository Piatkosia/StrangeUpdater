using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace StrangeUpdater
{
    public class Downloader
    {
        private string _percentage;
        private string _lastPercentage = "";

        public string Percentage
        {
            get { return _percentage; }
            set
            {
                if (_percentage != value)
                {
                    _lastPercentage = _percentage;
                    _percentage = value;
                    if (PercentageChanged != null)
                        PercentageChanged(this, new ValChangedEventArgs(_lastPercentage, _percentage));
                }

            }
        }
        private string _speed;
        private string _lastspeed = "";

        public string Speed
        {
            get { return _speed; }
            set
            {
                if (_speed != value)
                {
                    _lastspeed = _speed;
                    _speed = value;
                    if (SpeedChanged != null) SpeedChanged(this, new ValChangedEventArgs(_lastspeed, _speed));
                }
            }
        }

        private string _downloadedFileInfo;

        public string DownloadedFileInfo
        {
            get { return _downloadedFileInfo; }
            set
            {
                if (_downloadedFileInfo != value)
                {
                    _downloadedFileInfo = value;
                    if (DownloadedInfoChanged != null)
                        DownloadedInfoChanged(this, new ValSetEventArgs(_downloadedFileInfo));
                }
            }
        }

        private object locker = new object();
        public event ValChangedEventHandler SpeedChanged;
        public event ValChangedEventHandler PercentageChanged;
        public event ValSetEventHandler DownloadedInfoChanged;

        public State DownloadingState { get; set; }
        public State LastState { get; private set; }

        WebClient _webClient;
        Stopwatch _stopWatch = new Stopwatch();
        private ManualResetEvent _signalEvent;
        public State DownloadFile(string fromUrl, string placeToSave)
        {
            _signalEvent = new ManualResetEvent(false);

            //tutaj dołożyć właściwą funkcjonalność
            Uri URL = fromUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? new Uri(fromUrl) : new Uri("http://" + fromUrl); //na razie olewam s, bo certyfikaty etc;)
            _stopWatch.Start();
            long length;
            try
            {
                if (RemoteFileExists(URL.ToString(), out length))
                {
                    LastState = State.Downloading;
                    Directory.CreateDirectory(Path.GetDirectoryName(placeToSave));
                    using (_webClient = new WebClient())
                    {
                        var syncObj = new Object();
                        _webClient.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
                        _webClient.DownloadFileCompleted += _webClient_DownloadFileCompleted;
                        _webClient.DownloadProgressChanged += _webClient_DownloadProgressChanged;
                        _webClient.DownloadFileAsync(URL, placeToSave);
                        while (_webClient.IsBusy)
                        {
                            DoEvents();
                        }
                        System.Threading.Thread.Sleep(100);
                    }
                }
                else LastState = State.NotFound;
                return LastState;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return State.Error;
                //potem to uzależnić od błędu
            }
            finally
            {
                _webClient.DownloadFileCompleted -= _webClient_DownloadFileCompleted;
                _webClient.DownloadProgressChanged -= _webClient_DownloadProgressChanged;
            }
          
            return LastState;
        }
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }
        private void _webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Speed = string.Format("{0} kb/s",
                (e.BytesReceived / 1024d / _stopWatch.Elapsed.TotalSeconds).ToString("0.00"));
            Percentage = e.ProgressPercentage.ToString() + "%";
            DownloadedFileInfo = (e.BytesReceived / 1024d).ToString("0.00") + "kb / " +
                                 (e.TotalBytesToReceive / 1024d).ToString("0.00");
        }

        private void _webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            LastState = e.Cancelled ? State.Cancelled : State.Downloaded;
        }
        private bool RemoteFileExists(string url, out long length)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                length = response.ContentLength;
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                length = 0;
                return false;
            }
        }
    }

    public enum State
    {
        Unknown,
        Downloaded,
        Downloading,
        Cancelled,
        Error, 
        NotFound
    }

}
