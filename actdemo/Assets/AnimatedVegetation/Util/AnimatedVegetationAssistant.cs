using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AnimatedVegetationAssistant : MonoBehaviour 
{
    public static bool isGlobalWind = false;

	[HideInInspector]
	public bool autoDestroy = true;
	
	private void Start()
	{
		if (autoDestroy)
		{
			GameObject.DestroyImmediate(gameObject);
		}
	}
	
	#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if(transform == null)
		{
			return;
		}

        if (isGlobalWind)
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, Vector3.forward * 100);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector3.right * 100);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, Vector3.up * 100);
        }
        else
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.forward * 100);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.right * 100);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.up * 100);
        }
	}
	#endif
}
