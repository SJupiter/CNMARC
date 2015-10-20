using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Marc
{
    public class SubField
    {
        /// <summary>
        /// Topengine的MARCXML方案
        /// </summary>
        private const string XmlName = "sf";
        private const string IdAttr = "name";

        private char id;
        private string data;

        public char Id
        {
            get
            {
                return id;
            }
        }

        public string Data
        {
            get
            {
                return data;
            }
        }

        public SubField(char id, string data)
        {
            this.id = id;
            this.data = data;
        }

        public SubField(string strSubFile)
        {
            this.id = strSubFile[0];
            this.data = strSubFile.Substring(1);
        }

        public SubField(char id)
        {
            this.id = id;
        }

        public void SetData(string data)
        {
            this.data = data;
        }
             
        public int GetByteLength(Encoding encoding)
        {
            return encoding.GetByteCount(id + data);
        }

        public string GetData()
        {
            return data;
        }

        /// <summary>
        /// TopEngine的MARCXML方案
        /// </summary>
        /// <returns></returns>
        public string ToXML()
        {
            XElement xe = new XElement(XmlName, data);
            xe.Add(new XAttribute(IdAttr, id));

            return xe.ToString();
        }

        public override string ToString()
        {
            return id + data;
        }
    }
}
