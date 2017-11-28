using System;
using UnityEngine;
using UnityEngine.UI;

namespace CanvasUtils
{
    public static class CanvasUtil
    {
        public static Font trajanBold;
        public static Font trajanNormal;

        public class RectData
        {
            public Vector2 rectSize;
            public Vector2 position;
            public Vector2 anchorMin;
            public Vector2 anchorMax;
            public Vector2 anchorPivot;

            public RectData(Vector2 size, Vector2 pos)
            {
                rectSize = size;
                position = pos;
                anchorMin = new Vector2(0.5f, 0.5f);
                anchorMax = new Vector2(0.5f, 0.5f);
                anchorPivot = new Vector2(0.5f, 0.5f);
            }

            public RectData(Vector2 size, Vector2 pos, Vector2 min, Vector2 max)
            {
                rectSize = size;
                position = pos;
                anchorMin = min;
                anchorMax = max;
                anchorPivot = new Vector2(0.5f, 0.5f);
            }

            public RectData(Vector2 size, Vector2 pos, Vector2 min, Vector2 max, Vector2 pivot)
            {
                rectSize = size;
                position = pos;
                anchorMin = min;
                anchorMax = max;
                anchorPivot = pivot;
            }
        }

        public static void createFonts()
        {

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

        public static Sprite nullSprite()
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadRawTextureData(new byte[] { 0x00, 0x00, 0x00, 0x00 });
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.zero);
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

        public static GameObject createBasePanel(GameObject parent, RectData rd)
        {
            GameObject basePanel = new GameObject();
            if (parent != null)
            {
                basePanel.transform.SetParent(parent.transform);
                basePanel.transform.localScale = new Vector3(1, 1, 1);
            }
            basePanel.AddComponent<CanvasRenderer>();
            addRectTransform(basePanel, rd);
            return basePanel;
        }

        public static void addRectTransform(GameObject go, RectData rd)
        {
            //Create a rectTransform
            //Set the total size of the content
            //all you need to know is, 
            //--

            //sizeDelta is size of the difference of the anchors multiplied by screen size so
            //the sizeDelta width is actually = ((anchorMax.x-anchorMin.x)*screenWidth) + sizeDelta.x
            //so assuming a streched horizontally rectTransform on a 1920 screen, this would be
            //((1-0)*1920)+sizeDelta.x
            //1920 + sizeDelta.x
            //so if you wanted a 100pixel wide box in the center of the screen you'd do -1820, height as 1920+-1820 = 100
            //and if you wanted a fullscreen wide box, its just 0 because 1920+0 = 1920
            //the same applies for height

            //anchorPosition is basically an offset to the center of the anchors multiplies by screen size so
            //a 0.5,0.5 min and 0.5,0.5 max, would put the anchor in the middle of the screen but anchorPosition just offsets that 
            //i.e on a 1920x1080 screen
            //anchorPosition 100,100 would do (1920*0.5)+100,(1080*0.5)+100, so 1060,640

            //ANCHOR MIN / MAX
            //--
            //0,0 = bottom left
            //0,1 = top left
            //1,0 = bottom right
            //1,1 = top right
            //--


            //The only other rects I'd use are
            //anchorMin = 0.0, yyy anchorMax = 1.0, yyy (strech horizontally) y = 0.0 is bottom, y = 0.5 is center, y = 1.0 is top
            //anchorMin = xxx, 0.0 anchorMax = xxx, 1.0 (strech vertically) x = 0.0 is left, x = 0.5 is center, x = 1.0 is right
            //anchorMin = 0.0, 0.0 anchorMax = 1.0, 1.0 (strech to fill)
            //--
            //technically you can anchor these anywhere on the screen
            //you can even use negative values to float something offscreen

            //as for the pivot, the pivot determines where the "center" of the rect is which is useful if you want to rotate something by its corner, note that this DOES offset the anchor positions
            //i.e. with a 100x100 square, setting the pivot to be 1,1 will put the top right of the square at the anchor position (-50,-50 from its starting position)

            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMax = rd.anchorMax;
            rt.anchorMin = rd.anchorMin;
            rt.pivot = rd.anchorPivot;
            rt.sizeDelta = rd.rectSize;
            rt.anchoredPosition = rd.position;
        }

