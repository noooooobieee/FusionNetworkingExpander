using UnityEngine;
using System.Diagnostics;

namespace FusionNetworkingPlus.Utilities
{
    public class PerformanceMonitor : MonoBehaviour
    {
        private float updateInterval = 1.0f;
        private float timeLeft;
        private int frames;
        private float accum;

        void Start()
        {
            timeLeft = updateInterval;
        }

        void Update()
        {
            timeLeft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            // Interval ended - update and display FPS
            if (timeLeft <= 0.0)
            {
                float fps = accum / frames;
                timeLeft = updateInterval;
                accum = 0.0f;
                frames = 0;

                // Log FPS and memory usage
                long totalMem = Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024);
                Debug.Log($"[PerformanceMonitor] FPS: {fps:F2}, Memory: {totalMem} MB");
            }
        }
    }
}
