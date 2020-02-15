using UnityEngine;
using System.Collections;

public class AnimatedVegetationAgent
{
    public Transform target = null;

    public Vector3 position;

    public Vector3 velocity;

    public float radius = 0.0f;

    public float radiusOverride = 0.0f;

    public float velocityFactor = 0.0f;

    public float velocityFactorOverride = 0.0f;

    public float volumeFactor = 0.0f;

    public float volumeFactorOverride = 0.0f;

    public void Init(Transform target, float radius, float velocityFactor, float volumeFactor)
    {
        this.target = target;
        this.position = target.position;
        this.velocity = Vector3.zero;
        this.radius = radius;
        this.velocityFactor = velocityFactor;
        this.volumeFactor = volumeFactor;
    }

    public float GetRadius()
    {
        return Mathf.Approximately(radiusOverride, 0.0f) ? radius : radiusOverride;
    }

    public float GetVelocityFactor()
    {
        return Mathf.Approximately(velocityFactorOverride, 0.0f) ? velocityFactor : velocityFactorOverride;
    }

    public float GetVolumeFactor()
    {
        return Mathf.Approximately(volumeFactorOverride, 0.0f) ? volumeFactor : volumeFactorOverride;
    }

    public void Update()
    {
        if(target == null)
        {
            return;
        }

        Vector3 position = target.position;
        this.velocity = (position - this.position) / Time.deltaTime;
        this.position = position;
    }
}
