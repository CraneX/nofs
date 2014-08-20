using System.Text;

namespace Nofs.Net.Utils
{
    internal sealed class XmlUtil
    {
        private XmlUtil()
        {
        }

        public static string DecodeXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return string.Empty;
            }
            else
            {
                return xml.Replace("&amp;", "&")
                    .Replace("&gt;", ">")
                    .Replace("&lt;", "<")
                    .Replace("&apos;", "'")
                    .Replace("&quot;", "\"");
            }
        }

        /// <summary>
        /// Encode string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncodeXml(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            else
            {
                return EncodeXmlString(StringUtil.Trim(text), true, true, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bLtGt"></param>
        /// <param name="bQuote"></param>
        /// <param name="bApos"></param>
        /// <returns></returns>
        private static string EncodeXmlString(string text, bool bLtGt, bool bQuote, bool bApos)
        {
            StringBuilder enCodeText = new StringBuilder(text);
            //encode
            enCodeText = enCodeText.Replace("&", "&amp;");
            if (bApos)
            {
                enCodeText = enCodeText.Replace("'", "&apos;");
            }
            if (bQuote)
            {
                enCodeText = enCodeText.Replace("\"", "&quot;");
            }
            if (bLtGt)
            {
                enCodeText = enCodeText.Replace("<", "&lt;").Replace(">", "&gt;");
            }

            return enCodeText.ToString();
        }

    }
}
