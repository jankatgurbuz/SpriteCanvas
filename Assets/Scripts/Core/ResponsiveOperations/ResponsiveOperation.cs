using UnityEngine;

namespace Core.ResponsiveOperations
{
    public interface IResponsiveOperation
    {
        public void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform,
            Vector3 referencePosition, float balance);
    }

    public abstract class ResponsiveOperation : IResponsiveOperation
    {
        [SerializeField] protected Vector3 LocalScale = new(1, 1, 1);
        [SerializeField] protected float _topOffset;
        [SerializeField] protected float _horizontalOffset;

        public abstract void AdjustUI(float screenHeight, float screenWidth,
            Vector3 uiItemSize, Transform uiItemTransform,
            Vector3 referencePosition, float balance);

        protected Vector3 GetPosition(float screenHeight, float screenWidth,
            Vector3 uiItemSize, Transform uiItemTransform, float balance, Vector2 positionFactors)
        {
            var scaledHeight = uiItemSize.y * uiItemTransform.localScale.y;
            var scaledWidth = uiItemSize.x * uiItemTransform.localScale.x;

            var x = positionFactors.x * screenWidth - scaledWidth * positionFactors.x -
                    _horizontalOffset * Mathf.Sign(positionFactors.x) * balance;

            var y = positionFactors.y * screenHeight - scaledHeight * positionFactors.y -
                    _topOffset * Mathf.Sign(positionFactors.y) * balance;

            var z = 0;

            return new Vector3(x, y, z);
        }

        protected Vector3 GetScale(float balance)
        {
            return LocalScale * balance;
        }

        protected Vector3 GetScale(float screenHeight, float screenWidth, Vector3 uiItemSize, float balance,
            float edgeOffset, float maxSize, Vector2 positionFactors)
        {
            var scaleRatioX = screenWidth / uiItemSize.x;
            var scaleRatioY = screenHeight / uiItemSize.y;

            var clampedX = Mathf.Clamp(scaleRatioX - edgeOffset * balance * positionFactors.x, 0,
                maxSize * balance * positionFactors.x);
            var clampedY = Mathf.Clamp(scaleRatioY - edgeOffset * balance * positionFactors.y, 0,
                maxSize * balance * positionFactors.y);

            var finalX = Mathf.Max(clampedX, LocalScale.x);
            var finalY = Mathf.Max(clampedY, LocalScale.y);

            return new Vector3(finalX, finalY, LocalScale.z);
        }
    }

    public class TopCenter : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance)
        {
            uiItemTransform.localScale = GetScale(balance);
            uiItemTransform.position = referencePosition + GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, new Vector2(0, 0.5f));
        }
    }

    public class TopRight : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance)
        {
            uiItemTransform.localScale = GetScale(balance);
            uiItemTransform.position = referencePosition + GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, new Vector2(0.5f, 0.5f));
        }
    }

    public class TopLeft : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance)
        {
            uiItemTransform.localScale = GetScale(balance);
            uiItemTransform.position = referencePosition + GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, new Vector2(-0.5f, 0.5f));
        }
    }

    public class TopStretch : ResponsiveOperation
    {
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10_000;

        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance)
        {
            uiItemTransform.localScale =
                GetScale(screenHeight, screenWidth, uiItemSize, balance, _edgeOffset, _maxSize, new Vector2(1, 0));

            uiItemTransform.position = referencePosition + GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, new Vector2(0, 0.5f));
        }
    }

    public class BottomCenter : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance)
        {
            uiItemTransform.localScale = GetScale(balance);
            uiItemTransform.position = referencePosition + GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, new Vector2(0, -0.5f));
        }
    }

    public class BottomRight : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance)
        {
            uiItemTransform.localScale = GetScale(balance);
            uiItemTransform.position = referencePosition + GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, new Vector2(0.5f, -0.5f));
        }
    }

    public class BottomLeft : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance)
        {
            uiItemTransform.localScale = GetScale(balance);
            uiItemTransform.position = referencePosition + GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, new Vector2(-0.5f, -0.5f));
        }
    }

    public class BottomStretch : ResponsiveOperation
    {
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10_000;

        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance)
        {
            uiItemTransform.localScale =
                GetScale(screenHeight, screenWidth, uiItemSize, balance, _edgeOffset, _maxSize, new Vector2(1, 0));

            uiItemTransform.position = referencePosition + GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, new Vector2(0, -0.5f));
        }
    }

    public class Center : ResponsiveOperation
    {
        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance)
        {
            uiItemTransform.localScale = GetScale(balance);
            uiItemTransform.position = referencePosition + GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, new Vector2(0, 0));
        }
    }

    public class LeftStretch : ResponsiveOperation
    {
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10_000;

        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance)
        {
            uiItemTransform.localScale =
                GetScale(screenHeight, screenWidth, uiItemSize, balance, _edgeOffset, _maxSize, new Vector2(0, 1));

            uiItemTransform.position = referencePosition + GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, new Vector2(-0.5f, 0));
        }
    }

    public class RightStretch : ResponsiveOperation
    {
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10_000;

        public override void AdjustUI(float screenHeight, float screenWidth, Vector3 uiItemSize,
            Transform uiItemTransform, Vector3 referencePosition, float balance)
        {
            uiItemTransform.localScale =
                GetScale(screenHeight, screenWidth, uiItemSize, balance, _edgeOffset, _maxSize, new Vector2(0, 1));

            uiItemTransform.position = referencePosition + GetPosition(screenHeight, screenWidth, uiItemSize,
                uiItemTransform, balance, new Vector2(0.5f, 0));
        }
    }
}