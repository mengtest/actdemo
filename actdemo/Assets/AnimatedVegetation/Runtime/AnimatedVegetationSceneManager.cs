using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

public class AnimatedVegetationSceneManager : MonoBehaviour
{
#if UNITY_EDITOR
    public void FillSceneCache(AnimatedVegetationSceneCache sceneCache)
    {
        if (sceneCache == null)
        {
            return;
        }

        this.sceneCache = sceneCache;
        if (sceneCache.areas == null)
        {
            sceneCache.areas = new List<AnimatedVegetationSceneCache.Area>();
        }
        sceneCache.areas.Clear();

        List<GameObject> cacheList = new List<GameObject>();
        float minX = float.MaxValue;
        float minZ = float.MaxValue;

        GameObject[] gameObjectsInScene = GameObject.FindObjectsOfType<GameObject>();
        int numGameObjectsInScene = gameObjectsInScene == null ? 0 : gameObjectsInScene.Length;
        for (int i = 0; i < numGameObjectsInScene; ++i)
        {
            GameObject gameObjectInScene = gameObjectsInScene[i];
            if (gameObjectInScene != null && gameObjectInScene.transform != null)
            {
                Object prefab = PrefabUtility.GetPrefabObject(gameObjectInScene);
                if (prefab != null)
                {
                    string prefabPath = AssetDatabase.GetAssetPath(prefab).Replace('\\', '/');
                    if (prefabPath.StartsWith("Assets/AnimatedVegetation/Library/") && prefabPath.EndsWith(".prefab"))
                    {
                        cacheList.Add(gameObjectInScene);

                        Vector3 posInScene = gameObjectInScene.transform.position;
                        minX = Mathf.Min(posInScene.x, minX);
                        minZ = Mathf.Min(posInScene.z, minZ);
                    }
                }
            }
        }

        sceneCache.minX = minX;
        sceneCache.minZ = minZ;

        int numCacheItems = cacheList == null ? 0 : cacheList.Count;
        for (int i = 0; i < numCacheItems; ++i)
        {
            GameObject target = cacheList[i];
            if (target != null && target.transform != null)
            {
                Vector3 pos = target.transform.position;
                int row = 0;
                int col = 0;
                if (ConvertPositionToRowCol(pos, out row, out col))
                {
                    int rowcolid = EncodeRowcolid(row, col);
                    AnimatedVegetationSceneCache.Area area = GetArea(rowcolid);
                    if (area != null)
                    {
                        area.row = row;
                        area.col = col;
                    
                        MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
                        if(meshRenderer != null)
                        {
                            AnimatedVegetationSceneCache.Object obj = new AnimatedVegetationSceneCache.Object();
                            obj.transform = target.transform;
                            obj.meshRenderer = meshRenderer;
                            if (meshRenderer != null && meshRenderer.sharedMaterial != null)
                            {
                                obj.isForceEnabled = meshRenderer.sharedMaterial.IsKeywordEnabled("FORCE_ENABLED");
                            }
                            if (area.objects == null)
                            {
                                area.objects = new List<AnimatedVegetationSceneCache.Object>();
                            }
                            area.objects.Add(obj);
                        }
                    }
                }
            }
        }
    }
#endif

    [HideInInspector]
    [SerializeField]
    public AnimatedVegetationSceneCache sceneCache = null;

    public static AnimatedVegetationSceneManager instance = null;

    private List<AnimatedVegetationSceneCache.Object> objects = null;

    private List<AnimatedVegetationSceneCache.Area> areas = null;

    private List<AnimatedVegetationAgent> agents = null;

    private List<AnimatedVegetationEffect> effects = null;

    private AnimatedVegetationVirtualAgents m_virtualAgents = null;
    public AnimatedVegetationVirtualAgents virtualAgents
    {
        get
        {
            return m_virtualAgents;
        }
    }

    private int propertyID_force = 0;

    private AnimatedVegetationEffectObjectPoolManager effectObjectPoolManager = null;

    private void Awake()
    {
        instance = this;
        objects = new List<AnimatedVegetationSceneCache.Object>();
        areas = new List<AnimatedVegetationSceneCache.Area>();
        agents = new List<AnimatedVegetationAgent>();
        effects = new List<AnimatedVegetationEffect>();
        m_virtualAgents = new AnimatedVegetationVirtualAgents();
        propertyID_force = Shader.PropertyToID("_Force");
        effectObjectPoolManager = new AnimatedVegetationEffectObjectPoolManager();
    }

