using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;

namespace N2e.MarkDown.Syntax
{

    public class MarkdownBlockQuote : MarkDownModel
    {
        public override char TriggerValue => '>';
        public override MdType TypeName => MdType.BlockQuote;
        public override bool StopRecursion => true;
    }
}