        /*  
         *  ██████╗ █████╗ ███╗   ██╗██╗   ██╗ █████╗ ███████╗
         * ██╔════╝██╔══██╗████╗  ██║██║   ██║██╔══██╗██╔════╝
         * ██║     ███████║██╔██╗ ██║██║   ██║███████║███████╗
         * ██║     ██╔══██║██║╚██╗██║╚██╗ ██╔╝██╔══██║╚════██║
         * ╚██████╗██║  ██║██║ ╚████║ ╚████╔╝ ██║  ██║███████║
         *  ╚═════╝╚═╝  ╚═╝╚═╝  ╚═══╝  ╚═══╝  ╚═╝  ╚═╝╚══════╝           
         */
        public static GameObject createCanvas(RenderMode renderMode, int PPU)
        {
            GameObject c = new GameObject();
            c.AddComponent<Canvas>().renderMode = renderMode;
            CanvasScaler cs = c.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            cs.referencePixelsPerUnit = PPU;
            c.AddComponent<GraphicRaycaster>();
            c.AddComponent<CanvasGroup>();
            c.GetComponent<CanvasGroup>().blocksRaycasts = false;
            c.GetComponent<CanvasGroup>().interactable = false;
            return c;
        }
        public static GameObject createCanvas(RenderMode renderMode, Vector2 size)
        {
            GameObject c = new GameObject();
            c.AddComponent<Canvas>().renderMode = renderMode;
            CanvasScaler cs = c.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = size;
            c.AddComponent<GraphicRaycaster>();
            c.AddComponent<CanvasGroup>();
            c.GetComponent<CanvasGroup>().blocksRaycasts = false;
            c.GetComponent<CanvasGroup>().interactable = false;
            return c;
        }
        /*
         * ████████╗███████╗██╗  ██╗████████╗
         * ╚══██╔══╝██╔════╝╚██╗██╔╝╚══██╔══╝
         *    ██║   █████╗   ╚███╔╝    ██║   
         *    ██║   ██╔══╝   ██╔██╗    ██║   
         *    ██║   ███████╗██╔╝ ██╗   ██║   
         *    ╚═╝   ╚══════╝╚═╝  ╚═╝   ╚═╝  
         */

        public static GameObject createTextPanel(GameObject parent, string text, int fontSize, TextAnchor textAnchor, RectData rectData, bool bold = true)
        {
            GameObject panel = createBasePanel(parent, rectData);

            Text textObj = panel.AddComponent<Text>();
            if (bold)
                textObj.font = trajanBold;
            else
                textObj.font = trajanNormal;
            textObj.text = text;
            textObj.supportRichText = true;
            textObj.fontSize = fontSize;
            textObj.alignment = textAnchor;
            return panel;
        }
        /*
         * ██╗███╗   ███╗ █████╗  ██████╗ ███████╗
         * ██║████╗ ████║██╔══██╗██╔════╝ ██╔════╝
         * ██║██╔████╔██║███████║██║  ███╗█████╗  
         * ██║██║╚██╔╝██║██╔══██║██║   ██║██╔══╝  
         * ██║██║ ╚═╝ ██║██║  ██║╚██████╔╝███████╗
         * ╚═╝╚═╝     ╚═╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝
         */

        public static GameObject createImagePanel(GameObject parent, Sprite spr, RectData rectData)
        {
            GameObject panel = createBasePanel(parent, rectData);

            Image img = panel.AddComponent<Image>();
            img.sprite = spr;
            img.preserveAspect = true;
            return panel;
        }

        /*
         * ██████╗ ██╗   ██╗████████╗████████╗ ██████╗ ███╗   ██╗
         * ██╔══██╗██║   ██║╚══██╔══╝╚══██╔══╝██╔═══██╗████╗  ██║
         * ██████╔╝██║   ██║   ██║      ██║   ██║   ██║██╔██╗ ██║
         * ██╔══██╗██║   ██║   ██║      ██║   ██║   ██║██║╚██╗██║
         * ██████╔╝╚██████╔╝   ██║      ██║   ╚██████╔╝██║ ╚████║
         * ╚═════╝  ╚═════╝    ╚═╝      ╚═╝    ╚═════╝ ╚═╝  ╚═══╝
         */

