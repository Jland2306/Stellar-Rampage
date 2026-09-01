using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.Miscellaneous
{
    public class Subtitle
    {
        public string text;
        public float timer;
        public float duration;

        public Subtitle(string text, float timer, float duration)
        {
            this.text = text;
            this.timer = timer;
            this.duration = duration;
        }
    }
}
