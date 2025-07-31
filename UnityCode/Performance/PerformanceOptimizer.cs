using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Collections;

namespace FootballMaster.Performance
{
    [System.Serializable]
    public class PerformanceSettings
    {
        [Header("Target Performance")]
        public int targetFrameRate = 120;
        public int minimumFrameRate = 60;
        public float targetFrameTime = 8.33f; // 120fps = 8.33ms per frame
        
        [Header("Quality Levels")]
        [Range(0, 5)]
        public int qualityLevel = 3;
        public bool adaptiveQuality = true;
        public float performanceCheckInterval = 2f;
        
        [Header("Memory Management")]
        public int maxMemoryMB = 1500; // 1.5GB max for 2GB devices
        public bool enableGarbageCollection = true;
        public float gcInterval = 30f;
        
        [Header("LOD Settings")]
        public bool enableLODSystem = true;
        public float lodBias = 1.0f;
        public int maximumLODLevel = 0;
        
        [Header("Culling")]
        public bool enableFrustumCulling = true;
        public bool enableOcclusionCulling = true;
        public float cullingDistance = 50f;
        
        [Header("Shadows")]
        public ShadowQuality shadowQuality = ShadowQuality.HardOnly;
        public ShadowResolution shadowResolution = ShadowResolution.Medium;
        public float shadowDistance = 25f;
        
        [Header("Texture Quality")]
        public int textureQuality = 0; // 0 = full, 1 = half, 2 = quarter
        public bool enableTextureStreaming = true;
        public int anisotropicFiltering = 2;
        
        [Header("Particle Systems")]
        public bool limitParticles = true;
        public int maxParticles = 500;
        public bool enableParticleLOD = true;
        
        [Header("Audio")]
        public bool enableAudioCompression = true;
        public int maxVoiceCount = 16;
        public float audioFadeDistance = 30f;
    }

    public class PerformanceOptimizer : MonoBehaviour
    {
        [Header("Performance Settings")]
        public PerformanceSettings settings;
        
        [Header("Monitoring")]
        public bool enableFPSCounter = true;
        public bool enableMemoryMonitor = true;
        public bool showDebugUI = false;
        
        [Header("Adaptive Settings")]
        public bool enableAdaptivePerformance = true;
        public float performanceThreshold = 0.8f; // 80% of target performance
        
        // Performance monitoring
        private float frameTime = 0f;
        private float averageFrameTime = 0f;
        private int frameCount = 0;
        private float deltaTime = 0f;
        private Queue<float> frameTimes = new Queue<float>();
        private const int frameTimeHistorySize = 60;
        
        // Memory monitoring
        private long currentMemoryUsage = 0;
        private long peakMemoryUsage = 0;
        private float lastGCTime = 0f;
        
        // Quality management
        private int currentQualityLevel;
        private bool isOptimizing = false;
        private float lastPerformanceCheck = 0f;
        
        // Object pooling
        private Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();
        
        // Culling system
        private Camera mainCamera;
        private Plane[] cameraFrustumPlanes = new Plane[6];
        
        // Events
        public event System.Action<float> OnFrameRateChanged;
        public event System.Action<long> OnMemoryUsageChanged;
        public event System.Action<int> OnQualityLevelChanged;
        
        void Start()
        {
            InitializePerformanceOptimizer();
            ApplyInitialSettings();
            StartCoroutine(PerformanceMonitoringCoroutine());
        }
        
        void InitializePerformanceOptimizer()
        {
            // Set target frame rate
            Application.targetFrameRate = settings.targetFrameRate;
            QualitySettings.vSyncCount = 0; // Disable VSync for better frame rate control
            
            // Get main camera
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
            
            currentQualityLevel = settings.qualityLevel;
            
            Debug.Log($"Performance Optimizer initialized - Target FPS: {settings.targetFrameRate}");
        }
        
        void ApplyInitialSettings()
        {
            // Apply quality settings
            ApplyQualityLevel(currentQualityLevel);
            
            // Configure LOD system
            ConfigureLODSystem();
            
            // Configure shadows
            ConfigureShadows();
            
            // Configure texture settings
            ConfigureTextures();
            
            // Configure particle systems
            ConfigureParticles();
            
            // Configure audio
            ConfigureAudio();
            
            Debug.Log($"Initial performance settings applied - Quality Level: {currentQualityLevel}");
        }
        
