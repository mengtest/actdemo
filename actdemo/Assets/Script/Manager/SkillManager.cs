using UnityEngine;
using System.Collections;

public class SkillManager
{
    private static SkillManager _Instance = null;
    private SkillManager() { }

    public static SkillManager Instance
    {
        private set { }
        get
        {
            if (_Instance == null)
            {
                _Instance = new SkillManager();
            }

            return _Instance;
        }
    }


}