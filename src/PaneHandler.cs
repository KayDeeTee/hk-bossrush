using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CanvasUtils;

namespace ModInventory
{

    public enum MoveDirection
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    public class InvItem
    {
        public string n;
        public string v;
        public Vector2 position;
        public Vector2 rectSize;
        public Vector2 selSize;
        public Func<MoveDirection, string, InvItem> onMove;
        public Func<string, string, Sprite> getSprite;
        public Func<string, string, string> getText;

        public Action<string, string> onSelect;
        public Action<string, string> onSubmit;

        public bool enabled;

        public GameObject gameObject;

        public float startTime;
        public float duration;
        public Vector2 startPos;
        public Vector2 endPos;
        public Action<string, string> animCallback;

        public void doAnim(Vector2 start, Vector2 end, float d, Action<string, string> callback)
        {
            startTime = Time.realtimeSinceStartup;
            duration = d;
            startPos = start;
            endPos = end;
            animCallback = callback;
            if (PaneHandler.animating.Contains(this))
                PaneHandler.animating.Remove(this);
            PaneHandler.animating.Add(this);
        }

        public void doUpdate()
        {
            float t = (Time.realtimeSinceStartup - startTime) / duration;
            if (t < 1)
                gameObject.transform.localPosition = new Vector2(Mathf.Lerp(startPos.x, endPos.x, t), Mathf.Lerp(startPos.y, endPos.y, t));
            else
            {
                gameObject.transform.localPosition = endPos;
                if (animCallback != null)
                    animCallback(n, v);
                PaneHandler.toRemove.Add(this);
            }
        }

        /*
         * name to check against in function, and to search for
         * position on screen to display, relative to centre of screen,
         * size of image to display,
         * function to run when you attempt to move whilst having this item selected, returns the item you want to select,
         * funciton that sends the name of the item and the variable you set, expects the sprite you want it to render,
         * name of the variable you want to lookup in playerData ( or anywhere else you control it
         */
        public InvItem(string name, Vector2 pos, Vector2 size, Func<MoveDirection, string, InvItem> moved, Func<string, string, Sprite> loadSprite, Action<string, string> sel, Action<string, string> submit, string variable, bool enable)
        {
            n = name;
            position = pos;
            rectSize = size;
            selSize = size;
            onMove = moved;
            getSprite = loadSprite;
            getText = null;

            onSelect = sel;
            onSubmit = submit;

            v = variable;
            enabled = enable;
        }

        public InvItem(string name, Vector2 pos, Vector2 size, Vector2 sSize, Func<MoveDirection, string, InvItem> moved, Func<string, string, Sprite> loadSprite, Action<string, string> sel, Action<string, string> submit, string variable, bool enable)
        {
            n = name;
            position = pos;
            rectSize = size;
            selSize = sSize;
            onMove = moved;
            getSprite = loadSprite;
            getText = null;

            onSelect = sel;
            onSubmit = submit;

            v = variable;
            enabled = enable;
        }

        public InvItem(string name, Vector2 pos, Vector2 size, Func<MoveDirection, string, InvItem> moved, Func<string, string, string> loadText, Action<string, string> sel, Action<string, string> submit, string variable, bool enable)
        {
            n = name;
            position = pos;
            rectSize = size;
            selSize = size;
            onMove = moved;
            getSprite = null;
            getText = loadText;
            onSelect = sel;
            onSubmit = submit;

            v = variable;
            enabled = enable;
        }

        public InvItem(string name, Vector2 pos, Vector2 size, Vector2 sSize, Func<MoveDirection, string, InvItem> moved, Func<string, string, string> loadText, Action<string, string> sel, Action<string, string> submit, string variable, bool enable)
        {
            n = name;
            position = pos;
            rectSize = size;
            selSize = sSize;
            onMove = moved;
            getSprite = null;
            getText = loadText;
            onSelect = sel;
            onSubmit = submit;

            v = variable;
            enabled = enable;
        }

    }

    public abstract class PaneHandler : MonoBehaviour
    {

        public static bool hidden = true;

        public static GameObject invCanvas;
        public static GameObject menuGroup;
        public static PaneHandler firstPane;

