using System;
using Nofs.Net.Common.Interfaces.Library;
using Nofs.Net.AnnotationDriver;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

namespace Nofs.Net.nofs.stocks
{
    [RootFolderObject]
    [DomainObject]
    [FolderObject((FolderOperatorObject.CanAdd | FolderOperatorObject.CanRemove))]
    public class Portfolio
    {
        private IDomainObjectContainerManager _containerManager;
        private List<Stock> _stocks;

        public Portfolio()
        {
            _stocks = new List<Stock>();
        }

        [NeedsContainerManager]
        public IDomainObjectContainerManager ContainerManager
        {
            set
            {
                _containerManager = value;
            }
        }

        [FolderObject((FolderOperatorObject.CanAdd | FolderOperatorObject.CanRemove))]
        public IEnumerable<Stock> Stocks()
        {
            UpdateStockData();
            return _stocks;
        }

        public void AddAStockForTesting(Stock stock)
        {
            _stocks.Add(stock);
        }

        private void UpdateStockData()
        {
            String url = BuildURL();
            List<String> dataLines = getDataFromURL(url);
            foreach (Stock stock in _stocks)
            {
                String dataLine = null;
                foreach (String line in dataLines)
                {
                    if (line.StartsWith("\"" + stock.Ticker))
                    {
                        dataLine = line;
                        break;
                    }
                }
                if (dataLine != null)
                {
                    stock.UpdateData(dataLine);
                }
            }
        }

        private String BuildURL()
        {
            List<string> tickers = new List<string>();
            foreach (Stock stock in _stocks)
            {
                tickers.Add(stock.Ticker);
            }
            StringBuilder url = new StringBuilder();
            url.Append(@"http://download.finance.yahoo.com/d/quotes.csv?s=");
            url.Append(string.Join(",", tickers.ToArray()));
            url.Append("&f=sl1d1t1c1ohgv&e=.csv");
            return url.ToString();
        }

        private List<String> getDataFromURL(String url)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.KeepAlive = true;
            request.Timeout = 10 * 60 * 1000;

            byte[] array = null;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream os = response.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        const int bufferLength = 1024;
                        byte[] buf = new byte[bufferLength];
                        int read = 0;
                        while ((read = os.Read(buf, 0, bufferLength)) > 0)
                        {
                            ms.Write(buf, 0, read);
                        }
                        array = ms.ToArray();
                    }
                }
            }

            string s = new UTF8Encoding(true, true).GetString(array);

            List<String> lines = new List<String>();
            foreach (String line in s.Split("\r\n".ToCharArray()))
            {
                lines.Add(line);
            }
            return lines;
        }

        [Executable]
        public void AddAStock(String ticker)
        {
            var stockContainer = _containerManager.GetContainer(typeof(Stock));
            Stock stock = stockContainer.NewPersistentInstance() as Stock;
            stock.Ticker = ticker;
            _stocks.Add(stock);
            stockContainer.ObjectChanged(stock);
            _containerManager.GetContainer(typeof(Portfolio)).ObjectChanged(this);
        }
    }
}
