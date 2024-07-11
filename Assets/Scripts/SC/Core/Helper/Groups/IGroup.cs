using System.Collections.Generic;
using SC.Core.UI;

namespace SC.Core.Helper.Groups
{
    public interface IGroup
    {
        public List<GroupElementProperty> GetUIElementList { get; }
        public UIElement GetUIElement { get; }
        public void AdjustGroup();
    }

    public interface IHorizontalGroup : IGroup
    {
    }
}