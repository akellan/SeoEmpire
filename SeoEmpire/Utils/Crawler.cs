using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Net;
using System.Timers;

namespace SeoEmpire.Utils
{
    public class Crawler
    {
        private readonly Regex _rNextPage;
        private readonly Regex _rLinks;
        private Uri _curPage;
        private readonly string _baseAddress;
        private int _curIterator = 0;

        private int _dwonloadErrorCount = 3;

        private static readonly Queue<Uri> QUrls = new Queue<Uri>();
        public static readonly object _qLock = new object();

        public static Queue<Uri> Urls
        {
            get { return QUrls; }
        }

        private readonly Timer _timer = new Timer();
        private int _maxIterator;

        public Crawler(Uri firstPage, string baseAddress, Regex nextPage, Regex links, int maxItertaor)
        {
            _rNextPage = nextPage;
            _rLinks = links;
            _curPage = firstPage;
            _baseAddress = baseAddress;
            
            _timer.Elapsed += WorkerDoWork;
            
            _timer.Interval = 2500;
            _maxIterator = maxItertaor;
        }

        void WorkerDoWork(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            WebClient client = new WebClient();
            string content;

            try
            {
                 content = client.DownloadString(_curPage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Crawler: DownloadString - " + ex.Message);

                _dwonloadErrorCount--;
                if (_dwonloadErrorCount <= 0)
                {
                    return;
                }

                _timer.Start();
                return;
            }

            _dwonloadErrorCount = 3;
            

            Match nextPageMatch = _rNextPage.Match(content);
            if (nextPageMatch != null)
            {
                try
                {
                    _curPage = new Uri(_baseAddress + nextPageMatch.Groups[1].Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Crawler: GetNextPage - {0}" + ex.Message);
                    _timer.Stop();
                }
                
            }

            Console.WriteLine("Crawler: CurPage - {0}", _curPage.OriginalString);

            MatchCollection linksMatches = _rLinks.Matches(content);
            lock (_qLock)
            {
                foreach (Match match in linksMatches)
                {
                    QUrls.Enqueue(new Uri(match.Groups[1].Value));
                    Console.WriteLine("Crawler: AddLink - {0}", match.Groups[1].Value);
                }

                Console.WriteLine("Crawler: QueueCount: {0} Iterator:{1}",QUrls.Count, _curIterator);
            }

            _curIterator++;


            if(_curIterator >= _maxIterator)
                return;

            _timer.Start();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

    }
}
