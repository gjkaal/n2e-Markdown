using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;

namespace N2e.MarkDown.Syntax
{
    public class MarkdownLabel : MarkDownInline
    {
        public override char TriggerValue => '(';
        public override char TriggerEnd => ')';
        public override MdType TypeName => MdType.Label;
        public override bool StopRecursion => true;
    }
}
