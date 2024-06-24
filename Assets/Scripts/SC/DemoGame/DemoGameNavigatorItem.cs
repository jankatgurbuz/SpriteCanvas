using TMPro;
using UnityEngine;

namespace SC.DemoGame
{
    public class DemoGameNavigatorItem : MonoBehaviour
    {
        [SerializeField] private DemoGameButtonClickForGroup _button;
        [SerializeField] private TextMeshPro _tmpProText;

        public void OnItemSelectionChanged(int index)
        {
            _tmpProText.gameObject.SetActive(_button.ButtonIndex == index);
        }
    }
}