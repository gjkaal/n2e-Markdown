
using N2e.MarkDown.Abstractions;

namespace N2e.MarkDown.Core
{


    public abstract class MarkDownInline : MarkDownModel
    {
        public override bool InlineElement => true;
        public override bool SpaceRequired => false;
        public virtual char TriggerEnd => ' ';

        public override IMarkdownContent ElementComplete(ref string value, ref int i)
        {
            if (i >= value.Length) return null;

            var blockEnd = value.IndexOf(TriggerEnd, i + 1);
            if (blockEnd < 0) return null;
            if (blockEnd == i + 1) return null;
            var content = value.Substring(i+1, blockEnd - i - 1);

            // content should not contain anotrher trigger
            if (content.IndexOf(TriggerValue)>=0) return null;

            var result = new MarkdownContent
            {
                Type = TypeName,
                Content = content,
                StopRecursion = StopRecursion
            };
            i = blockEnd+1;
            return result;
        }
    }
}
