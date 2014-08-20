using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Nofs.Net.Utils
{
    public sealed class StreamUtil
    {
        private StreamUtil()
        {
        }

        public static string SerializeToString(object o)
        {
            try
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.CloseOutput = true;
                xws.Encoding = Encoding.UTF8;
                xws.Indent = true;

                bool xmlSerializer = true;

                foreach (var item in o.GetType().GetCustomAttributes(true))
                {
                    if (item.GetType() == typeof(DataContractAttribute))
                    {
                        xmlSerializer = false;
                        break;
                    }
                }

                if (xmlSerializer)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (XmlWriter tw = XmlTextWriter.Create(memoryStream, xws))
                        {
                            XmlSerializer xs = new XmlSerializer(o.GetType());
                            xs.Serialize(tw, o);
                        }
                        return UTF8ByteArrayToString(memoryStream.ToArray());
                    }
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (XmlWriter tw = XmlTextWriter.Create(memoryStream, xws))
                        {
                            DataContractSerializer serializer = new DataContractSerializer(o.GetType());
                            serializer.WriteObject(tw, o);
                        }
                        return UTF8ByteArrayToString(memoryStream.ToArray());
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            const string Blank = "\r\n";
            return new UTF8Encoding(true, true).GetString(characters) + Blank;
        }

        /// <summary>
        /// Converts the String to UTF8 Byte array and is used in De serialization
        /// </summary>
        /// <param name="pXmlString"></param>
        /// <returns></returns>
        private static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            return new UTF8Encoding(true, true).GetBytes(pXmlString);
        }

        public static bool SerializeToFile(object graph, string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                using (FileStream fs = File.Create(fileName))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, graph);
                    fs.Close();
                }
            }
            catch
            {
            }
            return false;
        }

        public static T Deserialize<T>(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using (FileStream fs = File.OpenRead(fileName))
                    {
                        return StreamUtil.Deserialize<T>(fs);
                    }
                }
            }
            catch
            {
            }
            return default(T);
        }

        public static T Deserialize<T>(Stream stream)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(stream);
            }
            catch
            {
                // do nothing, just ignore any possible errors
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }

            }
            return default(T);
        }
    }
}
