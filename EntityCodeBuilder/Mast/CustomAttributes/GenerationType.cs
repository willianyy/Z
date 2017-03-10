using System;
using System.Collections.Generic;
using System.Text;

namespace Mast.CustomAttributes
{
    public class GenerationType 
    {        
        public const int INDENTITY = 1;//自动增长
        public const int GUID = 2;//GUID
        public const int FILL = 3;//提前生成并填充

        private GenerationType() { }//私有构造函数，不可被实例化对象
    }
}
