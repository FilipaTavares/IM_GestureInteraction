using Microsoft.Kinect.VisualGestureBuilder;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;
using Newtonsoft.Json.Linq;

namespace GestureModality
{
    class GestureFrameHandler
    {

        private List<CustomGesture> loadGestures; //jsonLoad
        //state
        private HashSet<KeyValuePair<int, CustomGesture>> state;
        private bool firstFrame;
        private int frames;


        public GestureFrameHandler() {
            state = new HashSet<KeyValuePair<int, CustomGesture>>();
            firstFrame = true;
            frames = 0;
        }

        public void load(string jsonName) {
            using (StreamReader r = new StreamReader(jsonName))
            {
                var jsonString = r.ReadToEnd();
                JObject json = (JObject)JsonConvert.DeserializeObject(jsonString);

                loadGestures = new List<CustomGesture>();

                foreach (KeyValuePair<string, JToken> kp in json) {
                    CustomGesture toAdd = new CustomGesture(kp.Key);

                    foreach (JToken gesture in (JArray)json[kp.Key])
                    {
                        foreach (KeyValuePair<string, JToken> pair in ((JObject)gesture))
                        {
                            toAdd.addDiscreteGesture(pair.Key, (float)pair.Value);
                        }
                    }

                    loadGestures.Add(toAdd);
                }
            }
        }
       
        public string handleFrame(IReadOnlyDictionary<Gesture, DiscreteGestureResult> discriteGestures) {
            /*
            if (state.Count > 0)
            {
                Console.WriteLine("State");
                foreach (var kvp in state)
                {
                    Console.WriteLine("\t" + kvp.Key + " " + kvp.Value.GestureName);
                }
            }*/

            if (!firstFrame && ++frames > 60) {
                Console.WriteLine("State reseted affter " + frames);
                resetState();
                
            }

            //Console.WriteLine("");
            foreach (KeyValuePair<Gesture, DiscreteGestureResult> kvp in discriteGestures)
            {
                foreach (var customGesture in loadGestures)
                {
                    for (int i = 0; i < customGesture.DiscreteGestures.Count; i++)
                    {
                        if (customGesture.DiscreteGestures[i].Key.Equals(kvp.Key.Name) && kvp.Value.Confidence > customGesture.DiscreteGestures[i].Value) {
                            if (matchComplete(customGesture, i)) {
                                return customGesture.GestureName;
                            }
                        }
                    }

                }
            }

            //se adicionei algum gesto para ser detetado
            if (firstFrame && state.Count!= 0 && ++frames > 15)
            {//initial 10 sec window for more frames

                Console.WriteLine("Initial Window finish with state "+ state.Count);
                firstFrame = false;
                frames = 0;
                
            } 

            return null;
        }

        private bool matchComplete(CustomGesture gestureToMatch, int index) {
            //Console.WriteLine("MATCH!! " + gestureToMatch.GestureName + " index " + index);
            if (firstFrame && index == 0) //tambem é primeiro frame 
            {
                Console.WriteLine("fisrtMatch " + index + " " + gestureToMatch.GestureName);
                state.Add(new KeyValuePair<int, CustomGesture>(index, gestureToMatch));
            }else
            {// deteção vai depender do estado anterior
                var newState = new HashSet<KeyValuePair<int, CustomGesture>>();
                foreach (var gest in state) {//loop pelos possiveis gestos
                    if (gest.Value == gestureToMatch && index == gest.Key+1) //this guesture is the next
                    {
                        if (index == gestureToMatch.DiscreteGestures.Count-1)//last gesture so FIND MATCH!!!
                        {
                            Console.WriteLine("Complete match " + index + " " + gestureToMatch.GestureName);
                            resetState();
                            return true;
                        }
                        else
                        {
                            Console.WriteLine("Continue match " + index + " " + gestureToMatch.GestureName);
                            newState.Add(new KeyValuePair<int, CustomGesture>(index, gestureToMatch));
                        }
                    }
                }
                if (newState.Count>0) //transição de estado se a continuação do gesto for valida
                    state = newState;
            }

            return false;
        }

        private void resetState()
        {
            //reset state
            state.Clear();
            firstFrame = true;
            frames = 0;
        }

    }
}
