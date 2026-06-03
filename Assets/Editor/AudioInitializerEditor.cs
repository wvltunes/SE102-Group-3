#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioInitializer))]
public class AudioInitializerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AudioInitializer script = (AudioInitializer)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Tự Động Tìm & Cắt Đoạn Im Lặng (Auto-detect Silence)", GUILayout.Height(30)))
        {
            SerializedProperty clipProp = serializedObject.FindProperty("audioClip");
            AudioClip clip = clipProp.objectReferenceValue as AudioClip;
            
            if (clip != null)
            {
                float[] samples = new float[clip.samples * clip.channels];
                clip.GetData(samples, 0);
                
                int firstSample = 0;
                for (int i = 0; i < samples.Length; i++)
                {
                    if (Mathf.Abs(samples[i]) > 0.05f) // Threshold
                    {
                        firstSample = i / clip.channels;
                        break;
                    }
                }
                
                float skipTime = (float)firstSample / clip.frequency;
                
                SerializedProperty skipProp = serializedObject.FindProperty("skipSilenceTime");
                if (skipProp != null)
                {
                    skipProp.floatValue = skipTime;
                    serializedObject.ApplyModifiedProperties();
                    Debug.Log($"[AudioInitializer] Tìm thấy nhạc bắt đầu ở giây thứ {skipTime:F3}. Đã tự động điền vào Skip Silence Time!");
                }
            }
            else
            {
                Debug.LogWarning("Vui lòng gán AudioClip trước khi tự động tìm!");
            }
        }
    }
}
#endif
