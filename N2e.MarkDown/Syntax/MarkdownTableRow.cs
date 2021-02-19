using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;
using System;
using System.Linq;
using N2e.MarkDown.Extensions;

namespace N2e.MarkDown.Syntax
{
    public class MarkdownCode: MarkDownModel
    {
        public override char TriggerValue => '\0';
        public override MdType TypeName => MdType.Code;
        public override Func<string, IMarkdownContent, int, bool> Trigger => (s, _, i) => {
            char c;
            if (i > s.Length - 3) return false;
            if (i == 0)
            {
                // edge case for md starting with code
                c = '\n';
            }
            else
            {
                c = s[i-1];
            }
            if (!(c == '\n' || c == '\r')) return false;
            if ( s.Substring(i, 3) != "```") return false;
            return true;
        };

        public override IMarkdownContent ElementComplete(ref string value, ref int i)
        {
            // find all data until end of code block
            var start = i;

            // get first line with code indicator
            var eol = value.IndexOfAny(new[] { '\n', '\r' }, start);
            if (eol < 0) return null;
            var firstLine = value.Substring(start, eol - start);
            var codeStart = value.FindNextLine(start);
            if (codeStart < 0) return null;
            var codeEnd = value.IndexOf("```", codeStart);
            if (codeEnd < 0) return null;
            var c = value[codeEnd - 1];
            if (!(c == '\r' || c == '\n')) return null;
            i = codeEnd;
            c = value[i];
            while (!(c == '\r' || c == '\n') && i<value.Length)
            {
                i++;
                c = value[i];
            }

            return new MarkdownContent
            {
                Type = TypeName,
                Tag = firstLine.Substring(firstLine.LastIndexOf('`')),
                StopRecursion = true,
                Content = value.Substring(codeStart, codeEnd - codeStart)
            };
        }

        
    }

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