        public static List<InvItem> animating;
        public static List<InvItem> toRemove;

        public PaneHandler prevPane;
        public PaneHandler nextPane;

        public static PaneHandler selectedPane;

        public Sprite paneSprite;

        public Sprite selectBg;
        public Sprite selectCorner;
        public Sprite vignette;
        public Sprite border;
        public Sprite TFleur;
        public Sprite BFleur;
        public Sprite BCFleur;
        public Sprite TCFleur;

        public static GameObject selectBackGround;
        public static GameObject selectTLCorner;
        public static GameObject selectTRCorner;
        public static GameObject selectBLCorner;
        public static GameObject selectBRCorner;

        public static GameObject currPaneName;
        public static GameObject nextPaneName;
        public static GameObject prevPaneName;

        public static InvItem paneLeft;
        public static InvItem paneRight;

        public List<InvItem> invItems;
        public InvItem selected;

        public GameObject rootPanel;

        public bool isEnabled;

        public int startPos;
        public int endPos;
        public static float duration = 0.75f;
        public float startTime = 0f;

        public virtual string PaneTitle { get; set; }

        public string sName;
        public bool preInit;
        abstract public void PreInit();
        abstract public void Init();
        abstract public void PostInit();

        public void Start()
        {
            setUp();
        }

        public void setUp()
        {
            DontDestroyOnLoad(this.gameObject);
            if (invCanvas == null)
            {
                animating = new List<InvItem>();
                toRemove = new List<InvItem>();

                selectBg = CanvasUtil.createSprite(BossRush.ResourceLoader.bluebg(), 0, 0, 239, 196);
                selectCorner = CanvasUtil.createSprite(BossRush.ResourceLoader.selectCorner(), 0, 0, 53, 53);
                vignette = CanvasUtil.createSprite(BossRush.ResourceLoader.menuVignette(), 0, 0, 254, 151);
                border = CanvasUtil.createSprite(BossRush.ResourceLoader.menuBorder(), 0, 0, 256, 152);
                TFleur = CanvasUtil.createSprite(BossRush.ResourceLoader.invTop(), 0, 0, 1028, 88);
                BFleur = CanvasUtil.createSprite(BossRush.ResourceLoader.invBot(), 0, 0, 744, 54);
                BCFleur = CanvasUtil.createSprite(BossRush.ResourceLoader.invBotCorner(), 0, 0, 208, 181);
                TCFleur = CanvasUtil.createSprite(BossRush.ResourceLoader.invTopCorner(), 0, 0, 316, 209);

                paneSprite = CanvasUtil.createSprite(BossRush.ResourceLoader.changePane(), 0, 0, 58, 133);

                isEnabled = true;
                firstPane = this;
                firstPane.nextPane = this;
                firstPane.prevPane = this;
                CanvasUtil.createFonts();
                invCanvas = CanvasUtil.createCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
                invCanvas.name = "Canvas";
                DontDestroyOnLoad(invCanvas);
                this.gameObject.transform.SetParent(invCanvas.transform);

                CanvasUtil.createImagePanel(invCanvas, vignette, new CanvasUtil.RectData(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(1, 1))).transform.localScale = new Vector3(1.15f, 1.15f, 1);

                menuGroup = CanvasUtil.createBasePanel(invCanvas, new CanvasUtil.RectData(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(1, 1)));
                menuGroup.name = "Menu";
                menuGroup.AddComponent<CanvasGroup>();

                CanvasUtil.createImagePanel(invCanvas, border, new CanvasUtil.RectData(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), new Vector2(1, 1))).transform.localScale = new Vector3(1.15f, 1.15f, 1);

                currPaneName = CanvasUtil.createTextPanel(invCanvas, PaneTitle, 32, TextAnchor.MiddleCenter, new CanvasUtil.RectData(new Vector2(1920, 200), new Vector2(0, 410)), true);
                nextPaneName = CanvasUtil.createTextPanel(invCanvas, PaneTitle, 24, TextAnchor.MiddleRight, new CanvasUtil.RectData(new Vector2(1920, 200), new Vector2(-100, 415)), true);
                prevPaneName = CanvasUtil.createTextPanel(invCanvas, PaneTitle, 24, TextAnchor.MiddleLeft, new CanvasUtil.RectData(new Vector2(1920, 200), new Vector2(100, 415)), true);

                CanvasUtil.createImagePanel(invCanvas, TFleur, new CanvasUtil.RectData(new Vector2(1028, 88), new Vector2(0, 358))).transform.localScale = new Vector2(0.85f, 0.85f);
                CanvasUtil.createImagePanel(invCanvas, BFleur, new CanvasUtil.RectData(new Vector2(744, 54), new Vector2(0, -440))).transform.localScale = new Vector2(0.85f, 0.85f);

                CanvasUtil.createImagePanel(invCanvas, TCFleur, new CanvasUtil.RectData(new Vector2(316, 209), new Vector2(765, 325))).transform.localScale = new Vector2(-0.85f, 0.85f);
                CanvasUtil.createImagePanel(invCanvas, TCFleur, new CanvasUtil.RectData(new Vector2(316, 209), new Vector2(-765, 325))).transform.localScale = new Vector2(0.85f, 0.85f);

                CanvasUtil.createImagePanel(invCanvas, BCFleur, new CanvasUtil.RectData(new Vector2(208, 181), new Vector2(800, -400))).transform.localScale = new Vector2(0.85f, 0.85f);
                CanvasUtil.createImagePanel(invCanvas, BCFleur, new CanvasUtil.RectData(new Vector2(208, 181), new Vector2(-800, -400))).transform.localScale = new Vector2(-0.85f, 0.85f);

                selectBackGround = CanvasUtil.createImagePanel(invCanvas, selectBg, new CanvasUtil.RectData(new Vector2(0, 0), new Vector2(0, 0)));
                selectBackGround.GetComponent<UnityEngine.UI.Image>().preserveAspect = false;
                selectBackGround.GetComponent<UnityEngine.UI.Image>().color = new Color(1, 1, 1, 0.25f);
                selectBackGround.name = "selBG";

                selectTLCorner = CanvasUtil.createImagePanel(invCanvas, selectCorner, new CanvasUtil.RectData(new Vector2(53, 53), new Vector2(0, 0)));
                selectTRCorner = CanvasUtil.createImagePanel(invCanvas, selectCorner, new CanvasUtil.RectData(new Vector2(53, 53), new Vector2(0, 0)));
                selectBLCorner = CanvasUtil.createImagePanel(invCanvas, selectCorner, new CanvasUtil.RectData(new Vector2(53, 53), new Vector2(0, 0)));
                selectBRCorner = CanvasUtil.createImagePanel(invCanvas, selectCorner, new CanvasUtil.RectData(new Vector2(53, 53), new Vector2(0, 0)));

                selectTLCorner.name = "selTL";
                selectTRCorner.name = "selTR";
                selectBLCorner.name = "selBL";
                selectBRCorner.name = "selBR";

                selectTLCorner.transform.localScale = new Vector3(1, 1, 1);
                selectTRCorner.transform.localScale = new Vector3(-1, 1, 1);
                selectBLCorner.transform.localScale = new Vector3(1, -1, 1);
                selectBRCorner.transform.localScale = new Vector3(-1, -1, 1);

                paneLeft = new InvItem("PaneLeft", new Vector2(-900, 0), new Vector2(58, 133), internalMove, (System.Func<string,string,Sprite>)internalLoad, internalSelect, internalSubmit, "", true);
                paneRight = new InvItem("PaneRight", new Vector2(900, 0), new Vector2(58, 133), internalMove, (System.Func<string, string, Sprite>)internalLoad, internalSelect, internalSubmit, "", true);

                GameObject paneL = CanvasUtil.createImagePanel(invCanvas, paneLeft.getSprite(paneLeft.n, paneLeft.v), new CanvasUtil.RectData(paneLeft.rectSize, paneLeft.position));
                paneL.name = paneLeft.n;
                GameObject paneR = CanvasUtil.createImagePanel(invCanvas, paneRight.getSprite(paneRight.n, paneRight.v), new CanvasUtil.RectData(paneRight.rectSize, paneRight.position));
                paneR.name = paneRight.n;
                paneR.transform.localScale = new Vector3(-1, 1, 1);

                paneLeft.gameObject = paneL;
                paneRight.gameObject = paneR;

                hideMenu();
            }
            else
            {
                isEnabled = false;
            }

