using System;
using System.Collections.Generic;
using UnityEngine;

namespace BossRush
{
    public class BossInfo
    {

        public static Dictionary<string, BossInfo> bossInfo;
        public static string[] bossOrder;
        public static int currentBoss;
        public static GameObject battleScene;
        public static PlayMakerFSM[] bsComponents;

        public BossInfo(string iN, string vN, string sN, Vector2 spawn, Vector2 i1, Vector2 i2, Vector2 i3)
        {
            internName = iN;
            varName = vN;
            sceneName = sN;
            spawnPos = spawn;
            item1 = i1;
            item2 = i2;
            item3 = i3;
            //itemHandler = new ItemHandler(iN);
        }

        private string internName;
        private string varName;
        private string sceneName;
        private Vector2 spawnPos;
        private Vector2 item1;
        private Vector2 item2;
        private Vector2 item3;
        private ItemHandler itemHandler;

        public string InternName { get { return internName; } }
        public string VarName { get { return varName; } }
        public string SceneName { get { return sceneName; } }
        public Vector2 ScenePos { get { return spawnPos; } }

        public Vector2 getItem(int id)
        {
            switch (id)
            {
                case 0: return item1;
                case 1: return item2;
                case 2: return item3;
                default: return new Vector2(-999, -999);
            }
        }

        public static void assignItems()
        {
            if (ItemInfo.itemInfo.Count < 3)
            {
                ItemInfo.itemInfo.AddRange(ItemInfo.unusedItems);
                ItemInfo.unusedItems.Clear();
            }
            bossInfo[bossOrder[currentBoss]].updateItemHandler(bossOrder[currentBoss]);
            //foreach (KeyValuePair<string, BossInfo> bi in bossInfo)
            //{
            //    bi.Value.updateItemHandler(bi.Key);
            //}
        }

        public void updateItemHandler(string K)
        {
            this.itemHandler = new ItemHandler(K);
        }

        public static void pickupItem(int i)
        {
            bossInfo[bossOrder[currentBoss]].itemHandler.activate(i);
        }

        public static void updateText()
        {
            bossInfo[bossOrder[currentBoss]].itemHandler.updateText();
        }

        public static string itemName(int i)
        {
            return bossInfo[bossOrder[currentBoss]].itemHandler.getName(i);
        }

        public static bool SpawnAll()
        {
            return bossInfo[bossOrder[currentBoss]].itemHandler.SpawnAll();
        }

        public static void toNextBoss(bool b)
        {
            string scene = bossInfo[bossOrder[currentBoss]].SceneName;
            Vector2 pos = bossInfo[bossOrder[currentBoss]].ScenePos;
            if (b)
            {
                ItemData[] itemData = bossInfo[bossOrder[currentBoss]].itemHandler.getUnusedItems();
                foreach (ItemData item in itemData)
                {
                    ItemInfo.unusedItems.Add(new ItemInfo(item.iternName, item.lanSheet, item.varName, item.stepAmount, item.lanKeys));
                }
                BossInfo.currentBoss++;
                assignItems();
            }
            BossRush.Teleport(scene, pos);
        }

        public static string getBossScene()
        {
            return bossInfo[bossOrder[currentBoss]].SceneName;
        }

        public static bool isBossDead()
        {
            if (BossInfo.bossInfo[bossOrder[currentBoss]].VarName == "Battle Scene")
            {
                if (battleScene == null)
                {
                    battleScene = GameObject.Find("Battle Scene");
                    bsComponents = battleScene.GetComponents<PlayMakerFSM>();
                }
                if (bsComponents != null)
                {
                    foreach (PlayMakerFSM fsm in bsComponents)
                    {
                        if (fsm.FsmVariables.GetFsmBool("Activated") != null)
                        {
                            return fsm.FsmVariables.GetFsmBool("Activated").Value;
                        }
                    }
                }
            }
            return PlayerData.instance.GetBoolInternal(BossInfo.bossInfo[bossOrder[currentBoss]].VarName); 
        }

