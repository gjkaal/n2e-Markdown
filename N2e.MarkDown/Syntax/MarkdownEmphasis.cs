using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;

namespace N2e.MarkDown.Syntax
{

    public class MarkdownEmphasis : MarkDownInline
    {
        public override char TriggerValue => '_';
        public override char TriggerValueAlt => '*';
        public override MdType TypeName => MdType.Emphasis;

        public override IMarkdownContent ElementComplete(ref string value, ref int i)
        {
            var iStart = i;
            var emphasisCharacter = value[iStart];
            var countTrigger = 0;
            while (value[iStart] == emphasisCharacter)
            {
                iStart++;
                countTrigger++;
            }

            var blockEnd = value.IndexOf(emphasisCharacter, iStart);
            if (blockEnd < 0) return null;

            // end count should equal count trigger
            for (var n = 0; n < countTrigger; n++)
            {
                if (blockEnd + n > value.Length) return null;
                if (emphasisCharacter != value[blockEnd + n]) return null;
            }

            var result = new MarkdownContent
            {
                Type = countTrigger == 1 ? TypeName : MdType.Bold,
                Content = value.Substring(i + countTrigger, blockEnd - i - countTrigger),
                CountTrigger = countTrigger
            };
            i = blockEnd + countTrigger;
            return result;
        }
    }
}
