using System;
using System.Collections.Generic;
using System.Xml;
using Nofs.Net.Common.Interfaces.Cache;

namespace Nofs.Net.Cache.Impl
{
    public class XmlRepresentationBuilder : IRepresentationBuilder
    {
        private XmlDocument _document;

        public XmlRepresentationBuilder()
        {
            _document = new XmlDocument();
        }

        public void PopulateWith(string data)
        {
            try
            {
                _document.LoadXml(data);
            }
            catch (System.Xml.XmlException de)
            {
                throw new NoFSSerializationException(de);
            }
        }

        private class XmlFolderReference : IFolderReference
        {
            public XmlNode branch;

            public XmlFolderReference(XmlNode branch)
            {
                this.branch = branch;
            }


            public string Name
            {
                get
                {
                    return branch.LocalName;
                }
            }
        }

        public string TranslateToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            using (System.IO.StringWriter sw = new System.IO.StringWriter(sb))
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                _document.Save(writer);
                writer.Flush();
            }

            return sb.ToString();
        }

        public IFolderReference GetRoot()
        {
            return new XmlFolderReference(_document);
        }


        public IFolderReference AddFolder(IFolderReference folder, string name)
        {
            XmlNode newNode = ((XmlFolderReference)folder).branch.OwnerDocument.CreateElement(name);
            return new XmlFolderReference(((XmlFolderReference)folder).branch.AppendChild(newNode));
        }


        //@SuppressWarnings("unchecked")
        public IFolderReference FindChildByName(IFolderReference parent, string name)
        {
            for (XmlNode iter = ((XmlFolderReference)parent).branch; iter.HasChildNodes; )
            {
                object obj = iter.NextSibling;
                if (obj is XmlNode)
                {
                    XmlNode child = (XmlNode)obj;
                    if (child.LocalName.CompareTo(name) == 0)
                    {
                        return new XmlFolderReference(child);
                    }
                }
            }
            throw new System.Exception("could not find xml node: '" + name + "' as child of '" + parent.Name + "'");
        }


        //@SuppressWarnings("unchecked")
        public List<IFolderReference> GetChildren(IFolderReference folder)
        {
            List<IFolderReference> children = new List<IFolderReference>();
            for (XmlNode iter = ((XmlFolderReference)folder).branch; iter.HasChildNodes; )
            {
                object obj = iter.NextSibling;
                if (obj is XmlNode)
                {
                    children.Add(new XmlFolderReference((XmlNode)obj));
                }
            }
            return children;
        }


        public string GetFolderValue(IFolderReference folder)
        {
            return ((XmlFolderReference)folder).branch.InnerText;
        }


        public void SetFolderValue(IFolderReference folder, object value)
        {
            ((XmlFolderReference)folder).branch.InnerText = value.ToString();
        }

    }

}
