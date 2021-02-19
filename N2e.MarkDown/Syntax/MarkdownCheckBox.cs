using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;

namespace N2e.MarkDown.Syntax
{

    public class MarkdownCheckBox : MarkDownInline
    {
        public override char TriggerValue => '[';
        public override MdType TypeName => MdType.CheckBox;

        public override IMarkdownContent ElementComplete(ref string value, ref int i)
        {
            var blockEnd = i + 2;
            if (blockEnd > value.Length) return null;
            if (value[blockEnd] != ']') return null;
            var result = new MarkdownContent
            {
                Type = TypeName,
                Content = value[i + 1].ToString(),
                Selected = value[i + 1] != ' '
            };
            i = blockEnd + 1;
            return result;
        }
    }
}
