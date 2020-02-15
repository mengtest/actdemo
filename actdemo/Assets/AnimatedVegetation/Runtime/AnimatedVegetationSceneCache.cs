using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatedVegetationSceneCache : MonoBehaviour 
{
    [SerializeField]
    [HideInInspector]
    public float cellSize = 2.0f;

    [SerializeField]
    [HideInInspector]
    public float minX = float.MaxValue;

    [SerializeField]
    [HideInInspector]
    public float minZ = float.MaxValue;

    [SerializeField]
    [HideInInspector]
    public List<Area> areas = null;

    [System.Serializable]
    public class Area
    {
        [SerializeField]
        [HideInInspector]
        public int rowcolid = 0;

        [SerializeField]
        public int row = 0;

        [SerializeField]
        public int col = 0;

        [SerializeField]
        public List<Object> objects = null;
    }

    [System.Serializable]
    public class Object
    {
        [SerializeField]
        public Transform transform = null;

        [SerializeField]
        public MeshRenderer meshRenderer = null;

        [SerializeField]
        public bool isForceEnabled = true;

        [System.NonSerialized]
        public MaterialPropertyBlock data = null;

        [System.NonSerialized]
        public Vector3 force;
    }
}
