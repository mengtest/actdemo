

namespace Fm_ClientNet
{
    public class Propetry
    {

        //属性名称
        private string prop_name;
        //属性的数据类型
        private int prop_type;

        public Propetry(string name, int type)
        {
            this.prop_name = name;
            this.prop_type = type;
        }

        public string getPropName()
        {
            return this.prop_name;
        }

        public int getPropType()
        {
            return this.prop_type;
        }


    }

}

