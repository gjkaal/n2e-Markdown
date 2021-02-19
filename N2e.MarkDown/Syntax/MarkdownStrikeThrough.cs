using N2e.MarkDown.Abstractions;
using N2e.MarkDown.Core;

namespace N2e.MarkDown.Syntax
{

    public class MarkdownStrikeThrough : MarkDownInline
    {
        public override char TriggerValue => '~';
        public override MdType TypeName => MdType.Strikethrough;

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

            if (countTrigger != 2) return null;

            // end count should equal count trigger
            for (var n = 0; n < countTrigger; n++)
            {
                if (blockEnd + n > value.Length) return null;
                if (emphasisCharacter != value[blockEnd + n]) return null;
            }

            var result = new MarkdownContent
            {
                Type = TypeName,
                Content = value.Substring(i + countTrigger, blockEnd - i - countTrigger),
                CountTrigger = countTrigger
            };
            i = blockEnd + countTrigger;
            return result;
        }
    }
}
