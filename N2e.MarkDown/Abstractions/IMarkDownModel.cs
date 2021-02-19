using System;

namespace N2e.MarkDown.Abstractions
{
    public interface IMarkDownModel
    {
        char TriggerValue { get; }
        char TriggerValueAlt { get; }
        Func<string, IMarkdownContent, int, bool> Trigger { get; }
        bool InlineElement { get; }
        IMarkdownContent ElementComplete(ref string value, ref int i);
    }
}
