using System;
using System.Runtime.Serialization;
using Nofs.Net.AnnotationDriver;

namespace Nofs.Net.nofs.stocks
{
    [DomainObject(DomainOperatorObject.CanWrite)]
    [DataContract]
    public class Stock
    {
        public Stock() 
        {
        }

        public Stock(String ticker)
        {
            Ticker = ticker;
        }

        [ProvidesName]
        public String Ticker
        {
            get;
            set;
        }

        public void UpdateData(String data)
        {
            string[] array = (""+data).Split(',');
            Price = (array.Length < 2 ? "unknown" : array[1].Trim());
            Date = (array.Length < 3 ? "unknown" : array[2].Replace("\"", "").Trim());
            Time = (array.Length < 4 ? "unknown" : array[3].Replace("\"", "").Trim());
            Diff = (array.Length < 5 ? "unknown" : array[4].Trim());
        }


        [DataMember]
        public String Price
        {
            get;
            set;
        }

        [DataMember]
        public String Date
        {
            get;
            set;
        }

        [DataMember]
        public String Time
        {
            get;
            set;
        }

        [DataMember]
        public String Diff
        {
            get;
            set;
        }
    }
}
