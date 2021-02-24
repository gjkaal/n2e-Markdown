
using N2e.MarkDown.Abstractions;
using System.IO;

namespace N2e.MarkDown.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static TextWriter RenderHtmlPrefix(this TextWriter stream, MdType type)
        {
            switch (type)
            {
                case MdType.Header:
                    stream.Write("<h1>");
                    break;
            }

            return stream;
        }

        public static TextWriter RenderHtmlPostfix(this TextWriter stream, MdType type)
        {
            switch (type)
            {
                case MdType.Header:
                    stream.Write("</h1>");
                    break;
            }

            return stream;
        }
    }

    public static class StringExtensions
    {
        public static string GetLine(this string value, int startPos, int endPos)
        {
            if (startPos < 0) return value.Substring(0, endPos);
            if (endPos < 0) return string.Empty;
            return value.Substring(startPos, endPos - startPos);
        }

        private readonly static char[] NewLine = new[] { '\n', '\r' };
        public static int FindNextLine(this string value, int position, bool inclusive)
        {
            var len = value.Length;
            if (len <= 0) return -1;
            if (position < 0) position = 0;
            position = value.IndexOfAny(NewLine, position);
            if (position < 0) return len;

            if (inclusive)
            {
                var c = value[position];
                while (c == '\n' || c == '\r')
                {
                    position++;
                    if (position >= len) return len;
                    c = value[position];
                }
                return position;
            }
            else
            {
                return position;
            }
            
        }

        public static int FindStartLine(this string value, int position)
        {
            if (position < 0) return -1;
            var c = value[position];
            while (!(c == '\n' || c == '\r'))
            {
                position--;
                if (position < 0) return -1;
                c = value[position];
            }
            return position;
        }
    }
}
