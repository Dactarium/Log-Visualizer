using System;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LogVisualizer.Scripts.Editor
{
    public class LogVisualizerWindow : EditorWindow
    {
        public static LogVisualizerWindow Window
        {
            get
            {
                if(!m_Window)
                    m_Window = GetWindow<LogVisualizerWindow>();

                return m_Window;
            }

            set => m_Window = value;
        }
        
        private static LogVisualizerWindow m_Window;
        
        private static bool              m_Init;
        private static LogVisualizerSettings Settings => Window.m_Settings;

        private LogVisualizerSettings m_Settings;

        private Sprite m_CurrentSprite;
        
        public int LogCount
        {
            get => m_LogCount;

            set
            {
                m_LogCount = value;
                
                if(!Window || !Settings || Settings.LogVisuals == null || Settings.LogVisuals.Length == 0)
                    return;

                int selection = 0;
                m_CurrentSprite = m_Settings.LogVisuals[0].sprite;
                
                foreach (LogVisual visual in Settings.LogVisuals){

                    if(LogCount > selection)
                        m_CurrentSprite = visual.sprite;
                    else
                        break;

                    selection += visual.threshold;
                }

                
                if(m_CurrentSprite != null)
                    Window.Repaint();
            }
        }
        
        private int m_LogCount;

        private EditorCoroutine m_CalmCoroutine;
        
        [MenuItem("Tools/LogVisualizer/Open")]
        public static void ShowWindow(){
            
            Window = GetWindow<LogVisualizerWindow>();
            Window.titleContent = new GUIContent("Log Visualizer");
            
            if(!Settings){
                string[] result = AssetDatabase.FindAssets("t:LogVisualizerSettings");

                if(result is { Length: > 0 }){
                    Window.m_Settings = AssetDatabase.LoadAssetAtPath<LogVisualizerSettings>(AssetDatabase.GUIDToAssetPath(result[0]));
                }
                else{
                    Window.m_Settings = CreateInstance<LogVisualizerSettings>();
                    AssetDatabase.CreateAsset(Settings, "Assets/LogVisualizerSettings.asset");
                }
            }
            
            Window.m_LogCount = 0;
            
            Window.Show();
        }

        [MenuItem("Tools/LogVisualizer/Refresh")]
        public static void RefreshWindow(){

            if(Window){
                Window.Close();
            }
            ShowWindow();
        }

        private void OnFocus(){
            if(Window != null)
                return;

            Window = GetWindow<LogVisualizerWindow>();
        }

        public void CreateGUI(){
            if(!Settings || Settings.LogVisuals == null || Settings.LogVisuals.Length == 0)
                return;

            rootVisualElement.style.width = new Length(100, LengthUnit.Percent);
            rootVisualElement.style.height = new Length(100, LengthUnit.Percent);
            rootVisualElement.style.backgroundImage = new StyleBackground(m_CurrentSprite);
            rootVisualElement.style.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;
        }

        private void OnGUI(){
            Window.rootVisualElement.style.width = new Length(100, LengthUnit.Percent);
            Window.rootVisualElement.style.height = new Length(100, LengthUnit.Percent);
            Window.rootVisualElement.style.backgroundImage = new StyleBackground(m_CurrentSprite);
            Window.rootVisualElement.style.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;
        }

        private void OnEnable(){
            Application.logMessageReceived += HandleLog;
            EditorApplication.playModeStateChanged += OnStateChange;
        }

        private void OnDisable(){
            Application.logMessageReceived -= HandleLog;
            EditorApplication.playModeStateChanged -= OnStateChange;
            
            if(m_CalmCoroutine != null)
                EditorCoroutineUtility.StopCoroutine(m_CalmCoroutine);
        }

        private void OnValidate(){
            if(m_Settings && m_Settings.ClearOnValidate)
                LogCount = 0;
        }

        private void OnStateChange(PlayModeStateChange playModeStateChange){
            if(playModeStateChange == PlayModeStateChange.EnteredPlayMode && Window &&Settings && Settings.ClearOnEnterPlayMode){
                LogCount = 0;
            }
            
            if(playModeStateChange == PlayModeStateChange.ExitingPlayMode && Window &&Settings && Settings.ClearOnExitPlayMode){
                LogCount = 0;
            }
        }

        private void HandleLog(string logString, string stackTrace, LogType type){

            if(!Window || !Settings || !Settings.LogTypes.Contains(type))
                return;
            
            LogCount++;
            StartCalm();
        }
        
        private void StartCalm(){
            if(m_CalmCoroutine != null)
                EditorCoroutineUtility.StopCoroutine(m_CalmCoroutine);
            
            m_CalmCoroutine = EditorCoroutineUtility.StartCoroutine(_Calm(), Window);;
            
            IEnumerator _Calm(){
                while (LogCount > 0){
                    yield return new EditorWaitForSeconds(Settings.CalmCooldown);
                    LogCount--;
                }
                if(LogCount != 0) LogCount = 0;
                m_CalmCoroutine = null;
            }
        }
    }
}