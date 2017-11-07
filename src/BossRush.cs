using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using GlobalEnums;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Drawing;
using BossRush.Properties;
using UnityEngine;
using UnityEngine.UI;
using HutongGames.PlayMaker;
using System.Reflection;
using TMPro;

namespace BossRush
{
    public class BossRush : Mod
    {
        private static string version = "0.6.1";
        public override string GetVersion()
        {
            return version;
        }

        public static Dictionary<string, BossInfo> bossInfo;

        public static Texture2D bgTex;
        public static GameObject canvas;
        public static UnityEngine.UI.Image bgImg;

        public static List<ItemInfo> items;

        public static int grimmLevel, oldGrimmLevel = 0;

        public static int quakeLevel = 0;

        public static int currentBoss;

        public static GameObject shiny;

        public static GameObject shinySlot1;
        public static GameObject shinySlot2;
        public static GameObject shinySlot3;
        public static bool spawnSlot1, spawnSlot2, spawnSlot3;

        public static GameManager gm;
        public static HeroController hc;

        public static int pickups = 0;
        public static bool flawless;

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
            bgImg.enabled = true;
            BossInfo.battleScene = null;

            pickups = 0;
            flawless = true;

            quakeLevel = PlayerData.instance.quakeLevel;
            PlayerData.instance.quakeLevel = 0;

            PlayerData.instance.canDash = PlayerData.instance.hasDash;
            PlayerData.instance.canWallJump = PlayerData.instance.hasWalljump;

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
            ModHooks.ModLog("Initializing BossRush");

            BossInfo.createBossInfo();
            ItemInfo.createItemInfo();
            BossInfo.assignItems();

            shiny = null;
            
            byte[] bg = ResourceLoader.loadBackground();
            bgTex = new Texture2D(1280, 720);
            bgTex.LoadImage(bg);

            canvas = new GameObject();
            canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler cs = canvas.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1280, 720);
            canvas.AddComponent<GraphicRaycaster>();

            GameObject panel = new GameObject();
            panel.transform.parent = canvas.transform;
            panel.AddComponent<CanvasRenderer>();
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1280, 720);
            rt.anchorMax = new Vector2(0, 0);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchoredPosition = new Vector2(640, 360);
            bgImg = panel.AddComponent<UnityEngine.UI.Image>();
            bgImg.sprite = Sprite.Create(bgTex, new Rect(0, 0, 1280, 720), Vector2.zero);

            GameObject.DontDestroyOnLoad(canvas);
            GameObject.DontDestroyOnLoad(bgImg);

            bgImg.enabled = false;

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += onSceneLoad;

            ModHooks.Instance.ColliderCreateHook += onCollider;
            ModHooks.Instance.NewGameHook += resetData;
            ModHooks.Instance.TakeDamageHook += tookDamage;

            ModHooks.Instance.LanguageGetHook += PromptOverride;

            ModHooks.ModLog("Initialized BossRush");
        }

        public string PromptOverride(string key, string sheet)
        {
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

        public int tookDamage(ref int type, int d)
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
            HeroController.instance.RegainControl();
            HeroController.instance.cState.Reset();
            bgImg.enabled = false;

            RemoveTransitions();
            PlayerData.instance.quakeLevel = quakeLevel;
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
            currentBoss = 0;
            BossInfo.currentBoss = 0;
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
            GameObject label = new GameObject("Label");

            GameObject oldLabel = go.transform.FindChild("Arrow Prompt(Clone)/Labels/Inspect").gameObject;

            //RectTransform
            label.AddComponent<RectTransform>(oldLabel.GetComponent<RectTransform>());
            //MeshRenderer
            label.AddComponent<MeshRenderer>(oldLabel.GetComponent<MeshRenderer>());
            //MeshFilter
            label.AddComponent<MeshFilter>(oldLabel.GetComponent<MeshFilter>());
            //TextContainer
            label.AddComponent<TextContainer>(oldLabel.GetComponent<TextContainer>());
            //TextMeshPro
            label.AddComponent<TextMeshPro>(oldLabel.GetComponent<TextMeshPro>());
            //ChangeFontByLanguage
            label.AddComponent<ChangeFontByLanguage>(oldLabel.GetComponent<ChangeFontByLanguage>());

            label.transform.parent = oldLabel.transform.parent.parent.parent;
            label.transform.localPosition = label.transform.localPosition + (Vector3.up * 2.4f);
            label.transform.localPosition = label.transform.localPosition + (Vector3.right * 0.14f);
            label.SetActive(true);

            TextMeshPro tmp = label.GetComponent<TextMeshPro>();
            ChangeFontByLanguage fonts = label.GetComponent<ChangeFontByLanguage>();
            tmp.font = fonts.defaultFont;
            string a = Language.Language.CurrentLanguage().ToString();
            if (a == "JA")
            {
                tmp.font = fonts.fontJA;
            }
            if (a == "RU")
            {
                tmp.font = fonts.fontRU;
            }
            tmp.fontSize = 7;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = BossInfo.itemName(i);
            tmp.faceColor = new Color32(label.GetComponent<TextMeshPro>().faceColor.r, label.GetComponent<TextMeshPro>().faceColor.g, label.GetComponent<TextMeshPro>().faceColor.b, 0);

            ItemTextFader itf = go.AddComponent<ItemTextFader>();
            itf.tmp = tmp;
            itf.id = i;

        }

        public void onCollider(GameObject go)
        {

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
            if (gm == null)
            {
                gm = GameManager.instance;
                gm.gameObject.AddComponent<BossRushUpdate>();
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
