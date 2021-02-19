using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;
using System;

namespace N2e.MarkDown.Syntax
{

    public class MarkdownTableCell : MarkDownModel
    {
        public override char TriggerValue => '\0';
        public override MdType TypeName => MdType.TableCell;
        public override Func<string, IMarkdownContent, int, bool> Trigger => (s, p, _) => {
            // should be a single line, starting with a space, 
            // as subelement from a table
            if (p == null) return false;
            if (!(p.Type == MdType.TableRow || p.Type == MdType.SplitRow)) return false;
            if (s.IndexOfAny(new[] { '\r', '\n' }) >= 0) return false;
            if (s[0] != ' ') return false;
            return true;
        };

        public override IMarkdownContent ElementComplete(ref string value, ref int i)
        {
            var columnEnd = value.IndexOf('|', i);
            if (columnEnd < 0) columnEnd = value.Length;
            if (columnEnd <= 0) return null;
            var content = value.Substring(i, columnEnd - i);
            i = Math.Min(columnEnd + 1, value.Length);
            var result = new MarkdownContent
            {
                Content = content.Trim(),
                Type = MdType.TableCell,
            };
            var leftAlign = result.Content.StartsWith(":---");
            var rightAlign = result.Content.EndsWith("---:");
            if (leftAlign && rightAlign)
            {
                result.Type = MdType.TableCellCenter;
            }
            else if (rightAlign)
            {
                result.Type = MdType.TableCellRight;
            }

            return result;
        }
    }
}