        void Update()
        {
            // Update performance metrics
            UpdatePerformanceMetrics();
            
            // Check for adaptive performance adjustments
            if (enableAdaptivePerformance && Time.time - lastPerformanceCheck > settings.performanceCheckInterval)
            {
                CheckAndAdjustPerformance();
                lastPerformanceCheck = Time.time;
            }
            
            // Update culling
            if (settings.enableFrustumCulling && mainCamera != null)
            {
                UpdateFrustumCulling();
            }
            
            // Garbage collection management
            if (settings.enableGarbageCollection && Time.time - lastGCTime > settings.gcInterval)
            {
                PerformGarbageCollection();
                lastGCTime = Time.time;
            }
        }
        
        void UpdatePerformanceMetrics()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            frameTime = deltaTime * 1000f; // Convert to milliseconds
            
            // Update frame time history
            frameTimes.Enqueue(frameTime);
            if (frameTimes.Count > frameTimeHistorySize)
            {
                frameTimes.Dequeue();
            }
            
            // Calculate average frame time
            float totalFrameTime = 0f;
            foreach (float ft in frameTimes)
            {
                totalFrameTime += ft;
            }
            averageFrameTime = totalFrameTime / frameTimes.Count;
            
            frameCount++;
            
            // Update memory usage
            currentMemoryUsage = System.GC.GetTotalMemory(false);
            if (currentMemoryUsage > peakMemoryUsage)
            {
                peakMemoryUsage = currentMemoryUsage;
            }
            
            // Fire events
            if (frameCount % 30 == 0) // Update every 30 frames
            {
                OnFrameRateChanged?.Invoke(1000f / averageFrameTime);
                OnMemoryUsageChanged?.Invoke(currentMemoryUsage);
            }
        }
        
        void CheckAndAdjustPerformance()
        {
            if (isOptimizing) return;
            
            float currentPerformance = (1000f / averageFrameTime) / settings.targetFrameRate;
            
            if (currentPerformance < performanceThreshold)
            {
                // Performance is below threshold, reduce quality
                StartCoroutine(OptimizePerformance());
            }
            else if (currentPerformance > 1.2f && currentQualityLevel < 5)
            {
                // Performance is good, try to increase quality
                StartCoroutine(IncreaseQuality());
            }
        }
        
        IEnumerator OptimizePerformance()
        {
            isOptimizing = true;
            
            Debug.Log("Performance below threshold, optimizing...");
            
            // Progressive optimization steps
            if (currentQualityLevel > 0)
            {
                SetQualityLevel(currentQualityLevel - 1);
                yield return new WaitForSeconds(2f);
                
                float newPerformance = (1000f / averageFrameTime) / settings.targetFrameRate;
                if (newPerformance >= performanceThreshold)
                {
                    isOptimizing = false;
                    yield break;
                }
            }
            
            // Additional optimizations if quality reduction isn't enough
            OptimizeShadows();
            yield return new WaitForSeconds(1f);
            
            OptimizeParticles();
            yield return new WaitForSeconds(1f);
            
            OptimizeTextures();
            yield return new WaitForSeconds(1f);
            
            // Last resort optimizations
            if ((1000f / averageFrameTime) < settings.minimumFrameRate)
            {
                EmergencyOptimization();
            }
            
            isOptimizing = false;
        }
        
        IEnumerator IncreaseQuality()
        {
            isOptimizing = true;
            
            Debug.Log("Performance is good, trying to increase quality...");
            
            int newQualityLevel = Mathf.Min(currentQualityLevel + 1, 5);
            SetQualityLevel(newQualityLevel);
            
            yield return new WaitForSeconds(3f);
            
            float newPerformance = (1000f / averageFrameTime) / settings.targetFrameRate;
            if (newPerformance < performanceThreshold)
            {
                // Revert if performance dropped too much
                SetQualityLevel(currentQualityLevel);
                Debug.Log("Quality increase reverted due to performance drop");
            }
            
            isOptimizing = false;
        }
        
        public void SetQualityLevel(int level)
        {
            level = Mathf.Clamp(level, 0, 5);
            currentQualityLevel = level;
            
            ApplyQualityLevel(level);
            OnQualityLevelChanged?.Invoke(level);
            
            Debug.Log($"Quality level set to: {level}");
        }
        
