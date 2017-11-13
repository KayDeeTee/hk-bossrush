using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using GlobalEnums;
using UnityEngine;
using UnityEngine.SceneManagement;
using BossRush.Properties;
using UnityEngine.UI;
using HutongGames.PlayMaker;
using System.Reflection;
using TMPro;
using CanvasUtils;

namespace BossRush
{
    public class BossRush : Mod
    {
        //private static string version = "0.6.1";
        public override string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString(); 
        }

        public static Dictionary<string, BossInfo> bossInfo;

        //public static Texture2D bgTex;
        public static GameObject canvas;
        public static Image bgImg, bossSelectImg;

        public static bool selectingStage;

        public static int selectX,selectY;
        public static RectTransform selectPos;

        public static Image[] bossFaces;
        public static Sprite[] bossFace1,bossFace2,bossFace3;
        public static Text[] bossText;

        public static Image imageSkip;
        public static Text textSkip, imageText;
        public static RectTransform rectSkip;

        public static Texture2D timeTex;

        public static List<ItemInfo> items;

        public static int grimmLevel, oldGrimmLevel = 0;
        public static int quakeLevel = 0;
        public static int currentBoss;

        public static GameObject shiny;

        public static GameObject shinySlot1,shinySlot2,shinySlot3;
        public static bool spawnSlot1, spawnSlot2, spawnSlot3;

        public static GameObject timeCounter;
        public static TextMesh timeText;
        public static float currentTime;

        public static GameManager gm;
        public static HeroController hc;

        public static int pickups = 0;
        public static bool flawless;

        public static UIButtonSkins uib;
        public static InputHandler ih;

        public static void FadeInSkip(float x)
        {
            textSkip.CrossFadeAlpha(1, x, true);
            imageSkip.CrossFadeAlpha(1, x, true);
            imageText.CrossFadeAlpha(1, x, true);
        }

        public static void FadeOutSkip(float x)
        {
            textSkip.CrossFadeAlpha(0, x, true);
            imageSkip.CrossFadeAlpha(0, x, true);
            imageText.CrossFadeAlpha(0, x, true);
        }

        public static void Teleport(string scenename, Vector3 pos)
        {

            //PlayerData.instance.nailDamage = 65;

            if (hc == null)
            {
                hc = gm.hero_ctrl;

                for (int i = 1; i <= 40; i++)
                {
                    PlayerData.instance.SetIntInternal("charmCost_" + i, 0);
                }
            }

            HeroController.instance.RegainControl();
            HeroController.instance.StartAnimationControl();
            HeroController.instance.cState.invulnerable = false;
            PlayerData.instance.disablePause = false;

            GameCameras.instance.hudCamera.enabled = true;

            bgImg.enabled = true;
            BossInfo.battleScene = null;

            BossRush.selectX = 1;
            BossRush.selectY = 1;
            BossRush.selectingStage = false;

            bossSelectImg.enabled = false;
            selectPos.GetComponent<Image>().enabled = false;
            for (int i = 0; i < bossFaces.Length; i++)
            {
                bossFaces[i].enabled = false;
                bossText[i].enabled = false;
            }
            if (BossInfo.defeatedBosses % 9 == 8)
            {
                if (BossInfo.defeatedBosses < 9)
                    for (int i = 0; i < bossFaces.Length; i++)
                        bossFaces[i].sprite = bossFace2[i];
                else
                    for (int i = 0; i < bossFaces.Length; i++)
                        bossFaces[i].sprite = bossFace3[i];
            }

            pickups = 0;

            PlayerData.instance.soulLimited = false;
            PlayerData.instance.EndSoulLimiter();
            if (!ItemInfo.upgrades.ContainsKey("quakeLevel"))
                ItemInfo.upgrades["quakeLevel"] = 0;
            quakeLevel = ItemInfo.upgrades["quakeLevel"];
            if (scenename == "Ruins1_24")
                PlayerData.instance.quakeLevel = 0;
            else
                PlayerData.instance.quakeLevel = quakeLevel;
            //PlayerData.instance.quakeLevel = 0;

            PlayerData.instance.canDash = PlayerData.instance.hasDash;
            PlayerData.instance.canWallJump = PlayerData.instance.hasWalljump;

            if (PlayerData.instance.royalCharmState > 0 && !PlayerData.instance.gotCharm_36)
            {
                PlayerData.instance.gotCharm_36 = true;
                PlayerData.instance.equippedCharm_36 = true;
            }

            oldGrimmLevel = grimmLevel;

            PlayerData.instance.metGrimm = true;
            PlayerData.instance.foughtGrimm = true;
            if (scenename == "Grimm_Main_Tent")
                PlayerData.instance.SetIntInternal("grimmChildLevel", 2);
            else
                PlayerData.instance.SetIntInternal("grimmChildLevel", grimmLevel);
            PlayerData.instance.SetIntInternal("flamesCollected", 3);
            PlayerData.instance.SetBoolInternal("grimmChildAwoken", false);
            PlayerData.instance.SetBoolInternal("foughtGrimm", false);
            PlayerData.instance.SetBoolInternal("killedGrimm", false);

            if (grimmLevel > 0 || scenename == "Grimm_Main_Tent")
            {
                PlayerData.instance.equippedCharm_40 = true;
            }

            PlayerData.instance.hasCharm = true;
            PlayerData.instance.hasSpell = true;
            PlayerData.instance.hasNailArt = true;
            PlayerData.instance.hasDreamNail = true;

            PlayerData.instance.fragileGreed_unbreakable = true;
            PlayerData.instance.fragileHealth_unbreakable = true;
            PlayerData.instance.fragileStrength_unbreakable = true;

            hc.proxyFSM.SendEvent("HeroCtrl-MaxHealth");
            hc.proxyFSM.SendEvent("HeroCtrl-Healed");
            hc.proxyFSM.SendEvent("BENCHREST");
            hc.proxyFSM.SendEvent("HERO REVIVED");
            GameCameras.instance.soulOrbFSM.SendEvent("MP GAIN");
            gm.soulVessel_fsm.SendEvent("MP RESERVE UP");
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");

            PlayerData.instance.MaxHealth();
            HeroController.instance.CharmUpdate();
            PlayerData.instance.UpdateBlueHealth();
            PlayMakerFSM.BroadcastEvent("UPDATE BLUE HEALTH");

            //VVV TELEPORT CODE VVV

            hc.transform.position = pos;

            hc.EnterWithoutInput(false);
            hc.proxyFSM.SendEvent("HeroCtrl-LeavingScene");
            hc.transform.SetParent(null);

            gm.NoLongerFirstGame();
            gm.SaveLevelState();
            gm.SetState(GameState.EXITING_LEVEL);
            gm.entryGateName = "dreamGate";
            gm.cameraCtrl.FreezeInPlace(false);

            hc.ResetState();

            gm.LoadScene(scenename);
        }

