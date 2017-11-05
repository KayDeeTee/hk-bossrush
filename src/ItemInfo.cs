using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BossRush
{
    public class ItemInfo
    {
        public static List<ItemInfo> itemInfo;
        public static List<ItemInfo> unusedItems;

        public string internalName,sheetName,varName;
        public string[] keyNames;
        public int stepAmount;
        public static Dictionary<string, int> upgrades;

        public ItemInfo(string iN, string sN, string vN, int step, params string[] kN)
        {
            internalName = iN;
            sheetName = sN;
            keyNames = kN;
            varName = vN;
            stepAmount = step;
        }

        public static ItemInfo getRandomItem()
        {
            int r = UnityEngine.Random.RandomRange(0, ItemInfo.itemInfo.Count());
            ItemInfo ret = ItemInfo.itemInfo.ElementAt(r);
            ItemInfo.itemInfo.RemoveAt(r);
            return ret;
        }

        public static void createItemInfo()
        {
            upgrades = new Dictionary<string, int>();

            unusedItems = new List<ItemInfo>();
            itemInfo = new List<ItemInfo>();
            //ItemInfo(string iN, string sN, string kN, string vN, int step)
            for (int i = 1; i < 40; i++)
            {
                if (i != 36)
                    itemInfo.Add( new ItemInfo("CHARM_"+i.ToString(), "UI", "equippedCharm_"+i, 0, "CHARM_NAME_"+i ) );
            }

            itemInfo.Add(new ItemInfo("MothwingCloak", "UI", "hasDash", 0, "INV_NAME_DASH", "INV_NAME_SHADOWDASH")); itemInfo.Add(new ItemInfo("MothwingCloak", "UI", "hasDash", 0, "INV_NAME_DASH", "INV_NAME_SHADOWDASH"));
            itemInfo.Add(new ItemInfo("MonarchWings", "UI", "hasDoubleJump", 0, "INV_NAME_DOUBLEJUMP")); itemInfo.Add(new ItemInfo("MantisClaw", "UI", "hasWallJump", 0, "INV_NAME_WALLJUMP"));
            itemInfo.Add(new ItemInfo("Fireball", "UI", "fireballLevel", 1, "INV_NAME_SPELL_FIREBALL1", "INV_NAME_SPELL_FIREBALL2")); itemInfo.Add(new ItemInfo("Fireball", "UI", "fireballLevel", 1, "INV_NAME_SPELL_FIREBALL1", "INV_NAME_SPELL_FIREBALL2"));
            itemInfo.Add(new ItemInfo("Dive", "UI", "quakeLevel", 1, "INV_NAME_SPELL_QUAKE1", "INV_NAME_SPELL_QUAKE2")); itemInfo.Add(new ItemInfo("Dive", "UI", "quakeLevel", 1, "INV_NAME_SPELL_QUAKE1", "INV_NAME_SPELL_QUAKE2"));
            itemInfo.Add(new ItemInfo("Scream", "UI", "screamLevel", 1, "INV_NAME_SPELL_SCREAM1", "INV_NAME_SPELL_SCREAM2")); itemInfo.Add(new ItemInfo("Scream", "UI", "screamLevel", 1, "INV_NAME_SPELL_SCREAM1", "INV_NAME_SPELL_SCREAM2"));
            itemInfo.Add(new ItemInfo("nailLevel", "UI", "nailDamage", 4, "INV_NAME_NAIL2", "INV_NAME_NAIL3", "INV_NAME_NAIL4", "INV_NAME_NAIL5"));
            itemInfo.Add(new ItemInfo("nailLevel", "UI", "nailDamage", 4, "INV_NAME_NAIL2", "INV_NAME_NAIL3", "INV_NAME_NAIL4", "INV_NAME_NAIL5"));
            itemInfo.Add(new ItemInfo("nailLevel", "UI", "nailDamage", 4, "INV_NAME_NAIL2", "INV_NAME_NAIL3", "INV_NAME_NAIL4", "INV_NAME_NAIL5"));
            itemInfo.Add(new ItemInfo("nailLevel", "UI", "nailDamage", 4, "INV_NAME_NAIL2", "INV_NAME_NAIL3", "INV_NAME_NAIL4", "INV_NAME_NAIL5"));
            itemInfo.Add(new ItemInfo("hpLevel", "RAW", "maxHealth", 1, "HP")); itemInfo.Add(new ItemInfo("hpLevel", "RAW", "maxHealth", 1, "HP"));
            itemInfo.Add(new ItemInfo("hpLevel", "RAW", "maxHealth", 1, "HP")); itemInfo.Add(new ItemInfo("hpLevel", "RAW", "maxHealth", 1, "HP"));
            itemInfo.Add(new ItemInfo("mpLevel", "RAW", "maxMp", 33, "MP"));
            itemInfo.Add(new ItemInfo("mpLevel", "RAW", "maxMp", 33, "MP"));
            itemInfo.Add(new ItemInfo("mpLevel", "RAW", "maxMp", 33, "MP"));
            itemInfo.Add(new ItemInfo("dashSlash", "UI", "hasUpwardSlash", 0, "INV_NAME_ART_UPPER"));
            itemInfo.Add(new ItemInfo("cyclone", "UI", "hasCyclone", 0, "INV_NAME_ART_CYCLONE"));
            itemInfo.Add(new ItemInfo("greatSlash", "UI", "hasDashSlash", 0, "INV_NAME_ART_DASH"));
            itemInfo.Add(new ItemInfo("grimmChild", "UI", "grimmChildLevel", 1, "CHARM_NAME_40"));
            itemInfo.Add(new ItemInfo("grimmChild", "UI", "grimmChildLevel", 1, "CHARM_NAME_40"));
            itemInfo.Add(new ItemInfo("grimmChild", "UI", "grimmChildLevel", 1, "CHARM_NAME_40"));
            itemInfo.Add(new ItemInfo("grimmChild", "UI", "grimmChildLevel", 1, "CHARM_NAME_40"));
        }

    }
}
