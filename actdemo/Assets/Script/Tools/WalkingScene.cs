using UnityEngine;
using System.Collections;

public class WalkingScene : MonoBehaviour
{
    public float speed = 10.0f;

	// Use this for initialization
	void Start () {
        GameObject obj = GameObject.Find("Main Camera");
        CameraMove cm = obj.GetComponent<CameraMove>();
        if (cm == null)
        {
            cm = obj.AddComponent<CameraMove>();
        }

        cm.SetRoleGameObject(gameObject);
	}
	
	// Update is called once per frame
	void Update ()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x.Equals(0.0f) && z.Equals(0.0f))
        {
            return;
        }

        Vector3 vec = new Vector3(x, 0.0f, z);
        gameObject.transform.Translate(vec.normalized * Time.deltaTime * speed);
    }
}