        public override void Initialize()
        {
            ModHooks.Logger.Log("Initializing BossRush");

            BossInfo.createBossInfo();
            ItemInfo.createItemInfo();
            BossInfo.assignItems();

            CanvasUtil.createFonts();

            shiny = null;

            uib = UIManager.instance.uiButtonSkins;
            ih = GameManager.instance.inputHandler;

            canvas = CanvasUtil.createCanvas(1920,1080);

            textSkip = CanvasUtil.createTextPanel(canvas, "Press to skip without picking up items", 960, 30, 1920, 50, 24).GetComponent<Text>();
            imageSkip = CanvasUtil.createImagePanel(canvas, 960, 90, 58, 58).GetComponent<Image>();
            imageText = CanvasUtil.createTextPanel(canvas, "", 960, 90, 58, 58, 24).GetComponent<Text>();
            rectSkip = imageSkip.gameObject.GetComponent<RectTransform>();

            FadeOutSkip(0);

            byte[] bossFaces1 = ResourceLoader.loadBossFaces1();
            bossFaces = new UnityEngine.UI.Image[9];
            bossFace1 = new Sprite[9];
            bossFace2 = new Sprite[9];
            bossFace3 = new Sprite[9];
            bossText = new UnityEngine.UI.Text[9];

            bgImg = CanvasUtil.createImagePanel(canvas, ResourceLoader.loadBackground(), 960, 540, 1920,1080, 0, 0, 1280, 720).GetComponent<Image>();
            bossSelectImg = CanvasUtil.createImagePanel(canvas, ResourceLoader.loadBossSelect(), 960, 540, 1920, 1080, 0, 0, 1920, 1080).GetComponent<Image>();
            selectPos = CanvasUtil.createImagePanel(canvas, ResourceLoader.loadSelect(), 960, 540, 229, 193, 0, 0, 228, 193).GetComponent<RectTransform>();

            for (int i = 0; i < bossFaces.Length; i++)
            {
                int x = (i%3);
                int y = i / 3;
                bossFaces[i] = CanvasUtil.createImagePanel(canvas, ResourceLoader.loadBossFaces1(), (233 * x) + 727, (232 * y) + 325, 211, 176, (x * 211) + 1, (y * 177) + 1, 207, 174).GetComponent<UnityEngine.UI.Image>();
                bossText[i] = CanvasUtil.createTextPanel(canvas, (233 * x) + 727, (232 * y) + 217, 211, 30).GetComponent<UnityEngine.UI.Text>();
            }
            for (int i = 0; i < bossFaces.Length; i++)
            {
                int x = (i%3);
                int y = i / 3;
                bossFace1[i] = CanvasUtil.createSprite(ResourceLoader.loadBossFaces1(), (x * 211) + 1, (y * 177) + 1, 207, 174);
                bossFace2[i] = CanvasUtil.createSprite(ResourceLoader.loadBossFaces2(), (x * 211) + 1, (y * 177) + 1, 207, 174);
                bossFace3[i] = CanvasUtil.createSprite(ResourceLoader.loadBossFaces3(), (x * 211) + 1, (y * 177) + 1, 207, 174);
            }

           selectPos.SetAsLastSibling();

            GameObject.DontDestroyOnLoad(canvas);

            bgImg.enabled = false;
            bossSelectImg.enabled = false;
            selectPos.GetComponent<UnityEngine.UI.Image>().enabled = false;
            for (int i = 0; i < bossFaces.Length; i++)
            {
                bossFaces[i].enabled = false;
                bossText[i].enabled = false;
            }

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += onSceneLoad;

            ModHooks.Instance.TakeHealthHook += tookDamage;
            ModHooks.Instance.ColliderCreateHook += onCollider;
            ModHooks.Instance.NewGameHook += resetData;
            //ModHooks.Instance.TakeDamageHook += tookDamage;

            ModHooks.Instance.LanguageGetHook += PromptOverride;

            ModHooks.Logger.Log("Initialized BossRush");
        }

