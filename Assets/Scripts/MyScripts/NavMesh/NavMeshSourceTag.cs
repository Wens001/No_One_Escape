using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

// Tagging component for use with the LocalNavMeshBuilder
// Supports mesh-filter and terrain - can be extended to physics and/or primitives
[DefaultExecutionOrder(-200)]
public class NavMeshSourceTag : MonoBehaviour
{
    public static List<MeshFilter> m_Meshes = new List<MeshFilter>();
    public static List<NavMeshModifierVolume> VolumeModifiers = new List<NavMeshModifierVolume>();
    private MeshFilter[] mf;
    private NavMeshModifierVolume volumes;
    private void Awake()
    {
        mf = GetComponentsInChildren<MeshFilter>();
        TryGetComponent(out volumes);
    }

    void OnEnable()
    {
        if (mf != null)
        {
            for (int i = 0; i < mf.Length; i++)
                m_Meshes.Add(mf[i]);
        }
        
        if (volumes != null)
            VolumeModifiers.Add(volumes);
    }

    void OnDisable()
    {
        if (volumes != null)
            VolumeModifiers.Remove(volumes);
        if (mf != null)
        {
            for (int i = 0; i < mf.Length; i++)
                m_Meshes.Remove(mf[i]);
        }
    }

    public static void Collect(ref List<NavMeshBuildSource> sources)
    {
        sources.Clear();
        for (var i = 0; i < m_Meshes.Count; ++i)
        {
            var mf = m_Meshes[i];
            if (mf == null)
                continue;

            var m = mf.sharedMesh;
            if (m == null)
                continue;

            var s = new NavMeshBuildSource
            {
                shape = NavMeshBuildSourceShape.Mesh,
                sourceObject = m,
                transform = mf.transform.localToWorldMatrix,
                area = 0
            };
            sources.Add(s);
        }
    }

    //----------------------------------------------------------------------------------------
    public static void CollectModifierVolumes( ref List<NavMeshBuildSource> _sources)
    {
        foreach (var m in VolumeModifiers)
        {
            var mcenter = m.transform.TransformPoint(m.center);
            var scale = m.transform.lossyScale;
            var msize = new Vector3(m.size.x * Mathf.Abs(scale.x), m.size.y * Mathf.Abs(scale.y), m.size.z * Mathf.Abs(scale.z));
            
            var src = new NavMeshBuildSource
            {
                shape = NavMeshBuildSourceShape.ModifierBox,
                transform = Matrix4x4.TRS(mcenter, m.transform.rotation, Vector3.one),
                size = msize,
                area = m.area,
                
            };
            _sources.Add(src);
        }
    }
}
