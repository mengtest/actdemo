using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour 
{
	public Transform mTarget;
	private Camera mCamera;

	void Awake()
	{
		mCamera = GetComponent<Camera> ();
	}

	void Update()
	{
		if (mCamera == null || mTarget == null)
		{
			return;
		}

		mCamera.transform.LookAt (mTarget.position);
	}
}