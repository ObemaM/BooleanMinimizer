using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooleanMinimizerLibrary
{
    public class KarnaughStep
    {
        public string Description { get; set; }
        public List<List<string>> Map { get; set; }

        public KarnaughStep(string desc, List<List<string>> map)
        {
            Description = desc;
            Map = map;
        }
    }
}
