using SC.Core.Helper.ScaleHandler;
using SC.Core.SpriteCanvasAttribute;
using UnityEngine;
using UnityEngine.Events;

namespace SC.Core.Helper.Groups
{
    public class GroupSelector : MonoBehaviour
    {
        private IGroup _uiGroup;

        [Button("DebugFunction", typeof(GroupSelector), "Debug"), SerializeField]
        private int _currentSelectedIndex = 0;

        [SerializeField, Space, SCHorizontalLine(EColor.White, 2)]
        private float _selectedItemScale = 2;

        [SerializeField] private float _unselectedItemScale = 1;
        [SerializeField] private float _animationDuration;
        [SerializeField] private AnimationCurve _scaleCurve;
        private IGroupSelectorHandler _selector; // todo set

        [Space, SCHorizontalLine(EColor.White, 2)]
        public UnityEvent<int> OnSelectionChanged;

        private void Awake()
        {
            _uiGroup = GetComponent<IGroup>();
            CreateSelectorHandler();
        }

        private void CreateSelectorHandler()
        {
#if SPRITECANVAS_UNITASK_SUPPORT
            _selector = new ScaleTaskHandler();
#else
            _selector = new ScaleCoroutineHandler();
#endif
        }

        public void UpdateItemScales(int selectedIndex)
        {
            _currentSelectedIndex = selectedIndex;
            UpdateItemScales();
        }

        private void UpdateItemScales()
        {
            _selector.AdjustItemsScale(this, _animationDuration, _currentSelectedIndex, _selectedItemScale,
                _unselectedItemScale, _scaleCurve, _uiGroup, OnSelectionChanged);
        }

        public void DebugFunction(float value)
        {
            CreateSelectorHandler();
            _uiGroup = GetComponent<IGroup>();
            UpdateItemScales();
        }
    }
}