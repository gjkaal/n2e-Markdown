
namespace N2e.MarkDown.Extensions
{
    public static class StringExtensions
    {
        private readonly static char[] NewLine = new[] { '\n', '\r' };
        public static int FindNextLine(this string value, int position)
        {
            var len = value.Length;
            position = value.IndexOfAny(NewLine, position);
            if (position < 0) return -1;
            var c = value[position];
            while (c == '\n' || c == '\r')
            {
                position++;
                if (position >= len) return -1;
                c = value[position];
            }
            return position;
        }
    }
}
