using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using GlobalEnums;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Drawing;
using BossRush.Properties;
using UnityEngine.UI;
using HutongGames.PlayMaker;

namespace BossRush
{
    public class BossRush : Mod
    {
        private static string version = "0.5.0";
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
        public static GameManager gm;
        public static HeroController hc;

        public static int pickups = 0;
        public static bool flawless;

        public static byte currentlyLoading;

        public static bool spawnedFirst;
        public static bool spawnedSecond;
        public static bool spawnedThird;

        public static void Teleport(string scenename, Vector3 pos)
        {
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

            spawnedFirst = false;
            spawnedSecond = false;
            spawnedThird = false;

            quakeLevel = PlayerData.instance.quakeLevel;
            PlayerData.instance.quakeLevel = 0;

            PlayerData.instance.canDash = PlayerData.instance.hasDash;
            PlayerData.instance.canWallJump = PlayerData.instance.hasWalljump;

            PlayerData.instance.equippedCharm_40 = true;

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

            currentlyLoading = 255;

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
            {
                BossInfo.updateText();
                return BossInfo.itemName(1);
            }
            if (sheet == "item0")
                return BossInfo.itemName(0);
            if (sheet == "item1")
                return BossInfo.itemName(1);
            if (sheet == "item2")
                return BossInfo.itemName(2);
            return Language.Language.GetInternal(key, sheet);
        }

        public int tookDamage(ref int type, int d)
        {
            if( d > 0 )
                flawless = false;
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
            BossInfo.createBossInfo();       
            ItemInfo.createItemInfo();
            BossInfo.assignItems();
                 
        }

        public void onCollider(GameObject go)
        {
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
            if (shiny == null)
            {
                if (go.name == "Inspect Region")
                {
                    shiny = UnityEngine.GameObject.Instantiate(go.transform.parent.gameObject);

                    UnityEngine.GameObject.DontDestroyOnLoad(shiny);

                    UnityEngine.GameObject.Destroy(go.transform.parent.gameObject);
                }
            }
            else
            {
                if (go.name == "Inspect Region")
                {
                    PlayMakerFSM dash_check = FSMUtility.LocateFSM(go.transform.parent.gameObject, "Shiny Control");
                    if (dash_check.FsmVariables.GetFsmBool("Dash Cloak").Value)
                        UnityEngine.GameObject.Destroy(go.transform.parent.gameObject);
                    if (dash_check.FsmVariables.GetFsmInt("Trinket Num").Value == 1)
                    {
                        spawnedFirst = true;
                    }
                    if (dash_check.FsmVariables.GetFsmInt("Trinket Num").Value == 2)
                    {
                        spawnedSecond = true;
                    }
                    if (dash_check.FsmVariables.GetFsmInt("Trinket Num").Value == 3)
                    {
                        spawnedThird = true;
                    }
                    if (spawnedFirst & spawnedSecond & spawnedThird)
                        BossInfo.updateText();
                }
            }
        }

    }
}
