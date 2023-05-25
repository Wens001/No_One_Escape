
using UnityEditor;
using UnityEngine;

namespace MGS.CubemapRenderer
{
    public class CubemapRenderer : ScriptableWizard
    {
        #region Field and Property
        [Tooltip("要渲染立方体贴图的源摄影机.")]
        public Camera renderCamera;

        [Tooltip("立方体面的宽度和高度（像素）.")]
        public int faceSize = 128;

        [Tooltip("用于立方体映射的像素数据格式.")]
        public TextureFormat textureFormat = TextureFormat.ARGB32;

        [Tooltip("是否应创建mipmap?")]
        public bool mipmap = false;
        #endregion

        #region Private Method
        [MenuItem("MyTools/渲染Cubemap")]
        private static void ShowEditor()
        {
            DisplayWizard("渲染Cubemap", typeof(CubemapRenderer), "Render");
        }

        private void OnEnable()
        {
            renderCamera = Camera.main;
        }

        private void OnWizardUpdate()
        {
            if (renderCamera && faceSize > 0)
            {
                isValid = true;
            }
            else
            {
                isValid = false;
            }
        }

        private void OnWizardCreate()
        {
            var newCubemapPath = EditorUtility.SaveFilePanelInProject(
                "Save New Render Cubemap",
                "NewRenderCubemap",
                "cubemap",
                "输入文件名以保存新的渲染立方体映射.");

            if (newCubemapPath == string.Empty)
            {
                return;
            }

            var newRenderCubemap = new Cubemap(faceSize, textureFormat, mipmap);
            renderCamera.RenderToCubemap(newRenderCubemap);

            AssetDatabase.CreateAsset(newRenderCubemap, newCubemapPath);
            AssetDatabase.Refresh();
            Selection.activeObject = newRenderCubemap;
        }
        #endregion
    }
}