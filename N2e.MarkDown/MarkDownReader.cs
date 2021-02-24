using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;
using N2e.MarkDown.Syntax;
using System.Collections.Generic;
using System.IO;
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
            models.Add(new MarkdownAnchor());
            models.Add(new MarkdownLabel());
            models.Add(new MarkdownBlockQuote());
            models.Add(new MarkdownStrikeThrough());
            models.Add(new MarkdownHorizontalRule());

            // code and inline code use the same tage, so order for model
            // will infuence the evaluation
            models.Add(new MarkdownCode());
            models.Add(new MarkdownInlineCode());
            models.Add(new MarkdownPreformatted());
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

        public void WriteHtmlContent(TextWriter stream, IMarkdownContent content)
        {
            if (stream == null || content == null) return;

            WriteHtml(stream, content.Type, content.Content, content.SubElements);
        }

        private void WriteHtml(TextWriter stream, MdType contentType, string content, IList<IMarkdownContent> subItems)
        {
            if (content.Length == 0) return;
            var i = 0;
            var nCount = 0;

            stream.RenderHtmlPrefix(contentType);

            while (i < content.Length)
            {
                var c = content[i];
                // ignore linefeed characters
                if (c == '\r') { i++;  continue; }
                if (c == '{' )
                {
                    if (ValidatePlaceholder(stream, content, ref i))
                    {
                        i++;
                        var p = content.IndexOf('}', i);
                        if (int.TryParse(content.Substring(i, p - i), out int index))
                        {
                            if (index < subItems.Count)
                            {
                                var subItem = subItems[index];
                                WriteHtml(stream, subItem.Type, subItem.Content, subItem.SubElements);
                            }
                        }
                        i = p + 1;
                    }
                }
                else
                {
                    // double line feed renders to <br />
                    if (c == '\n')
                    {
                        nCount++;
                        if (nCount==2)
                        {
                            stream.Write("<br />\n");
                            nCount = 0;
                        }
                        else
                        {
                            stream.Write('\n');
                        }
                    }                   
                    else
                    {
                        stream.Write(c);
                        nCount = 0;
                    }
                }
                i++;
            }

            stream.RenderHtmlPostFix(contentType);
        }

        private static bool ValidatePlaceholder(TextWriter stream, string content, ref int i)
        {
            // edge case detection
            if (i + 1 >= content.Length)
            {
                stream.Write('{');
                return false;
            }

            // find placeholder for subcontent
            if (content[i + 1] == '{')
            {
                // ignore placeholder
                i += 2;
                var n = 2;
                stream.Write("{{");
                while (n > 0 && i < content.Length)
                {
                    var c = content[i];
                    if (c == '}') n--;
                    stream.Write(c);
                    i++;
                }
                i--;
                return false;
            }
            return true;
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
                            goto next;
                        }
                    }
                }
                newLine = (c == '\n' || c == '\r' || (newLine && (c == ' ' || c == '\t')));
                i++;
            next:
                ;
            }
            // add completion
            sb.Append(content.Substring(start));
            value.UpdateContent(sb.ToString());

            // parse inner elements
            foreach (var item in value.SubElements)
            {
                ParseResult(item);
            }
        }
    }
}
