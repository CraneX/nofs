using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Nofs.Net.Utils
{
    public sealed class StringUtil
    {
        private StringUtil()
        {
        }

        public static bool ContainsIgnoreCase(string strA, string strB)
        {
            if (string.IsNullOrEmpty(strA) || string.IsNullOrEmpty(strB))
            {
                return false;
            }
            else
            {
                return strA.IndexOf(strB, StringComparison.OrdinalIgnoreCase) != -1;
            }
        }

        public static bool StartsWithIgnoreCase(string strA, string strB)
        {
            Debug.Assert(strA != null);
            Debug.Assert(strB != null);

            return strA.StartsWith(strB, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWithIgnoreCase(string strA, string strB)
        {
            Debug.Assert(strA != null);
            Debug.Assert(strB != null);

            return strA.EndsWith(strB, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EqualsIgnoreCase(string strA, string strB)
        {
            Debug.Assert(strA != null);
            Debug.Assert(strB != null);

            return (string.Compare(strA, strB, true, CultureInfo.InvariantCulture) == 0);
        }

        public static bool EqualsUsingShorterLength(string strA, string strB)
        {
            Debug.Assert(strA != null);
            Debug.Assert(strB != null);

            if (strA.Length > strB.Length)
            {
                return strA.Substring(0, strB.Length) == strB;
            }
            else if (strA.Length < strB.Length)
            {
                return strB.Substring(0, strA.Length) == strA;
            }
            else
            {
                return strA == strB;
            }
        }

        public static string Join(string separator, IEnumerable values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string val in values)
            {
                sb.Append(val);
                sb.Append(separator);
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length - separator.Length, separator.Length);
            return sb.ToString();
        }

        public static IList<string> Split(string text, char separator)
        {
            string[] arr = text.Split(separator);
            IList<string> list = new List<string>();
            foreach (string s in arr)
            {
                list.Add(s);
            }
            return list;
        }

        public static string RepeatChar(char chr, int numTimes)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < numTimes; ++i)
            {
                s.Append(chr);
            }
            return s.ToString();
        }

        public static int ASCIICode(string text)
        {
            if (text.Length == 0)
            {
                return 0;
            }
            else if (text.Length > 1)
            {
                text = text[0].ToString();
            }

            return (int)System.Convert.ToChar(text);
        }


        public static string ASCIICodeChar(int asciiCode)
        {
            return System.Convert.ToChar(asciiCode).ToString();
        }

        public static string Right(string text, int length)
        {
            if (text.Length <= length)
            {
                return text;
            }
            else
            {
                return text.Substring(text.Length - length);
            }
        }

        public static string Left(string text, int length)
        {
            if (text.Length <= length)
            {
                return text;
            }
            else
            {
                return text.Substring(0, length);
            }
        }

        public static string Mid(string text, int index, int length)
        {
            if (text.Length <= length)
            {
                return text;
            }
            else
            {
                return text.Substring(index, length);
            }
        }

        public static string RemoveNoneDigits(string text)
        {
            Regex reg = new Regex("[^0-9|.|-|,]");
            return reg.Replace(text, string.Empty);
        }

        public static bool IsTrue(string text)
        {
            return !string.IsNullOrEmpty(text)
                && (text == "1" || string.Compare(text, "true", CultureInfo.InvariantCulture, CompareOptions.OrdinalIgnoreCase) == 0);
        }

        public static bool IsTrue(string text, bool defaultValue)
        {
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }
            else
            {
                return IsTrue(text);
            }
        }

        public static string Abbreviate(string text, int length)
        {
            if (!string.IsNullOrEmpty(text) && text.Length > length)
            {
                return text.Substring(0, length) + "...";
            }
            return text;
        }

        /// <summary>
        /// Convert colot from hexadecimal to RGB
        /// </summary>
        /// <param name="color">eg.FFFFFF</param>
        /// <returns>RGB color(R,G,B,)</returns>
        public static string Hex2RGB(string color)
        {
            int red = 0, green = 0, blue = 0;
            char[] rgb;
            color = color.TrimStart('#');
            color = Regex.Replace(color.ToLower(), "[g-zG-Z]", "");
            switch (color.Length)
            {
                case 3:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0] + rgb[0].ToString(), 16);
                    green = Convert.ToInt32(rgb[1] + rgb[1].ToString(), 16);
                    blue = Convert.ToInt32(rgb[2] + rgb[2].ToString(), 16);
                    break;
                case 6:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[1].ToString(), 16);
                    green = Convert.ToInt32(rgb[2].ToString() + rgb[3].ToString(), 16);
                    blue = Convert.ToInt32(rgb[4].ToString() + rgb[5].ToString(), 16);
                    break;
            }
            return "rgb(" + red.ToString() + "," + green.ToString() + "," + blue.ToString() + ")";
        }

        public static string RGB2Hex(string rgbColor)
        {
            string text = StringUtil.RemoveNoneDigits(rgbColor);
            string[] arr = text.Split(',');
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = string.Format(CultureInfo.InvariantCulture, "{0:X2}", int.Parse(arr[i]));
            }

            return string.Join("", arr);
        }

        public static string EncodeUrl(string text)
        {
            return text.Replace("?", "%3F").Replace("&", "%26").Replace("=", "%3D");
        }

        public static string Max(string strA, string strB)
        {
            if (string.Compare(strA, strB, CultureInfo.InvariantCulture, CompareOptions.Ordinal) >= 0)
            {
                return strA;
            }
            else
            {
                return strB;
            }
        }

        public static string Min(string strA, string strB)
        {
            if (string.Compare(strA, strB, CultureInfo.InvariantCulture, CompareOptions.Ordinal) <= 0)
            {
                return strA;
            }
            else
            {
                return strB;
            }
        }

        public static string ReplaceIgnoreCase(string inputStr, string strA, string strB)
        {
            Regex reg = new Regex(strA,
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return reg.Replace(inputStr, strB);
        }

        public static string Trim(string text)
        {
            return string.IsNullOrEmpty(text) ? text : text.Trim();
        }

        public static bool IsValidGUID(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Regex reg = new Regex(@"^[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}$", RegexOptions.IgnoreCase);
                return reg.IsMatch(id);
            }
            return false;
        }

        public static string VerifyFileName(string fileName, string sep)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (sep == null)
                {
                    sep = string.Empty;
                }

                Regex reg = new Regex(@"[\/:*?<>|]", RegexOptions.IgnoreCase);
                fileName = reg.Replace(fileName, sep);
            }
            return fileName;
        }

        public static string TrimEnd(string s)
        {
            return string.IsNullOrEmpty(s) ? string.Empty : s.TrimEnd();
        }

        public static string TrimStart(string s)
        {
            return string.IsNullOrEmpty(s) ? string.Empty : s.TrimStart();
        }

        public static int ConvertToInt(string s, int defaultValue)
        {
            int value;
            if (string.IsNullOrEmpty(s))
            {
                return defaultValue;
            }
            else if (int.TryParse(s, out value))
            {
                return value;
            }

            return defaultValue;
        }

        public static string ConvertBoolean(bool b)
        {
            return b ? "1" : "0";
        }

        public static string EncodeString(string s)
        {
            return Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(s));
        }

        public static string DecodeString(string s)
        {
            try
            {
                return UTF8Encoding.UTF8.GetString(Convert.FromBase64String(s));
            }
            catch (System.FormatException)
            {
                return s;
            }
        }

        public static bool IsNumber(string input)
        {
            Regex reg = new Regex(@"[\d]+");
            return reg.IsMatch(input);
        }
    }
}
