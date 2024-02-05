//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ImageTracking_CloudRecognition
{
    public class CloudRecognizerSample : MonoBehaviour
    {
        public ARSession Session;
        public UnityEngine.UI.Text Status;
        public bool UseOfflineCache = true;
        public string OfflineCachePath;
        public Button BackButton;

        private CloudRecognizerFrameFilter cloudRecognizer;
        private ImageTrackerFrameFilter tracker;
        private List<GameObject> targetObjs = new List<GameObject>();
        private List<string> loadedCloudTargetUids = new List<string>();
        private int cachedTargetCount;
        private ResolveInfo resolveInfo;
        private float resolveInterval = 1f;
        private bool isTracking;
        private bool resolveOn;
        private int minResolveTimeout;
        private int maxResolveTimeout = 20000;

        private void Awake()
        {
            tracker = Session.GetComponentInChildren<ImageTrackerFrameFilter>();
            cloudRecognizer = Session.GetComponentInChildren<CloudRecognizerFrameFilter>();
            minResolveTimeout = cloudRecognizer.ResolveTimeout;

            if (UseOfflineCache)
            {
                if (string.IsNullOrEmpty(OfflineCachePath))
                {
                    OfflineCachePath = Application.persistentDataPath + "/CloudRecognizerSample";
                }
                if (!Directory.Exists(OfflineCachePath))
                {
                    Directory.CreateDirectory(OfflineCachePath);
                }
                if (Directory.Exists(OfflineCachePath))
                {
                    var targetFiles = Directory.GetFiles(OfflineCachePath);
                    foreach (var file in targetFiles)
                    {
                        if (Path.GetExtension(file) == ".etd")
                        {
                            LoadOfflineTarget(file);
                        }
                    }
                }
            }

            var launcher = "AllSamplesLauncher";
            if (Application.CanStreamedLevelBeLoaded(launcher))
            {
                var button = BackButton.GetComponent<Button>();
                button.onClick.AddListener(() => { UnityEngine.SceneManagement.SceneManager.LoadScene(launcher); });
            }
            else
            {
                BackButton.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            StartAutoResolve();
        }

        private void Update()
        {
            Status.text =
            (cloudRecognizer.ResolveTimeout == maxResolveTimeout ? $"Your network condition is possibility not good, resolve timeout has been set to {maxResolveTimeout}" + Environment.NewLine + Environment.NewLine : "") +
            "Resolve: " + ((resolveInfo == null || resolveInfo.Index == 0 || isTracking) ? "OFF" : "ON") + Environment.NewLine +
            "\tIndex: " + ((resolveInfo == null || resolveInfo.Index == 0) ? "-" : (resolveInfo.Index).ToString()) + Environment.NewLine +
            "\tCostTime: " + ((resolveInfo == null || resolveInfo.Index == 0) ? "-" : (resolveInfo.CostTime).ToString() + "s") + Environment.NewLine +
            "\tTargetName: " + ((resolveInfo == null || resolveInfo.Index == 0) ? "-" : (resolveInfo.TargetName).ToString()) + Environment.NewLine +
            "\tCloudStatus: " + ((resolveInfo == null || resolveInfo.Index == 0) ? "-" : (resolveInfo.CloudStatus).ToString()) + Environment.NewLine +
            "\tErrorMessage: " + ((resolveInfo == null || resolveInfo.Index == 0) ? "-" : (resolveInfo.UnknownErrorMessage).ToString()) + Environment.NewLine +
            "CachedTargets: " + cachedTargetCount + Environment.NewLine +
            "LoadedTargets: " + loadedCloudTargetUids.Count;

            AutoResolve();
        }

        private void OnDestroy()
        {
            foreach (var obj in targetObjs)
            {
                Destroy(obj);
            }
        }

        public void ClearAll()
        {
            if (Directory.Exists(OfflineCachePath))
            {
                var targetFiles = Directory.GetFiles(OfflineCachePath);
                foreach (var file in targetFiles)
                {
                    if (Path.GetExtension(file) == ".etd")
                    {
                        File.Delete(file);
                    }
                }
            }
            foreach (var obj in targetObjs)
            {
                Destroy(obj);
            }
            targetObjs.Clear();
            loadedCloudTargetUids.Clear();
            cachedTargetCount = 0;
        }

        public void StartAutoResolve()
        {
            if (Session != null && resolveInfo == null)
            {
                resolveInfo = new ResolveInfo();
                resolveOn = true;
                cloudRecognizer.ResolveTimeout = minResolveTimeout;
            }
        }

        public void StopResolve()
        {
            if (Session != null)
            {
                resolveInfo = null;
                resolveOn = false;
            }
        }

        private void AutoResolve()
        {
            var time = Time.time;
            if (!resolveOn || isTracking || resolveInfo.Running || time - resolveInfo.ResolveTime < resolveInterval)
            {
                return;
            }

            resolveInfo.Running = true;

            cloudRecognizer.Resolve((frame) =>
            {
                resolveInfo.ResolveTime = time;
            }, (resultO, error) =>
            {
                if (resolveInfo == null)
                {
                    return;
                }
                resolveInfo.Running = false;
                resolveInfo.TargetName = "-";

                if (resultO.OnNone)
                {
                    resolveInfo.CostTime = 0;
                    resolveInfo.CloudStatus = CloudRecognizationStatus.UnknownError;
                    resolveInfo.UnknownErrorMessage = error;
                    return;
                }
                var result = resultO.Value;

                resolveInfo.Index++;
                resolveInfo.CostTime = Time.time - resolveInfo.ResolveTime;
                resolveInfo.CloudStatus = result.getStatus();
                resolveInfo.UnknownErrorMessage = result.getUnknownErrorMessage();

                resolveInfo.SlowResponseCount = resolveInfo.CloudStatus == CloudRecognizationStatus.UnknownError && resolveInfo.CostTime > cloudRecognizer.ResolveTimeout / 1000f ? resolveInfo.SlowResponseCount + 1 : 0;
                resolveInfo.FastResponseCount = resolveInfo.CostTime < minResolveTimeout / 1000f ? resolveInfo.FastResponseCount + 1 : 0;
                if (resolveInfo.SlowResponseCount == 3 && cloudRecognizer.ResolveTimeout < maxResolveTimeout)
                {
                    cloudRecognizer.ResolveTimeout = maxResolveTimeout;
                    resolveInfo.SlowResponseCount = 0;
                    resolveInfo.FastResponseCount = 0;
                }
                if (resolveInfo.FastResponseCount == 5 && cloudRecognizer.ResolveTimeout > minResolveTimeout)
                {
                    cloudRecognizer.ResolveTimeout = minResolveTimeout;
                    resolveInfo.SlowResponseCount = 0;
                    resolveInfo.FastResponseCount = 0;
                }

                var target = result.getTarget();
                if (target.OnSome)
                {
                    using (var targetValue = target.Value)
                    {
                        resolveInfo.TargetName = targetValue.name();

                        if (!loadedCloudTargetUids.Contains(targetValue.uid()))
                        {
                            LoadCloudTarget(targetValue.Clone());
                        }
                    }
                }
            });
        }

        private void LoadCloudTarget(ImageTarget target)
        {
            var uid = target.uid();
            loadedCloudTargetUids.Add(uid);
            var go = new GameObject(uid);
            targetObjs.Add(go);
            var targetController = go.AddComponent<ImageTargetController>();
            targetController.SourceType = ImageTargetController.DataSource.Target;
            targetController.TargetSource = target;
            LoadTargetIntoTracker(targetController);

            targetController.TargetLoad += (loadedTarget, result) =>
            {
                if (!result)
                {
                    Debug.LogErrorFormat("target {0} load failed", uid);
                    return;
                }
                AddCubeOnTarget(targetController);
            };

            if (UseOfflineCache && Directory.Exists(OfflineCachePath))
            {
                if (target.save(OfflineCachePath + "/" + target.uid() + ".etd"))
                {
                    cachedTargetCount++;
                }
            }
        }

        private void LoadOfflineTarget(string file)
        {
            var go = new GameObject(Path.GetFileNameWithoutExtension(file) + "-offline");
            targetObjs.Add(go);
            var targetController = go.AddComponent<ImageTargetController>();
            targetController.SourceType = ImageTargetController.DataSource.TargetDataFile;
            targetController.TargetDataFileSource.PathType = PathType.Absolute;
            targetController.TargetDataFileSource.Path = file;
            LoadTargetIntoTracker(targetController);

            targetController.TargetLoad += (loadedTarget, result) =>
            {
                if (!result)
                {
                    Debug.LogErrorFormat("target data {0} load failed", file);
                    return;
                }
                loadedCloudTargetUids.Add(loadedTarget.uid());
                cachedTargetCount++;
                AddCubeOnTarget(targetController);
            };
        }

        private void LoadTargetIntoTracker(ImageTargetController controller)
        {
            controller.Tracker = tracker;
            controller.TargetFound += () =>
            {
                isTracking = true;
            };
            controller.TargetLost += () =>
            {
                isTracking = false;
            };
        }

        private void AddCubeOnTarget(ImageTargetController controller)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material = Resources.Load("Materials/EasyAR") as Material;
            cube.transform.parent = controller.transform;
            cube.transform.localPosition = new Vector3(0, 0, -0.1f);
            cube.transform.eulerAngles = new Vector3(0, 0, 180);
            cube.transform.localScale = new Vector3(0.5f, 0.5f / controller.Target.aspectRatio(), 0.2f);
        }

        private class ResolveInfo
        {
            public int Index = 0;
            public bool Running = false;
            public float ResolveTime = 0;
            public float CostTime = 0;
            public string TargetName = "-";
            public Optional<string> UnknownErrorMessage;
            public CloudRecognizationStatus CloudStatus = CloudRecognizationStatus.UnknownError;
            public int SlowResponseCount = 0;
            public int FastResponseCount = 0;
        }
    }
}
