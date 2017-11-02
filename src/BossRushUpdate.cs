using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GlobalEnums;
using FsmUtils;

namespace BossRush
{
    public class BossRushUpdate : MonoBehaviour
    {
        private bool spawnedItems = false;
        Rect itemRect = new Rect(new Vector2(5, (int)Screen.height / 2), new Vector2(500, 500));

        public string getItemName(int x)
        {
            return BossRush.boolToString(BossRush.bossData[BossRush.currentBoss].drops[x]);
        }

        public void OnGUI()
        {
            if (BossRush.gm.gameState == GameState.PLAYING && spawnedItems)
                GUI.Label(itemRect, String.Format("L : {0}\nC : {1}\nR : {2}", getItemName(1), getItemName(0), getItemName(2)) );
        }

        public void Update()
        {
            if (BossRush.gm.gameState == GameState.PLAYING)
            {   
                if (BossRush.gm.sceneName == "Tutorial_01")
                {
                    BossRush.Teleport(BossRush.getScene(), BossRush.getPos());
                }
                if (!BossRush.bossKilled() && BossRush.gm.sceneName != BossRush.getScene())
                {
                    BossRush.Teleport(BossRush.getScene(), BossRush.getPos());
                }
                if (BossRush.bossKilled() && BossRush.gm.sceneName != "Dream_01_False_Knight" && BossRush.gm.sceneName != "Dream_02_Mage_Lord" && BossRush.gm.sceneName != "Dream_03_Infected_Knight" && BossRush.gm.sceneName != "Grimm_Nightmare")
                {
                    if (!spawnedItems)
                    {
                        Vector2 pos;
                        GameObject s1 = Instantiate(BossRush.shiny);
                        pos = BossRush.getItemPos(2);
                        if (pos.x < -900)
                            s1.transform.position = new Vector2(BossRush.hc.transform.position.x, BossRush.hc.transform.position.y);
                        else
                            s1.transform.position = pos;
                        PlayMakerFSM shiny1FSM = FSMUtility.LocateFSM(s1, "Shiny Control");
                        shiny1FSM.FsmVariables.GetFsmString("PD Bool Name").Value = "gotCharm_1";
                        shiny1FSM.FsmVariables.GetFsmBool("Charm").Value = true;
                        shiny1FSM.FsmVariables.GetFsmInt("Charm ID").Value = 1;

                        FsmUtil.ChangeTransition(shiny1FSM, "PD Bool?", "COLLECTED", "Fling?");

                        GameObject s2 = Instantiate(BossRush.shiny);
                        pos = BossRush.getItemPos(1);
                        if (pos.x < -900)
                            s2.transform.position = new Vector2(BossRush.hc.transform.position.x-2.5f, BossRush.hc.transform.position.y);
                        else
                            s2.transform.position = pos;
                        PlayMakerFSM shiny2FSM = FSMUtility.LocateFSM(s2, "Shiny Control");
                        shiny2FSM.FsmVariables.GetFsmString("PD Bool Name").Value = "gotCharm_2";
                        shiny2FSM.FsmVariables.GetFsmBool("Charm").Value = true;
                        shiny2FSM.FsmVariables.GetFsmInt("Charm ID").Value = 2;

                        FsmUtil.ChangeTransition(shiny2FSM, "PD Bool?", "COLLECTED", "Fling?");

                        GameObject s3 = Instantiate(BossRush.shiny);
                        pos = BossRush.getItemPos(3);
                        if (pos.x < -900)
                            s3.transform.position = new Vector2(BossRush.hc.transform.position.x + 2.5f, BossRush.hc.transform.position.y);
                        else
                            s3.transform.position = pos;
                        PlayMakerFSM shiny3FSM = FSMUtility.LocateFSM(s3, "Shiny Control");
                        shiny3FSM.FsmVariables.GetFsmString("PD Bool Name").Value = "gotCharm_3";
                        shiny3FSM.FsmVariables.GetFsmBool("Charm").Value = true;
                        shiny3FSM.FsmVariables.GetFsmInt("Charm ID").Value = 3;

                        FsmUtil.ChangeTransition(shiny3FSM, "PD Bool?", "COLLECTED", "Fling?");

                        spawnedItems = true;

                    }
                    else
                    {
                        if (PlayerData.instance.gotCharm_1)
                            BossRush.onPickup("gotCharm_1", true);
                        if (PlayerData.instance.gotCharm_2)
                            BossRush.onPickup("gotCharm_2", true);
                        if (PlayerData.instance.gotCharm_3)
                            BossRush.onPickup("gotCharm_3", true);

                        if (BossRush.getCurrentItemValue(0) || BossRush.getCurrentItemValue(1) || BossRush.getCurrentItemValue(2))
                        {
                            BossRush.currentBoss++;
                            spawnedItems = false;
                            BossRush.Teleport(BossRush.getScene(), BossRush.getPos());
                        }
                    }
                }
                    
            }
        }
    }
}
