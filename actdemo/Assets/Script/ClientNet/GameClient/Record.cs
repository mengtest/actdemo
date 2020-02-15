
namespace Fm_ClientNet
{
    //游戏世界中的表格
    public class Record
    {

        //表格名
        private string name;

        //表格最大列数
        private int maxCols;

        //表格各列的类型
        private int[] colType;

        public string getRecordName()
        {
            return this.name;
        }

        public int getMaxCols()
        {
            return this.maxCols;
        }


        public int[] getColTypes()
        {
            return this.colType;
        }

        public int getColType(int col)
        {
            if (col >= 0 && col < maxCols)
            {
                return this.colType[col];
            }
            LogSystem.LogError("col error");
            return 0;
        }


        public Record(string name, int maxCols)
        {
            if (maxCols < 1)
            {
                return;
            }

            this.name = name;
            this.maxCols = maxCols;
            colType = new int[maxCols];
        }

        /// <summary>
        /// 设置表格列的数据类型.
        /// </summary>
        /// <returns>
        /// The record col type.
        /// </returns>
        /// <param name='col'>
        /// If set to <c>true</c> col.
        /// </param>
        /// <param name='type'>
        /// 对应列的数据类型（可参照 类OuterDataType）
        /// </param>
        public bool setRecordColType(int col, int type)
        {
            if (col >= 0 && col < this.maxCols)
            {
                this.colType[col] = type;
                return true;
            }

            return false;
        }

    }
}


