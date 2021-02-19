using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;
using System;

namespace N2e.MarkDown.Syntax
{

    public class MarkdownListItem : MarkDownModel
    {
        public override char TriggerValue => '\0';
        public override MdType TypeName => MdType.ListItem;
        public override Func<string, IMarkdownContent, int, bool> Trigger => (s, _, i) =>
        {
            if (i >= s.Length - 2) return false;
            var c = s[i];
            var space = s[i + 1];
            return ((c == '-' || c == '*' || c == '+') && space == ' ');
        };

        public override IMarkdownContent ElementComplete(ref string value, ref int i)
        {
            if (i >= value.Length) return null;

            // search depth
            var space = i - 1;
            int depth = 0;
            while (space > 0 && (value[space] == ' ' || value[space] == '\t'))
            {
                depth += 1;
                space -= 1;
            }
            // skip hypen and start with space
            i += 1;            
            var result = base.ElementComplete(ref value, ref i);
            result.UpdateDepth(depth);
            return result;
        }
    }
}
