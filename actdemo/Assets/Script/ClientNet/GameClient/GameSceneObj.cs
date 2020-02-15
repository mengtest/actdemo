using Fm_ClientNet.Interface;

namespace Fm_ClientNet
{
    public class GameSceneObj : GameObj, IGameSceneObj
    {
        private float m_fPosiX;
        private float m_fPosiY;
        private float m_fPosiZ;
        private float m_fOrient;
        private float m_fDestX;
        private float m_fDestY;
        private float m_fDestZ;
        private float m_fDestOrient;
        private float m_fMoveSpeed;
        private float m_fRotateSpeed;
        private float m_fJumpSpeed;
        private float m_fGravity;
        private int m_nPosiIndex;
        private int m_nDestIndex;
        int m_nMode;
        string m_strLinkIdent;

        float m_fLinkX;
        float m_fLinkY;
        float m_fLinkZ;
        float m_fLinkOrient;

        public GameSceneObj()
        {
        }


        public float GetPosiX()
        {
            return this.m_fPosiX;
        }

        public float GetPosiY()
        {
            return this.m_fPosiY;
        }

        public float GetPosiZ()
        {
            return this.m_fPosiZ;
        }

        public float GetOrient()
        {
            return this.m_fOrient;
        }

        public float GetDestX()
        {
            return this.m_fDestX;
        }

        public float GetDestY()
        {
            return this.m_fDestY;
        }

        public float GetDestZZ()
        {
            return this.m_fDestZ;
        }
        public int GetPosiIndex()
        {
            return this.m_nPosiIndex;
        }

        public int GetDestIndex()
        {
            return this.m_nDestIndex;
        }

        public float GetDestOrient()
        {
            return this.m_fDestOrient;
        }

        public float GetMoveSpeed()
        {
            return this.m_fMoveSpeed;
        }

        public float GetRotateSpeed()
        {
            return this.m_fRotateSpeed;
        }

        public float GetJumpSpeed()
        {
            return this.m_fJumpSpeed;
        }

        public float GetGravity()
        {
            return this.m_fGravity;
        }

        public void SetMode(int value)
        {
            this.m_nMode = value;
        }

        public int GetMode()
        {
            return this.m_nMode;
        }

        // 设置位置
        public void SetLocation(int index, float orient)
        {
            m_nPosiIndex = index;
            m_fOrient = orient;
        }

        public bool SetLocation(float x, float y, float z, float orient)
        {
            m_fPosiX = x;
            m_fPosiY = y;
            m_fPosiZ = z;
            m_fOrient = orient;
            return true;
        }
        // 设置移动目标
        public void SetDestination(int index, float orient, float move_speed)
        {
            m_nDestIndex = index;
            m_fDestOrient = orient;
            m_fMoveSpeed = move_speed;
        }

        public bool SetDestination(float x, float y, float z, float orient,
            float move_speed, float rotate_speed, float jump_speed, float gravity)
        {
            m_fDestX = x;
            m_fDestY = y;
            m_fDestZ = z;
            m_fDestOrient = orient;
            m_fMoveSpeed = move_speed;
            m_fRotateSpeed = rotate_speed;
            m_fJumpSpeed = jump_speed;
            m_fGravity = gravity;
            return true;
        }

        public void SetLinkIdent(string value)
        {
            this.m_strLinkIdent = value;
        }



        public string GetLinkIdent()
        {
            return this.m_strLinkIdent;
        }

        public float GetLinkX()
        {
            return this.m_fLinkX;
        }

        public float GetLinkY()
        {
            return this.m_fLinkY;
        }

        public float GetLinkZ()
        {
            return this.m_fLinkZ;
        }

        public float GetLinkOrient()
        {
            return this.m_fLinkOrient;
        }

        public bool SetLinkPos(float x, float y, float z, float orient)
        {
            m_fLinkX = x;
            m_fLinkY = y;
            m_fLinkZ = z;
            m_fLinkOrient = orient;
            return true;
        }
    }
}



