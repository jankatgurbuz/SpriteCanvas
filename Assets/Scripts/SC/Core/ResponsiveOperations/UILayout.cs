using SC.Core.Helper.UIElementHelper;
using UnityEngine;

namespace SC.Core.ResponsiveOperations
{
    public abstract class UILayout : ResponsiveOperation
    {
        private readonly Vector3 _positionOffset;

        protected UILayout(Vector3 positionOffset)
        {
            _positionOffset = positionOffset;
        }

        public override void AdjustUI(ResponsiveUIProp prop)
        {
            prop.UiItemTransform.localScale = AdjustScale(prop);
            prop.UiItemTransform.position = AdjustPosition(prop, _positionOffset);
            prop.UiItemTransform.rotation = AdjustRotation(prop);
        }
    }

    public abstract class UILayoutStretch : ResponsiveOperation
    {
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10_000;
        private Vector2 _positionOffset { get; }
        private Vector2 _scaleAnchor { get; }

        protected UILayoutStretch(Vector2 positionOffset, Vector2 scaleAnchor)
        {
            _positionOffset = positionOffset;
            _scaleAnchor = scaleAnchor;
        }

        public override void AdjustUI(ResponsiveUIProp prop)
        {
            prop.UiItemTransform.localScale = AdjustScale(prop, _edgeOffset, _maxSize, _scaleAnchor);
            prop.UiItemTransform.position = AdjustPosition(prop, _positionOffset);
            prop.UiItemTransform.rotation = AdjustRotation(prop);
        }
    }

    public class TopCenter : UILayout
    {
        public TopCenter() : base(new Vector2(0, 0.5f))
        {
        }
    }

    public class TopRight : UILayout
    {
        public TopRight() : base(new Vector2(0.5f, 0.5f))
        {
        }
    }

    public class TopLeft : UILayout
    {
        public TopLeft() : base(new Vector2(-0.5f, 0.5f))
        {
        }
    }

    public class BottomCenter : UILayout
    {
        public BottomCenter() : base(new Vector2(0, -0.5f))
        {
        }
    }

    public class BottomRight : UILayout
    {
        public BottomRight() : base(new Vector2(0.5f, -0.5f))
        {
        }
    }

    public class BottomLeft : UILayout
    {
        public BottomLeft() : base(new Vector2(-0.5f, -0.5f))
        {
        }
    }

    public class Center : UILayout
    {
        public Center() : base(new Vector2(0, 0))
        {
        }
    }

    public class TopStretch : UILayoutStretch
    {
        public TopStretch() : base(new Vector2(0, 0.5f), new Vector2(1, 0))
        {
        }
    }

    public class BottomStretch : UILayoutStretch
    {
        public BottomStretch() : base(new Vector2(0, -0.5f), new Vector2(1, 0))
        {
        }
    }

    public class RightStretch : UILayoutStretch
    {
        public RightStretch() : base(new Vector2(0.5f, 0), new Vector2(0, 1))
        {
        }
    }

    public class LeftStretch : UILayoutStretch
    {
        public LeftStretch() : base(new Vector2(-0.5f, 0), new Vector2(0, 1))
        {
        }
    }

    public class FullStretch : UILayoutStretch
    {
        public FullStretch() : base(new Vector2(0, 0), new Vector2(0, 0))
        {
        }
    }
}