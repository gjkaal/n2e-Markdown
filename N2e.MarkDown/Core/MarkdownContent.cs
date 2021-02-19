using N2e.MarkDown.Abstractions;
using System.Collections.Generic;

namespace N2e.MarkDown.Core
{
    public class MarkdownContent : IMarkdownContent
    {
        public string Content { get; internal set; }
        public List<IMarkdownContent> SubElements { get; } = new List<IMarkdownContent>();
        public MdType Type { get; internal set; }
        public int CountTrigger { get; internal set; }
        public int Index { get; internal set; }
        public int Depth { get; internal set; }
        public bool Selected { get; internal set; }
        public bool StopRecursion { get; internal set; }
        public string Tag { get; internal set; }

        public void UpdateContent(string value)
        {
            Content = value;
        }

        public void UpdateDepth(int value)
        {
            Depth = value;
        }

        public void UpdateIndex(int value)
        {
            Index = value;
        }
    }
}
