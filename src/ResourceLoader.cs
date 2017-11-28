using BossRush.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BossRush
{
    //This class is required due to ambiguity between UnityEngine.Resources and Project.Properties.Resources, which can not be resolved by being more precise, as Project.Properties isn't a real member
    public static class ResourceLoader
    {

        public static byte[] menuBorder()
        {
            return Resources.Menu_Border_Black;
        }
        public static byte[] menuVignette()
        {
            return Resources.Menu_Vignette;
        }

        public static byte[] bluebg()
        {
            return Resources.bluebg;
        }
        public static byte[] changePane()
        {
            return Resources.changePane;
        }
        public static byte[] invTopCorner()
        {
            return Resources.intTopCorner;
        }
        public static byte[] invBot()
        {
            return Resources.invBot;
        }
        public static byte[] invBotCorner()
        {
            return Resources.invBotCorner;
        }
        public static byte[] invTop()
        {
            return Resources.invTop;
        }
        public static byte[] placeholder()
        {
            return Resources.placeholder;
        }
        public static byte[] selectCorner()
        {
            return Resources.selectCorner;
        }

        public static byte[] loadSelect()
        {
            return Resources.select;
        }
        public static byte[] loadBossFaces1()
        {
            return Resources.boss_face_1;
        }
        public static byte[] loadBossFaces2()
        {
            return Resources.boss_face_2;
        }
        public static byte[] loadBossFaces3()
        {
            return Resources.boss_face_3;
        }
        public static byte[] loadBossSelect()
        {
            return Resources.boss_select;
        }
        public static byte[] loadBackground()
        {
            return Resources.background;
        }
        public static byte[] loadTimer()
        {
            return Resources.stopwatch;
        }
    }
}