    private void OnDestroy()
    {
        if (objects != null)
        {
            objects.Clear();
            objects = null;
        }
        if (areas != null)
        {
            areas.Clear();
            areas = null;
        }
        if (agents != null)
        {
            agents.Clear();
            agents = null;
        }
        if (effects != null)
        {
            int numEffects = effects.Count;
            for (int i = 0; i < numEffects; ++i)
            {
                AnimatedVegetationEffect effect = effects[i];
                if (effect != null)
                {
                    effect.Destroy();
                }
            }
            effects.Clear();
            effects = null;
        }
        if (m_virtualAgents != null)
        {
            m_virtualAgents.Destroy();
            m_virtualAgents = null;
        }
        if (effectObjectPoolManager != null)
        {
            effectObjectPoolManager.Destroy();
            effectObjectPoolManager = null;
        }
        instance = null;
    }

    private void Update()
    {
        UpdateEffects();
        UpdateAgents();
        UpdateMaterials();
    }

    private void UpdateEffects()
    {
        if (effects == null)
        {
            return;
        }

        int numEffects = effects.Count;
        for (int i = 0; i < numEffects; ++i)
        {
            AnimatedVegetationEffect effect = effects[i];
            if (effect != null)
            {
                if (effect.IsComplete() || effect.markAsDestroy)
                {
                    effect.markAsDestroy = true;
                    effect.Destroy();
                    effects.RemoveAt(i);
                    if (effectObjectPoolManager != null)
                    {
                        var objectPool = effectObjectPoolManager.GetEffectObjectPool(effect.type);
                        if (objectPool != null)
                        {
                            objectPool.Release(effect); 
                        }
                    }
                    --i;
                    --numEffects;
                }
                else
                {
                    effect.Update();
                }
            }
        }
    }

    private void UpdateAgents()
    {
        if (agents == null)
        {
            return;
        }

        int numAgents = agents.Count;
        for (int i = 0; i < numAgents; ++i)
        {
            AnimatedVegetationAgent agent = agents[i];
            if (agent != null)
            {
                agent.Update();
                UpdateForces(agent.position, agent.velocity, agent.GetVelocityFactor(), agent.GetVolumeFactor(), agent.GetRadius());
            }
        }
    }

    private void UpdateMaterials()
    {
        if (objects == null)
        {
            return;
        }

        var list = objects;
        int length = list.Count;
        for (int i = 0; i < length; ++i)
        {
            var item = list[i];
            if (item != null && item.transform != null && item.meshRenderer != null)
            {
                if (item.data == null)
                {
                    item.data = new MaterialPropertyBlock();
                }

                if (item.force.sqrMagnitude < 0.001f)
                {
                    item.data.Clear();
                    item.meshRenderer.SetPropertyBlock(null);
                    list.RemoveAt(i);
                    --i;
                    --length;
                }
                else
                {
                    item.data.SetVector(propertyID_force, item.force);
                    item.meshRenderer.SetPropertyBlock(item.data);
                    item.force -= item.force * Mathf.Min(1.0f, Time.deltaTime * 5.0f);
                }
            }
        }
    }

#if UNITY_EDITOR
    [System.NonSerialized]
    public bool drawGizmos = false;

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            DrawGizmos();
        }
    }

    private void DrawGizmos()
    {
        if (sceneCache == null || sceneCache.areas == null)
        {
            return;
        }

        Gizmos.matrix = Matrix4x4.identity;

        Vector3 lineFrom = Vector3.zero;
        Vector3 lineTo = Vector3.zero;
        Vector3 cubeCenter = Vector3.zero;
        Vector3 cubeSize = new Vector3(sceneCache.cellSize, sceneCache.cellSize, sceneCache.cellSize);

        int numAreas = sceneCache.areas.Count;
        for (int i = 0; i < numAreas; ++i)
        {
            var area = sceneCache.areas[i];
            if (area != null)
            {
                bool isSeleced = false;
                if (area.objects != null)
                {
                    int numObjects = area.objects.Count;
                    for (int j = 0; j < numObjects; ++j)
                    {
                        var obj = area.objects[j];
                        if(obj != null)
                        {
                            foreach (var selectedGo in Selection.gameObjects)
                            {
                                if (selectedGo.transform == obj.transform)
                                {
                                    isSeleced = true;
                                    break;
                                }
                            }
                        }
                        if (isSeleced)
                        {
                            break;
                        }
                    }
                }

                Gizmos.color = isSeleced ? Color.green : Color.yellow;

                if (isSeleced)
                {
                    cubeCenter.x = sceneCache.minX + area.col * sceneCache.cellSize + sceneCache.cellSize * 0.5f;
                    cubeCenter.z = sceneCache.minZ + area.row * sceneCache.cellSize + sceneCache.cellSize * 0.5f;
                    Gizmos.DrawWireCube(cubeCenter, cubeSize);
                }
                else
                {
                    lineFrom.x = sceneCache.minX + area.col * sceneCache.cellSize;
                    lineFrom.z = sceneCache.minZ + area.row * sceneCache.cellSize;
                    lineTo.x = lineFrom.x + sceneCache.cellSize;
                    lineTo.z = lineFrom.z;
                    Gizmos.DrawLine(lineFrom, lineTo);

                    lineFrom.x = sceneCache.minX + area.col * sceneCache.cellSize;
                    lineFrom.z = sceneCache.minZ + area.row * sceneCache.cellSize;
                    lineTo.x = lineFrom.x;
                    lineTo.z = lineFrom.z + sceneCache.cellSize;
                    Gizmos.DrawLine(lineFrom, lineTo);

                    lineFrom.x = sceneCache.minX + area.col * sceneCache.cellSize + sceneCache.cellSize;
                    lineFrom.z = sceneCache.minZ + area.row * sceneCache.cellSize + sceneCache.cellSize;
                    lineTo.x = lineFrom.x - sceneCache.cellSize;
                    lineTo.z = lineFrom.z;
                    Gizmos.DrawLine(lineFrom, lineTo);

                    lineFrom.x = sceneCache.minX + area.col * sceneCache.cellSize + sceneCache.cellSize;
                    lineFrom.z = sceneCache.minZ + area.row * sceneCache.cellSize + sceneCache.cellSize;
                    lineTo.x = lineFrom.x;
                    lineTo.z = lineFrom.z - sceneCache.cellSize;
                    Gizmos.DrawLine(lineFrom, lineTo);
                }
            }
        }
    }
