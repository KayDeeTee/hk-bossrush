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
        public static bool spawnedItems = false;
        Rect itemRect = new Rect(new Vector2(5, (int)Screen.height / 2), new Vector2(500, 500));

        //public void OnGUI()
        //{
        //    if (BossRush.gm.gameState == GameState.PLAYING && spawnedItems)
        //        GUI.Label(itemRect, String.Format("L : {0}\nC : {1}\nR : {2}", BossInfo.itemName(1), BossInfo.itemName(0), BossInfo.itemName(2)));
        //}

        public void Update()
        {
            if( BossRush.gm.inputHandler.inputActions.paneLeft.WasPressed)
            if (BossRush.selectingStage)
            {
                HeroController.instance.RelinquishControl();
                HeroController.instance.StopAnimationControl();
                HeroController.instance.cState.invulnerable = true;
                if (BossRush.gm.inputHandler.inputActions.up.WasPressed)
                {
                    BossRush.selectY -= 1;
                    BossRush.selectY = BossRush.selectY < 0 ? 2 : BossRush.selectY;
                }
                if (BossRush.gm.inputHandler.inputActions.down.WasPressed)
                {
                    BossRush.selectY += 1;
                    BossRush.selectY = BossRush.selectY > 2 ? 0 : BossRush.selectY;
                }
                if (BossRush.gm.inputHandler.inputActions.left.WasPressed)
                {
                    BossRush.selectX -= 1;
                    BossRush.selectX = BossRush.selectX < 0 ? 2 : BossRush.selectX;
                }
                if (BossRush.gm.inputHandler.inputActions.right.WasPressed)
                {
                    BossRush.selectX += 1;
                    BossRush.selectX = BossRush.selectX > 2 ? 0 : BossRush.selectX;
                }
                if (BossRush.gm.inputHandler.inputActions.menuSubmit.WasPressed)
                {
                    
                    int bossId = BossRush.selectX + (BossRush.selectY*3);
                    int j = bossId + ((int)Mathf.Floor(BossInfo.defeatedBosses / 9) * 9);
                    bool skip = false;
                    if (BossInfo.defeatedBosses >= 18 && BossInfo.defeatedBosses < 25 && (BossRush.selectX == 2 && BossRush.selectY == 2))
                        skip = true;
                    if (BossInfo.defeatedBosses >= 18 && (BossRush.selectX == 1 && BossRush.selectY == 1))
                        skip = true;
                    if (!skip && !BossInfo.bossState[j])
                    {
                        spawnedItems = false;
                        BossRush.pickups = 0;
                        BossRush.flawless = true;
                        BossInfo.toNextBoss(true, j);
                    }                        
                }
                int x = BossRush.selectX + (BossRush.selectY * 3);
                int y = x + ((int)Mathf.Floor(BossInfo.defeatedBosses / 9) * 9);
                if (BossRush.selectingStage)
                    BossRush.timeText.text = String.Format("{0}", y.ToString() );

                BossRush.selectPos.anchoredPosition = new Vector2(960 + ((BossRush.selectX - 1) * 233), 558 - ((BossRush.selectY - 1) * 232));
            }


            if (BossRush.gm.gameState == GameState.PLAYING)
            {
                if (!BossInfo.isBossDead() && BossRush.gm.sceneName == BossInfo.getBossScene())
                    BossRush.currentTime += Time.deltaTime;

                int h = (int)BossRush.currentTime / 3600;
                int m = ((int)BossRush.currentTime % 3600) / 60;
                int s = ((int)BossRush.currentTime % 3600 ) % 60;

                if(!BossRush.selectingStage)
                    BossRush.timeText.text = String.Format("{0}:{1}:{2}", h.ToString("00"), m.ToString("00"), s.ToString("00"));


                if (!BossInfo.isBossDead() && BossRush.gm.sceneName != BossInfo.getBossScene() )
                {
                    BossInfo.toNextBoss(false);
                }
                if (BossInfo.isBossDead() && PlayerData.instance.health > 0 && BossRush.gm.sceneName != "Tutorial_01" && BossRush.gm.sceneName != "Dream_04_White_Defender" && BossRush.gm.sceneName != "Dream_Mighty_Zote" && BossRush.gm.sceneName != "Dream_01_False_Knight" && BossRush.gm.sceneName != "Dream_02_Mage_Lord" && BossRush.gm.sceneName != "Dream_03_Infected_Knight" && BossRush.gm.sceneName != "Grimm_Nightmare")
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
                            if (!BossRush.selectingStage)
                            {
                                BossInfo.bossState[BossInfo.currentBoss] = true;
                                BossInfo.defeatedBosses++;
                                //spawnedItems = false;
                                HeroController.instance.RelinquishControl();
                                HeroController.instance.StopAnimationControl();
                                HeroController.instance.cState.invulnerable = true;
                                BossRush.selectY = 1;
                                BossRush.selectX = 1;

                                BossRush.bossSelectImg.enabled = true;
                                BossRush.selectPos.GetComponent<UnityEngine.UI.Image>().enabled = true;
                                for (int i = 0; i < BossRush.bossFaces.Length; i++)
                                {
                                    int j = i+((int)Mathf.Floor(BossInfo.defeatedBosses/9)*9);
                                    int k = j;
                                    if (i < 3)
                                        k = j + 6;
                                    if (i > 5)
                                        k = j - 6;
                                    if (BossInfo.bossState[k])
                                        BossRush.bossText[i].text = "RIP";
                                    else
                                        BossRush.bossText[i].text = Language.Language.Get(BossInfo.bossName[j] + "_SUPER", "Titles") + " " + Language.Language.Get(BossInfo.bossName[j] + "_MAIN", "Titles") + " " + Language.Language.Get(BossInfo.bossName[j] + "_SUB", "Titles");
                                }
                                for (int i = 0; i < BossRush.bossFaces.Length; i++)
                                {
                                    BossRush.bossFaces[i].enabled = true;
                                    BossRush.bossText[i].enabled = true;
                                }
                                BossRush.selectingStage = true;
                                PlayerData.instance.disablePause = true;
                            }
                            //BossInfo.toNextBoss(true);
                        }
                    }
                }
                    
            }
        }
    }
}
