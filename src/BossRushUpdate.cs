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

        //public void OnGUI()
        //{
        //    if (BossRush.gm.gameState == GameState.PLAYING && spawnedItems)
        //        GUI.Label(itemRect, String.Format("L : {0}\nC : {1}\nR : {2}", BossInfo.itemName(1), BossInfo.itemName(0), BossInfo.itemName(2)));
        //}

        public void Update()
        {
            if (BossRush.gm.gameState == GameState.PLAYING)
            {   
                if (!BossInfo.isBossDead() && BossRush.gm.sceneName != BossInfo.getBossScene() )
                {
                    BossInfo.toNextBoss(false);
                }
                if (BossInfo.isBossDead() && PlayerData.instance.health > 0 && BossRush.gm.sceneName != "Tutorial_01" && BossRush.gm.sceneName != "Dream_01_False_Knight" && BossRush.gm.sceneName != "Dream_02_Mage_Lord" && BossRush.gm.sceneName != "Dream_03_Infected_Knight" && BossRush.gm.sceneName != "Grimm_Nightmare")
                {
                    if (!spawnedItems)
                    {
                        if(BossInfo.SpawnAll())
                            spawnedItems = true;
                    }
                    else
                    {
                        if (PlayerData.instance.trinket1 > 0)
                            BossInfo.pickupItem(0);
                        if (PlayerData.instance.trinket2 > 0)
                            BossInfo.pickupItem(1);
                        if (PlayerData.instance.trinket3 > 0)
                            BossInfo.pickupItem(2);

                        if (BossRush.pickups > (BossRush.flawless ? 1 : 0))
                        {
                            spawnedItems = false;
                            BossInfo.toNextBoss(true);
                        }
                    }
                }
                    
            }
        }
    }
}