            if (!preInit)
            {
                //insert at end of list
                prevPane = firstPane.prevPane;
                prevPane.nextPane = this;
                firstPane.prevPane = this;
                nextPane = firstPane;

                selectedPane = firstPane;
                rootPanel = CanvasUtil.createBasePanel(menuGroup, new CanvasUtil.RectData(new Vector2(0, 0), new Vector2(0, 0)));
                //rootPanel.SetActive (tr);

                PreInit();
                preInit = true;
            }

            invItems = new List<InvItem>();
            Init();
            foreach (InvItem item in invItems)
            {
                if (item.getSprite != null)
                    item.gameObject = CanvasUtil.createImagePanel(rootPanel, item.getSprite(item.n, item.v), new CanvasUtil.RectData(item.rectSize, item.position));
                else
                    item.gameObject = CanvasUtil.createTextPanel(rootPanel, item.getText(item.n, item.v), 24, TextAnchor.MiddleCenter, new CanvasUtil.RectData(item.rectSize, item.position));
                item.gameObject.name = item.n;
            }

            selected = invItems[0];
            //setSelectedPos();

            PostInit();

            updateTitles();
        }

        protected InvItem findItemByName(string name)
        {
            foreach (InvItem i in invItems)
            {
                if (i.n == name)
                    return i;
            }
            return null;
        }