        void ApplyQualityLevel(int level)
        {
            switch (level)
            {
                case 0: // Potato mode - Maximum performance
                    QualitySettings.pixelLightCount = 0;
                    QualitySettings.shadowCascades = 0;
                    QualitySettings.shadowDistance = 0;
                    QualitySettings.shadows = ShadowQuality.Disable;
                    QualitySettings.antiAliasing = 0;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                    QualitySettings.masterTextureLimit = 2;
                    break;
                    
                case 1: // Low quality
                    QualitySettings.pixelLightCount = 1;
                    QualitySettings.shadowCascades = 1;
                    QualitySettings.shadowDistance = 15;
                    QualitySettings.shadows = ShadowQuality.HardOnly;
                    QualitySettings.antiAliasing = 0;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    QualitySettings.masterTextureLimit = 1;
                    break;
                    
                case 2: // Medium quality
                    QualitySettings.pixelLightCount = 2;
                    QualitySettings.shadowCascades = 2;
                    QualitySettings.shadowDistance = 25;
                    QualitySettings.shadows = ShadowQuality.HardOnly;
                    QualitySettings.antiAliasing = 2;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    QualitySettings.masterTextureLimit = 0;
                    break;
                    
                case 3: // Good quality (default)
                    QualitySettings.pixelLightCount = 3;
                    QualitySettings.shadowCascades = 2;
                    QualitySettings.shadowDistance = 35;
                    QualitySettings.shadows = ShadowQuality.All;
                    QualitySettings.antiAliasing = 4;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    QualitySettings.masterTextureLimit = 0;
                    break;
                    
                case 4: // High quality
                    QualitySettings.pixelLightCount = 4;
                    QualitySettings.shadowCascades = 4;
                    QualitySettings.shadowDistance = 50;
                    QualitySettings.shadows = ShadowQuality.All;
                    QualitySettings.antiAliasing = 4;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    QualitySettings.masterTextureLimit = 0;
                    break;
                    
                case 5: // Ultra quality
                    QualitySettings.pixelLightCount = 6;
                    QualitySettings.shadowCascades = 4;
                    QualitySettings.shadowDistance = 75;
                    QualitySettings.shadows = ShadowQuality.All;
                    QualitySettings.antiAliasing = 8;
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    QualitySettings.masterTextureLimit = 0;
                    break;
            }
        }
        
        void ConfigureLODSystem()
        {
            if (settings.enableLODSystem)
            {
                QualitySettings.lodBias = settings.lodBias;
                QualitySettings.maximumLODLevel = settings.maximumLODLevel;
            }
        }
        
        void ConfigureShadows()
        {
            QualitySettings.shadows = settings.shadowQuality;
            QualitySettings.shadowResolution = settings.shadowResolution;
            QualitySettings.shadowDistance = settings.shadowDistance;
        }
        
        void ConfigureTextures()
        {
            QualitySettings.masterTextureLimit = settings.textureQuality;
            QualitySettings.streamingMipmapsActive = settings.enableTextureStreaming;
            
            switch (settings.anisotropicFiltering)
            {
                case 0:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                    break;
                case 1:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    break;
                default:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                    break;
            }
        }
        
        void ConfigureParticles()
        {
            if (settings.limitParticles)
            {
                QualitySettings.particleRaycastBudget = settings.maxParticles;
                
                // Find and configure particle systems
                ParticleSystem[] particleSystems = FindObjectsOfType<ParticleSystem>();
                foreach (var ps in particleSystems)
                {
                    if (settings.enableParticleLOD)
                    {
                        ConfigureParticleSystemLOD(ps);
                    }
                }
            }
        }
        
        void ConfigureParticleSystemLOD(ParticleSystem ps)
        {
            var main = ps.main;
            
            // Reduce max particles based on distance to camera
            if (mainCamera != null)
            {
                float distance = Vector3.Distance(ps.transform.position, mainCamera.transform.position);
                float distanceRatio = distance / settings.cullingDistance;
                
                if (distanceRatio > 0.5f)
                {
                    main.maxParticles = Mathf.RoundToInt(main.maxParticles * (1f - distanceRatio));
                }
            }
        }
        
        void ConfigureAudio()
        {
            AudioConfiguration audioConfig = AudioSettings.GetConfiguration();
            audioConfig.speakerMode = AudioSpeakerMode.Stereo;
            audioConfig.sampleRate = 44100;
            AudioSettings.Reset(audioConfig);
            
            // Configure 3D audio settings
            AudioListener.volume = 1f;
        }
        
        void UpdateFrustumCulling()
        {
            GeometryUtility.CalculateFrustumPlanes(mainCamera, cameraFrustumPlanes);
            
            // This would typically be used with a custom culling system
            // For now, just ensure the camera's culling distance is set
            mainCamera.farClipPlane = settings.cullingDistance;
        }
        
