using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace BossRush
{
    class ItemTextFader : MonoBehaviour
    {
        public TextMeshPro tmp;
        public PlayMakerFSM fsm;
        public float opacity = 0;
        public int id = 0;


        public void Start()
        {
            fsm = base.gameObject.GetComponent<PlayMakerFSM>();
            opacity = 0;
        }

        public void Update()
        {
            if (fsm.ActiveStateName == "In Range" || fsm.ActiveStateName == "Can Inspect?" || fsm.ActiveStateName == "Cancel Frame")
            {
                opacity += Time.deltaTime*5;
            }
            else
            {
                opacity -= Time.deltaTime*5;
            }
            opacity = Mathf.Clamp01(opacity);
            byte trans = ((byte)(opacity * 255));
            string hexAlpha = BitConverter.ToString( new byte[]{ trans } );
            tmp.text = String.Format("<alpha=#{0}>{1}", hexAlpha, BossInfo.itemName(id));
            tmp.faceColor = new Color32(255, 255, 255, 255);     
        }
    }
}