#endif

    public void AnimationTrigger(Transform target, int triggerId)
    {
        AnimatedVegetationXmlTriggerProperty trigger = AnimatedVegetationXml.GetInstance().GetTrigger(triggerId);
        if (trigger != null)
        {
            int[] effectIds = trigger.effectIds;
            int numEffectIds = effectIds == null ? 0 : effectIds.Length;
            for (int i = 0; i < numEffectIds; ++i)
            {
                AnimatedVegetationXmlEffectProperty aveEffectProperty = AnimatedVegetationXml.GetInstance().GetEffect(effectIds[i]);
                if (aveEffectProperty != null && aveEffectProperty.effect != null)
                {
                    AnimatedVegetationEffect effect = AnimatedVegetationSceneManager.instance.NewAgentEffect(aveEffectProperty.effect.type);
                    aveEffectProperty.effect.InitTarget(effect);
                    AnimatedVegetationSceneManager.instance.AddAgentEffect(target, effect);
                }
            }
        }
    }

    public AnimatedVegetationAgent AddAgent(Transform target, float radius, float velocityFactor, float volumeFactor)
    {
        if (target == null)
        {
            return null;
        }

        AnimatedVegetationAgent agent = new AnimatedVegetationAgent();
        agent.Init(target, radius, velocityFactor, volumeFactor);
        agents.Add(agent);

        return agent;
    }

    public void RemoveAgent(Transform target)
    {
        if (target == null)
        {
            return;
        }

        int numAgents = agents == null ? 0 : agents.Count;
        for (int i = 0; i < numAgents; ++i)
        {
            AnimatedVegetationAgent agent = agents[i];
            if (agent != null && agent.target == target)
            {
                agents.RemoveAt(i);
                RemoveAgentEffects(agent.target);
                break;
            }
        }
    }

    public AnimatedVegetationEffect NewAgentEffect(AnimatedVegetationEffectType effectType)
    {
        if (effectObjectPoolManager == null)
        {
            return null;
        }

        var objectPool = effectObjectPoolManager.GetEffectObjectPool(effectType);
        if (objectPool == null)
        {
            return null;
        }

        return objectPool.Get();
    }

    public void AddAgentEffect(Transform target, AnimatedVegetationEffect effect)
    {
        if (effect == null || effects == null)
        {
            return;
        }

        effect.reference = target;
        if (effect.isSpecificTarget)
        {
            if (target == null)
            {
                return;
            }

            int numAgents = agents == null ? 0 : agents.Count;
            for (int i = 0; i < numAgents; ++i)
            {
                AnimatedVegetationAgent agent = agents[i];
                if (agent != null && agent.target == target)
                {
                    effect.agent = agent;
                    effects.Add(effect);
                    break;
                }
            }
        }
        else
        {
            effects.Add(effect);
        }
    }

    public void RemoveAgentEffect(AnimatedVegetationEffect effect)
    {
        if (effects == null || effect == null)
        {
            return;
        }

        int numEffects = effects == null ? 0 : effects.Count;
        for (int i = 0; i < numEffects; ++i)
        {
            if (effects[i] == effect)
            {
                effect.markAsDestroy = true;
                break;
            }
        }
    }

    public void RemoveAgentEffects(Transform target)
    {
        if (target == null || effects == null)
        {
            return;
        }

        int numEffects = effects == null ? 0 : effects.Count;
        for (int i = 0; i < numEffects; ++i)
        {
            AnimatedVegetationEffect effect = effects[i];
            if (effect != null && effect.agent != null && effect.agent.target == target)
            {
                effect.markAsDestroy = true;
            }
        }
    }

    private void UpdateForces(Vector3 position, Vector3 velocity, float velocityFactor, float volumeFactor, float radius)
    {
        if(sceneCache == null || this.objects == null || areas == null)
        {
            return;
        }

        int expandedCircles = Mathf.CeilToInt(radius / sceneCache.cellSize);
        velocity.y = 0;
        position.y = 0;

        areas.Clear();
        GetAreas(position, expandedCircles, ref areas);
        int numAreas = areas == null ? 0 : areas.Count;
        for (int i = 0; i < numAreas; ++i)
        {
            var area = areas[i];
            if (area != null)
            {
                var objects = area.objects;
                if (objects != null)
                {
                    int numObjects = objects == null ? 0 : objects.Count;
                    for (int j = 0; j < numObjects; ++j)
                    {
                        var obj = objects[j];
                        if (obj != null && obj.transform != null && obj.isForceEnabled)
                        {
                            Vector3 objPosition = obj.transform.position;
                            objPosition.y = 0;
                            float dist = Vector3.Distance(position, objPosition); 
                            if (dist < radius)
                            {
                                obj.force += velocity * Time.deltaTime * velocityFactor;
                                obj.force += (objPosition - position).normalized * Time.deltaTime * volumeFactor;
                                obj.force.y = 0;
                                if (!this.objects.Contains(obj))
                                {
                                    this.objects.Add(obj);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void GetAreas(Vector3 position, int expandedCircles, ref List<AnimatedVegetationSceneCache.Area> o_areas)
    {
        if (o_areas == null)
        {
            return;
        }

        if(sceneCache == null)
        {
            return;
        }
        
        if(expandedCircles == 0)
        {
            AnimatedVegetationSceneCache.Area area = GetArea(position);
            if(area != null)
            {
                o_areas.Add(area);
            }
            return;
        }

        expandedCircles = Mathf.Max(expandedCircles, 0);

        float xFrom = position.x - sceneCache.cellSize * expandedCircles;
        float xTo = position.x + sceneCache.cellSize * expandedCircles + 0.00001f;
        float zFrom = position.z - sceneCache.cellSize * expandedCircles;
        float zTo = position.z + sceneCache.cellSize * expandedCircles + 0.00001f;

        Vector3 pos = Vector3.zero;
        for (float x = xFrom; x < xTo; x += sceneCache.cellSize)
        {
            for (float z = zFrom; z < zTo; z += sceneCache.cellSize)
            {
                pos.x = x;
                pos.z = z;
                AnimatedVegetationSceneCache.Area area = GetArea(pos);
                if (area != null)
                {
                    o_areas.Add(area);
                }
            }
        }
    }

    public AnimatedVegetationSceneCache.Area GetArea(Vector3 position)
    {
        int row = 0;
        int col = 0;
        if (!ConvertPositionToRowCol(position, out row, out col))
        {
            return null;
        }
        int rowcolid = EncodeRowcolid(row, col);
        return GetArea(rowcolid);
    }

    public AnimatedVegetationSceneCache.Area GetArea(int rowcolid)
    {
        if (sceneCache == null)
        {
            return null;
        }

        List<AnimatedVegetationSceneCache.Area> areas = sceneCache.areas;
        if (areas == null)
        {
            return null;
        }

        int numAreas = areas.Count;
        for (int i = 0; i < numAreas; ++i)
        {
            AnimatedVegetationSceneCache.Area area = areas[i];
            if (area != null && area.rowcolid == rowcolid)
            {
                return area;
            }
        }

        AnimatedVegetationSceneCache.Area newArea = new AnimatedVegetationSceneCache.Area();
        newArea.rowcolid = rowcolid;
        areas.Add(newArea);

        return newArea;
    }

    public bool ConvertPositionToRowCol(Vector3 position, out int row, out int col)
    {
        row = -1;
        col = -1;

        if (sceneCache == null)
        {
            return false;
        }

        if (position.x < sceneCache.minX || position.z < sceneCache.minZ)
        {
            return false;
        }

        position.x -= sceneCache.minX;
        position.z -= sceneCache.minZ;

        row = (int)(position.z / sceneCache.cellSize);
        col = (int)(position.x / sceneCache.cellSize);

        if (!IsLegalRowCol(row) || !IsLegalRowCol(col))
        {
            row = -1;
            col = -1;
            return false;
        }

        return true;
    }

    public bool IsLegalRowCol(int rowORcol)
    {
        return rowORcol <= 9999 && rowORcol >= 0;
    }

    public int EncodeRowcolid(int row, int col)
    {
        return row * 10000 + col;
    }

    public void DecodeRowcolid(int rowcolid, out int row, out int col)
    {
        row = rowcolid / 10000;
        col = rowcolid - row * 10000;
    }
}
