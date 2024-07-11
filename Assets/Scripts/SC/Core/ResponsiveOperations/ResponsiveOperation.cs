using SC.Core.Helper.UIElementHelper;
using UnityEngine;
using UnityEngine.Serialization;

namespace SC.Core.ResponsiveOperations
{
    public abstract class ResponsiveOperation : IResponsiveOperation
    {
        [SerializeField] private Vector3 _localScale = new(1, 1, 1);
        [SerializeField] private float _topOffset;
        [SerializeField] private bool _isOffsetProportionalToScreenWidth;
        [SerializeField] private float _horizontalOffset;
        public Vector3 GetLocalScale() => _localScale;
        public abstract void AdjustUI(ResponsiveUIProp prop);

        protected Vector3 AdjustScale(ResponsiveUIProp prop)
        {
            var scale = GetScale(prop.Balance);
            scale = GetLocalScaleIgnoringParent(prop.UiItemTransform, scale);
            scale = ScaleWithGroup(scale, prop.GroupAxisConstraint, prop.UiItemTransform.localScale);
            scale = IgnoreScale(scale, prop.IgnoreXScale, prop.IgnoreYScale, prop.UiItemTransform);
            return scale;
        }

        protected Vector3 AdjustScale(ResponsiveUIProp prop, float edgeOffset, float maxSize, Vector2 pivot)
        {
            var scale = GetScale(prop.Height, prop.Width, prop.UiItemSize, prop.Balance, edgeOffset, maxSize, pivot);
            scale = GetLocalScaleIgnoringParent(prop.UiItemTransform, scale);
            scale = ScaleWithGroup(scale, prop.GroupAxisConstraint, prop.UiItemTransform.localScale);
            scale = IgnoreScale(scale, prop.IgnoreXScale, prop.IgnoreYScale, prop.UiItemTransform);
            return scale;
        }

        protected Vector3 AdjustPosition(ResponsiveUIProp prop, Vector2 pivot)
        {
            return GetPosition(prop.Height, prop.Width, prop.UiItemSize,
                prop.UiItemTransform, prop.Balance, prop.ReferencePosition, pivot,
                prop.IgnoreXPosition, prop.IgnoreYPosition, prop.Camera);
        }

        protected Quaternion AdjustRotation(ResponsiveUIProp prop)
        {
            return prop.Camera.transform.rotation;

            //* Quaternion.Euler(_rotation);
        }

        private Vector3 GetPosition(float screenHeight, float screenWidth,
            Vector3 uiItemSize, Transform uiItemTransform, float balance, Vector3 referencePosition,
            Vector2 positionFactors, bool ignoreXPosition, bool ignoreYPosition, Camera camera)
        {
            var scale = uiItemTransform.localScale;
            scale = GetScaleRelativeToParent(uiItemTransform, scale);

            var scaledHeight = uiItemSize.y * scale.y;
            var scaledWidth = uiItemSize.x * scale.x;

            var offset = !_isOffsetProportionalToScreenWidth
                ? _horizontalOffset
                : _horizontalOffset * screenWidth / balance;

            var x = positionFactors.x * screenWidth - scaledWidth * positionFactors.x - offset *
                Mathf.Sign(positionFactors.x) * balance;

            var y = positionFactors.y * screenHeight - scaledHeight * positionFactors.y -
                    _topOffset * Mathf.Sign(positionFactors.y) * balance;

            var localPosition = new Vector3(x, y, 0);
            var rotatedPosition = camera.transform.rotation * localPosition;

            var position = rotatedPosition + referencePosition;

            var lp = new Vector3(
                !ignoreXPosition ? position.x : uiItemTransform.position.x,
                !ignoreYPosition ? position.y : uiItemTransform.position.y,
                position.z);

            return lp;
        }

        private Vector3 GetScale(float balance)
        {
            return _localScale * balance;
        }

        private Vector3 GetLocalScaleIgnoringParent(Transform uiItemTransform, Vector3 scale)
        {
            if (uiItemTransform.parent == null) return scale;

            var parentScale = uiItemTransform.parent.lossyScale;
            scale = new Vector3(
                scale.x / parentScale.x,
                scale.y / parentScale.y,
                scale.z / parentScale.z
            );

            return scale;
        }

        private Vector3 GetScaleRelativeToParent(Transform uiItemTransform, Vector3 scale)
        {
            if (uiItemTransform.parent == null) return scale;

            var parentScale = uiItemTransform.parent.lossyScale;
            scale = new Vector3(
                scale.x * parentScale.x,
                scale.y * parentScale.y,
                scale.z * parentScale.z
            );

            return scale;
        }

        private Vector3 GetScale(float screenHeight, float screenWidth, Vector3 uiItemSize, float balance,
            float edgeOffset, float maxSize, Vector2 positionFactors)
        {
            if (positionFactors == Vector2.zero)
            {
                var scaleX = (screenWidth - edgeOffset * 2) / uiItemSize.x;
                var scaleY = (screenHeight - edgeOffset * 2) / uiItemSize.y;
                return new Vector3(scaleX, scaleY, _localScale.z);
            }

            var primaryDimension = screenHeight * (1 - positionFactors.x) + screenWidth * positionFactors.x;
            var itemSizeDimension = uiItemSize.y * (1 - positionFactors.x) + uiItemSize.x * positionFactors.x;

            var scaleRatio = primaryDimension / itemSizeDimension;
            var positionBalanceFactor =
                positionFactors.y * (1 - positionFactors.x) +
                positionFactors.x * positionFactors.x;

            var clampedScale = Mathf.Clamp(scaleRatio - edgeOffset * balance * positionBalanceFactor, 0,
                maxSize * balance * positionBalanceFactor);

            var finalScaleX = balance * _localScale.x * (1 - positionFactors.x) + clampedScale * positionFactors.x;
            var finalScaleY = clampedScale * (1 - positionFactors.x) + balance * _localScale.y * positionFactors.x;

            return new Vector3(finalScaleX, finalScaleY, _localScale.z);
        }


        private Vector3 ScaleWithGroup(Vector3 scale, Vector3 groupAxisConstraint, Vector3 uiItemScale)
        {
            var multiplication = new Vector3(scale.x * groupAxisConstraint.x, scale.y * groupAxisConstraint.y);

            return new Vector3(
                multiplication.x != 0 ? multiplication.x : uiItemScale.x,
                multiplication.y != 0 ? multiplication.y : uiItemScale.y,
                uiItemScale.z);
        }

        private Vector3 IgnoreScale(Vector3 scale, bool ignoreXScale, bool ignoreYScale, Transform uiItemTransform)
        {
            return new Vector3(
                !ignoreXScale ? scale.x : uiItemTransform.localScale.x,
                !ignoreYScale ? scale.y : uiItemTransform.localScale.y,
                scale.z);
        }
    }
}