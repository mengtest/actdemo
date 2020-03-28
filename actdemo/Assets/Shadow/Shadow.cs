using UnityEngine;
using System.Collections;

public class Shadow : MonoBehaviour 
{
    public static Shadow instance = null;
    public Transform followTarget = null;
    public Vector3 offset;
    public Shader cullBack = null;
    public Shader doubleSide = null;
    public Color shadowColor = Color.black;
    public static int shadowmapSize = 512;
    // 永远是用双面，否则部分双面材质的角色阴影有问题
    private bool useDoubleSide = true;
    private Camera shadowLightCam = null;
    private Transform shadowLightTransform = null;
    private RenderTexture shadowMap = null;
    private bool m_isWorking = false;
    private bool applicationIsQuit = false;
    public bool isWorking
    {
        set
        {
            if (value != m_isWorking)
            {
                m_isWorking = value;
                if (!m_isWorking)
                {
                    ReleaseShadowMap();
                    Shader.SetGlobalTexture("_x_shadowMap", Texture2D.whiteTexture);
                }
            }
        }
        get
        {
            return m_isWorking;
        }
    }

    private void Start()
    {
        isWorking = true;
        instance = this;
        shadowLightCam = gameObject.GetComponent<Camera>();
        shadowLightTransform = shadowLightCam.transform; 
    }

    private void Update()
    {
        if (isWorking && cullBack != null && doubleSide != null)
        {
            if (CreateNewShadowMap())
            {
                Margin4RTInit();
                oldMarginInPixelSize = 0;
            }

            Margin4RT();

            Color shadowColor = this.shadowColor;

            Shader.SetGlobalColor("_shadowColor", shadowColor);
            Shader.SetGlobalTexture("_x_shadowMap", shadowMap == null ? (Texture)Texture2D.whiteTexture : shadowMap);
            //Shader.SetGlobalMatrix("_x_lightVP", shadowLight.projectionMatrix * shadowLight.worldToCameraMatrix);
            Shader.SetGlobalMatrix("_x_lightVP", originProjectMatrix * shadowLightCam.worldToCameraMatrix);
            
            // 区分单面还是双面
            //shadowLight.RenderWithShader(cullBack, "RenderType");
            //shadowLight.clearFlags = CameraClearFlags.Nothing;
            //shadowLight.RenderWithShader(doubleSide, "RenderType");
            //shadowLight.clearFlags = CameraClearFlags.SolidColor;

            shadowLightCam.RenderWithShader(useDoubleSide ? doubleSide : cullBack, null);
        }
    }

    private void LateUpdate()
    {
        if (followTarget == null)
        {
            return;
        }
        shadowLightTransform.position = followTarget.position + offset;
    }

    private bool CreateNewShadowMap()
    {
        if (shadowLightCam == null)
        {
            return false;
        }
        if (!isWorking)
        {
            return false;
        }

        bool createNewShadowMap = false;
        if (shadowMap == null)
        {
            createNewShadowMap = true;
        }
        else
        {
            if (shadowMap.width != shadowmapSize)
            {
                ReleaseShadowMap();
                createNewShadowMap = true;
            }
        }
        if (createNewShadowMap)
        {
            RenderTextureFormat format = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB565) ? RenderTextureFormat.RGB565 : RenderTextureFormat.Default;
            shadowMap = RenderTexture.GetTemporary(shadowmapSize, shadowmapSize, 0, format);
            shadowLightCam.targetTexture = shadowMap;
        }
        return createNewShadowMap;
    }

    private void ReleaseShadowMap()
    {
        if (shadowLightCam != null)
        {
            shadowLightCam.targetTexture = null;
        }
        if (shadowMap != null)
        {
            RenderTexture.ReleaseTemporary(shadowMap);
            shadowMap = null;
        }
    }

    private void OnApplicationQuit()
    {
        applicationIsQuit = true;
    }

    private void OnDestroy(){
        if (applicationIsQuit) return;
        ReleaseShadowMap();
        instance = null;

        Margin4RTDestroy();

        Shader.SetGlobalTexture("_x_shadowMap", Texture2D.whiteTexture);
    }


    // 为了优化Shader中不用去判断边界增加的代码。（2015.05.30 Chengang）
    // 方法是在RT中留出指定像素的边界
    public int marginInPixelSize = 2;
    private int oldMarginInPixelSize = 0;

    private Matrix4x4 originProjectMatrix;
    private float originSize;

    private GameObject clearCameraGO = null;
    Camera clearCam = null;

    private void Margin4RTInit()
    { 
        originProjectMatrix = GetComponent<Camera>().projectionMatrix;
        originSize = GetComponent<Camera>().orthographicSize;

        // 构建专门用于Clear的摄像机
        if (clearCameraGO == null)
        {
            clearCameraGO = new GameObject("ShadowLight Clear Camera", typeof(Camera));
            clearCam = clearCameraGO.GetComponent<Camera>();
        }
        if (clearCam != null)
        {
            clearCam.cullingMask = 0;
            clearCam.depth = GetComponent<Camera>().depth - 1;
            clearCam.backgroundColor = Color.white;
            clearCam.clearFlags = CameraClearFlags.SolidColor;
            clearCam.targetTexture = shadowMap;
            clearCam.enabled = false;
        }
        else
        {
            //Debug.LogError( "ClearCam is NOT found." );
        }
    }

    private void Margin4RTDestroy()
    {
        if (clearCameraGO != null)
            Destroy(clearCameraGO);
    }

    private void Margin4RT()
    {
        if (clearCam != null && shadowLightCam != null && shadowLightCam.orthographic && oldMarginInPixelSize != marginInPixelSize)
        {
            marginInPixelSize = Mathf.Max(0, marginInPixelSize);
            oldMarginInPixelSize = marginInPixelSize;
            
            //计算缩放率
            Vector2 deltaInNormal;
            if (shadowLightCam.targetTexture)
            {
                deltaInNormal = oldMarginInPixelSize * shadowLightCam.targetTexture.texelSize;
            }
            else
            {
                deltaInNormal = oldMarginInPixelSize * ( new Vector2( 1.0f / Screen.width, 1.0f / Screen.height ) );
            }
            Vector2 scale = Vector2.one - 2.0f * deltaInNormal;

            //Matrix4x4 scaledMatrix = Matrix4x4.identity;
            //scaledMatrix.SetRow(0, new Vector4(scale.x, 0, 0, 0));
            //scaledMatrix.SetRow(1, new Vector4(0, scale.y, 0, 0));

            // 修改投影变换
            shadowLightCam.orthographicSize = originSize * scale.y;
            //camera.projectionMatrix = scaledMatrix * camera.projectionMatrix;

            // 修改视口变换
            shadowLightCam.rect = new Rect(deltaInNormal.x, deltaInNormal.y, scale.x, scale.y);

            clearCam.Render();   
        }
    }
}
