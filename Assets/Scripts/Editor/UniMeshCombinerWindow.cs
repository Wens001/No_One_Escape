
/****************************************************
 * FileName:		UniMeshCombinerWindow.cs
 * CompanyName:		
 * Author:			
 * Email:			
 * CreateTime:		2020-05-20-01:58:13
 * Version:			1.0
 * UnityVersion:	2019.3.2f1
 * Description:		Nothing
 * 
*****************************************************/
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace UniMeshCombiner
{
    public class UniMeshCombinerWindow : EditorWindow
    {
        private GameObject _combineTarget = null;

        [MenuItem("MyTools/网格合并")]
        static void Open()
        {
            GetWindow<UniMeshCombinerWindow>("网格合并").Show();
        }

        void OnGUI()
        {
            _combineTarget = (GameObject)EditorGUILayout.ObjectField("合并目标", _combineTarget, typeof(GameObject), true);
            if (GUILayout.Button("合并"))
            {
                if (_combineTarget == null)
                {
                    return;
                }
                CombineMesh();
            }
        }

        private string newMeshPath;
        void CombineMesh()
        {
            newMeshPath = "";
            newMeshPath = EditorUtility.SaveFilePanelInProject(
                "保存新合并网格",
                "NewMesh",
                "asset",
                "输入文件名以保存新合并网格.");

            if (newMeshPath == string.Empty)
            {
                return;
            }


            var meshFilters = _combineTarget.GetComponentsInChildren<MeshFilter>();
            var combineMeshInstanceDictionary = new Dictionary<Material, List<CombineInstance>>();

            foreach (var meshFilter in meshFilters)
            {
                var mesh = meshFilter.sharedMesh;
                var vertices = new List<Vector3>();
                var materials = meshFilter.GetComponent<Renderer>().sharedMaterials;
                var subMeshCount = meshFilter.sharedMesh.subMeshCount;
                mesh.GetVertices(vertices);

                for (var i = 0; i < subMeshCount; i++)
                {
                    var material = materials[i];
                    var triangles = new List<int>();
                    mesh.GetTriangles(triangles, i);

                    var newMesh = new Mesh
                    {
                        vertices = vertices.ToArray(),
                        triangles = triangles.ToArray(),
                        uv = mesh.uv,
                        normals = mesh.normals
                    };

                    if (!combineMeshInstanceDictionary.ContainsKey(material))
                    {
                        combineMeshInstanceDictionary.Add(material, new List<CombineInstance>());
                    }

                    var combineInstance = new CombineInstance
                    { transform = meshFilter.transform.localToWorldMatrix, mesh = newMesh };
                    combineMeshInstanceDictionary[material].Add(combineInstance);
                }
            }

            _combineTarget.SetActive(false);

            foreach (var kvp in combineMeshInstanceDictionary)
            {
                var newObject = new GameObject(kvp.Key.name);

                var meshRenderer = newObject.AddComponent<MeshRenderer>();
                var meshFilter = newObject.AddComponent<MeshFilter>();

                meshRenderer.material = kvp.Key;
                var mesh = new Mesh();
                mesh.CombineMeshes(kvp.Value.ToArray());
                Unwrapping.GenerateSecondaryUVSet(mesh);

                meshFilter.sharedMesh = mesh;
                newObject.transform.parent = _combineTarget.transform.parent;

                ExportMesh(mesh, kvp.Key.name);
            }
        }

        void ExportMesh(Mesh mesh, string fileName)
        {
            if (Path.GetExtension(fileName) != ".asset")
            {
                fileName += ".asset";
            }
            AssetDatabase.CreateAsset(mesh, newMeshPath);
        }
    }
}