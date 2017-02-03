using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GLOR_Demo.DataClasses
{
    public static class GlobalVariable
    {
        //Routing mode: Manual/Automatic
        public static bool IsManual { get; set; }
        public static bool IsBeelineEnabled { get; set; }
        public static List<Image> imageList { get; set; }

        //KeyValue pair for static string
        public static string KVMessage = "MSG";
        public static string KVAcknowledgment = "ACK";

        //Function to get random number
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
    }
}
