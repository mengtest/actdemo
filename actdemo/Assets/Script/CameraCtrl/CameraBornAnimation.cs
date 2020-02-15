using UnityEngine;
using System.Collections;

public class CameraBornAnimation : MonoBehaviour {

    public Vector3 initPos = new Vector3(-85.11255f, 10.0f, -81.3f);

    public Vector3 initEuler = new Vector3(23.21758f, 0, 0);

    public float initTime = 3.0f;

	IEnumerator Start () 
    {
        CameraMove cameraMode = GetComponent<CameraMove>();
        Component dof = GetComponent("DepthOfField34");
        if (cameraMode != null && dof != null)
        {
            Vector3 oldEuler = transform.eulerAngles;

            cameraMode.enabled = false;
            transform.position = initPos;
            transform.eulerAngles = initEuler;

            yield return new WaitForSeconds(initTime);

            cameraMode.enabled = true;
            cameraMode.lookAtTarget = true;

            while (true)
            {
                System.Reflection.FieldInfo f = dof.GetType().GetField("smoothness");
                f.SetValue(dof, ((float)f.GetValue(dof)) + 60.0f * Time.deltaTime);
                yield return null;
                if ((float)f.GetValue(dof) > 250.0f)
                {
                    Object.Destroy(dof);
                    break;
                }
            }
            cameraMode.lookAtTarget = false;
        }
	}
}
