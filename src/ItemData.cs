using System;
using UnityEngine;
using FsmUtils;
using System.Collections.Generic;

namespace BossRush
{
    public class ItemData : MonoBehaviour
    {
        public string iternName; //Debug Use
        public string lanSheet;
        public string[] lanKeys;
        public string varName;
        public int stepAmount;
        public Vector2 position;
        public int id;
        public bool activated;
        GameObject shiny;

        public ItemData(string i_n, string l_s, string v_n, int s_a, Vector2 pos, int _id, params string[] l_k)
        {
            iternName = i_n;
            lanSheet = l_s;
            lanKeys = l_k;
            varName = v_n;
            stepAmount = s_a;
            position = pos;
            id = _id;
        }

        public void destroy()
        {
            Destroy(shiny);
        }

        public string getExternName()
        {
            return getName();
        }

        private string getName()
        {
            if (!ItemInfo.upgrades.ContainsKey(varName))
                ItemInfo.upgrades[varName] = 0;
            if (lanSheet == "RAW")
                return lanKeys[0];
            //Modding.ModHooks.ModLog(String.Format("[BR] 0th key : {0}, upgrades : {1}, length {2}, sheet : {3}", lanKeys[0], ItemInfo.upgrades[varName], lanKeys.Length, lanSheet));
            return Language.Language.GetInternal(lanKeys[ItemInfo.upgrades[varName] % lanKeys.Length], lanSheet);
        }

        public void activate()
        {
            activated = true;
            if (!ItemInfo.upgrades.ContainsKey(varName))
                ItemInfo.upgrades[varName] = 0;
            ItemInfo.upgrades[varName]++;
            BossRush.pickups++;
            if (varName == "grimmChildLevel")
                BossRush.grimmLevel++;
            if (varName == "equippedCharm_13")
            {
                BossRush.hc.normalSlash.SetMantis(true);
                BossRush.hc.alternateSlash.SetMantis(true);
                BossRush.hc.upSlash.SetMantis(true);
                BossRush.hc.downSlash.SetMantis(true);
                BossRush.hc.wallSlash.SetMantis(true);
            }
            if (varName == "equippedCharm_18")
            {
                BossRush.hc.normalSlash.SetLongnail(true);
                BossRush.hc.alternateSlash.SetLongnail(true);
                BossRush.hc.upSlash.SetLongnail(true);
                BossRush.hc.downSlash.SetLongnail(true);
                BossRush.hc.wallSlash.SetLongnail(true);
            }
            if (varName == "maxHealth" || varName == "maxMp")
            {
                if (varName == "maxHealth")
                    PlayerData.instance.AddToMaxHealth(1);
                else
                    PlayerData.instance.AddToMaxMPReserve(33);
                BossRush.hc.proxyFSM.BroadcastMessage("MAX HP UP");
                PlayMakerFSM.BroadcastEvent("MAX HP UP");
            }
            else
            {
                if (varName == "hasDash")
                    varName = ItemInfo.upgrades[varName] == 1 ? "hasDash" : "hasShadowDash";
                if (stepAmount == 0)
                    PlayerData.instance.SetBoolInternal(varName, true);
                else
                    PlayerData.instance.SetIntInternal(varName, PlayerData.instance.GetIntInternal(varName) + stepAmount);
                if (varName.Contains("equippedCharm"))
                    PlayerData.instance.SetBoolInternal("gotCharm_" + varName.Split('_')[1], true);
            }
            PlayerData.instance.trinket1 = 0;
            PlayerData.instance.trinket2 = 0;
            PlayerData.instance.trinket3 = 0;
        }

        public void Spawn()
        {
            if (shiny == null)
            {
                if (id == 0)
                {
                    shiny = Instantiate(BossRush.shinySlot1);
                }
                if (id == 1)
                {
                    shiny = Instantiate(BossRush.shinySlot2);
                }
                if (id == 2)
                {
                    shiny = Instantiate(BossRush.shinySlot3);
                }
            }
            if (position.x < -900)
                shiny.transform.position = new Vector2((BossRush.hc.transform.position.x + (id == 1 ? -2.75f : 0) + (id == 2 ? 2.75f : 0)), BossRush.hc.transform.position.y);
            else
                shiny.transform.position = position;
            PlayMakerFSM shinyFSM = FSMUtility.LocateFSM(shiny, "Shiny Control");
            shinyFSM.FsmVariables.GetFsmInt("Trinket Num").Value = id+1;
            FsmUtil.ChangeTransition(shinyFSM, "PD Bool?", "COLLECTED", "Fling?");

            shiny.name = "ITEM_"+id;
        }

    }
}