        abstract public void onSelect(string n, string v);
        abstract public void onSubmit(string n, string v);

        private void updateTitles()
        {
            currPaneName.GetComponent<UnityEngine.UI.Text>().text = selectedPane.PaneTitle;
            prevPaneName.GetComponent<UnityEngine.UI.Text>().text = selectedPane.prevPane.PaneTitle;
            nextPaneName.GetComponent<UnityEngine.UI.Text>().text = selectedPane.nextPane.PaneTitle;
        }

        public abstract void onMenuOpen();

        public static void showMenu()
        {
            PaneHandler.invCanvas.GetComponent<CanvasGroup>().alpha = 1;
            PaneHandler cPane = firstPane;
            cPane.onMenuOpen();
            cPane = cPane.nextPane;
            while (cPane != firstPane)
            {
                cPane.onMenuOpen();
                cPane = cPane.nextPane;
            }
            hidden = false;
        }

        public static void hideMenu()
        {
            PaneHandler.invCanvas.GetComponent<CanvasGroup>().alpha = 0;
            //invCanvas.SetActive(false);
            hidden = true;
        }

        private InvItem internalMove(MoveDirection md, string name)
        {
            if (name == "PaneLeft")
            {
                if (md == MoveDirection.LEFT)
                {
                    if (prevPane != this)
                    {

                        selectedPane.prevPane.slideIn(MoveDirection.LEFT);
                        selectedPane.slideOut(MoveDirection.LEFT);

                        selectedPane = selectedPane.prevPane;
                        updateTitles();
                    }
                }
                if (md == MoveDirection.RIGHT)
                {
                    return selectedPane.invItems[0];
                }
                return paneLeft;
            }
            if (name == "PaneRight")
            {
                if (md == MoveDirection.RIGHT)
                {
                    if (nextPane != this)
                    {
                        selectedPane.nextPane.slideIn(MoveDirection.RIGHT);
                        selectedPane.slideOut(MoveDirection.RIGHT);

                        selectedPane = selectedPane.nextPane;
                        updateTitles();
                    }
                }
                if (md == MoveDirection.LEFT)
                {
                    return selectedPane.invItems[invItems.Count - 1];
                }
                return paneRight;
            }
            return selectedPane.invItems[0];
        }

        private Sprite internalLoad(string name, string var)
        {
            if (name == "PaneLeft" || name == "PaneRight")
                return paneSprite;
            return CanvasUtil.nullSprite();
        }

        //Send this pane off the screen in the direction passed in
        private void slideOut(MoveDirection md)
        {
            startTime = Time.realtimeSinceStartup;
            isEnabled = false;
            if (md == MoveDirection.LEFT)
                endPos = 1920;
            else
                endPos = -1920;
            startPos = 0;
        }

