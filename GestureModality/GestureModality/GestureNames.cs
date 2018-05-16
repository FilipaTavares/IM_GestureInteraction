using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureModality
{
    static class GestureNames
    {
        /// <summary> Path to the gesture database that was trained with VGB </summary>
        public readonly static string gestureDatabase = @"Database\Gestures.gbd";
        
        /// <summary> Name of the discrete gesture in the database that we want to track </summary>
        public readonly static string hungryTop = "HungryTop";

        /// <summary> Name of the discrete gesture in the database that we want to track </summary>
        public readonly static string hungryBottom = "HungryBottom";

        /// <summary> Name of the discrete gesture in the database that we want to track </summary>
        public readonly static string hungryMiddle = "HungryMiddle";

        /// <summary> Name of the discrete gesture in the database that we want to track </summary>
        public readonly static string hungryProgress = "HungryProgress";

        public readonly static int hungryIndex = 0;

    }
}
