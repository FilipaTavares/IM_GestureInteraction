using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureModality
{
    class CustomGesture
    {
        private string gestureName;
        private List<KeyValuePair<string, float>> discreteGestures;

        public string GestureName
        {
            get
            {
                return gestureName;
            }
        }

        public List<KeyValuePair<string, float>> DiscreteGestures
        {
            get
            {
                return discreteGestures;
            }
        }

        public CustomGesture(string gestureName)
        {
            this.gestureName = gestureName;
            this.discreteGestures = new List<KeyValuePair<string, float>>();
        }

        public void addDiscreteGesture(string name, float confidence) {
            DiscreteGestures.Add(new KeyValuePair<string, float>(name, confidence));
        }


    }
}
