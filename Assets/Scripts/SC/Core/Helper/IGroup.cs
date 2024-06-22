using System.Collections.Generic;

namespace SC.Core.Helper
{
    public interface IGroup
    {
        public List<UIElementProperty> GetUIElementList { get; }
        public void AdjustGroup();
    }

    public interface IHorizontalGroup : IGroup
    {
    }
}