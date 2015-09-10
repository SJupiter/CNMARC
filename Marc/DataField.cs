using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marc
{
    public class DataField : Field
    {
        private const char SubfieldPlain = '$';
        private const char SubfieldWor = '@';

        private string indicator;

        private List<SubField> subfields;

        public List<SubField> SubFields
        {
            get { return subfields; }
        }

        
        public DataField(string tag)
        {
            this.tag = tag;
            this.indicator = "  ";
            this.subfields = new List<SubField>();
        }

        public DataField(string tag, string strData)
        {
            string[] strSubfields = strData.Split(Field.SubFieldStart);

            this.tag = tag;
            this.indicator = strSubfields[0];

            this.subfields= new List<SubField>();

            for (int i = 1; i < strSubfields.Length;i++ )
            {
                string strSf = strSubfields[i];
                if (!string.IsNullOrEmpty(strSf))
                    subfields.Add(new SubField(strSf));
            }
        }

        public bool Contains(char sfstart)
        {
            SubField subf = subfields.Find(sf => sf.Id == sfstart);

            if (subf == null)
                return false;
            else
                return true;
        }
      

        private string GetContentString(char sfStart)
        {
            string content = string.Empty;

            //content += indicator;

            foreach (SubField sf in subfields)
            {
                content += sfStart + sf.ToString();
            }
            return content;
        }

        public string GetSubfieldData(char sfId)
        {
            SubField subfield = subfields.Find(sf => sf.Id == sfId);
            if (subfield == null)
                return string.Empty;
            else
                return subfield.GetData();
        }

        public string GetSubfieldData(char sfId,int sfIndex)
        {
            List<SubField> findSf = subfields.FindAll(sf => sf.Id == sfId);
            if (sfIndex < findSf.Count)
                return findSf[sfIndex].GetData();
            else
                return string.Empty;
        }

        public override int GetByteLength(Encoding encoding)
        {
            int byteLength = 0;
            byteLength += encoding.GetByteCount(indicator);
            foreach (SubField sf in subfields)
            {
                byteLength += encoding.GetByteCount(Field.SubFieldStart.ToString());
                byteLength += sf.GetByteLength(encoding);
            }
            byteLength += encoding.GetByteCount(Field.FieldEnd.ToString());
            return byteLength;
        }

        public override string GetContent()
        {
            return GetContentString(SubfieldPlain);
        }

        public override string GetPlainContent()
        {     

            string plainContent = GetContentString(SubfieldPlain);

            return plainContent;
        }

        public override string GetIndicator()
        {
            string indiView = string.Empty;

            indiView += (indicator[0] == ' ' ? "-" : indicator[0].ToString());
            indiView += (indicator[1] == ' ' ? "-" : indicator[1].ToString());

            return indiView;
        }

        public override string ToXML()
        {
            throw new NotImplementedException();
        }

        public override string ToWorString(int lineLength)
        {
            string strWor = string.Empty;
            string content = GetContentString(SubfieldWor).Substring(2);

            int maxContent = lineLength - 5;
            int lineCount;
            if (content.Length < maxContent)
                lineCount = 1;
            else
            {
                lineCount = content.Length / maxContent;

                if (content.Length % maxContent != 0)
                    lineCount++;
            }            

            for (int i = 0; i < lineCount; i++)
            {

                if (i == 0)
                    strWor += tag + indicator;
                else
                    strWor += "     ";

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
            string marcString = GetContentString(Field.SubFieldStart);
            marcString += Field.FieldEnd;

            return marcString;
        }

        public void AddSubField(char sfId, string data)
        {
            SubField sf = new SubField(sfId, data);
            subfields.Add(sf);
        }


        public void SetIndicator(string indicator)
        {
            if (indicator.Length == 2)
                this.indicator = indicator;
            if (indicator.Length > 2)
                this.indicator = indicator.Substring(0, 2);
            if (indicator.Length == 1)
                this.indicator = indicator[0] + " ";
            if (indicator.Length < 1)
                this.indicator = "  ";
        }


        public override string GetViewContent()
        {
            string content = string.Empty;

            foreach(SubField sf in subfields)
            {
                content += sf.Id.ToString() + " " + sf.Data + "\n";
            }

            content = content.Substring(0, content.Length - 1);
            return content;
        }
    }
}