        public static GameObject createButton(GameObject parent, Action<int> action, int id, Sprite spr, string text, int fontSize, TextAnchor textAnchor, RectData rectData, bool bold = true, params Sprite[] extraSprites)
        {
            GameObject panel = createBasePanel(parent, rectData);

            createTextPanel(panel, text, fontSize, textAnchor, rectData, bold);

            Image img = panel.AddComponent<Image>();
            img.sprite = spr;

            Button button = panel.AddComponent<Button>();
            button.targetGraphic = img;
            button.onClick.AddListener(delegate
            {
                action(id);
            });

            if (extraSprites.Length == 3)
            {
                button.transition = Selectable.Transition.SpriteSwap;
                button.targetGraphic = img;
                SpriteState sprState = new SpriteState();
                sprState.highlightedSprite = extraSprites[0];
                sprState.pressedSprite = extraSprites[1];
                sprState.disabledSprite = extraSprites[2];

                button.spriteState = sprState;
            }
            else
            {
                button.transition = Selectable.Transition.None;
            }

            return panel;
        }

        /*
             ██████╗██╗  ██╗███████╗ ██████╗██╗  ██╗██████╗  ██████╗ ██╗  ██╗
            ██╔════╝██║  ██║██╔════╝██╔════╝██║ ██╔╝██╔══██╗██╔═══██╗╚██╗██╔╝
            ██║     ███████║█████╗  ██║     █████╔╝ ██████╔╝██║   ██║ ╚███╔╝ 
            ██║     ██╔══██║██╔══╝  ██║     ██╔═██╗ ██╔══██╗██║   ██║ ██╔██╗ 
            ╚██████╗██║  ██║███████╗╚██████╗██║  ██╗██████╔╝╚██████╔╝██╔╝ ██╗
             ╚═════╝╚═╝  ╚═╝╚══════╝ ╚═════╝╚═╝  ╚═╝╚═════╝  ╚═════╝ ╚═╝  ╚═╝                                                     
         */
        public static GameObject createToggle(GameObject parent, Action<bool> action, Sprite boxBg, Sprite boxFg, string text, int fontSize, TextAnchor textAnchor, RectData rectData, bool bold = true, bool isOn = false)
        {
            GameObject panel = createBasePanel(parent, rectData);

            GameObject boxBG = createImagePanel(panel, boxBg, rectData);
            GameObject boxFG = createImagePanel(boxBG, boxFg, rectData);
            GameObject label = createTextPanel(panel, text, fontSize, TextAnchor.UpperLeft, rectData, bold);

            Toggle toggle = panel.AddComponent<Toggle>();
            toggle.isOn = isOn;

            toggle.targetGraphic = boxBG.GetComponent<Image>();
            toggle.graphic = boxFG.GetComponent<Image>();

            toggle.transition = Selectable.Transition.ColorTint;

            ColorBlock cb = new ColorBlock();
            cb.normalColor = new Color(1, 1, 1, 1);
            cb.highlightedColor = new Color(1, 1, 1, 1);
            cb.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
            cb.disabledColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
            cb.fadeDuration = 0.1f;
            toggle.colors = cb;

            toggle.onValueChanged.AddListener(delegate(bool b)
            {
                action(b);
            });

            ToggleGroup group = parent.GetComponent<ToggleGroup>();

            if (group != null)
                toggle.group = group;

            return panel;
        }

        public static GameObject createToggleGroup()
        {
            GameObject panel = new GameObject();

            addRectTransform(panel, new RectData(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)));

            panel.AddComponent<ToggleGroup>();

            return panel;
        }

        public static GameObject createRectMask2dPanel(GameObject parent, RectData rectData)
        {
            GameObject panel = createBasePanel(parent, rectData);

            panel.AddComponent<RectMask2D>();
            return panel;
        }

    }
}
