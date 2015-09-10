using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Marc
{
    public class ControlField : Field
    {
        private string content;

        public string Content
        {
            get
            {
                return content;
            }
        }
        
        public ControlField(string tag, string content)
        {
            this.tag = tag;

            this.content = content;
        }        

        public override int GetByteLength(Encoding encoding)
        {
            return encoding.GetByteCount(content + Field.FieldEnd);
        }

        public override string GetPlainContent()
        {
            return content;
        }

        public override string ToXML()
        {
            XElement xe = new XElement(FieldXmlName, content);
            xe.Add(new XAttribute(TagAttr, tag));
            return xe.ToString();
        }

        public override string ToWorString(int lineLength)
        {
            int maxContent = lineLength - 3;
            int lineCount;
            if (content.Length < maxContent)
                lineCount = 1;
            else
            {                
                lineCount = content.Length / maxContent;

                if (content.Length % maxContent != 0)
                    lineCount++;
            }
            string strWor = string.Empty;

            for (int i = 0; i < lineCount; i++)
            {

                if (i == 0)
                    strWor += tag;
                else
                    strWor.PadRight(strWor.Length + 3);               

                string worline;
                if (content.Length < maxContent)
                    worline = content;
                else if (i == lineCount - 1)
                    worline = content.Substring(i * maxContent);
                else
                    worline = content.Substring(i * maxContent, maxContent);

                if (i != lineCount - 1)
                    strWor += worline + "\r\n";
                else
                    strWor += worline;
            }
          
            return strWor;
        }

        public override string ToMarcString()
        {
            return content + Field.FieldEnd;
        }

        public override string GetContent()
        {
            return content;
        }

        public override string GetViewContent()
        {
            return content;
        }

        public override string GetIndicator()
        {
            return string.Empty;
        }
    }
}
