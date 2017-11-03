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
        public static byte[] loadBackground()
        {
            return Resources.background;
        }
    }
}
