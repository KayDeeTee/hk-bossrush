using UnityEngine;

namespace BossRush
{
    public class ItemPos
    {
        public Vector2 item1; // LEFT
        public Vector2 item2; // CENTER
        public Vector2 item3; // RIGHT

        public ItemPos()
        {
            //-999 means use hero positions
            item1 = new Vector2(-999, -999); 
            item2 = new Vector2(-999, -999);
            item3 = new Vector2(-999, -999);
        }

        public ItemPos(float x1, float x2, float y)
        {
            item1 = new Vector2(x1, y);
            item2 = new Vector2((x1+x2)/2.0f, y);
            item3 = new Vector2(x2, y);
        }

        public ItemPos(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            item1 = new Vector2(x1, y1);
            item2 = new Vector2(x2, y2);
            item3 = new Vector2(x3, y3);
        }

        public ItemPos(Vector2 one, Vector2 two, Vector2 three)
        {
            item1 = one;
            item2 = two;
            item3 = three;
        }
    }
}
