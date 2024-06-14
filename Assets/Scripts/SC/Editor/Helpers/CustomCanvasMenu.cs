using SC.Core.UI;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace SC.Editor.Helpers
{
    public class CustomCanvasMenu
    {
        [MenuItem("GameObject/Sprite Canvas/Canvas", false, 10)]
        static void CreateSpriteCanvas(MenuCommand menuCommand)
        {
            var spriteCanvas = new GameObject("SpriteCanvas");
            spriteCanvas.AddComponent<SpriteCanvas>();
            GameObjectUtility.SetParentAndAlign(spriteCanvas, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(spriteCanvas, "Create " + spriteCanvas.name);
            Selection.activeObject = spriteCanvas;
        }

        [MenuItem("GameObject/Sprite Canvas/Image", false, 9)]
        static void CreateImage(MenuCommand menuCommand)
        {
            var image = new GameObject("Image");
            var spriteRenderer = image.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSprite();

            image.AddComponent<UISprite>();
            GameObjectUtility.SetParentAndAlign(image, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(image, "Create " + image.name);
            Selection.activeObject = image;
        }

        [MenuItem("GameObject/Sprite Canvas/TextMeshPro", false, 8)]
        static void CreateTextMeshPro(MenuCommand menuCommand)
        {
            var tmpObj = new GameObject("TextMeshPro");
            var tmp = tmpObj.AddComponent<TextMeshPro>();
            tmp.text = "Sample Text";
            
            tmpObj.AddComponent<UITextMeshPro>();
            GameObjectUtility.SetParentAndAlign(tmpObj, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(tmpObj, "Create " + tmpObj.name);
            Selection.activeObject = tmpObj;
        }
        [MenuItem("GameObject/Sprite Canvas/Button", false, 7)]
        static void CreateButton(MenuCommand menuCommand)
        {
            var image = new GameObject("Button");
            var spriteRenderer = image.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSprite();

            image.AddComponent<UIButton>();
            GameObjectUtility.SetParentAndAlign(image, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(image, "Create " + image.name);
            Selection.activeObject = image;
        }

        private static Sprite CreateSprite()
        {
            var texture = new Texture2D(100, 100);

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                    texture.SetPixel(x, y, Color.white);
            }

            texture.Apply();

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            sprite.name = "Temp";
            return sprite;
        }
    }
}