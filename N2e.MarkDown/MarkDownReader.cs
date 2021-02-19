using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;
using N2e.MarkDown.Syntax;
using System.Collections.Generic;
using System.Text;

namespace N2e.MarkDown
{
    public class MarkDownReader
    {
        private readonly List<IMarkDownModel> models = new List<IMarkDownModel>();
        public MarkDownReader()
        {
            models.Add(new MarkdownHeader());
            models.Add(new MarkdownListItem());
            models.Add(new MarkdownNumberedList());
            models.Add(new MarkdownTableRow());
            models.Add(new MarkdownTableCell());
            models.Add(new MarkdownCheckBox());
            models.Add(new MarkdownEmphasis());
            models.Add(new MarkdownStrikeThrough());
            models.Add(new MarkdownInlineCode());
        }

        public IMarkdownContent Parse(string value)
        {
            var result = new MarkdownContent
            {
                Type = MdType.Document,
                Content = value,
                Index = 0,
            };
            if (string.IsNullOrEmpty(value)) return result;

            // get outer element
            ParseResult(result);
            return result;
        }

        private void ParseResult(IMarkdownContent value)
        {
            if (value.StopRecursion) return;

            var sb = new StringBuilder();
            var start = 0;
            var i = 0;
            var newLine = true;
            var n = 0;

            var content = value.Content;
            while (i < content.Length)
            {
                var c = content[i];
                foreach (var model in models)
                {
                    if ((model.TriggerValue == c || model.TriggerValueAlt == c || model.Trigger != null && model.Trigger(content, value, i))
                     && (newLine && !model.InlineElement || model.InlineElement))
                    {
                        var iCurrent = i;
                        var subSet = model.ElementComplete(ref content, ref i);
                        if (subSet != null)
                        {
                            sb.Append(content.Substring(start, iCurrent - start));
                            sb.Append($"{{{n}}}");
                            value.SubElements.Add(subSet);
                            n++;
                            start = i;
                            if (i < content.Length) c = content[i];
                        }
                    }
                }

                newLine = (c == '\n' || c == '\r' || (newLine && (c == ' ' || c == '\t')));
                i++;
            }
            // add completion
            sb.Append(content.Substring(start));
            value.UpdateContent( sb.ToString());

            // parse inner elements
            foreach (var item in value.SubElements)
            {
                ParseResult(item);
            }
        }
    }
}
