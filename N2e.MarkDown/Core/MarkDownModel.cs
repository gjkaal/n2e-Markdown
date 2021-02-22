using N2e.MarkDown.Abstractions;
using System;

namespace N2e.MarkDown.Core
{
    public abstract class MarkDownModel : IMarkDownModel
    {
        public virtual bool StopRecursion => false;
        public virtual bool InlineElement => false;
        public virtual bool SpaceRequired => true;
        public abstract char TriggerValue { get; }
        public virtual char TriggerValueAlt => '\0';
        public abstract MdType TypeName { get; }
        public virtual Func<string, IMarkdownContent, int, bool> Trigger => null;        

        public virtual IMarkdownContent ElementComplete(ref string value, ref int i)
        {
            // End markdown tag with eof or cr/lf cr/lf
            var iStart = i;
            var found = false;
            var space = false;
            var countTrigger = 0;
            var eof = value.Length - 1;
            while (i < value.Length)
            {
                var c = value[i];
                if (c == TriggerValue && !space)
                {
                    countTrigger += 1;
                }
                else if (c == ' ' && !space)
                {
                    space = true;
                }
                else if (!space && SpaceRequired)
                {
                    // invalid token
                    break;
                }

                if (c == '\n' || c == '\r' && space || i == eof)
                {
                    found = true;
                    break;
                }
                else
                {
                    i++;
                }
            }

            if (i > iStart && space && found)
            {
                if (i == eof) i++;
                var substring = value.Substring(iStart, i - iStart).Substring(countTrigger);
                return new MarkdownContent
                {
                    Content = substring,
                    Type = TypeName,
                    CountTrigger = countTrigger
                };
            }
            else
            {
                return null;
            }
        }
    }
}
