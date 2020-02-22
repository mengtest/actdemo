using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerCtrlManager : MonoBehaviour
{
	AnimatorComponent m_AnimatorManager;

	private float speed;

	private float direction;

	private VCAnalogJoystickBase mJoy;

    private Transform mDirection;

    private RaycastHit hit;
    private Ray ray;
    private bool suc;

    private Transform m_Cam;

    private Vector3 m_CamForward;             // The current forward direction of the camera
    private Vector3 m_Move;

    void Start()
    {
        // get the transform of the main camera
        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }
    }

	public void JoyCtrl()
	{
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");

        if (m_Cam != null)
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v * m_CamForward + h * m_Cam.right;
            Debug.Log(m_Move);
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            m_Move = v * Vector3.forward + h * Vector3.right;
        }

        if (m_AnimatorManager == null)
		{
			if(this.gameObject.GetComponent<AnimatorComponent>())
			{
				m_AnimatorManager = this.gameObject.GetComponent<AnimatorComponent>();
			}
			else
			{
				Debug.LogError("this.gameObject  AnimatorManager == null");
				return;
			}
		}

        Debug.Log(m_Move);
        Debug.Log("h:" + h + "v:" + v);

        if (h != 0.0f || v != 0.0f)
        {
            float degree = 180.0f / Mathf.PI * Mathf.Atan2(m_Move.z, m_Move.x);// + 180.0f;
            PlayMove(m_Move.z, m_Move.x, degree);
        }
        else
        {
            m_AnimatorManager.Stand();
        }

		//if (mJoy == null)
		//{
  //          mJoy = VCAnalogJoystickBase.GetInstance("JoyStick");

		//	return;
		//}

		//if (mJoy.AxisX != 0.0f || mJoy.AxisY != 0.0f)
		//{

		//	Vector3 v = new Vector3(mJoy.AxisX , 0,mJoy.AxisY);

		//	if(v.magnitude < 0.2f)
		//	{
		//		return;
		//	}
			

		//	PlayMove(mJoy.AxisX, mJoy.AxisY, mJoy.AngleDegrees);

  //          if (mDirection != null)
  //          {
  //              mDirection.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, mJoy.AngleDegrees));
  //          }
  //      }
		//if ((Input.GetAxis("Vertical") != 0.0f || Input.GetAxis("Horizontal") != 0.0f))
		//{
		//	float	degree = 180.0f / Mathf.PI * Mathf.Atan2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));// + 180.0f;
		//	PlayMove(Input.GetAxis("Horizontal") , Input.GetAxis("Vertical"),degree);
		//}
		////else   if ( Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Horizontal") == 0.0f) 
		//else
		//{
		//	speed = 0;
		//	m_AnimatorManager.Stand();
  //      }


        if (Input.GetKeyDown(KeyCode.O))
        {
            gameObject.GetComponent<SkillComponent>().DoSkill(600009);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
		{
            gameObject.GetComponent<SkillComponent>().DoSkill(600005);
		}
		else if(Input.GetKeyUp(KeyCode.Alpha1))
		{
			PlayAttackSkillEnd();
		}

		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
            gameObject.GetComponent<SkillComponent>().DoSkill(600006);
		}
		else if(Input.GetKeyUp(KeyCode.Alpha2))
		{
			PlayAttackSkillEnd();
		}

		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
            gameObject.GetComponent<SkillComponent>().DoSkill(600007);
        }
		else if(Input.GetKeyUp(KeyCode.Alpha3))
		{
			PlayAttackSkillEnd();
		}


		if(Input.GetKeyDown(KeyCode.Alpha4))
		{
            gameObject.GetComponent<SkillComponent>().DoSkill(600008);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            PlayAttackSkillEnd();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			//m_AnimatorManager.BlowupAnim(true);
		}
		else if(Input.GetKeyUp(KeyCode.Alpha5))
		{
			//m_AnimatorManager.BlowupAnim(false);
		}

		if(Input.GetKeyDown(KeyCode.Alpha6))
		{
		}

		if(Input.GetKeyDown(KeyCode.Alpha7))
		{
		}

		if(Input.GetKeyDown(KeyCode.Alpha8))
		{
		}

		if(Input.GetKeyDown(KeyCode.Alpha9))
		{
			m_AnimatorManager.OpenAnim();
		}

		if(Input.GetKeyDown(KeyCode.Alpha0))
		{
			m_AnimatorManager.DeathAnim();
		}
	}

	void PlayMove(float x, float y , float degrees)
    {
        if (Mathf.Approximately(x, 0.0f) && Mathf.Approximately(y, 0.0f))
        {
            return;
        }

        m_AnimatorManager.RoleMoveCtrl();

        Vector3 q = this.transform.localEulerAngles;
        q.y = 90 -degrees;

        Quaternion to = Quaternion.Euler(q);
        m_AnimatorManager.SetRotation(to);
	}

	//攻击;
	void PlayAttack()
	{
		m_AnimatorManager.AttackCtrl();
	}

	void PlayAttackSkill(int type)
	{
		m_AnimatorManager.SkillCtrl(0, type);
	}

	void PlayAttackSkillEnd()
	{
		m_AnimatorManager.SkillCtrl(0, 0);
	}

	void FixedUpdate () {
		//JoyCtrl();
	}

    private void Update()
    {
        JoyCtrl();
    }
}