        private void slideIn(MoveDirection md)
        {
            startTime = Time.realtimeSinceStartup;
            isEnabled = true;

            if (md == MoveDirection.LEFT)
            {
                startPos = -1920;
                selected = paneLeft;
            }
            else
            {
                startPos = 1920;
                selected = paneRight;
            }
            endPos = 0;
        }

        private void internalSelect(string n, string v)
        {
        }

        private void internalSubmit(string n, string v)
        {
            if (name == "PaneLeft")
            {
                if (prevPane != this)
                {
                    prevPane.slideIn(MoveDirection.LEFT);
                    slideOut(MoveDirection.LEFT);
                }
            }
            if (name == "PaneRight")
            {
                if (nextPane != this)
                {
                    nextPane.slideIn(MoveDirection.RIGHT);
                    slideOut(MoveDirection.RIGHT);
                }
            }
        }

        public void setSelectedPos()
        {
            selectBackGround.GetComponent<RectTransform>().sizeDelta = selected.selSize;
            selectBackGround.GetComponent<RectTransform>().anchoredPosition = selected.position;

            float xOffset = (selected.selSize.x / (2.5f + (Mathf.Sin(Time.realtimeSinceStartup * 5) / 4)));
            float yOffset = (selected.selSize.y / (2.5f + (Mathf.Sin(Time.realtimeSinceStartup * 5) / 4)));

            selectTLCorner.GetComponent<RectTransform>().anchoredPosition = new Vector2(selected.position.x - xOffset, selected.position.y + yOffset);
            selectTRCorner.GetComponent<RectTransform>().anchoredPosition = new Vector2(selected.position.x + xOffset, selected.position.y + yOffset);
            selectBLCorner.GetComponent<RectTransform>().anchoredPosition = new Vector2(selected.position.x - xOffset, selected.position.y - yOffset);
            selectBRCorner.GetComponent<RectTransform>().anchoredPosition = new Vector2(selected.position.x + xOffset, selected.position.y - yOffset);

        }

        public void Update()
        {
            if (isEnabled)
            {
                if (GameManager.instance.inputHandler.inputActions.openInventory.WasPressed && false)
                {
                    if (hidden)
                        showMenu();
                    else
                        hideMenu();
                    hidden = !hidden;
                }
                setSelectedPos();
            }
            if (Time.realtimeSinceStartup - startTime < duration)
            {
                rootPanel.transform.localPosition = new Vector2(Mathf.Lerp(startPos, endPos, (Time.realtimeSinceStartup - startTime) / duration), 0);
            }
            else
            {
                if (isEnabled && !hidden)
                {
                    foreach (InvItem i in animating)
                        i.doUpdate();
                    foreach (InvItem i in toRemove)
                        animating.Remove(i);
                    toRemove.Clear();
                    rootPanel.transform.localPosition = new Vector2(0, 0);
                    if ( GameManager.instance.inputHandler.inputActions.right.WasPressed )
                    {
                        selected = selected.onMove(MoveDirection.RIGHT, selected.n);
                        onSelect(selected.n, selected.v);
                    }
                    if (GameManager.instance.inputHandler.inputActions.left.WasPressed)
                    {
                        selected = selected.onMove(MoveDirection.LEFT, selected.n);
                        onSelect(selected.n, selected.v);
                    }
                    if ( GameManager.instance.inputHandler.inputActions.up.WasPressed )
                    {
                        selected = selected.onMove(MoveDirection.UP, selected.n);
                        onSelect(selected.n, selected.v);
                    }
                    if ( GameManager.instance.inputHandler.inputActions.up.WasPressed )
                    {
                        selected = selected.onMove(MoveDirection.DOWN, selected.n);
                        onSelect(selected.n, selected.v);
                    }
                    if ( GameManager.instance.inputHandler.inputActions.menuSubmit.WasPressed )
                    {
                        selected.onSubmit(selected.n, selected.v);
                        onSubmit(selected.n, selected.v);
                    }
                }
                else
                {
                    rootPanel.transform.localPosition = new Vector2(1920, 0);
                }
            }
            sName = selected.n;
        }

    }
}
