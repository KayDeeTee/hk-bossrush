using System;
using UnityEngine;
using FsmUtils;

namespace BossRush
{
    public class ItemData : MonoBehaviour
    {
        private string iternName; //Debug Use
        private string externName; //Name to Display
        private string lanSheet;
        private string[] lanKeys;
        private string varName;
        private int stepAmount;
        private Vector2 position;
        private int id;

        public ItemData(string i_n, string l_s, string v_n, int s_a, Vector2 pos, int _id, params string[] l_k)
        {
            iternName = i_n;
            lanSheet = l_s;
            lanKeys = l_k;
            varName = v_n;
            stepAmount = s_a;
            position = pos;
            id = _id;
            externName = getName();
        }

        public string getExternName()
        {
            return externName;
        }

        private string getName()
        {
            if (!ItemInfo.upgrades.ContainsKey(varName))
                ItemInfo.upgrades[varName] = 0;
            if (lanSheet != "RAW")
                return Language.Language.Get(lanKeys[0], lanSheet);
            return lanKeys[ItemInfo.upgrades[iternName]%lanKeys.Length];
        }

        public void activate()
        {
            if (!ItemInfo.upgrades.ContainsKey(varName))
                ItemInfo.upgrades[varName] = 0;
            ItemInfo.upgrades[varName]++;
            BossRush.pickups++;
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
                    varName = ItemInfo.upgrades[iternName] == 1 ? "hasDash" : "hasShadowDash";
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
            GameObject shiny = Instantiate(BossRush.shiny);
            if (position.x < -900)
                shiny.transform.position = new Vector2((BossRush.hc.transform.position.x + (id == 1 ? -2.5f : 0) + (id == 2 ? 2.5f : 0)), BossRush.hc.transform.position.y);
            else
                shiny.transform.position = position;
            PlayMakerFSM shinyFSM = FSMUtility.LocateFSM(shiny, "Shiny Control");
            shinyFSM.FsmVariables.GetFsmInt("Trinket Num").Value = id+1;
            FsmUtil.ChangeTransition(shinyFSM, "PD Bool?", "COLLECTED", "Fling?");
        }

    }
}
