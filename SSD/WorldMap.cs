using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSD
{
    public class WorldMap
    {
        public WorldMap()
        {
            XSize = 4000;
           // YSize = 20;
            ZSize = 4000;
        }

        public float XSize { get; set; }
        //public float YSize { get; set; }
        public float ZSize { get; set; }
    }
}
