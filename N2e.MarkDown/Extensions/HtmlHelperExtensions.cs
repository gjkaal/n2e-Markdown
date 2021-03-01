
using N2e.MarkDown.Abstractions;
using System.IO;

namespace N2e.MarkDown.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static TextWriter RenderHtmlPrefix(this TextWriter stream, MdType type, int count, bool selected)
        {
            switch (type)
            {
                case MdType.Header:
                    stream.Write($"<h{count}>");
                    break;
                case MdType.BlockQuote:
                    stream.Write("<div class='block'>");
                    break;
                case MdType.HorizontalRule:
                    stream.Write("<hr />");
                    break;
                case MdType.OrderedList:
                    stream.Write("<ol>");
                    break;
                case MdType.List:
                    stream.Write("<ul>");
                    break;
                case MdType.ListItem:
                case MdType.NumberedItem:
                    stream.Write("<li>");
                    break;
                case MdType.Table:
                    stream.Write("<table>");
                    break;
                case MdType.TableHeader:
                    stream.Write("<th>");
                    break;
                case MdType.TableRow:
                    stream.Write("<tr>");
                    break;
                case MdType.TableCell:
                    stream.Write("<td>");
                    break;
                case MdType.TableCellCenter:
                    stream.Write("<td align='center'>");
                    break;
                case MdType.TableCellRight:
                    stream.Write("<td align='right'>");
                    break;
                case MdType.Strikethrough:
                    stream.Write("<del>");
                    break;
                case MdType.InlineCode:
                    stream.Write("<pre>");
                    break;
                case MdType.CheckBox:
                    stream.Write($"<div class='checkBox'><input type='checkBox'{(selected ? "checked='checked' />" : string.Empty)}>");
                    break;
                case MdType.Emphasis:
                    stream.Write($"<{(count==1? "em" : "b")}>");
                    break;
                case MdType.Code:
                    stream.Write("<div class='code'><code><pre>\n");
                    break;
            }

            return stream;
        }

        public static TextWriter RenderHtmlPostfix(this TextWriter stream, MdType type, int count)
        {
            switch (type)
            {
                case MdType.Header:
                    stream.Write($"</h{count}>");
                    break;
                case MdType.BlockQuote:
                    stream.Write("</div>");
                    break;
                case MdType.HorizontalRule:
                    // do nothing
                    break;
                case MdType.OrderedList:
                    stream.Write("</ol>");
                    break;
                case MdType.List:
                    stream.Write("</ul>");
                    break;
                case MdType.ListItem:
                case MdType.NumberedItem:
                    stream.Write("</li>");
                    break;
                case MdType.Table:
                    stream.Write("</table>");
                    break;
                case MdType.TableHeader:
                    stream.Write("</th>");
                    break;
                case MdType.TableRow:
                    stream.Write("</tr>");
                    break;
                case MdType.TableCell:
                case MdType.TableCellCenter:
                case MdType.TableCellRight:
                    stream.Write("</td>");
                    break;
                case MdType.Strikethrough:
                    stream.Write("</del>");
                    break;
                case MdType.InlineCode:
                    stream.Write("</pre>");
                    break;
                case MdType.CheckBox:
                    stream.Write("</div>");
                    break;
                case MdType.Emphasis:
                    stream.Write($"</{(count == 1 ? "em" : "b")}>");
                    break;
                case MdType.Code:
                    stream.Write("</pre></code></div>");
                    break;

            }

            return stream;
        }
    }
}
