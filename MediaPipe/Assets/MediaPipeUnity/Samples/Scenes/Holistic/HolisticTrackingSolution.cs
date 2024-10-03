// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;
using UnityEngine;
using VRMAvatar;

namespace Mediapipe.Unity.Holistic
{
    public class HolisticTrackingSolution : ImageSourceSolution<HolisticTrackingGraph>
    {
        [SerializeField] private RectTransform _worldAnnotationArea;
        [SerializeField] private DetectionAnnotationController _poseDetectionAnnotationController;
        [SerializeField] private HolisticLandmarkListAnnotationController _holisticAnnotationController;
        [SerializeField] private PoseWorldLandmarkListAnnotationController _poseWorldLandmarksAnnotationController;
        [SerializeField] private MaskAnnotationController _segmentationMaskAnnotationController;
        [SerializeField] private NormalizedRectAnnotationController _poseRoiAnnotationController;
        public AvatarHandLeft lh;
        public AvatarHandRight rh;
        public VisibilityManager vm;
        public AvatarBody dac;
        public AvatarFace daf;

        public HolisticTrackingGraph.ModelComplexity modelComplexity
        {
            get => graphRunner.modelComplexity;
            set => graphRunner.modelComplexity = value;
        }

        public bool smoothLandmarks
        {
            get => graphRunner.smoothLandmarks;
            set => graphRunner.smoothLandmarks = value;
        }

        public bool refineFaceLandmarks
        {
            get => graphRunner.refineFaceLandmarks;
            set => graphRunner.refineFaceLandmarks = value;
        }

        public bool enableSegmentation
        {
            get => graphRunner.enableSegmentation;
            set => graphRunner.enableSegmentation = value;
        }

        public bool smoothSegmentation
        {
            get => graphRunner.smoothSegmentation;
            set => graphRunner.smoothSegmentation = value;
        }

        public float minDetectionConfidence
        {
            get => graphRunner.minDetectionConfidence;
            set => graphRunner.minDetectionConfidence = value;
        }

        public float minTrackingConfidence
        {
            get => graphRunner.minTrackingConfidence;
            set => graphRunner.minTrackingConfidence = value;
        }

        protected override void SetupScreen(ImageSource imageSource)
        {
            base.SetupScreen(imageSource);
            _worldAnnotationArea.localEulerAngles = imageSource.rotation.Reverse().GetEulerAngles();
        }

        protected override void OnStartRun()
        {
            if (!runningMode.IsSynchronous())
            {
                graphRunner.OnPoseDetectionOutput += OnPoseDetectionOutput;
                graphRunner.OnFaceLandmarksOutput += OnFaceLandmarksOutput;
                graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
                graphRunner.OnLeftHandLandmarksOutput += OnLeftHandLandmarksOutput;
                graphRunner.OnRightHandLandmarksOutput += OnRightHandLandmarksOutput;
                graphRunner.OnPoseWorldLandmarksOutput += OnPoseWorldLandmarksOutput;
                graphRunner.OnSegmentationMaskOutput += OnSegmentationMaskOutput;
                graphRunner.OnPoseRoiOutput += OnPoseRoiOutput;
                graphRunner.OnFaceClassificationsFromBlendShapesOutput += OnFaceClassificationsFromBlendShapesOutput;

            }

            var imageSource = ImageSourceProvider.ImageSource;
            SetupAnnotationController(_poseDetectionAnnotationController, imageSource);
            SetupAnnotationController(_holisticAnnotationController, imageSource);
            //SetupAnnotationController(_poseWorldLandmarksAnnotationController, imageSource);
            SetupAnnotationController(_segmentationMaskAnnotationController, imageSource);
            _segmentationMaskAnnotationController.InitScreen(imageSource.textureWidth, imageSource.textureHeight);
            SetupAnnotationController(_poseRoiAnnotationController, imageSource);
        }

        protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
        {
            graphRunner.AddTextureFrameToInputStream(textureFrame);
        }

        protected override IEnumerator WaitForNextValue()
        {
            Detection poseDetection = null;
            NormalizedLandmarkList faceLandmarks = null;
            NormalizedLandmarkList poseLandmarks = null;
            NormalizedLandmarkList leftHandLandmarks = null;
            NormalizedLandmarkList rightHandLandmarks = null;
            LandmarkList poseWorldLandmarks = null;
            ImageFrame segmentationMask = null;
            NormalizedRect poseRoi = null;
            ClassificationList faceBlendShapes = null;

            if (runningMode == RunningMode.Sync)
            {
                var _ = graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out faceLandmarks, out leftHandLandmarks, out rightHandLandmarks, out poseWorldLandmarks, out segmentationMask, out poseRoi, out faceBlendShapes, true);
                ProcessPoseLandmark(poseLandmarks);
                ProcessFace(faceLandmarks);
                ProcessPoseWorld(poseWorldLandmarks);
                ProcessLeftHand(leftHandLandmarks);
                ProcessRightHand(rightHandLandmarks);
                ProcessFaceBlendShapes(faceBlendShapes);
            }
            else if (runningMode == RunningMode.NonBlockingSync)
            {
                yield return new WaitUntil(() =>
                  graphRunner.TryGetNext(out poseDetection, out poseLandmarks, out faceLandmarks, out leftHandLandmarks, out rightHandLandmarks, out poseWorldLandmarks, out segmentationMask, out poseRoi, out faceBlendShapes, false));
            }

