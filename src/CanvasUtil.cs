using UnityEngine;
using UnityEngine.UI;

namespace CanvasUtils
{
    public static class CanvasUtil
    {
        public static Font trajanBold;
        public static Font trajanNormal;

        public static void createFonts(){
            foreach (Font f in Resources.FindObjectsOfTypeAll<UnityEngine.Font>())
            {
                if (f != null && f.name == "TrajanPro-Bold")
                {
                    trajanBold = f;
                }

                if (f != null && f.name == "TrajanPro-Regular")
                {
                    trajanNormal = f;
                }
            }
        }

        public static GameObject createCanvas(int w, int h)
        {
            GameObject c = new GameObject();
            c.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler cs = c.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(w, h);
            c.AddComponent<GraphicRaycaster>();
            return c;
        }

        public static Sprite createSprite(byte[] data, int x, int y, int w, int h)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(data);
            tex.anisoLevel = 0;
            int width = tex.width;
            int height = tex.height;
            return Sprite.Create(tex, new Rect(x, y, w, h), Vector2.zero);
        }

        public static GameObject createImagePanel(GameObject parent, byte[] sprite_data, int x, int y, int w, int h, int sprite_x, int sprite_y, int sprite_w, int sprite_h)
        {
            GameObject panel = new GameObject();
            panel.transform.parent = parent.transform;
            panel.AddComponent<CanvasRenderer>();
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(w, h);
            rt.anchorMax = new Vector2(0, 0);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchoredPosition = new Vector2(x, y);
            UnityEngine.UI.Image img = panel.AddComponent<UnityEngine.UI.Image>();
            img.sprite = createSprite(sprite_data, sprite_x, sprite_y, sprite_w, sprite_h);
            return panel;
        }

        public static GameObject createTextPanel(GameObject parent, int x, int y, int w, int h)
        {
            GameObject panel = new GameObject();
            panel.transform.parent = parent.transform;
            panel.AddComponent<CanvasRenderer>();
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(w, h);
            rt.anchorMax = new Vector2(0, 0);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchoredPosition = new Vector2(x, y);
            Text text = panel.AddComponent<Text>();
            text.font = trajanBold;
            text.text = "%TEXT%";
            text.fontSize = 15;
            text.alignment = TextAnchor.MiddleCenter;
            return panel;
        }

        public static GameObject createTextPanel(GameObject parent, string defaultText, int x, int y, int w, int h, int s)
        {
            GameObject panel = new GameObject();
            panel.transform.parent = parent.transform;
            panel.AddComponent<CanvasRenderer>();
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(w, h);
            rt.anchorMax = new Vector2(0, 0);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchoredPosition = new Vector2(x, y);
            Text text = panel.AddComponent<Text>();
            text.font = trajanBold;
            text.text = defaultText;
            text.fontSize = s;
            text.alignment = TextAnchor.MiddleCenter;
            return panel;
        }

        public static GameObject createImagePanel(GameObject parent, int x, int y, int w, int h)
        {
            GameObject panel = new GameObject();
            panel.transform.parent = parent.transform;
            panel.AddComponent<CanvasRenderer>();
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(w, h);
            rt.anchorMax = new Vector2(0, 0);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchoredPosition = new Vector2(x, y);
            Image img = panel.AddComponent<Image>();
            return panel;
        }


    }
}