        void OptimizeShadows()
        {
            if (QualitySettings.shadows != ShadowQuality.Disable)
            {
                QualitySettings.shadowDistance = Mathf.Max(15f, QualitySettings.shadowDistance * 0.8f);
                
                if (QualitySettings.shadowCascades > 1)
                {
                    QualitySettings.shadowCascades = Mathf.Max(1, QualitySettings.shadowCascades - 1);
                }
            }
        }
        
        void OptimizeParticles()
        {
            ParticleSystem[] particleSystems = FindObjectsOfType<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                var main = ps.main;
                main.maxParticles = Mathf.RoundToInt(main.maxParticles * 0.7f);
                
                var emission = ps.emission;
                emission.rateOverTime = emission.rateOverTime.constant * 0.7f;
            }
        }
        
        void OptimizeTextures()
        {
            QualitySettings.masterTextureLimit = Mathf.Min(2, QualitySettings.masterTextureLimit + 1);
        }
        
        void EmergencyOptimization()
        {
            Debug.LogWarning("Emergency optimization activated!");
            
            // Disable all non-essential effects
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.antiAliasing = 0;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.masterTextureLimit = 2;
            
            // Reduce particle count drastically
            ParticleSystem[] particleSystems = FindObjectsOfType<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                var main = ps.main;
                main.maxParticles = Mathf.RoundToInt(main.maxParticles * 0.3f);
            }
            
            // Reduce LOD quality
            QualitySettings.lodBias = 0.5f;
            QualitySettings.maximumLODLevel = 2;
        }
        
        void PerformGarbageCollection()
        {
            long memoryBefore = System.GC.GetTotalMemory(false);
            System.GC.Collect();
            long memoryAfter = System.GC.GetTotalMemory(true);
            
            long memoryFreed = memoryBefore - memoryAfter;
            if (memoryFreed > 1048576) // More than 1MB freed
            {
                Debug.Log($"Garbage collection freed {memoryFreed / 1048576}MB");
            }
        }
        
        // Object Pooling System
        public GameObject GetPooledObject(string poolName, GameObject prefab)
        {
            if (!objectPools.ContainsKey(poolName))
            {
                objectPools[poolName] = new Queue<GameObject>();
            }
            
            if (objectPools[poolName].Count > 0)
            {
                GameObject pooledObject = objectPools[poolName].Dequeue();
                pooledObject.SetActive(true);
                return pooledObject;
            }
            else
            {
                return Instantiate(prefab);
            }
        }
        
        public void ReturnToPool(string poolName, GameObject obj)
        {
            if (!objectPools.ContainsKey(poolName))
            {
                objectPools[poolName] = new Queue<GameObject>();
            }
            
            obj.SetActive(false);
            objectPools[poolName].Enqueue(obj);
        }
        
        // Public API
        public float GetCurrentFPS()
        {
            return 1000f / averageFrameTime;
        }
        
        public float GetFrameTime()
        {
            return averageFrameTime;
        }
        
        public long GetMemoryUsage()
        {
            return currentMemoryUsage;
        }
        
        public int GetCurrentQualityLevel()
        {
            return currentQualityLevel;
        }
        
        public bool IsPerformanceGood()
        {
            float currentPerformance = (1000f / averageFrameTime) / settings.targetFrameRate;
            return currentPerformance >= performanceThreshold;
        }
        
        // Debug UI
        void OnGUI()
        {
            if (!showDebugUI) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label($"FPS: {GetCurrentFPS():F1} (Target: {settings.targetFrameRate})");
            GUILayout.Label($"Frame Time: {averageFrameTime:F2}ms");
            GUILayout.Label($"Memory: {currentMemoryUsage / 1048576}MB / {settings.maxMemoryMB}MB");
            GUILayout.Label($"Quality Level: {currentQualityLevel}");
            GUILayout.Label($"Performance: {(IsPerformanceGood() ? "Good" : "Poor")}");
            
            if (GUILayout.Button("Force GC"))
            {
                PerformGarbageCollection();
            }
            
            if (GUILayout.Button("Emergency Optimize"))
            {
                EmergencyOptimization();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        IEnumerator PerformanceMonitoringCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                
                // Check memory usage
                if (currentMemoryUsage > settings.maxMemoryMB * 1048576 * 0.9f) // 90% of max
                {
                    Debug.LogWarning("Memory usage approaching limit, performing garbage collection");
                    PerformGarbageCollection();
                }
                
                // Check for memory leaks
                if (currentMemoryUsage > peakMemoryUsage * 1.5f)
                {
                    Debug.LogError("Potential memory leak detected!");
                }
            }
        }
    }
}