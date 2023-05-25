using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace UnityToolbarExtender
{
    [InitializeOnLoad]
    public static class RestartPlayMode
    {

        static RestartPlayMode()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }
        static void OnToolbarGUI()
        {
            EditorGUIUtility.SetIconSize(new Vector2(20,20));
            if (GUILayout.Button(new GUIContent((Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Scripts/Editor/CustomToolbar/Icons/LookDevResetEnv@2x.png", typeof(Texture2D))), ToolbarStyles.commandButtonStyle))
            {
                if (EditorApplication.isPlaying)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
            Time.timeScale = EditorGUILayout.Slider("", Time.timeScale, 0.01f, 20,GUILayout.Width(150));
        }
    }
}