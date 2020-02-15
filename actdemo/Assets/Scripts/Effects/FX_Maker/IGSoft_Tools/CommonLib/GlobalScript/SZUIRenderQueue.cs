using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SZUIRenderQueue : MonoBehaviour
{

    public int renderQueue = 3000;
    public bool runOnlyOnce = false;
    private Renderer thisRenderer = null;

    void Start()
    {
        Update();
    }

    void Update()
    {
        if (thisRenderer == null)
        {
            thisRenderer = this.GetComponent<Renderer>();
        }
        if (thisRenderer == null)
        {
            if (Application.isPlaying)
            {
                this.enabled = false;
            }
        }
        else
        {
            if (!thisRenderer.isVisible && Application.isPlaying)
            {
                return;
            }
            if (thisRenderer.sharedMaterial != null)
            {
                thisRenderer.sharedMaterial.renderQueue = renderQueue;
            }
            if (runOnlyOnce && Application.isPlaying)
            {
                this.enabled = false;
            }
        }
    }
}
