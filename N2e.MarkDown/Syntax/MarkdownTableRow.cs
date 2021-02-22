using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;
using System;
using System.Linq;
using N2e.MarkDown.Extensions;

namespace N2e.MarkDown.Syntax
{
    public class MarkdownTableRow : MarkDownModel
    {
        public override char TriggerValue => '|';
        public override MdType TypeName => MdType.TableRow;

        public override IMarkdownContent ElementComplete(ref string value, ref int i)
        {
            // Get line
            var eol = value.IndexOfAny(new[] { '\n', '\r' }, i);
            if (eol < 0) return null;

            // Trim and remove optional trailing pipe
            var line = value.Substring(i, eol - i).Trim();
            if (line.EndsWith("|"))
            {
                line = line.Substring(0, line.Length - 1);
            }

            var columns = line.Count(c => c == '|') + 1;
            i = eol;
            var result = new MarkdownContent
            {
                Depth = columns,
                Type = TypeName,
                // skip leading pipe character
                Content = line.Substring(1)
            };
            if (System.Text.RegularExpressions.Regex.IsMatch(result.Content, "^[\\s-\\|:]+$"))
            {
                result.Type = MdType.SplitRow;
            }
            return result;
        }
    }
}
