using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour 
{
	private void Start () 
    {
        AnimatedVegetationXml.GetInstance().Init();
        if (AnimatedVegetationSceneManager.instance != null)
        {
            AnimatedVegetationSceneManager.instance.AddAgent(transform, 1f, 4f, 10f);
        }
	}
	
    private void OnDestroy()
    {
        if (AnimatedVegetationSceneManager.instance != null)
        {
            AnimatedVegetationSceneManager.instance.RemoveAgent(transform);
        }
    }

    private void OnGUI()
    {
        Color c = GUI.color;
        GUI.color = Color.red;
        GUILayout.Label("Move: A S D W");
        GUILayout.Label("Effect: U I O P");
        GUI.color = c;
    }

	private void Update () 
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            AnimatedVegetationSceneManager.instance.AnimationTrigger(transform, 10002);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            AnimatedVegetationSceneManager.instance.AnimationTrigger(transform, 20002);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            AnimatedVegetationSceneManager.instance.AnimationTrigger(transform, 30002);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            AnimatedVegetationSceneManager.instance.AnimationTrigger(transform, 40002);
        }
	}
}