         public static void createBossInfo()
         {
            //bossInfo.Add("SoulMaster", new BossInfo("Soul Master", "mageLordDefeated", "Ruins1_24", new Vector2(33.49188f, 29.40562f), new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            bossInfo = new Dictionary<string, BossInfo>();
            bossInfo.Add("FalseKnight",    
                new BossInfo("FalseKnight",    "falseKnightDefeated", 
                    "Crossroads_10", new Vector2(13.81485f, 27.40562f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)) );
            bossInfo.Add("GruzMother",
                new BossInfo("GruzMother",      "killedBigFly", 
                    "Crossroads_04", new Vector2(95.01903f, 15.40562f), 
                    new Vector2(90, 16), new Vector2(95, 16), new Vector2(100, 16)) );
            bossInfo.Add("Hornet",         
                new BossInfo("Hornet",          "hornet1Defeated", 
                    "Fungus1_04", new Vector2(26.61754f, 28.40562f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)) );
            bossInfo.Add("MantisLords",     
                new BossInfo("MantisLords",     "defeatedMantisLords", 
                    "Fungus2_15", new Vector2(30.3f, 7.405624f), 
                    new Vector2(25, 8), new Vector2(30, 8), new Vector2(35, 8)));
            bossInfo.Add("SoulMaster", 
                new BossInfo("Soul Master",     "mageLordDefeated", 
                    "Ruins1_24", new Vector2(33.49188f, 29.40562f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            bossInfo.Add("CG1",             
                new BossInfo("CG1",             "killedMegaBeamMiner", 
                    "Mines_18", new Vector2(30.07801f, 11.40562f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            bossInfo.Add("BroodingMawlek",  
                new BossInfo("BroodingMawlek",  "killedMawlek", 
                    "Crossroads_09", new Vector2(53.35388f, 4.405625f), 
                    new Vector2(57, 6), new Vector2(61.5f, 6), new Vector2(66, 6)));
            bossInfo.Add("WatcherKnights",  
                new BossInfo("WatcherKnights",  "killedBlackKnight", 
                    "Ruins2_03", new Vector2(44.2238f, 70.40561f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));        
            bossInfo.Add("DungDefender",    
                new BossInfo("DungDefender",    "defeatedDungDefender", 
                    "Waterways_05", new Vector2(83.09189f, 7.405624f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            bossInfo.Add("Grimm",           
                new BossInfo("Grimm",           "killedGrimm", 
                    "Grimm_Main_Tent", new Vector2(95.12892f, 6.405625f), 
                    new Vector2(85, 7), new Vector2(90, 7), new Vector2(95, 7)));
            bossInfo.Add("Nosk",            
                new BossInfo("Nosk",            "killedMimicSpider", 
                    "Deepnest_32", new Vector2(75.87415f, 4.405625f),
                    new Vector2(90.3f, 7.5f), new Vector2(96f, 7.5f), new Vector2(101.7f, 7.5f)));
            bossInfo.Add("Uumuu",           
                new BossInfo("Uumuu",           "defeatedMegaJelly", 
                    "Fungus3_archive_02", new Vector2(53.54183f, 110.4056f), 
                    new Vector2(50, 111), new Vector2(53, 111), new Vector2(56, 111)));
            bossInfo.Add("Flukemarm",       
                new BossInfo("Flukemarm",       "flukeMotherDefeated", 
                    "Waterways_12", new Vector2(27.38424f, 5.405624f),
                    new Vector2(23f, 6f), new Vector2(27f, 6f), new Vector2(31f, 6f)));
            bossInfo.Add("Collector",       
                new BossInfo("Collector",       "collectorDefeated", 
                    "Ruins2_11", new Vector2(47.50203f, 95.40561f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            bossInfo.Add("CG2",             
                new BossInfo("CG2",             "Battle Scene", 
                    "Mines_32", new Vector2(37.01696f, 11.40562f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            bossInfo.Add("TraitorLord",     
                new BossInfo("TraitorLord",     "killedTraitorLord", 
                    "Fungus3_23", new Vector2(33.74541f, 29.40562f), 
                    new Vector2(25, 31), new Vector2(30, 31), new Vector2(35, 31)));
            bossInfo.Add("Hornet2",         
                new BossInfo("Hornet2",         "hornetOutskirtsDefeated", 
                    "Deepnest_East_Hornet", new Vector2(24.43399f, 28.40562f),
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            bossInfo.Add("BrokenVessel",    
                new BossInfo("BrokenVessel",    "killedInfectedKnight", 
                    "Abyss_19",  new Vector2(24.92674f, 28.40562f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            bossInfo.Add("LostKin",         
                new BossInfo("LostKin",         "infectedKnightDreamDefeated", 
                    "Dream_03_Infected_Knight", new Vector2(44.98343f, 28.40562f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            bossInfo.Add("SoulTyrant",      
                new BossInfo("SoulTyrant",      "mageLordDreamDefeated", 
                    "Dream_02_Mage_Lord", new Vector2(44.98343f, 28.40562f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            bossInfo.Add("FailedKnight",    
                new BossInfo("FailedKnight",    "falseKnightDreamDefeated", 
                    "Dream_01_False_Knight", new Vector2(44.98343f, 28.40562f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            //bossInfo.Add("THK", 
            //  new BossInfo("", "defeatedMantisLords", "Fungus2_15", new Vector2(30.3f, 7.405624f), new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            //bossInfo.Add("WhiteDefender", 
            //  new BossInfo("", "defeatedMantisLords", "Fungus2_15", new Vector2(30.3f, 7.405624f), new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            //bossInfo.Add("GPZ", 
            //  new BossInfo("", "defeatedMantisLords", "Fungus2_15", new Vector2(30.3f, 7.405624f), new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            bossInfo.Add("NightmareKingGrimm", 
                new BossInfo("NKG",             "defeatedNightmareGrimm", 
                    "Grimm_Nightmare", new Vector2(92.09892f, 6.405625f), 
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));
            bossInfo.Add("Radiance", 
                new BossInfo("Radiance",        "",
                    "Dream_Final_Boss", new Vector2(60.4304f, 33.16534f),
                    new Vector2(-999, -999), new Vector2(-999, -999), new Vector2(-999, -999)));

            bossOrder = new string[]{
                "FalseKnight","GruzMother","Hornet","MantisLords","CG1","BroodingMawlek","WatcherKnights","DungDefender","Grimm","Nosk",
                "Uumuu","Flukemarm","Collector","CG2","TraitorLord","Hornet2","BrokenVessel","LostKin","SoulTyrant","FailedKnight",
                "NightmareKingGrimm","Radiance"
             };
        }
    }
}
