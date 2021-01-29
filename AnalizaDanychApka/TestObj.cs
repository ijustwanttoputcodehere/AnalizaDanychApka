using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizaDanychApka
{
    public class TestObj
    {
        public string label;
        public List<double> features;

        public TestObj(string label, List<double> features) 
        {

            this.label = label;
            this.features = features;
        
        }

       
    }
}
