using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace BossRush
{
    class ItemHandler
    {
        private ItemData[] itemData;

        public ItemHandler(string bossName){
            itemData = new ItemData[3];
            BossInfo bossInfo = BossInfo.bossInfo[bossName];
            for (int i = 0; i < 3; i++)
            {
                ItemInfo item = ItemInfo.getRandomItem();
                itemData[i] = new ItemData(item.internalName, item.sheetName, item.varName, item.stepAmount, bossInfo.getItem(i), i, item.keyNames);
            }
        }

        public ItemData[] getUnusedItems()
        {
            List<ItemData> ret = new List<ItemData>();
            for (int j = 0; j < 3; j++)
                if (!itemData[j].activated)
                    ret.Add(itemData[j]);
            return ret.ToArray();
        }

        public string getName(int i)
        {
            return itemData[i].getExternName();
        }

        public void activate(int i)
        {
            itemData[i].activate();
        }

        public void DestroyAll()
        {
            itemData[0].destroy();
            itemData[1].destroy();
            itemData[2].destroy();
        }

        public bool SpawnAll()
        {
            if (BossRush.shinySlot3 != null)
            {
                itemData[0].Spawn();
                itemData[1].Spawn();
                itemData[2].Spawn();
                //updateText();
                return true;
            }
            return false;
        }


    }
}
