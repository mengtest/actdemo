using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour
{
	public Transform mRole;
	private Transform mCamera;

	public float distance = 20.0f;

	public float height = 20.0f;

	public Vector3 offest = new Vector3(0, 1, -1);

	public float speed = 2.5f;

	public bool lookAtTarget = false;

	void Awake()
	{
        mCamera = transform;
        mCamera.transform.rotation = Quaternion.Euler(38.0f, 0.0f, 0.0f);
        mCamera.GetComponent<Camera>().fieldOfView = 55;
    }

    public void Reset()
    {
        mCamera = transform;
        mCamera.transform.rotation = Quaternion.Euler(38.0f, 0.0f, 0.0f);
        mCamera.GetComponent<Camera>().fieldOfView = 55;
    }

    public void SetRoleGameObject(GameObject go)
    {
        mRole = go.transform;
    }
	
	void LateUpdate () 
	{
		if (mRole == null)
		{
			if(GameObject.Find("Player") != null)
			{
				mRole = GameObject.Find("Player").transform;
			}
            return;
		}

		if (mRole)
		{
			Vector3 tagetVector = mRole.transform.position;
            
			offest = offest.normalized;

			Vector3 v = new Vector3(offest.x ,offest.y * height, offest.z * distance);

			mCamera.transform.position = Vector3.Lerp(mCamera.transform.position, v + tagetVector, speed * Time.deltaTime); 

			if (lookAtTarget)
			{
				mCamera.transform.LookAt(mRole);
			}
		}
	}
}