using System;
using Language;
using TMPro;
using UnityEngine;

namespace BossRush
{
    public class SetInspectText : MonoBehaviour
    {
        private void Awake()
        {
            this.textMesh = base.GetComponent<TextMeshPro>();
        }

        private void Start()
        {
            this.UpdateText();
        }

        public void UpdateText()
        {
            this.textMesh.text = BossInfo.itemName(id);
        }

        public void Update()
        {
            UpdateText();
        }

        private TextMeshPro textMesh;
        public int id;
    }

}
