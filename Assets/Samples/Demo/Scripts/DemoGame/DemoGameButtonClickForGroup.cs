using SC.Core.Helper.Groups;
using UnityEngine;

namespace Samples.Demo.Scripts.DemoGame
{
    public class DemoGameButtonClickForGroup : MonoBehaviour
    {
        [SerializeField] private int _buttonIndex = 0;
        [SerializeField] private GroupSelector _groupSelector;

        public int ButtonIndex => _buttonIndex;

        public void Click()
        {
            _groupSelector.UpdateItemScales(_buttonIndex);
        }
    }
}