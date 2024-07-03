using System.Collections.Generic;
using SC.Core.UI;

namespace SC.Core.Helper
{
    public interface IGroup
    {
        public List<GroupElementProperty> GetUIElementList { get; }
        public void AdjustGroup();
        public UIElement GetUIElement { get; }
    }

    public interface IHorizontalGroup : IGroup
    {
    }
}