        public string PromptOverride(string key, string sheet)
        {
            if (key == "KDT_SUPER")
                return "Thanks";
            if (key == "KDT_MAIN")
                return "for";
            if (key == "KDT_SUB")
                return "Playing!";
            if (key == "INSPECT")
                return "";
            if (key == "item0")
                return BossInfo.itemName(0);
            if (key == "item1")
                return BossInfo.itemName(1);
            if (key == "item2")
                return BossInfo.itemName(2);
            return Language.Language.GetInternal(key, sheet);
        }

        public int tookDamage(int d)
        {
            if (d > 0 && (hc.damageMode != DamageMode.NO_DAMAGE && !hc.cState.invulnerable && !hc.cState.recoiling && !PlayerData.instance.isInvincible && !hc.cState.dead && !hc.cState.hazardDeath))
            {
                flawless = false;
                if (PlayerData.instance.health - d > 1)
                {
                    BossRush.hc.normalSlash.SetFury(false);
                    BossRush.hc.normalSlashFsm.FsmVariables.GetFsmFloat("Multiplier").Value = 1.00f;
                    BossRush.hc.alternateSlash.SetFury(false);
                    BossRush.hc.alternateSlashFsm.FsmVariables.GetFsmFloat("Multiplier").Value = 1.00f;
                    BossRush.hc.upSlash.SetFury(false);
                    BossRush.hc.upSlashFsm.FsmVariables.GetFsmFloat("Multiplier").Value = 1.00f;
                    BossRush.hc.downSlash.SetFury(false);
                    BossRush.hc.downSlashFsm.FsmVariables.GetFsmFloat("Multiplier").Value = 1.00f;
                    BossRush.hc.wallSlash.SetFury(false);
                    BossRush.hc.wallSlashFsm.FsmVariables.GetFsmFloat("Multiplier").Value = 1.00f;
                }
            }
            return d;
        }

