using System;
using System.Runtime.Serialization;
using Nofs.Net.AnnotationDriver;

namespace Nofs.Net.nofs_addressbook
{
    [DataContract]
    public class BookInfo
    {
        public BookInfo()
        {
            CreatedBy = "System";
            ModifiedBy = "Fuse user";
            CreatedOn = DateTime.Today.ToLongDateString(); 
            Modified = DateTime.UtcNow.ToLongTimeString();
        }

        [DataMember]
        public String CreatedOn
        {
            get;
            private set;
        }

        [DataMember]
        public String CreatedBy
        {
            get;
            set;
        }

        [DataMember]
        public String Modified
        {
            get;
            set;
        }

        [DataMember]
        public String ModifiedBy
        {
            get;
            set;
        }
    }
}
