using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                _lastPercentage = _percentage;
                _percentage = value;
                if (PercentageChanged != null)
                    PercentageChanged(this, new ValChangedEventArgs(_lastPercentage, _percentage));

            }
        }
        private string _speed;
        private string _lastspeed = "";

        public string Speed
        {
            get { return _speed; }
            set
            {
                _lastspeed = _speed;
                _speed = value;
                if (SpeedChanged != null) SpeedChanged(this,  new ValChangedEventArgs(_lastspeed, _speed));
            }
        }

        public event ValChangedEventHandler SpeedChanged;
        public event ValChangedEventHandler PercentageChanged;

        public State DownloadingState { get; set; }
        public State LastState { get; private set; }

        WebClient _webClient;
        Stopwatch _stopWatch = new Stopwatch();
        private AutoResetEvent _signalEvent;
        public State DownloadFile(string fromUrl, string placeToSave)
        {
            _signalEvent = new AutoResetEvent(false);
            _webClient.DownloadFileCompleted -= _webClient_DownloadFileCompleted;
            _webClient.DownloadProgressChanged -= _webClient_DownloadProgressChanged;
            _webClient.DownloadFileCompleted += _webClient_DownloadFileCompleted;
            _webClient.DownloadProgressChanged += _webClient_DownloadProgressChanged;
            //tutaj dołożyć właściwą funkcjonalność
            Uri URL = fromUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? new Uri(fromUrl) : new Uri("http://" + fromUrl); //na razie olewam s, bo certyfikaty etc;)
            _stopWatch.Start();
            
            try
            {
                LastState = State.Downloading;
                _webClient.DownloadFileAsync(URL, placeToSave);
                _signalEvent.WaitOne();
                
            }
            catch (Exception ex)
            {
                return State.Error;
                //potem to uzależnić od błędu
            }
            _webClient.DownloadFileCompleted -= _webClient_DownloadFileCompleted;
            _webClient.DownloadProgressChanged -= _webClient_DownloadProgressChanged;
            return LastState;
        }

        private void _webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Speed = string.Format("{0} kb/s",
                (e.BytesReceived / 1024d / _stopWatch.Elapsed.TotalSeconds).ToString("0.00"));
        }

        private void _webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled) LastState = State.Cancelled;
            else LastState = State.Downloaded;
            try
            {
                _signalEvent.Set();
            }
            catch (Exception ex)
            {
                _signalEvent = new AutoResetEvent(true);
            }
        }
    }

    public enum State
    {
        Unknown,
        Downloaded,
        Downloading,
        Cancelled,
        Error
    }

}
