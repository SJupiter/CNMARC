using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marc
{
    public abstract class Field
    {        
        public const char FieldEnd = (char)0x1e;
        public const char SubFieldStart = (char)0x1f;

        protected const string FieldXmlName = "field";
        protected const string TagAttr = "name";

        protected string tag;

        public string Tag
        {
            get
            {
                return tag;
            }
        }

        public string Content
        {
            get
            {
                return GetContent();
            }
        }

        public string Indicator
        {
            get
            {
                return GetIndicator();
            }
        }
        

        /// <summary>
        /// 获得字段数据的字节长度
        /// </summary>
        /// <param name="encoding">编码方式</param>
        /// <returns>字段内容的字节长度</returns>
        public abstract int GetByteLength(Encoding encoding);

        public abstract string GetContent();

        public abstract string GetViewContent();

        /// <summary>
        /// 获取无格式数据内容
        /// </summary>
        /// <returns>无格式数据内容</returns>
        public abstract string GetPlainContent();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string ToMarcString();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract string ToXML();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lineLength">每行最大字符数</param>
        /// <returns></returns>
        public abstract string ToWorString(int lineLength);

        public abstract string GetIndicator();

    }
}
