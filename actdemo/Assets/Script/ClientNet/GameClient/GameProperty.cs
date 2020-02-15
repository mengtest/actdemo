
using SysUtils;

namespace Fm_ClientNet
{
    public struct GameProperty
    {
        public Var propValue  ;
        public static GameProperty zero = new GameProperty(Var.zero);
        public GameProperty(Var prop)
        {
            propValue = prop;
        }

        public int ValueType
        {
            get { return this.propValue.Type; }
        }

        public int getPropType()
        {
            return this.propValue.Type;
        }

        public Var PropValue
        {
            get { return propValue; }
            set { propValue = value; }
        }

        public void setPropValue(Var val)
        {
            propValue.Copy(val);
        }

        public Var getPropValue()
        {
            return this.propValue;
        }

        public bool getPropValueBool()
        {
            if (this.propValue.Type == VarType.Bool)
            {
                return propValue.GetBool();
            }
            else
            {
                LogSystem.Log("Error,GameProperty.getPropValueBool propType is not bool!");
                return false;
            }
        }

        public void setPropValueBool(bool value)
        {
            this.propValue.SetBool(value);
        }

        public int getPropValueInt()
        {
            if (this.propValue.Type == VarType.Int)
            {
                return propValue.GetInt();
            }
            else
            {
                LogSystem.Log("Error,GameProperty.getPropValueInt propType is not int!");
                return 0;
            }
        }

        public void setPropValueInt(int value)
        {
            this.propValue.SetInt(value);
        }

        public long getPropValueInt64()
        {
            if (this.propValue.Type == VarType.Int64)
            {
                return propValue.GetInt64();
            }
            else
            {
                LogSystem.Log("Error,GameProperty.getPropValueInt64 propType is not int64!");
                return 0;
            }
        }

        public void setPropValueInt64(long value)
        {
            this.propValue.SetInt64(value);
        }

        public float getPropValueFloat()
        {
            if (this.propValue.Type == VarType.Float)
            {
                return propValue.GetFloat();
            }
            else
            {
                LogSystem.Log("Error,GameProperty.getPropValueFloat propType is not float!");
                return 0.0f;
            }
        }

        public void setPropValueFloat(float value)
        {
            this.propValue.SetFloat(value);
        }

        public double getPropValueDouble()
        {
            if (this.propValue.Type == VarType.Double)
            {
                return propValue.GetDouble();
            }
            else
            {
                LogSystem.Log("Error,GameProperty.getPropValueDouble propType is not double!");
                return 0.0;
            }
        }

        public void setPropValueDouble(double value)
        {
            this.propValue.SetDouble(value);
        }

        public string getPropValueString()
        {
            if (this.propValue.Type == VarType.String)
            {
                return propValue.GetString();
            }
            else
            {
                LogSystem.Log("Error,GameProperty.getPropValueString propType is not string!");
                return string.Empty;
            }
        }

        public void setPropValueString(string value)
        {
            this.propValue.SetString(value);
        }

        public string getPropValueWideStr()
        {
            if (this.propValue.Type == VarType.WideStr)
            {
                return propValue.GetWideStr();
            }
            else
            {
                LogSystem.Log("Error,GameProperty.getPropValueWideStr propType is not wstring!");
                return string.Empty;
            }
        }

        public void setPropValueWideStr(string value)
        {
            this.propValue.SetWideStr(value);
        }

        public ObjectID getPropValueObject()
        {
            if (this.propValue.Type == VarType.Object)
            {
                return propValue.GetObject();
            }
            else
            {
                LogSystem.Log("Error,GameProperty.getPropValueObject propType is not object!");
                return ObjectID.zero;
            }
        }

        public void setPropValueObject(ObjectID value)
        {
            this.propValue.SetObject(value);
        }
    }
}