        public void onSceneLoad(Scene dst, LoadSceneMode lsm)
        {

            if (gm == null)
            {
                gm = GameManager.instance;
                gm.gameObject.AddComponent<BossRushUpdate>();
            }

            HeroController.instance.RegainControl();
            HeroController.instance.cState.Reset();
            bgImg.enabled = false;

            RemoveTransitions();

            if (PlayerData.instance.equippedCharm_6)
            {
                BossRush.hc.normalSlash.SetFury(true);
                BossRush.hc.normalSlashFsm.FsmVariables.GetFsmFloat("Multiplier").Value = 1.75f;
                BossRush.hc.alternateSlash.SetFury(true);
                BossRush.hc.alternateSlashFsm.FsmVariables.GetFsmFloat("Multiplier").Value = 1.75f;
                BossRush.hc.upSlash.SetFury(true);
                BossRush.hc.upSlashFsm.FsmVariables.GetFsmFloat("Multiplier").Value = 1.75f;
                BossRush.hc.downSlash.SetFury(true);
                BossRush.hc.downSlashFsm.FsmVariables.GetFsmFloat("Multiplier").Value = 1.75f;
                BossRush.hc.wallSlash.SetFury(true);
                BossRush.hc.wallSlashFsm.FsmVariables.GetFsmFloat("Multiplier").Value = 1.75f;
            }

            if( timeCounter == null ){
                timeCounter = GameObject.Instantiate(GameCameras.instance.geoCounter.gameObject);
                timeCounter.transform.position = new Vector3(-4.1f, -2.0f, 0);
                foreach (PlayMakerFSM fsm in timeCounter.GetComponentsInChildren<PlayMakerFSM>())
                {
                    GameObject.Destroy(fsm);
                }
                timeCounter.transform.FindChild("Add Text").GetComponent<TextMesh>().text = "";
                GameObject.Destroy(timeCounter.transform.FindChild("Add Text"));
                timeCounter.transform.FindChild("Subtract Text").GetComponent<TextMesh>().text = "";
                GameObject.Destroy(timeCounter.transform.FindChild("Subtract Text"));
                GameObject.Destroy(timeCounter.GetComponent<GeoCounter>());

                GameObject.Destroy(timeCounter.transform.FindChild("Geo Sprite").GetComponent<tk2dSprite>());
                timeCounter.transform.FindChild("Geo Sprite").gameObject.AddComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(timeTex, new Rect(0, 0, 64, 64), Vector2.zero);

                timeText = timeCounter.transform.FindChild("Geo Text").gameObject.GetComponent<TextMesh>();

                GameObject.DontDestroyOnLoad(timeCounter);
            }
                

            //dataDump(GameCameras.instance.geoCounter.gameObject, 1);
        }

