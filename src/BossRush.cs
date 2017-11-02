﻿using System;
using System.Collections.Generic;
using System.Linq;
using Modding;
using GlobalEnums;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BossRush
{
    public class BossRush : Mod
    {
        private static string version = "0.2.5";
        public override string GetVersion()
        {
            return version;
        }
        
        public static BossData[] bossData;
        public static int currentBoss;
 
        public static GameManager gm;
        public static HeroController hc;

        public static List<string> items;

        public static int fireballLevel = 0;
        public static int quakeLevel = 0;
        public static int screamLevel = 0;

        public static int hp;
        public static int mp;
        public static int nail;

        public static bool hasDash;

        public static GameObject shiny;

        public static bool bossKilled(){
            return PlayerData.instance.GetBoolInternal(bossData[currentBoss].killedVar); 
        }

        public static string boolToString(string b)
        {
            if (b.StartsWith("gotCharm")){
                return Language.Language.Get("CHARM_NAME_"+b.Split('_')[1], "UI");
            }
            if (b == "fireballLevel")
                return Language.Language.Get( fireballLevel == 0 ? "INV_NAME_SPELL_FIREBALL1" : "INV_NAME_SPELL_FIREBALL2", "UI");
            if (b == "quakeLevel")
                return Language.Language.Get(quakeLevel == 0 ? "INV_NAME_SPELL_QUAKE1" : "INV_NAME_SPELL_QUAKE2", "UI" );
            if (b == "screamLevel")
                return Language.Language.Get(fireballLevel == 0 ? "INV_NAME_SPELL_SCREAM1" : "INV_NAME_SPELL_SCREAM2", "UI" );
            if (b == "hpLevel")
                return "HP";
            if (b == "mpLevel")
                return "MP";

            //TODO: Language Specific 
            if (b == "nailLevel")
                return "Nail";
            if (b == "hasDashSlash")
                return "Great Slash";
            if (b == "hasUpwardSlash")
                return "Dash Slash";
            if (b == "hasCyclone")
                return "Cyclone";
            if (b == "hasDash")
                return PlayerData.instance.hasDash ? "Shadow Cloak" : "Mothwing Cloak";
            if (b == "hasDoubleJump")
                return "Monarch Wings";
            if (b == "hasWallJump")
                return "Mantis Claw";
            ModHooks.ModLog("[NGG] Unknown Item: " + b);
            return "???";
        }

        public static string getRandomItem()
        {
            int r = UnityEngine.Random.RandomRange(0, items.Count());
            string ret = items.ElementAt(r);
            items.RemoveAt(r);
            return ret;
        }


        public static string getScene()
        {
            return bossData[currentBoss].sceneName;
        }

        public static Vector2 getPos()
        {
            return bossData[currentBoss].scenePos;
        }

        public static string getCurrentBossDrops(int x)
        {
            return bossData[currentBoss].drops[x];
        }

        public static bool getCurrentItemValue(int x)
        {
            string pdbool = bossData[currentBoss].drops[x];

            if (pdbool == "hasDash")
                return hasDash ? PlayerData.instance.hasShadowDash : PlayerData.instance.hasDash;
            if (pdbool == "hpLevel")
                return PlayerData.instance.maxHealthBase > hp;
            if (pdbool == "mpLevel")
                return PlayerData.instance.MPReserveMax > mp;
            if (pdbool == "nailLevel")
                return PlayerData.instance.nailDamage > nail;
            if( pdbool != "fireballLevel" && pdbool != "quakeLevel" && pdbool != "screamLevel")
                return PlayerData.instance.GetBoolInternal(pdbool);
            else{
                if( pdbool == "fireballLevel")
                    return PlayerData.instance.GetIntInternal(pdbool) > fireballLevel;
                if (pdbool == "quakeLevel")
                    return PlayerData.instance.GetIntInternal(pdbool) > quakeLevel;
                return PlayerData.instance.GetIntInternal(pdbool) > screamLevel;
            }
                
        }

        public static void Teleport(string scenename, Vector3 pos)
        {
            if (hc == null)
            {
                hc = gm.hero_ctrl;
            }

            PlayerData.instance.equippedCharm_40 = true;

            PlayerData.instance.metGrimm = true;
            PlayerData.instance.foughtGrimm = true;
            if (scenename == "Grimm_Main_Tent")
                PlayerData.instance.SetIntInternal("grimmChildLevel", 2);
            else
                PlayerData.instance.SetIntInternal("grimmChildLevel", 0);
            PlayerData.instance.SetIntInternal("flamesCollected", 3);
            PlayerData.instance.SetBoolInternal("grimmChildAwoken", false);
            PlayerData.instance.SetBoolInternal("foughtGrimm", false);
            PlayerData.instance.SetBoolInternal("killedGrimm", false);

            PlayerData.instance.hasCharm = true;
            PlayerData.instance.hasSpell = true;
            PlayerData.instance.hasNailArt = true;
            PlayerData.instance.hasDreamNail = true;

            fireballLevel = PlayerData.instance.fireballLevel;
            quakeLevel = PlayerData.instance.quakeLevel;
            screamLevel = PlayerData.instance.screamLevel;

            //PlayerData.instance.nailDamage = 300;

            hp = PlayerData.instance.maxHealthBase;
            mp = PlayerData.instance.MPReserveMax;
            nail = PlayerData.instance.nailDamage;

            hasDash = PlayerData.instance.hasDash;

            hc.proxyFSM.SendEvent("HeroCtrl-MaxHealth");
            hc.proxyFSM.SendEvent("HeroCtrl-Healed");
            hc.proxyFSM.SendEvent("BENCHREST");
            hc.proxyFSM.SendEvent("HERO REVIVED");
            GameCameras.instance.soulOrbFSM.SendEvent("MP GAIN");
            gm.soulVessel_fsm.SendEvent("MP RESERVE UP");
            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");

            PlayerData.instance.MaxHealth();
            HeroController.instance.CharmUpdate();

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

        public static void newBossData(){
            bossData = new BossData[]{
                new BossData("False Knight", "Crossroads_10", new Vector2(13.81485f, 27.40562f), createDrops(), new ItemPos(), "falseKnightDefeated"),
                //Mawlek
                //Gruz Mother TODO: Fix item positions, to prevent items falling OOB
                new BossData("Gruz Mother", "Crossroads_04", new Vector2(95.01903f, 15.40562f), createDrops(), new ItemPos(), "killedBigFly"),
                new BossData("Hornet", "Fungus1_04", new Vector2(26.61754f, 28.40562f), createDrops(), new ItemPos(), "hornet1Defeated"),
                //Mantis Lords TODO: Fix item positions, to prevent items falling into spikes
                new BossData("Mantis Lords", "Fungus2_15", new Vector2(30.3f, 7.405624f), createDrops(), new ItemPos(), "defeatedMantisLords"),
                //Soul Master TODO: Dive breaks this fight, disable as loading
                //new BossData("Soul Master", "Ruins1_24", new Vector2(33.49188f, 29.40562f), createDrops(), "mageLordDefeated"),
                //CG1 TODO: Bench can cause softlocks
                new BossData("CG1", "Mines_18", new Vector2(30.07801f, 11.40562f), createDrops(), new ItemPos(), "killedMegaBeamMiner"),
                //Nosk
                //Flukemarm
	            new BossData("Watcher Knights", "Ruins2_03", new Vector2(44.2238f, 70.40561f), createDrops(), new ItemPos(), "killedBlackKnight"),                
	            new BossData("Dung Defender", "Waterways_05", new Vector2(83.09189f, 7.405624f), createDrops(), new ItemPos(), "defeatedDungDefender"),
                //Grimm TODO: Try and skip conversation and just start fight instantly
                new BossData("Grimm", "Grimm_Main_Tent", new Vector2(95.12892f, 6.405625f), createDrops(), new ItemPos(), "killedGrimm"),
                //Uumuu TODO: Fix item positions, to prevent items falling into acid
	            new BossData("Uumuu", "Fungus3_archive_02", new Vector2(53.54183f, 110.4056f), createDrops(), new ItemPos(), "defeatedMegaJelly"),
	            new BossData("Collector", "Ruins2_11", new Vector2(47.50203f, 95.40561f), createDrops(), new ItemPos(), "collectorDefeated"),
                //CG2
	            new BossData("Traitor Lord", "Fungus3_23", new Vector2(33.74541f, 29.40562f), createDrops(), new ItemPos(), "killedTraitorLord"),
	            new BossData("Hornet2", "Deepnest_East_Hornet", new Vector2(24.43399f, 28.40562f), createDrops(), new ItemPos(), "hornetOutskirtsDefeated"),
	            new BossData("Broken Vessel", "Abyss_19", new Vector2(24.92674f, 28.40562f), createDrops(), new ItemPos(), "killedInfectedKnight"),
                new BossData("Lost Kin", "Dream_03_Infected_Knight", new Vector2(44.98343f, 28.40562f), createDrops(), new ItemPos(), "infectedKnightDreamDefeated"),
                //Soul Tyrant possible to softlock by getting stuck under the ground
	            new BossData("Soul Tyrant", "Dream_02_Mage_Lord", new Vector2(44.98343f, 28.40562f), createDrops(), new ItemPos(), "mageLordDreamDefeated"),
                new BossData("Failed Knight", "Dream_01_False_Knight", new Vector2(44.98343f, 28.40562f), createDrops(), new ItemPos(), "falseKnightDreamDefeated"),
                //White Defender
                //GPZ
                new BossData("Nightmare King Grimm", "Grimm_Nightmare", new Vector2(92.09892f, 6.405625f), createDrops(), new ItemPos(), "defeatedNightmareGrimm"),
                //THK
	            new BossData("Radiance", "Dream_Final_Boss", new Vector2(44.98343f, 28.40562f), createDrops(), new ItemPos(), "killedHollowKnight")
            };
        }

        public override void Initialize()
        {
            ModHooks.ModLog("Initializing BossRush");

            items = new List<string>();

            for (int i = 4; i < 40; i++)
            {
                string charm = "gotCharm_" + i;
                if( i != 36 )
                    items.Add(charm);
            }
            items.Add("hasDash"); items.Add("hasDash"); items.Add("hasDoubleJump"); items.Add("hasWallJump");
            items.Add("fireballLevel"); items.Add("fireballLevel");
            items.Add("quakeLevel"); items.Add("quakeLevel");
            items.Add("screamLevel"); items.Add("screamLevel");
            items.Add("nailLevel"); items.Add("nailLevel"); items.Add("nailLevel"); items.Add("nailLevel");
            items.Add("hpLevel"); items.Add("hpLevel"); items.Add("hpLevel"); items.Add("hpLevel");
            items.Add("mpLevel"); items.Add("mpLevel"); items.Add("mpLevel");
            items.Add("hasDashSlash"); items.Add("hasUpwardSlash"); items.Add("hasCyclone");

            newBossData();

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += onSceneLoad;

            ModHooks.Instance.ColliderCreateHook += onCollider;
            ModHooks.Instance.NewGameHook += resetData;

            ModHooks.ModLog("Initialized BossRush");
        }

        public void onSceneLoad(Scene dst, LoadSceneMode lsm)
        {
            RemoveTransitions();
        }

        public static Vector2 getItemPos(int x)
        {
            if (x == 1)
                return bossData[currentBoss].itemPositions.item1;
            if (x == 2)
                return bossData[currentBoss].itemPositions.item2;
            return bossData[currentBoss].itemPositions.item3;
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
            currentBoss = 0;

            items = new List<string>();

            for (int i = 4; i < 40; i++)
            {
                string charm = "gotCharm_" + i;
                if (i != 36)
                    items.Add(charm);
            }
            items.Add("hasDash"); items.Add("hasDash"); items.Add("hasDoubleJump"); items.Add("hasWallJump");
            items.Add("fireballLevel"); items.Add("fireballLevel");
            items.Add("quakeLevel"); items.Add("quakeLevel");
            items.Add("screamLevel"); items.Add("screamLevel");
            items.Add("nailLevel"); items.Add("nailLevel"); items.Add("nailLevel"); items.Add("nailLevel");
            items.Add("hpLevel"); items.Add("hpLevel"); items.Add("hpLevel"); items.Add("hpLevel");
            items.Add("mpLevel"); items.Add("mpLevel"); items.Add("mpLevel");
            items.Add("hasDashSlash"); items.Add("hasUpwardSlash"); items.Add("hasCyclone");

            newBossData();

        }

        public static string[] createDrops()
        {
            return new string[] { getRandomItem(), getRandomItem(), getRandomItem() };
        }

        public static void onPickup(string b, bool v) {
            string pdbool = "";
            if ( b == "gotCharm_1" )
                pdbool = bossData[currentBoss].drops[0];
            if ( b == "gotCharm_2" )
                pdbool = bossData[currentBoss].drops[1];
            if ( b == "gotCharm_3" )
                pdbool = bossData[currentBoss].drops[2];
            if (pdbool.StartsWith("gotCharm"))
            {
                PlayerData.instance.SetBoolInternal(pdbool, true);
                PlayerData.instance.SetBoolInternal("equippedCharm_" + pdbool.Split('_')[1], true);
            }
            if (pdbool == "fireballLevel" || pdbool == "quakeLevel" || pdbool == "screamLevel")
                PlayerData.instance.SetIntInternal(pdbool, PlayerData.instance.GetIntInternal(pdbool) + 1);
            if (pdbool == "nailLevel")
                PlayerData.instance.nailDamage += 4;
            if (pdbool == "hpLevel")
            {
                hc.AddToMaxHealth(1);
                hc.proxyFSM.BroadcastMessage("MAX HP UP");
                PlayMakerFSM.BroadcastEvent("MAX HP UP");
            }
                //
            if (pdbool == "mpLevel")
                hc.AddToMaxMPReserve(33);
            if (pdbool == "hasDashSlash" || pdbool == "hasUpwardSlash" || pdbool == "hasCyclone" || pdbool == "hasDoubleJump" )
                PlayerData.instance.SetBoolInternal(pdbool, true);
            if (pdbool == "hasDash")
            {
                if (PlayerData.instance.canDash)
                {
                    PlayerData.instance.canShadowDash = true;
                    PlayerData.instance.hasShadowDash = true;
                }
                else
                {
                    PlayerData.instance.SetBoolInternal(pdbool, true);
                    PlayerData.instance.canDash = true;
                }
            }
            if (pdbool == "hasWallJump")
            {
                PlayerData.instance.SetBoolInternal(pdbool, true);
                PlayerData.instance.canWallJump = true;
            }

            PlayerData.instance.MaxHealth();
            HeroController.instance.CharmUpdate();

            PlayMakerFSM.BroadcastEvent("UPDATE NAIL DAMAGE");

            PlayerData.instance.gotCharm_1 = false;
            PlayerData.instance.gotCharm_2 = false;
            PlayerData.instance.gotCharm_3 = false;
        }

        public void onCollider(GameObject go)
        {
            if (gm == null)
            {
                gm = GameManager.instance;
                gm.gameObject.AddComponent<BossRushUpdate>();
            }                
            if (shiny == null)
            {
                if (go.name == "Inspect Region")
                {
                    shiny = UnityEngine.GameObject.Instantiate(go.transform.parent.gameObject);
                    UnityEngine.GameObject.DontDestroyOnLoad(shiny);
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
