
namespace N2e.MarkDown.Core
{


    public abstract class MarkDownInline : MarkDownModel
    {
        public override bool InlineElement => true;
        public override bool SpaceRequired => false;
    }
}