        private void RemoveTransitions()
        {
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                TransitionPoint gate = obj.GetComponent<TransitionPoint>();

                if (gate != null)
                {
                    GameObject.Destroy(gate);

                    GateSnap snap = obj.GetComponent<GateSnap>();
                    BoxCollider2D box = obj.GetComponent<BoxCollider2D>();

                    if (snap != null)
                    {
                        GameObject.Destroy(snap);
                    }

                    if (box != null)
                    {
                        box.isTrigger = false;
                    }

                    obj.layer = 8;

                    if (obj.name.Contains("door"))
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }

        public void resetData()
        {
            UnityEngine.Random.seed = System.DateTime.Now.Millisecond;
            GameObject.Destroy(shinySlot1);
            GameObject.Destroy(shinySlot2);
            GameObject.Destroy(shinySlot3);
            currentTime = 0;
            shiny = null;
            flawless = true;
            shinySlot1 = null;
            shinySlot2 = null;
            shinySlot3 = null;
            currentBoss = 4;
            grimmLevel = 0;
            oldGrimmLevel = 0;
            quakeLevel = 0;

            for (int i = 0; i < bossFaces.Length; i++)
                bossFaces[i].sprite = bossFace1[i];

            BossInfo.killedHK = false;
            BossInfo.currentBoss = 4;
            BossInfo.createBossInfo();       
            ItemInfo.createItemInfo();
            BossInfo.assignItems();
                 
        }

        public void dataDump(GameObject go, int depth)
        {
            ModHooks.ModLog(new String('-', depth) + go.name);
            foreach (Component comp in go.GetComponents<Component>())
            {
                ModHooks.ModLog(new String('_', depth) + comp.GetType().ToString());
            }
            foreach (Transform child in go.transform)
            {
                dataDump(child.gameObject, depth + 1);
            }
        }

        public void createLabel(GameObject go, int i)
        {
            if (go?.transform == null)
                return;
            
            GameObject label = new GameObject("Label");

            Transform child = go.transform.FindChild("Arrow Prompt(Clone)/Labels/Inspect");

            if (child?.gameObject == null)
                return;

            GameObject oldLabel = child.gameObject;

            if (oldLabel?.transform?.parent?.parent?.parent == null)
                return;


            //RectTransform
            RectTransform rectTransform = oldLabel.GetComponent<RectTransform>();
            if (rectTransform == null) return;

            label.AddComponent(rectTransform);

            //MeshRenderer
            MeshRenderer meshRenderer = oldLabel.GetComponent<MeshRenderer>();
            if (meshRenderer == null) return;

            label.AddComponent(meshRenderer);

            //MeshFilter
            MeshFilter meshFilter = oldLabel.GetComponent<MeshFilter>();
            if (meshFilter == null) return;

            label.AddComponent(meshFilter);

            //TextContainer
            TextContainer textContainer = oldLabel.GetComponent<TextContainer>();
            if (textContainer == null) return;

            label.AddComponent(textContainer);

            //TextMeshPro
            TextMeshPro textMeshPro = oldLabel.GetComponent<TextMeshPro>();
            if (textMeshPro == null) return;

            label.AddComponent(textMeshPro);

            //ChangeFontByLanguage
            ChangeFontByLanguage changeFontByLanguage = oldLabel.GetComponent<ChangeFontByLanguage>();
            if (changeFontByLanguage == null) return;

            label.AddComponent(changeFontByLanguage);


            if (label.transform?.localPosition == null)
                return;

            label.transform.parent = oldLabel.transform.parent.parent.parent;
            label.transform.localPosition = label.transform.localPosition + (Vector3.up * 2.4f);
            label.transform.localPosition = label.transform.localPosition + (Vector3.right * 0.14f);
            label.SetActive(true);

            TextMeshPro tmp = label.GetComponent<TextMeshPro>();
            if (tmp == null)
                return;

            ChangeFontByLanguage fonts = label.GetComponent<ChangeFontByLanguage>();
            if (fonts == null)
                return;

            tmp.font = fonts.defaultFont;
            string a = Language.Language.CurrentLanguage().ToString();

            switch (a)
            {
                case "JA":
                    tmp.font = fonts.fontJA;
                    break;
                case "RU":
                    tmp.font = fonts.fontRU;
                    break;
            }
            tmp.fontSize = 7;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = BossInfo.itemName(i);

            TextMeshPro textMeshPro2 = label.GetComponent<TextMeshPro>();
            if( textMeshPro2 != null)
                tmp.faceColor = new Color32(textMeshPro2.faceColor.r, textMeshPro2.faceColor.g, textMeshPro2.faceColor.b, 0);

            ItemTextFader itf = go.AddComponent<ItemTextFader>();
            if (itf != null)
            {
                itf.tmp = tmp;
                itf.id = i;
            }

        }

        public void onCollider(GameObject go)
        {

            if (go?.transform?.parent?.gameObject == null)
                return;

            if (gm?.sceneName == null)
                return;


            //ModHooks.ModLog(go.transform.parent.gameObject.name);
            if (go.transform.parent.gameObject.name == "ITEM_0")
            {
                createLabel(go, 0);
            }
            
            if (go.transform.parent.gameObject.name == "ITEM_1")
            {
                createLabel(go, 1);
            }
            if (go.transform.parent.gameObject.name.Contains("ITEM_2"))
            {
                createLabel(go, 2);
            }
            if (gm.sceneName == "Crossroads_10")
            {
                PlayMakerFSM hme = FSMUtility.LocateFSM(go, "health_manager_enemy");
                if (hme != null)
                {
                    if (hme.FsmVariables.GetFsmInt("hp").Value < 15)
                    {
                        UnityEngine.GameObject.Destroy(go);
                    }
                }
            }
            if (shinySlot3 == null)
            {
                if (go.name == "Inspect Region")
                {
                    //dataDump(go.transform.parent.gameObject, 1);

                    FsmUtils.FsmUtil.LogFSM(go.GetComponent<PlayMakerFSM>());

                    shinySlot1 = UnityEngine.GameObject.Instantiate(go.transform.parent.gameObject);
                    shinySlot1.name = "ITEM_0";
                    //shinySlot1.transform.position = new Vector2(-100, -100);

                    shinySlot2 = UnityEngine.GameObject.Instantiate(go.transform.parent.gameObject);
                    shinySlot2.name = "ITEM_1";
                    //shinySlot2.transform.position = new Vector2(-100, -100);

                    shinySlot3 = UnityEngine.GameObject.Instantiate(go.transform.parent.gameObject);
                    shinySlot3.name = "ITEM_2";
                    //shinySlot3.transform.position = new Vector2(-100, -100);

                    //UnityEngine.GameObject.DontDestroyOnLoad(shiny);
                    UnityEngine.GameObject.DontDestroyOnLoad(shinySlot1);
                    UnityEngine.GameObject.DontDestroyOnLoad(shinySlot2);
                    UnityEngine.GameObject.DontDestroyOnLoad(shinySlot3);
                }
            }
            else
            {
                if (go.name == "Inspect Region")
                {
                    PlayMakerFSM dash_check = FSMUtility.LocateFSM(go.transform.parent.gameObject, "Shiny Control");
                    if (dash_check.FsmVariables.GetFsmBool("Dash Cloak").Value)
                        UnityEngine.GameObject.Destroy(go.transform.parent.gameObject);
                }
            }
        }

    }
}
