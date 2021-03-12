using System.Collections.Generic;

namespace N2e.MarkDown.Abstractions
{
    public interface IMarkdownContent
    {
        string Content { get; }
        int CountTrigger { get; }
        int Depth { get; }
        int Index { get; }
        bool Selected { get; }
        bool StopRecursion { get; }
        List<IMarkdownContent> SubElements { get; }
        MdType Type { get; }

        void UpdateDepth(int depth);
        void UpdateIndex(int intValue);
        void UpdateContent(string v);
        void UpdateType(MdType type);
    }
}