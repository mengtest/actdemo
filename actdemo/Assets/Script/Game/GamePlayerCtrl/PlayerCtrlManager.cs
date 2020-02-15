using UnityEngine;
using System.Collections;

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

	void Awake()
    {
        return;
        GameObject parent = GameObject.Find("UI Root/Camera/JoyStickUI");
        parent.SetActive(true);
        mDirection = parent.transform.Find("BasePart/Direction");
        VCAnalogJoystickBase.lockJoystick = true;
    }

	public void JoyCtrl()
	{
		if(m_AnimatorManager == null)
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

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            suc = Physics.Raycast(ray, out hit, 1000);
            transform.LookAt(hit.point);
        }
        if (suc)
        {
            transform.position = Vector3.MoveTowards(transform.position, hit.point, Time.fixedDeltaTime * 5);
            m_AnimatorManager.RoleMoveCtrl();
        }

        if (Vector3.Distance(transform.position, hit.point) < 0.001f)
        {

            suc = false;
            m_AnimatorManager.Stand();
        }

        if (Input.GetKeyDown(KeyCode.Q))
		{
            gameObject.GetComponent<SkillComponent>().DoSkill(600005);
        }

		if(Input.GetKeyDown(KeyCode.W))
		{
            gameObject.GetComponent<SkillComponent>().DoSkill(600006);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            gameObject.GetComponent<SkillComponent>().DoSkill(600007);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.GetComponent<SkillComponent>().DoSkill(600008);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            gameObject.GetComponent<SkillComponent>().DoSkill(600009);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
		{
            gameObject.GetComponent<SkillComponent>().DoSkill(600003);
		}
		else if(Input.GetKeyUp(KeyCode.Alpha1))
		{
			PlayAttackSkillEnd();
		}

		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
            gameObject.GetComponent<SkillComponent>().DoSkill(600002);
		}
		else if(Input.GetKeyUp(KeyCode.Alpha2))
		{
			PlayAttackSkillEnd();
		}

		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
            gameObject.GetComponent<SkillComponent>().DoSkill(600001);
        }
		else if(Input.GetKeyUp(KeyCode.Alpha3))
		{
			PlayAttackSkillEnd();
		}


		if(Input.GetKeyDown(KeyCode.Alpha4))
		{
//			m_AnimatorManager.BeatenAnim(1f);
		}

		if(Input.GetKeyDown(KeyCode.Alpha5))
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
