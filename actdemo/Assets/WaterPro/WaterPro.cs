using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class WaterPro : MonoBehaviour 
{
    public enum ReflectionTextueSize
    {
        Size_128 = 128, 
        Size_256 = 256, 
        Size_512 = 512,
        Size_1024 = 1024
    }

    [Tooltip("水波速度")]
    public float waveSpeed = 1;

    /// <summary>
    /// 偏移累计量
    /// </summary>
    private float waveOffset = 0;
	
    /// <summary>
    /// 水面材质球
    /// </summary>
    private Material waterMaterial = null;

    [Tooltip("开启实时反射")]
    public bool reflectionEnabled = true;

    [Tooltip("反射贴图尺寸")]
    public ReflectionTextueSize reflectionTextureSize = ReflectionTextueSize.Size_256;

    [Tooltip("反射平面偏移")]
    public float reflectionClipPlaneOffset = 0.07f;

    [Tooltip("反射层")]
    public LayerMask m_ReflectLayers = -1;

    // 水面实时反射
    private Dictionary<Camera, Camera> m_ReflectionCameras = null; // Camera -> Camera table
    private RenderTexture m_ReflectionTexture = null;
    private int m_OldReflectionTextureSize = 0;
    private bool s_InsideWater = false;

    [Tooltip("高光范围")]
    [Range(0, 10)]
    public float specRange = 1;

    [Tooltip("高光强度")]
    [Range(0, 100)]
    public float specStrength = 1;

    [Tooltip("太阳光方向")]
    public Vector3 sunLightDir = Vector3.one;

    [Tooltip("接受实时阴影")]
    public bool shadowLightEnabled = true;

    private void OnDestroy()
    {
        waterMaterial = null;
        DestroyReflectionTexture();
        DestroyReflectionCamera();
    }
    
    private void DestroyReflectionTexture()
    {
        if (m_ReflectionTexture != null)
        {
#if UNITY_EDITOR
            RenderTexture.DestroyImmediate(m_ReflectionTexture);
#else
            RenderTexture.Destroy(m_ReflectionTexture);
#endif
            m_ReflectionTexture = null;
        }
    }

    private void DestroyReflectionCamera()
    {
        if (m_ReflectionCameras != null)
        {
            foreach (var camera in m_ReflectionCameras.Values)
            {
#if UNITY_EDITOR
                GameObject.DestroyImmediate(camera.gameObject);
#else
                GameObject.Destroy(camera.gameObject);
#endif
            }
            m_ReflectionCameras.Clear();
            m_ReflectionCameras = null;
        }
    }

	private void Update()
	{
        if (!reflectionEnabled)
        {
            DestroyReflectionTexture();
            DestroyReflectionCamera();
        }

        RefreshMaterialMultiCompileKeywords();

        waveOffset += Time.deltaTime * waveSpeed;
        Material waterMaterial = GetWaterMaterial();
        if (waterMaterial != null)
        {
            waterMaterial.SetFloat("_WaveOffset", waveOffset);
            waterMaterial.SetVector("_SunLightDir", sunLightDir);
            waterMaterial.SetFloat("_WaterSpecRange", specRange);
            waterMaterial.SetFloat("_WaterSpecStrength", specStrength);
        }
	}

    private Material GetWaterMaterial()
    {
        if (waterMaterial == null)
        {
            Renderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                if (Application.isPlaying)
                {
                    waterMaterial = renderer.material;
                }
                else
                {
                    waterMaterial = renderer.sharedMaterial;
                }
            }
        }
        return waterMaterial;
    }

    private void RefreshMaterialMultiCompileKeywords()
    {
        Material waterMaterial = GetWaterMaterial();
        if (waterMaterial != null)
        {
            if (shadowLightEnabled)
            {
                waterMaterial.EnableKeyword("SHADOW_LIGHT_ON");
            }
            else
            {
                waterMaterial.DisableKeyword("SHADOW_LIGHT_ON");
            }

            if (reflectionEnabled)
            {
                waterMaterial.EnableKeyword("REFLECTION_ON");
            }
            else
            {
                waterMaterial.DisableKeyword("REFLECTION_ON");
            }
        }
    }

    public void OnWillRenderObject()
    {
        if (!reflectionEnabled)
        {
            return;
        }

        if (!enabled || !GetComponent<Renderer>() || !GetComponent<Renderer>().sharedMaterial || !GetComponent<Renderer>().enabled)
            return;

        Camera cam = Camera.current;
        if (!cam)
            return;

        // Safeguard from recursive water reflections.		
        if (s_InsideWater)
            return;
        s_InsideWater = true;

        Camera reflectionCamera;
        CreateWaterObjects(cam, out reflectionCamera);

        // find out the reflection plane: position and normal in world space
        Vector3 pos = transform.position;
        Vector3 normal = transform.up;

        Material waterMaterial = GetWaterMaterial();
        if (waterMaterial == null)
        {
            return;
        }

        UpdateCameraModes(cam, reflectionCamera);

        {
            // Reflect camera around reflection plane
            float d = -Vector3.Dot(normal, pos) - reflectionClipPlaneOffset;
            Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            Matrix4x4 reflection = Matrix4x4.zero;
            CalculateReflectionMatrix(ref reflection, reflectionPlane);
            Vector3 oldpos = cam.transform.position;
            Vector3 newpos = reflection.MultiplyPoint(oldpos);
            reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

            // Setup oblique projection matrix so that near plane is our reflection
            // plane. This way we clip everything below/above it for free.
            Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
            reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

            reflectionCamera.cullingMask = ~(1 << 4) & m_ReflectLayers.value; // never render water layer
            reflectionCamera.targetTexture = m_ReflectionTexture;
            GL.invertCulling = true;
            reflectionCamera.transform.position = newpos;
            Vector3 euler = cam.transform.eulerAngles;
            reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
            reflectionCamera.Render();
            reflectionCamera.transform.position = oldpos;
            GL.invertCulling = false;
            waterMaterial.SetTexture("_ReflectionTex", m_ReflectionTexture);
        }

        s_InsideWater = false;
    }

    private void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera)
    {
        reflectionCamera = null;

        int reflectionTextureSizeValue = (int)reflectionTextureSize;
        // Reflection render texture
        if (!m_ReflectionTexture || m_OldReflectionTextureSize != reflectionTextureSizeValue)
        {
            if (m_ReflectionTexture)
                DestroyImmediate(m_ReflectionTexture);
            m_ReflectionTexture = new RenderTexture(reflectionTextureSizeValue, reflectionTextureSizeValue, 16);
            m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();
            m_ReflectionTexture.isPowerOfTwo = true;
            m_ReflectionTexture.hideFlags = HideFlags.DontSave;
            m_OldReflectionTextureSize = reflectionTextureSizeValue;
        }

        // Camera for reflection
        if (m_ReflectionCameras == null)
        {
            m_ReflectionCameras = new Dictionary<Camera, Camera>();
        }
        m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
        if (!reflectionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
        {
            GameObject go = new GameObject("Water Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
            reflectionCamera = go.GetComponent<Camera>();
            reflectionCamera.enabled = false;
            reflectionCamera.transform.position = transform.position;
            reflectionCamera.transform.rotation = transform.rotation;
            reflectionCamera.gameObject.AddComponent<FlareLayer>();
            go.hideFlags = HideFlags.HideAndDontSave;
            m_ReflectionCameras[currentCamera] = reflectionCamera;
        }
    }

    private void UpdateCameraModes(Camera src, Camera dest)
    {
        if (dest == null)
            return;
        // set water camera to clear the same way as current camera
        dest.clearFlags = CameraClearFlags.Color;
        dest.backgroundColor = Color.white;
        if (src.clearFlags == CameraClearFlags.Skybox)
        {
            Skybox sky = src.GetComponent(typeof(Skybox)) as Skybox;
            Skybox mysky = dest.GetComponent(typeof(Skybox)) as Skybox;
            if (!sky || !sky.material)
            {
                mysky.enabled = false;
            }
            else
            {
                mysky.enabled = true;
                mysky.material = sky.material;
            }
        }
        // update other values to match current camera.
        // even if we are supplying custom camera&projection matrices,
        // some of values are used elsewhere (e.g. skybox uses far plane)
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
    }

    // Given position/normal of the plane, calculates plane in camera space.
    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * reflectionClipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    // Calculates reflection matrix around the given plane
    private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }
}
