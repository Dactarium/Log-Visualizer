using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogVisualizer.Scripts
{
    public class LogVisualizerSettings : ScriptableObject
    {
        [field: SerializeField]
        public bool ClearOnValidate { get; private set; } = true;

        [field: SerializeField]
        public bool ClearOnEnterPlayMode { get; private set; } = true;

        [field: SerializeField]
        public bool ClearOnExitPlayMode { get; private set; } = true;
        
        [field: SerializeField, Tooltip("Every X seconds reduces log count")]
        public float CalmCooldown { get; private set; }

        [field: SerializeField]
        public List<LogType> LogTypes { get; private set; } = new List<LogType>() { LogType.Error };
        
        [field: SerializeField]
        public LogVisual[] LogVisuals { get; private set; }
        
        
    }
    [Serializable]
    public struct LogVisual
    {
        public int threshold;
        public Sprite sprite;
    }
}