            _poseDetectionAnnotationController.DrawNow(poseDetection);
            _holisticAnnotationController.DrawNow(faceLandmarks, poseLandmarks, leftHandLandmarks, rightHandLandmarks);
            //_poseWorldLandmarksAnnotationController.DrawNow(poseWorldLandmarks);
            _segmentationMaskAnnotationController.DrawNow(segmentationMask);
            _poseRoiAnnotationController.DrawNow(poseRoi);
            if (faceBlendShapes != null)
            {
            }

        }

        private void OnPoseDetectionOutput(object stream, OutputEventArgs<Detection> eventArgs)
        {
            _poseDetectionAnnotationController.DrawLater(eventArgs.value);
        }

        private void OnFaceLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {
            var faceLandmarks = eventArgs.value;
            ProcessFace(faceLandmarks);
            _holisticAnnotationController.DrawFaceLandmarkListLater(eventArgs.value);
        }

        private void ProcessFace(NormalizedLandmarkList faceLandmarks)
        {
            if (faceLandmarks?.Landmark[0] != null)
            {
                for (int i = 0; i < 478; i++)
                {
                    dac.faceLandmarks[i] = new Vector3((float)(faceLandmarks?.Landmark[i].X), (float)faceLandmarks?.Landmark[i].Y, (float)faceLandmarks?.Landmark[i].Z); ;
                }
            }
        }
        private void OnPoseLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {
            var poseLandmarks = eventArgs.value;
            ProcessPoseLandmark(poseLandmarks);
            _holisticAnnotationController.DrawPoseLandmarkListLater(eventArgs.value);
        }

        private void ProcessPoseLandmark(NormalizedLandmarkList poseLandmarks)
        {
            if (poseLandmarks?.Landmark[0] != null)
            {
                for (int i = 0; i < 33; i++)
                {
                    dac.PoseLandmarks[i] = new Vector3(poseLandmarks.Landmark[i].X, poseLandmarks.Landmark[i].Y, poseLandmarks.Landmark[i].Z);
                    dac.PoseVisibilities[i] = poseLandmarks.Landmark[i].Visibility;
                }
            }
        }
        private void OnLeftHandLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {
            var leftHandLandmarks = eventArgs.value;
            ProcessLeftHand(leftHandLandmarks);
            _holisticAnnotationController.DrawLeftHandLandmarkListLater(eventArgs.value);
        }

        private void ProcessLeftHand(NormalizedLandmarkList leftHandLandmarks)
        {
            lh.received = true;
            if (leftHandLandmarks?.Landmark[0] != null)
            {
                lh.validInput = true;
                for (int i = 0; i < 21; i++)
                {
                    lh.landmarks[i] = new Vector3(leftHandLandmarks.Landmark[i].X, leftHandLandmarks.Landmark[i].Y, leftHandLandmarks.Landmark[i].Z);
                }
            }
        }
        private void OnRightHandLandmarksOutput(object stream, OutputEventArgs<NormalizedLandmarkList> eventArgs)
        {
            var rightHandLandmarks = eventArgs.value;
            ProcessRightHand(rightHandLandmarks);
            _holisticAnnotationController.DrawRightHandLandmarkListLater(eventArgs.value);
        }

        private void ProcessRightHand(NormalizedLandmarkList rightHandLandmarks)
        {
            rh.received = true;
            if (rightHandLandmarks?.Landmark[0] != null)
            {
                rh.validInput = true;
                for (int i = 0; i < 21; i++)
                {
                    rh.landmarks[i] = new Vector3(rightHandLandmarks.Landmark[i].X, rightHandLandmarks.Landmark[i].Y, rightHandLandmarks.Landmark[i].Z);
                }
            }
        }
        private void OnPoseWorldLandmarksOutput(object stream, OutputEventArgs<LandmarkList> eventArgs)
        {
            var poseWorldLandmarks = eventArgs.value;
            ProcessPoseWorld(poseWorldLandmarks);
            //_poseWorldLandmarksAnnotationController.DrawLater(eventArgs.value);
        }

        private void ProcessPoseWorld(LandmarkList poseWorldLandmarks)
        {
            dac.received = true;
            vm.received = true;
            if (poseWorldLandmarks?.Landmark[0] != null)
            {
                dac.validInput = true;
                vm.valid = true;
                for (int i = 0; i < 33; i++)
                {
                    dac.WorldPoseLandmarks[i] = new Vector3(poseWorldLandmarks.Landmark[i].X, poseWorldLandmarks.Landmark[i].Y, poseWorldLandmarks.Landmark[i].Z);
                    dac.WorldPoseVisibilities[i] = poseWorldLandmarks.Landmark[i].Visibility;
                    vm.WorldPoseVisibilities[i] = poseWorldLandmarks.Landmark[i].Visibility;
                }
            }
        }
        private void OnSegmentationMaskOutput(object stream, OutputEventArgs<ImageFrame> eventArgs)
        {
            _segmentationMaskAnnotationController.DrawLater(eventArgs.value);
        }

        private void OnPoseRoiOutput(object stream, OutputEventArgs<NormalizedRect> eventArgs)
        {
            _poseRoiAnnotationController.DrawLater(eventArgs.value);
        }

        private void OnFaceClassificationsFromBlendShapesOutput(object stream, OutputEventArgs<ClassificationList> eventArgs)
        {
            ProcessFaceBlendShapes(eventArgs.value);
        }
        private void ProcessFaceBlendShapes(ClassificationList blendshapes)
        {
            daf.received = true;
            if (blendshapes != null)
            {
                for (var i = 0; i < 52; i++)
                {
                    daf.bsv[i] = blendshapes.Classification[i].Score;
                }
                daf.validInput = true;
            }
        }
    }
}
