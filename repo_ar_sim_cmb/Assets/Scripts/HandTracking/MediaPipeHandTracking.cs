using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe.Unity;

using Stopwatch = System.Diagnostics.Stopwatch;
using System;
using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Unity.CoordinateSystem;

namespace Mediapipe.Unity.Tutorial
{
    public class MediaPipeHandTracking : Singleton<MediaPipeHandTracking>
    {
        [SerializeField] private TextAsset _configAsset;
        [SerializeField] private RawImage _screen;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private int _fps;


        private int _maxNumHands = 1;

        private CalculatorGraph _graph;
        private ResourceManager _resourceManager;

        private WebCamTexture _webCamTexture;
        private Texture2D _inputTexture;
        private Color32[] _inputPixelData;
        private Texture2D _outputTexture;
        private Color32[] _outputPixelData;

        public Transform handSkeleton;
        public Transform leftHandSkeleton;
        public Transform rightHandSkeleton;

        Vector3[] handPointsTranslations = new Vector3[21];

        public Transform[] leftHandPoints;
        Vector3[] leftHandPointsTranslations = new Vector3[21];
        public Transform[] leftHandLines;

        public Transform[] rightHandPoints;
        Vector3[] rightHandPointsTranslations = new Vector3[21];
        public Transform[] rightHandLines;

        public float handDistanceRatio = 0.1f;

        /// <summary>
        /// The palm length as reference
        /// </summary>
        public float palmLength = 0.1f;

        /// <summary>
        /// Focal length of the camera for hand-tracking.
        /// </summary>
        public float focalLength = 680;

        /// <summary>
        /// The distance from the camera to the hand
        /// </summary>
        public float handDistance;
        public float leftHandDistance;
        public float rightHandDistance;

        /// <summary>
        /// The scale of the tracked palm. Used to calculate the hand distances.
        /// </summary>
        float handScale;
        float leftHandScale;
        float rightHandScale;

        /// <summary>
        /// The wrist position
        /// </summary>
        public Vector3 handRootTranslation;
        public Vector3 leftHandRootTranslation;
        public Vector3 rightHandRootTranslation;


        public float handDistanceScale = 0.2f;
        public Vector2 handRootScreenPos;

        public float leftHandDistanceScale = 0.2f;
        public Vector2 leftHandRootScreenPos;

        public float rightHandDistanceScale = 0.2f;
        public Vector2 rightHandRootScreenPos;

        public Vector3 palmNorm;
        public Vector3 palmForward;
        public Vector3 palmRight;

        public Vector3 leftPalmNorm;
        public Vector3 leftPalmForward;
        public Vector3 leftPalmRight;

        public Vector3 rightPalmNorm;
        public Vector3 rightPalmForward;
        public Vector3 rightPalmRight;

        public bool flipPalm = false;

        // public HandIK handIK;

        float leftThumbAngleVelocity;
        float leftIndexAngleVelocity;
        float leftMiddleAngleVelocity;
        float leftRingAngleVelocity;
        float leftPinkyAngleVelocity;

        float rightThumbAngleVelocity;
        float rightIndexAngleVelocity;
        float rightMiddleAngleVelocity;
        float rightRingAngleVelocity;
        float rightPinkyAngleVelocity;


        [Header("Thumb")]
        [Range(0, 90f)]
        public float minThumb = 0f;
        [Range(0, 90f)]
        public float maxThumb = 60f;

        [Header("Index")]
        [Range(0, 90f)]
        public float minIndex = 0f;
        [Range(0, 90f)]
        public float maxIndex = 60f;

        [Header("Middle")]
        [Range(0, 90f)]
        public float minMiddle = 0f;
        [Range(0, 90f)]
        public float maxMiddle = 60f;

        [Header("Ring")]
        [Range(0, 90f)]
        public float minRing = 0f;
        [Range(0, 90f)]
        public float maxRing = 60f;

        [Header("Pinky")]
        [Range(0, 90f)]
        public float minPinky = 0f;
        [Range(0, 90f)]
        public float maxPinky = 60f;

