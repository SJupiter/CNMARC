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

        /// <summary>
        /// 指示符
        /// </summary>
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

        /// <summary>
        /// 获得字段原始内容
        /// </summary>
        /// <returns>字段原始内容</returns>
        public abstract string GetContent();

        /// <summary>
        /// 获得字段查看内容
        /// </summary>
        /// <returns>字段查看内容</returns>
        public abstract string GetViewContent();

        /// <summary>
        /// 获取无格式数据内容
        /// </summary>
        /// <returns>无格式数据内容</returns>
        public abstract string GetPlainContent();

        /// <summary>
        /// 还原为MARC格式字符串
        /// </summary>
        /// <returns>MARC格式字符串</returns>
        public abstract string ToMarcString();

        /// <summary>
        /// 生成MARCXML格式（TopEngine方案）
        /// </summary>
        /// <returns>MARCXML格式字符串</returns>
        public abstract string ToXML();

        /// <summary>
        /// 生存WOR格式MARC字符串
        /// </summary>
        /// <param name="lineLength">每行最大字符数</param>
        /// <returns>WOR格式MARC字符串</returns>
        public abstract string ToWorString(int lineLength);

        /// <summary>
        /// 获得字段指示符，控制字段返回空字符
        /// </summary>
        /// <returns>字段指示符</returns>
        public abstract string GetIndicator();

    }
}
