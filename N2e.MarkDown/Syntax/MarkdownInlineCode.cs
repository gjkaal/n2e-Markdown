using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;

namespace N2e.MarkDown.Syntax
{
    public class MarkdownInlineCode : MarkDownInline
    {
        public override char TriggerValue => '`';
        public override MdType TypeName => MdType.InlineCode;

        public override IMarkdownContent ElementComplete(ref string value, ref int i)
        {
            var blockEnd = value.IndexOf('`', i + 1);
            if (blockEnd < 0) return null;
            var result = new MarkdownContent
            {
                Type = TypeName,
                Content = value.Substring(i, blockEnd - i),
                StopRecursion = true
            };
            i = blockEnd + 1;
            return result;
        }
    }
}