        public GameObject roboy;
        public GameObject ZoneManager;

        struct MPFinger
        {
            public int root;
            public Finger finger;

            public MPFinger(int root, Finger finger)
            {
                this.root = root;
                this.finger = finger;
            }
        }

        MPFinger[] fingers = new[]
            {
                new MPFinger(1,  new Finger(Handedness.Left, FingerType.Thumb) ),
                new MPFinger(5,  new Finger(Handedness.Left, FingerType.Index) ),
                new MPFinger(9,  new Finger(Handedness.Left, FingerType.Middle)),
                new MPFinger(13, new Finger(Handedness.Left, FingerType.Ring)  ),
                new MPFinger(17, new Finger(Handedness.Left, FingerType.Little)),

                new MPFinger(1,  new Finger(Handedness.Right, FingerType.Thumb) ),
                new MPFinger(5,  new Finger(Handedness.Right, FingerType.Index) ),
                new MPFinger(9,  new Finger(Handedness.Right, FingerType.Middle)),
                new MPFinger(13, new Finger(Handedness.Right, FingerType.Ring)  ),
                new MPFinger(17, new Finger(Handedness.Right, FingerType.Little))
            };


        private IEnumerator Start()
        {
            // leftHandKalmanFilter = new KalmanFilterVector3(Q, R);
            // rightHandKalmanFilter = new KalmanFilterVector3(Q, R);

            if (WebCamTexture.devices.Length == 0)
            {
                throw new System.Exception("No Web Camera devices detected. Disabling hand tracking.");
            }
            var webCamDevice = WebCamTexture.devices[0];
            _webCamTexture = new WebCamTexture(webCamDevice.name, _width, _height, _fps);
            _webCamTexture.Play();

            yield return new WaitUntil(() => _webCamTexture.width > 16);

            _screen.rectTransform.sizeDelta = new Vector2(_width, _height);

            _inputTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
            _inputPixelData = new Color32[_width * _height];
            _outputTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
            _outputPixelData = new Color32[_width * _height];

            // _screen.texture = _outputTexture;

            _resourceManager = new StreamingAssetsResourceManager();

            yield return _resourceManager.PrepareAssetAsync("hand_landmark_full.bytes");
            yield return _resourceManager.PrepareAssetAsync("palm_detection_full.bytes");

            var stopwatch = new Stopwatch();

            _graph = new CalculatorGraph(_configAsset.text);

            var poseLandmarksStreamPoler = _graph.AddOutputStreamPoller<NormalizedLandmarkList>("hand_landmarks");
            var poseLandmarksPacket = new Packet<NormalizedLandmarkList>();


            var sidePacket = new PacketMap();

            sidePacket.Emplace("num_hands", Packet.CreateInt(_maxNumHands));
            Logger.LogInfo($"Max Num Hands = {_maxNumHands}");

            _graph.StartRun(sidePacket); //.AssertOk();

            stopwatch.Start();

            var screenRect = _screen.GetComponent<RectTransform>().rect;

            while (true)
            {
                _inputTexture.SetPixels32(_webCamTexture.GetPixels32(_inputPixelData));
                var imageFrame = new ImageFrame(ImageFormat.Types.Format.Srgba, _width, _height, _width * 4, _inputTexture.GetRawTextureData<byte>());
                var currentTimestamp = stopwatch.ElapsedTicks / (System.TimeSpan.TicksPerMillisecond / 1000);
                _graph.AddPacketToInputStream("input_video", Packet.CreateImageFrameAt(imageFrame, (currentTimestamp)));

                yield return new WaitForEndOfFrame();

                if (poseLandmarksStreamPoler.Next(poseLandmarksPacket))
                {
                    // Debug.Log("detected");
                    if (!poseLandmarksPacket.IsEmpty())
                    {
                        var handLandmarks = poseLandmarksPacket.Get(NormalizedLandmarkList.Parser).Landmark;
                        
                            //var landmarks = handLandmarks[j];

                            handRootScreenPos = screenRect.GetPoint(handLandmarks[0]);
                            handRootTranslation = screenRect.GetPoint(handLandmarks[0]) / 100;
                            for (int i = 0; i < handLandmarks.Count; i++)
                            {
                                var landmark =handLandmarks[i];
                                handPointsTranslations[i] = (screenRect.GetPoint(landmark) / 100) - handRootTranslation;
                            }

                            // Now the hand landmarks are updated.
                            handScale = UpdateHandOrientation();
                            if (handSkeleton.GetComponent<MediaPipeHandTrackingSkeleton>()
                                .CanMoveHand(handPointsTranslations, ZoneManager, roboy))
                            {
                                for (int i = 0; i < 21; i++)
                                {
                                    handPointsTranslations[i] *= handScale;

                                    handSkeleton.GetComponent<MediaPipeHandTrackingSkeleton>()
                                        .SetPointLocalPosition(i, handPointsTranslations[i]);
                                }
                            }

                            foreach (var finger in fingers)
                            {
                                FingerController.Instance.UpdateFingerAngles(finger.finger, CalculateFingerAngles(finger, handPointsTranslations, palmForward, palmRight));
                            }

                            handRootTranslation *= handScale;
                            handDistance = focalLength * handScale * handDistanceScale;

                            // Set the skeleton visual
                            float videoScreenWidth = 640;
                            float videoScreenHeight = 480;
                            float widthSVRatio = UnityEngine.Screen.width / videoScreenWidth;
                            float heightSVRatio = UnityEngine.Screen.height / videoScreenHeight;

                            float handScreenPosX = widthSVRatio * (videoScreenWidth / 2 + handRootScreenPos[0]);
                            float handScreenPosY = heightSVRatio * (videoScreenHeight / 2 + handRootScreenPos[1]);

                            Vector3 handWorldPos = Camera.main.ScreenToWorldPoint(
                                new Vector3(handScreenPosX, handScreenPosY, handDistance * handDistanceRatio)
                                ) + handRootTranslation;

                            try
                            {
                                handSkeleton.position = handWorldPos;
                                handSkeleton.LookAt(handSkeleton.position + Camera.main.transform.forward);
                            }
                            catch { }

                            break;

                        
                    }
                }


            }
        }
        

