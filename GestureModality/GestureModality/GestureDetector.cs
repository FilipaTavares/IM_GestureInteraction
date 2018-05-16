using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureModality
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Microsoft.Kinect;
    using Microsoft.Kinect.VisualGestureBuilder;
    using mmisharp;

    /// <summary>
    /// Gesture Detector class which listens for VisualGestureBuilderFrame events from the service
    /// and updates the associated GestureResultView object with the latest results for the 'Seated' gesture
    /// </summary>
    public class GestureDetector : IDisposable
    {
        private LifeCycleEvents lce;

        private MmiCommunication mmic;

        /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        private MainWindow mainWindow;

        /// <summary>
        /// Initializes a new instance of the GestureDetector class along with the gesture frame source and reader
        /// </summary>
        /// <param name="kinectSensor">Active sensor to initialize the VisualGestureBuilderFrameSource object with</param>
        /// <param name="gestureResultView">GestureResultView object to store gesture results of a single body to</param>
        public GestureDetector(KinectSensor kinectSensor, GestureResultView gestureResultView, MainWindow window)
        {
            if (kinectSensor == null)
            {
                throw new ArgumentNullException("kinectSensor");
            }

            if (gestureResultView == null)
            {
                throw new ArgumentNullException("gestureResultView");
            }

            this.mainWindow = window;

            //init LifeCycleEvents..
            lce = new LifeCycleEvents("GESTURES", "FUSION", "gesture-1", "haptics", "command"); // LifeCycleEvents(string source, string target, string id, string medium, string mode)
            //mmic = new MmiCommunication("localhost",9876,"User1", "ASR");  //PORT TO FUSION - uncomment this line to work with fusion later
            mmic = new MmiCommunication("localhost", 8000, "User1", "GESTURES"); // MmiCommunication(string IMhost, int portIM, string UserOD, string thisModalityName)

            mmic.Send(lce.NewContextRequest());

            this.GestureResultView = gestureResultView;

            // create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
            this.vgbFrameSource = new VisualGestureBuilderFrameSource(kinectSensor, 0);
            this.vgbFrameSource.TrackingIdLost += this.Source_TrackingIdLost;

            // open the reader for the vgb frames
            this.vgbFrameReader = this.vgbFrameSource.OpenReader();
            if (this.vgbFrameReader != null)
            {
                this.vgbFrameReader.IsPaused = true;
                this.vgbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
            }

            // load the all gestures from the gesture database
            using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(GestureNames.gestureDatabase))
            {
                vgbFrameSource.AddGestures(database.AvailableGestures);
            }
        }

        /// <summary> Gets the GestureResultView object which stores the detector results for display in the UI </summary>
        public GestureResultView GestureResultView { get; private set; }

        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public ulong TrackingId
        {
            get
            {
                return this.vgbFrameSource.TrackingId;
            }

            set
            {
                if (this.vgbFrameSource.TrackingId != value)
                {
                    this.vgbFrameSource.TrackingId = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the detector is currently paused
        /// If the body tracking ID associated with the detector is not valid, then the detector should be paused
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return this.vgbFrameReader.IsPaused;
            }

            set
            {
                if (this.vgbFrameReader.IsPaused != value)
                {
                    this.vgbFrameReader.IsPaused = value;
                }
            }
        }

        /// <summary>
        /// Disposes all unmanaged resources for the class
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader objects
        /// </summary>
        /// <param name="disposing">True if Dispose was called directly, false if the GC handles the disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.vgbFrameReader != null)
                {
                    this.vgbFrameReader.FrameArrived -= this.Reader_GestureFrameArrived;
                    this.vgbFrameReader.Dispose();
                    this.vgbFrameReader = null;
                }

                if (this.vgbFrameSource != null)
                {
                    this.vgbFrameSource.TrackingIdLost -= this.Source_TrackingIdLost;
                    this.vgbFrameSource.Dispose();
                    this.vgbFrameSource = null;
                }
            }
        }

        /// <summary>
        /// Handles gesture detection results arriving from the sensor for the associated body tracking Id
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;
            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
            {
                if (frame != null)
                {

                    // percorrer dicionario à procura do gesto discreto com maior confiança
                    // não gosto mas como escolher o melhor gesto?

                    // get the discrete gesture results which arrived with the latest frame
                    IReadOnlyDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;

                    // get the discrete gesture results which arrived with the latest frame
                    IReadOnlyDictionary<Gesture, ContinuousGestureResult> continuousResults = frame.ContinuousGestureResults;

                    if (discreteResults != null)
                    {
                        //Console.WriteLine("PRINT DO DIC DISCRETO");
                        // PRINT DE TODOS OS GESTOS DISCRETOS DA DATABASE --> ACHAVA QUE ERA SÓ RECONHECIDOS MAS AFINAL NÃO

                        float confidence = 0.9F;
                        KeyValuePair<Gesture, DiscreteGestureResult> maxPair = new KeyValuePair<Gesture, DiscreteGestureResult>();

                        foreach (KeyValuePair<Gesture, DiscreteGestureResult> kvp in discreteResults)
                        {
                          //  Console.WriteLine(kvp.Key.Name + " ----> " +  kvp.Value.Detected + " ----> " + kvp.Value.Confidence);

                            if (kvp.Value.Detected && kvp.Value.Confidence > confidence)
                            {
                                confidence = kvp.Value.Confidence;
                                maxPair = kvp;
                            }
                        }

                        if (maxPair.Key != null)
                        {
                            this.GestureResultView.UpdateGestureResult(true, maxPair.Value.Detected, maxPair.Value.Confidence, maxPair.Key.Name);
                            this.gestureDetection(maxPair.Key.Name);
                        }

                        // ver depois mais gestos discretor da mesma gama tipo máximo é steer mas haver outros discretos steer detetados??
                        //List<KeyValuePair<Gesture, DiscreteGestureResult>> list = discreteResults.Where(pair => pair.Value.Detected && pair.Value.Confidence > 0.5).ToList();

                        /*

                        // ver se se pode eliminar este for caso não existam gestos que possam ser detetados mas que não existam 
                        //na base de dados - tirei mas pode ser preciso
                        foreach (Gesture gesture in this.vgbFrameSource.Gestures)
                        {
                            if (gesture.Name.Equals(GestureNames.hungryTop) && gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result != null)
                                {
                                    // update the GestureResultView object with new gesture result values
                                    this.GestureResultView.UpdateGestureResult(true, result.Detected, result.Confidence, gesture.Name);
                                }
                            }
                        }
                        */
                    }

                    /*

                    if (continuousResults != null)
                    {
                        if (gesture.Name.Equals(this.steerProgressGestureName) && gesture.GestureType == GestureType.Continuous)
                        {
                            ContinuousGestureResult result = null;
                            continuousResults.TryGetValue(gesture, out result);

                            if (result != null)
                            {
                                steerProgress = result.Progress;
                            }
                        }
                    }
                }
                */
                }
            }
        }

        /// <summary>
        /// Handles the TrackingIdLost event for the VisualGestureBuilderSource object
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            // update the GestureResultView object to show the 'Not Tracked' image in the UI
            this.GestureResultView.UpdateGestureResult(false, false, 0.0f, "TESTE");
        }

        private void gestureDetection(string gestureName)
        {
            if (gestureName.Contains("Hungry"))
            {
                this.mainWindow.canteens.Background = Brushes.Green;
            }

            string json = "{ \"recognized\": [" + "\"" + gestureName + "\"" + "] }";

            Console.WriteLine(json);

            var exNot = lce.ExtensionNotification(0 + "", 10 + "", 0, json);
            mmic.Send(exNot);
        }
    }
}
