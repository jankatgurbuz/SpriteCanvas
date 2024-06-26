using UnityEngine;

namespace SC.Core.ResponsiveOperations
{
    public interface IResponsiveOperation
    {
        public void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform,
            Vector3 referencePosition, float balance, Vector3 groupAxisConstraint, bool ignoreXPosition,
            bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale);

        // public void SetScale(Vector3 scale);
    }

    public abstract class ResponsiveOperation : IResponsiveOperation
    {
        [SerializeField] protected Vector3 LocalScale = new(1, 1, 1);
        [SerializeField] protected float _topOffset;
        [SerializeField] protected float _horizontalOffset;

        public abstract void AdjustUI(float screenHeight, float screenWidth,
            Vector3 uiItemSize, Transform uiItemTransform,
            Vector3 referencePosition, float balance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale);


        // public void SetScale(Vector3 scale)
        // {
        //     LocalScale = scale;
        // }

        protected Vector3 GetPosition(float screenHeight, float screenWidth,
            Vector3 uiItemSize, Transform uiItemTransform, float balance, Vector3 referencePosition,
            Vector2 positionFactors, bool ignoreXPosition, bool ignoreYPosition)
        {
            var scale = uiItemTransform.localScale;
            if (uiItemTransform.parent != null)
            {
                scale = uiItemTransform.parent.TransformVector(scale);
            }

            var scaledHeight = uiItemSize.y * scale.y;
            var scaledWidth = uiItemSize.x * scale.x;

            var x = positionFactors.x * screenWidth - scaledWidth * positionFactors.x -
                    _horizontalOffset * Mathf.Sign(positionFactors.x) * balance;

            var y = positionFactors.y * screenHeight - scaledHeight * positionFactors.y -
                    _topOffset * Mathf.Sign(positionFactors.y) * balance;

            var z = 0; // todo set

            var position = new Vector3(x, y, z) + referencePosition;

            return new Vector3(
                !ignoreXPosition ? position.x : uiItemTransform.position.x,
                !ignoreYPosition ? position.y : uiItemTransform.position.y,
                position.z);
        }

        protected Vector3 GetScale(float balance)
        {
            return LocalScale * balance;
        }

        protected Vector3 InverseTransformVector(Transform uiItemTransform, Vector3 scale)
        {
            if (uiItemTransform.parent != null)
            {
                scale = uiItemTransform.parent.InverseTransformVector(scale);
            }

            return scale;
        }

        protected Vector3 GetScale(float screenHeight, float screenWidth, Vector3 uiItemSize, float balance,
            float edgeOffset, float maxSize, Vector2 positionFactors)
        {
            var primaryDimension = screenHeight * (1 - positionFactors.x) + screenWidth * positionFactors.x;
            var itemSizeDimension = uiItemSize.y * (1 - positionFactors.x) + uiItemSize.x * positionFactors.x;

            var scaleRatio = primaryDimension / itemSizeDimension;
            var positionBalanceFactor =
                positionFactors.y * (1 - positionFactors.x) +
                positionFactors.x * positionFactors.x;

            var clampedScale = Mathf.Clamp(scaleRatio - edgeOffset * balance * positionBalanceFactor, 0,
                maxSize * balance * positionBalanceFactor);

            var finalScaleX = balance * LocalScale.x * (1 - positionFactors.x) + clampedScale * positionFactors.x;
            var finalScaleY = clampedScale * (1 - positionFactors.x) + balance * LocalScale.y * positionFactors.x;

            return new Vector3(finalScaleX, finalScaleY, LocalScale.z);
        }

        protected Vector3 ScaleWithGroup(Vector3 scale, Vector3 groupAxisConstraint, Vector3 uiItemScale)
        {
            var multiplication = new Vector3(scale.x * groupAxisConstraint.x, scale.y * groupAxisConstraint.y);

            return new Vector3(
                multiplication.x != 0 ? multiplication.x : uiItemScale.x,
                multiplication.y != 0 ? multiplication.y : uiItemScale.y,
                uiItemScale.z);
        }

        protected Vector3 IgnoreScale(Vector3 scale, bool ignoreXScale, bool ignoreYScale, Transform uiItemTransform)
        {
            return new Vector3(
                !ignoreXScale ? scale.x : uiItemTransform.localScale.x,
                !ignoreYScale ? scale.y : uiItemTransform.localScale.y,
                scale.z);
        }
    }

    public class TopCenter : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale)
        {
            var scale = GetScale(balance);
            scale = InverseTransformVector(uiItemTransform, scale);
            scale = ScaleWithGroup(scale, groupAxisConstraint, uiItemTransform.localScale);
            scale = IgnoreScale(scale, ignoreXScale, ignoreYScale, uiItemTransform);

            uiItemTransform.localScale = scale;

            uiItemTransform.position = GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, referencePosition, new Vector2(0, 0.5f), ignoreXPosition, ignoreYPosition);
        }
    }

    public class TopRight : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale)
        {
            var scale = GetScale(balance);
            scale = InverseTransformVector(uiItemTransform, scale);
            scale = ScaleWithGroup(scale, groupAxisConstraint, uiItemTransform.localScale);
            scale = IgnoreScale(scale, ignoreXScale, ignoreYScale, uiItemTransform);
            uiItemTransform.localScale = scale;

            uiItemTransform.position = GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, referencePosition, new Vector2(0.5f, 0.5f), ignoreXPosition, ignoreYPosition);
        }
    }

    public class TopLeft : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale)
        {
            var scale = GetScale(balance);
            scale = InverseTransformVector(uiItemTransform, scale);
            scale = ScaleWithGroup(scale, groupAxisConstraint, uiItemTransform.localScale);
            scale = IgnoreScale(scale, ignoreXScale, ignoreYScale, uiItemTransform);
            uiItemTransform.localScale = scale;

            uiItemTransform.position = GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, referencePosition, new Vector2(-0.5f, 0.5f), ignoreXPosition,
                ignoreYPosition);
        }
    }

    public class TopStretch : ResponsiveOperation
    {
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10_000;

        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale)
        {
            var scale = GetScale(screenHeight, screenWidth, uiItemSize, balance, _edgeOffset, _maxSize,
                new Vector2(1, 0));
            scale = InverseTransformVector(uiItemTransform, scale);
            scale = ScaleWithGroup(scale, groupAxisConstraint, uiItemTransform.localScale);
            scale = IgnoreScale(scale, ignoreXScale, ignoreYScale, uiItemTransform);
            uiItemTransform.localScale = scale;

            uiItemTransform.position = GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, referencePosition, new Vector2(0, 0.5f), ignoreXPosition, ignoreYPosition);
        }
    }

    public class BottomCenter : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale)
        {
            var scale = GetScale(balance);
            scale = InverseTransformVector(uiItemTransform, scale);
            scale = ScaleWithGroup(scale, groupAxisConstraint, uiItemTransform.localScale);
            scale = IgnoreScale(scale, ignoreXScale, ignoreYScale, uiItemTransform);
            uiItemTransform.localScale = scale;

            uiItemTransform.position = GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, referencePosition, new Vector2(0, -0.5f), ignoreXPosition, ignoreYPosition);
        }
    }

    public class BottomRight : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale)
        {
            var scale = GetScale(balance);
            scale = InverseTransformVector(uiItemTransform, scale);
            scale = ScaleWithGroup(scale, groupAxisConstraint, uiItemTransform.localScale);
            scale = IgnoreScale(scale, ignoreXScale, ignoreYScale, uiItemTransform);
            uiItemTransform.localScale = scale;

            uiItemTransform.position = GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, referencePosition, new Vector2(0.5f, -0.5f), ignoreXPosition,
                ignoreYPosition);
        }
    }

    public class BottomLeft : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale)
        {
            var scale = GetScale(balance);
            scale = InverseTransformVector(uiItemTransform, scale);
            scale = ScaleWithGroup(scale, groupAxisConstraint, uiItemTransform.localScale);
            scale = IgnoreScale(scale, ignoreXScale, ignoreYScale, uiItemTransform);
            uiItemTransform.localScale = scale;

            uiItemTransform.position = GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, referencePosition, new Vector2(-0.5f, -0.5f), ignoreXPosition,
                ignoreYPosition);
        }
    }

    public class BottomStretch : ResponsiveOperation
    {
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10_000;

        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale)
        {
            var scale = GetScale(screenHeight, screenWidth, uiItemSize, balance, _edgeOffset, _maxSize,
                new Vector2(1, 0));

            scale = InverseTransformVector(uiItemTransform, scale);
            scale = ScaleWithGroup(scale, groupAxisConstraint, uiItemTransform.localScale);
            scale = IgnoreScale(scale, ignoreXScale, ignoreYScale, uiItemTransform);
            uiItemTransform.localScale = scale;

            uiItemTransform.position = GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, referencePosition, new Vector2(0, -0.5f), ignoreXPosition, ignoreYPosition);
        }
    }

    public class Center : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale)
        {
            var scale = GetScale(balance);
            scale = InverseTransformVector(uiItemTransform, scale);
            scale = ScaleWithGroup(scale, groupAxisConstraint, uiItemTransform.localScale);
            scale = IgnoreScale(scale, ignoreXScale, ignoreYScale, uiItemTransform);
            uiItemTransform.localScale = scale;

            uiItemTransform.position = GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, referencePosition, new Vector2(0, 0), ignoreXPosition, ignoreYPosition);
        }
    }

    public class LeftStretch : ResponsiveOperation
    {
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10_000;

        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale)
        {
            var scale = GetScale(screenHeight, screenWidth, uiItemSize, balance, _edgeOffset, _maxSize,
                new Vector2(0, 1));
            scale = InverseTransformVector(uiItemTransform, scale);
            scale = ScaleWithGroup(scale, groupAxisConstraint, uiItemTransform.localScale);
            scale = IgnoreScale(scale, ignoreXScale, ignoreYScale, uiItemTransform);
            uiItemTransform.localScale = scale;

            uiItemTransform.position = GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, referencePosition, new Vector2(-0.5f, 0), ignoreXPosition, ignoreYPosition);
        }
    }

    public class RightStretch : ResponsiveOperation
    {
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10_000;

        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance, Vector3 groupAxisConstraint,
            bool ignoreXPosition, bool ignoreYPosition, bool ignoreXScale, bool ignoreYScale)
        {
            var scale = GetScale(screenHeight, screenWidth, uiItemSize, balance, _edgeOffset, _maxSize,
                new Vector2(0, 1));
            scale = InverseTransformVector(uiItemTransform, scale);
            scale = ScaleWithGroup(scale, groupAxisConstraint, uiItemTransform.localScale);
            scale = IgnoreScale(scale, ignoreXScale, ignoreYScale, uiItemTransform);
            uiItemTransform.localScale = scale;

            uiItemTransform.position = GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, referencePosition, new Vector2(0.5f, 0), ignoreXPosition, ignoreYPosition);
        }
    }
}