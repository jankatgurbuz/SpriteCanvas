using UnityEngine;

namespace SC.Core.SpriteCanvasAttribute
{
    public class SyncAlphaAttribute : PropertyAttribute
    {
        public bool RunTimeSync;

        public SyncAlphaAttribute(bool runTimeSync = false)
        {
            RunTimeSync = runTimeSync;
        }
    }
}