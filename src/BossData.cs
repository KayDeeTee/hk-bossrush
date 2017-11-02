using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BossRush
{
    public class BossData
    {
        public BossData(string n, string scene, Vector2 pos, string[] items, ItemPos p, string var)
        {
            name = n;
            sceneName = scene;
            scenePos = pos;
            drops = items;
            killedVar = var;
            itemPositions = p;
        }

        //Debug information
        public string name;
        //Scene Data
        public string sceneName; public Vector2 scenePos;
        //Item Data
        public string[] drops;public ItemPos itemPositions;
        //PlayerData variable to check when to progress
        public string killedVar;        
    }
}
