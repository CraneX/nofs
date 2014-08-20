using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Nofs.Net.Utils
{
    /**
    * The {@code StringTokenizer} class allows an application to break a string
    * into tokens by performing code point comparison. The {@code StringTokenizer}
    * methods do not distinguish among identifiers, numbers, and quoted strings,
    * nor do they recognize and skip comments.
    * <p>
    * The set of delimiters (the codepoints that separate tokens) may be specified
    * either at creation time or on a per-token basis.
    * <p>
    * An instance of {@code StringTokenizer} behaves in one of three ways,
    * depending on whether it was created with the {@code returnDelimiters} flag
    * having the value {@code true} or {@code false}:
    * <ul>
    * <li>If returnDelims is {@code false}, delimiter code points serve to separate
    * tokens. A token is a maximal sequence of consecutive code points that are not
    * delimiters.
    * <li>If returnDelims is {@code true}, delimiter code points are themselves
    * considered to be tokens. In this case a token will be received for each
    * delimiter code point.
    * </ul>
    * <p>
    * A token is thus either one delimiter code point, or a maximal sequence of
    * consecutive code points that are not delimiters.
    * <p>
    * A {@code StringTokenizer} object internally maintains a current position
    * within the string to be tokenized. Some operations advance this current
    * position past the code point processed.
    * <p>
    * A token is returned by taking a Substring of the string that was used to
    * create the {@code StringTokenizer} object.
    * <p>
    * Here's an example of the use of the default delimiter {@code StringTokenizer}
    * : <blockquote>
    *
    * <pre>
    * StringTokenizer st = new StringTokenizer(&quot;this is a test&quot;);
    * while (st.hasMoreTokens()) {
    *     println(st.nextToken());
    * }
    * </pre>
    *
    * </blockquote>
    * <p>
    * This prints the following output: <blockquote>
    *
    * <pre>
    *     this
    *     is
    *     a
    *     test
    * </pre>
    *
    * </blockquote>
    * <p>
    * Here's an example of how to use a {@code StringTokenizer} with a user
    * specified delimiter: <blockquote>
    *
    * <pre>
    * StringTokenizer st = new StringTokenizer(
    *         &quot;this is a test with supplementary characters \ud800\ud800\udc00\udc00&quot;,
    *         &quot; \ud800\udc00&quot;);
    * while (st.hasMoreTokens()) {
    *     println(st.nextToken());
    * }
    * </pre>
    *
    * </blockquote>
    * <p>
    * This prints the following output: <blockquote>
    *
    * <pre>
    *     this
    *     is
    *     a
    *     test
    *     with
    *     supplementary
    *     characters
    *     \ud800
    *     \udc00
    * </pre>
    *
    * </blockquote>
    */
    internal sealed class StringTokenizer 
    {

        private string str;

        private string delimiters;

        private bool returnDelimiters;

        private int position;

        /**
         * Constructs a new {@code StringTokenizer} for the parameter string using
         * whitespace as the delimiter. The {@code returnDelimiters} flag is set to
         * {@code false}.
         * 
         * @param string
         *            the string to be tokenized.
         */
        public StringTokenizer(string str)
            : this(str, " \t\n\r\f", false)
        { //$NON-NLS-1$
        }

        /**
         * Constructs a new {@code StringTokenizer} for the parameter string using
         * the specified delimiters. The {@code returnDelimiters} flag is set to
         * {@code false}. If {@code delimiters} is {@code null}, this constructor
         * doesn't throw an {@code Exception}, but later calls to some methods might
         * throw a {@code NullPointerException}.
         * 
         * @param string
         *            the string to be tokenized.
         * @param delimiters
         *            the delimiters to use.
         */
        public StringTokenizer(string str, string delimiters)
            : this(str, delimiters, false)
        {
        }

        /**
         * Constructs a new {@code StringTokenizer} for the parameter string using
         * the specified delimiters, returning the delimiters as tokens if the
         * parameter {@code returnDelimiters} is {@code true}. If {@code delimiters}
         * is null this constructor doesn't throw an {@code Exception}, but later
         * calls to some methods might throw a {@code NullPointerException}.
         * 
         * @param string
         *            the string to be tokenized.
         * @param delimiters
         *            the delimiters to use.
         * @param returnDelimiters
         *            {@code true} to return each delimiter as a token.
         */
        public StringTokenizer(string str, string delimiters,
                bool returnDelimiters)
        {
            if (str != null)
            {
                this.str = str;
                this.delimiters = delimiters;
                this.returnDelimiters = returnDelimiters;
                this.position = 0;
            }
            else
                throw new NullReferenceException();
        }

        /**
         * Returns the number of unprocessed tokens remaining in the string.
         * 
         * @return number of tokens that can be retreived before an {@code
         *         Exception} will result from a call to {@code nextToken()}.
         */
        public  int countTokens()
        {
            int count = 0;
            bool inToken = false;
            for (int i = position, length = str.Length; i < length; i++)
            {
                if (delimiters.IndexOf(str[i], 0) >= 0)
                {
                    if (returnDelimiters)
                        count++;
                    if (inToken)
                    {
                        count++;
                        inToken = false;
                    }
                }
                else
                {
                    inToken = true;
                }
            }
            if (inToken)
                count++;
            return count;
        }

        /**
         * Returns {@code true} if unprocessed tokens remain. This method is
         * implemented in order to satisfy the {@code Enumeration} interface.
         * 
         * @return {@code true} if unprocessed tokens remain.
         */
        public  bool hasMoreElements()
        {
            return hasMoreTokens();
        }

        /**
         * Returns {@code true} if unprocessed tokens remain.
         * 
         * @return {@code true} if unprocessed tokens remain.
         */
        public bool hasMoreTokens()
        {
            if (delimiters == null)
            {
                throw new NullReferenceException();
            }
            int length = str.Length;
            if (position < length)
            {
                if (returnDelimiters)
                    return true; // there is at least one character and even if
                // it is a delimiter it is a token

                // otherwise find a character which is not a delimiter
                for (int i = position; i < length; i++)
                    if (delimiters.IndexOf(str[i], 0) == -1)
                        return true;
            }
            return false;
        }

        /**
         * Returns the next token in the string as an {@code object}. This method is
         * implemented in order to satisfy the {@code Enumeration} interface.
         * 
         * @return next token in the string as an {@code object}
         * @throws NoSuchElementException
         *                if no tokens remain.
         */
        public object nextElement()
        {
            return nextToken();
        }

        /**
         * Returns the next token in the string as a {@code string}.
         * 
         * @return next token in the string as a {@code string}.
         * @throws NoSuchElementException
         *                if no tokens remain.
         */
        public string nextToken()
        {
            if (delimiters == null)
            {
                throw new NullReferenceException();
            }
            int i = position;
            int length = str.Length;

            if (i < length)
            {
                if (returnDelimiters)
                {
                    if (delimiters.IndexOf(str[position], 0) >= 0)
                        return str[position++].ToString();
                    for (position++; position < length; position++)
                        if (delimiters.IndexOf(str[position], 0) >= 0)
                            return str.Substring(i, position);
                    return str.Substring(i);
                }

                while (i < length && delimiters.IndexOf(str[i], 0) >= 0)
                    i++;
                position = i;
                if (i < length)
                {
                    for (position++; position < length; position++)
                        if (delimiters.IndexOf(str[position], 0) >= 0)
                            return str.Substring(i, position);
                    return str.Substring(i);
                }
            }
            throw new NullReferenceException();
        }

        /**
         * Returns the next token in the string as a {@code string}. The delimiters
         * used are changed to the specified delimiters.
         * 
         * @param delims
         *            the new delimiters to use.
         * @return next token in the string as a {@code string}.
         * @throws NoSuchElementException
         *                if no tokens remain.
         */
        public string nextToken(string delims)
        {
            this.delimiters = delims;
            return nextToken();
        }

    }
}