        float UpdateHandOrientation()
        {
            var trackedPalmLength = Vector3.Distance(handPointsTranslations[5], handPointsTranslations[0]);
            float handScale = palmLength / trackedPalmLength;

            Vector3 palmVec1 = handPointsTranslations[5] - handPointsTranslations[0];
            Vector3 palmVec2 = handPointsTranslations[17] - handPointsTranslations[0];

            palmNorm = (Vector3.Cross(palmVec1, palmVec2) * (flipPalm ? -1 : 1)).normalized;
            palmForward = (palmVec1 + palmVec2).normalized;
            palmRight = -Vector3.Cross(rightPalmForward, rightPalmNorm);

            return handScale;
        }

        float[] CalculateFingerAngles(MPFinger finger, Vector3[] handPointsTranslations, Vector3 palmForward, Vector3 palmRight)
        {

            var root = finger.root;
            Vector3 finger1 = Vector3.ProjectOnPlane((handPointsTranslations[root + 1] - handPointsTranslations[root]), palmRight);
            Vector3 finger2 = Vector3.ProjectOnPlane((handPointsTranslations[root + 2] - handPointsTranslations[root + 1]), palmRight);
            Vector3 finger3 = Vector3.ProjectOnPlane((handPointsTranslations[root + 3] - handPointsTranslations[root + 2]), palmRight);
            float angle1 = Vector3.SignedAngle(finger1, palmForward, palmRight);
            float angle2 = Vector3.SignedAngle(finger2, finger1, palmRight);
            float angle3 = Vector3.SignedAngle(finger3, finger2, palmRight);
            // Debug.Log($"angle1: {angle1}, angle2: {angle2}, angle3: {angle3}");

            // FingerController.Instance.UpdateFingerAngles(finger.finger, new[] { angle1, angle2, angle3 });
            return new[] { angle1, angle2, angle3 };
        }

