using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;

namespace N2e.MarkDown.Syntax
{
    public class MarkdownHeader : MarkDownModel
    {
        public override char TriggerValue => '#';
        public override MdType TypeName => MdType.Header;
    }
}
