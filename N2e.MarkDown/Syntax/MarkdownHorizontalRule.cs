using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;
using N2e.MarkDown.Extensions;
using System;

namespace N2e.MarkDown.Syntax
{
    public class MarkdownHorizontalRule : MarkDownModel
    {
        public override char TriggerValue => '\0';
        public override MdType TypeName => MdType.HorizontalRule;
        public override Func<string, IMarkdownContent, int, bool> Trigger => (s,p,i) => {
            if (i > s.Length - 3) return false;
            var tag = s.Substring(i, 3);
            if (tag=="---" || tag=="***" || tag == "___")
            {
                return true;
            }
            return false;
        };

        public override IMarkdownContent ElementComplete(ref string value, ref int i)
        {
            if (i >= value.Length) return null;
            var begin = value.FindStartLine(i);
            var pos = value.FindNextLine(begin+1, false);
            var tag = value.GetLine(begin, pos).Trim();
            if (tag == "---" || tag == "***" || tag == "___")
            {
                i = pos;
                return new MarkdownContent
                {
                    Type = TypeName,
                    Tag = tag,
                    StopRecursion = true
                };
            }
            return null;
        }
    }
}