        public void UpdateLines(Handedness handedness)
        {
            if (handedness == Handedness.Left)
            {
                UpdateLines(leftHandLines, leftHandPointsTranslations);
            }
            else if (handedness == Handedness.Right)
            {
                UpdateLines(rightHandLines, rightHandPointsTranslations);
            }
        }

        void UpdateLines(Transform[] handLines, Vector3[] handPointsTranslations)
        {
            for (int i = 0; i < 21; i++)
            {
                if (handPointsTranslations[i][0] == float.NaN) return;

                if (i == 4 || i == 16)
                {
                    float length = Vector3.Distance(handPointsTranslations[0], handPointsTranslations[i + 1]);
                    Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[0]).normalized;

                    handLines[i].transform.localPosition = handPointsTranslations[0];
                    handLines[i].transform.forward = forward;
                    handLines[i].transform.localScale = new Vector3(length, length, length);
                }

                else if (i == 8 || i == 12)
                {
                    float length = Vector3.Distance(handPointsTranslations[i - 3], handPointsTranslations[i + 1]);
                    Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[i - 3]).normalized;

                    handLines[i].transform.localPosition = handPointsTranslations[i - 3];
                    handLines[i].transform.forward = forward;
                    handLines[i].transform.localScale = new Vector3(length, length, length);

                }

                else if (i == 20)
                {
                    float length = Vector3.Distance(handPointsTranslations[13], handPointsTranslations[17]);
                    Vector3 forward = (handPointsTranslations[17] - handPointsTranslations[13]).normalized;

                    handLines[i].transform.localPosition = handPointsTranslations[13];
                    handLines[i].transform.forward = forward;
                    handLines[i].transform.localScale = new Vector3(length, length, length);

                }

                else
                {
                    float length = Vector3.Distance(handPointsTranslations[i], handPointsTranslations[i + 1]);
                    Vector3 forward = (handPointsTranslations[i + 1] - handPointsTranslations[i]).normalized;

                    handLines[i].transform.localPosition = handPointsTranslations[i];
                    handLines[i].transform.forward = forward;
                    handLines[i].transform.localScale = new Vector3(length, length, length);

                }
            }
        }

        private void OnDestroy()
        {
            if (_webCamTexture != null)
            {
                _webCamTexture.Stop();
            }

            if (_graph != null)
            {
                try
                {
                    _graph.CloseInputStream("input_video");//.AssertOk();
                    _graph.WaitUntilDone();
                }
                finally
                {

                    _graph.Dispose();
                }
            }
        }

        protected void SetImageTransformationOptions(PacketMap sidePacket, ImageSource imageSource, bool expectedToBeMirrored = false)
        {
            // NOTE: The origin is left-bottom corner in Unity, and right-top corner in MediaPipe.
            RotationAngle rotation = imageSource.rotation.Reverse();
            var inputRotation = rotation;
            var isInverted = CoordinateSystem.ImageCoordinate.IsInverted(rotation);
            var shouldBeMirrored = imageSource.isHorizontallyFlipped ^ expectedToBeMirrored;
            var inputHorizontallyFlipped = isInverted ^ shouldBeMirrored;
            var inputVerticallyFlipped = !isInverted;

            if ((inputHorizontallyFlipped && inputVerticallyFlipped) || rotation == RotationAngle.Rotation180)
            {
                inputRotation = inputRotation.Add(RotationAngle.Rotation180);
                inputHorizontallyFlipped = !inputHorizontallyFlipped;
                inputVerticallyFlipped = !inputVerticallyFlipped;
            }

            Logger.LogDebug($"input_rotation = {inputRotation}, input_horizontally_flipped = {inputHorizontallyFlipped}, input_vertically_flipped = {inputVerticallyFlipped}");

            sidePacket.Emplace("input_rotation", Packet.CreateInt((int)inputRotation));
            sidePacket.Emplace("input_horizontally_flipped",Packet.CreateBool(inputHorizontallyFlipped));
            sidePacket.Emplace("input_vertically_flipped", Packet.CreateBool(inputVerticallyFlipped));
        }

    }
}