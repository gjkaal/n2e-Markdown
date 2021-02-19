using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;
using System;

namespace N2e.MarkDown.Syntax
{

    public class MarkdownNumberedList : MarkDownModel
    {
        public override char TriggerValue => '\0';
        public override Func<string, IMarkdownContent, int, bool> Trigger => ((s, p, i) => {
            if (i >= s.Length - 1) return false;
            var c = s[i];
            if ("0123456789".IndexOf(c) <= 0) return false;
            var dot = s.IndexOf('.', i);
            if (dot >= s.Length - 1) return false;
            if (dot > 0 && s[dot + 1] == ' ')
            {
                var number = s.Substring(i, dot - i);
                return int.TryParse(number, out _);
            }
            else
            {
                return false;
            }
        });
        public override MdType TypeName => MdType.NumberedItem;

        public override IMarkdownContent ElementComplete(ref string value, ref int i)
        {
            // Get numbered value
            var dot = value.IndexOf('.', i);
            var number = value.Substring(i, dot - i);
            i = dot + 1;
            var result = base.ElementComplete(ref value, ref i);
            if (int.TryParse(number, out var intValue))
            {
                result.UpdateIndex(intValue);
            }           
            return result;
        }
    }
}
