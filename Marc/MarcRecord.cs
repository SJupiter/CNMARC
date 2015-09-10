using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Marc
{
    public class MarcRecord
    {
        private const char RecordEnd = (char)0x1d;
        private const int AddressItemLength = 12;

        private LDR ldr;

        private List<Field> fields;

        #region 构造函数

        private MarcRecord() { }

        private MarcRecord(LDR ldr)
        {
            this.ldr = ldr;
            this.fields = new List<Field>();
        }

        private MarcRecord(string strMarc)
        {
            this.ldr = new LDR(strMarc.Substring(0, LDR.LDRLength));
            this.fields = new List<Field>();

            string strAddress = strMarc.Substring(LDR.LDRLength, ldr.BaseAddress - LDR.LDRLength + 1);
            string strData = strMarc.Substring(ldr.BaseAddress);

            string[] strField = strData.Split(Field.FieldEnd);

            for (int i = 0; i < strAddress.Length / AddressItemLength; i++)
            {
                string addressItem = strAddress.Substring(i * AddressItemLength, AddressItemLength);

                string tag = addressItem.Substring(0, 3);

                Field fld;

                if (tag.Equals("-01") || tag.Equals("001") || tag.Equals("005"))
                {
                    fld = new ControlField(tag, strField[i]);
                }
                else
                {
                    fld = new DataField(tag, strField[i]);
                }

                fields.Add(fld);
            }            
        }

        #endregion

        public int FieldCount
        {
            get
            {
                return fields.Count;
            }
        }

        public List<Field> Fields
        {
            get
            {
                return fields;
            }
        }

        public Field GetField(string tag)
        {
            return fields.Find(fld => fld.Tag.Equals(tag));
        }

        public Field GetField(string tag, int fieldIndex)
        {
            List<Field> findFld = fields.FindAll(field => field.Tag.Equals(tag));
            if (fieldIndex < findFld.Count)
            {
                Field fld = findFld[fieldIndex];
                return fld;
            }
            else
            {
                return null;
            }
        }

        public Field[] GetFieldAll(string tag)
        {
            return fields.FindAll(fld => fld.Tag.Equals(tag)).ToArray();
        }

        public Field[] FindFields(Predicate<Field> match)
        {
            return fields.FindAll(match).ToArray();
        }

        private string GenerateAddress(Encoding encoding)
        {
            string addressStr = string.Empty;
            int offset = 0;

            foreach(Field f in fields)
            {
                int length = f.GetByteLength(encoding);
                addressStr += f.Tag;
                addressStr += length.ToString("D4");
                addressStr += offset.ToString("D5");

                offset += length;
            }
            addressStr += Field.FieldEnd;
            return addressStr;
        }

        private void UpdateLDR()
        {
            int recordLength = 24;
            recordLength += AddressItemLength * fields.Count + 1;
            int baseOffset = recordLength;

            foreach (Field f in fields)
            {
                recordLength += f.GetByteLength(Encoding.Default);
            }
            recordLength += 1;

            ldr.SetRecordLength(recordLength);
            ldr.SetBaseAddress(baseOffset);

        }

        public string ToWorString(int lineLength)
        {
            if (lineLength < LDR.LDRLength)
                return string.Empty;

            string strWor = string.Empty;
            strWor += ldr.ToString() + "\r\n";

            foreach (Field f in fields)
            {
                strWor += f.ToWorString(lineLength) + "\r\n";
            }
            strWor += "\r\n***\r\n";
            return strWor;
        }

        public string ToPlainString()
        {
            string plainStr = string.Empty;
            plainStr += "000 " + ldr.ToString()+"\n";
            foreach (Field f in fields)
            {
                plainStr += f.Tag + " " + f.GetPlainContent() + "\n";
               
            }
            return plainStr;
        }

        public string ToMarcString(Encoding encoding)
        {
            UpdateLDR();
            string strMarc = ldr.ToString();
            strMarc += GenerateAddress(encoding);

            foreach (Field f in fields)
            {
                strMarc += f.ToMarcString();
            }
            strMarc += RecordEnd;
            return strMarc;
        }

        public string GetFieldContent(string tag)
        {
            Field fld = GetField(tag);
            if (fld == null)
                return string.Empty;
            else
                return fld.GetContent(); 
        }
               
        public string GetFieldContent(string tag, char sfId)
        {
            DataField datafield = (DataField)GetField(tag);
            if (datafield == null)
                return string.Empty;
            else
                return datafield.GetSubfieldData(sfId);
        }

        public string GetFieldContent(string tag, int fieldIndex)
        {
            Field fld = GetField(tag, fieldIndex);
            if (fld == null)
                return string.Empty;
            else
                return fld.GetContent();
        }     

        public string GetFieldContent(string tag, char sfId, int fieldIndex)
        {
            Field fld = GetField(tag, fieldIndex);
            if (fld == null)
                return string.Empty;
            else
                return ((DataField)fld).GetSubfieldData(sfId);
            
               
        }

        public int GetFieldCount(string tag)
        {
            return fields.Where(field => field.Tag.Equals(tag)).Count();
        }

        public string[] GetAllFieldContent(string tag, char sfId)
        {
            List<Field> finds = fields.FindAll(f => f.Tag.Equals(tag));

            List<string> contents = new List<string>();

            foreach(Field f in finds)
            {
                if(f is DataField)
                {
                    string data = ((DataField)f).GetSubfieldData(sfId);
                    if (!string.IsNullOrEmpty(data))
                        contents.Add(data);
                }
            }

            return contents.ToArray();


        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="f"></param>
        public void AddField(Field f)
        {
            fields.Add(f);
        }

        /// <summary>
        /// 安装字段编号顺序添加字段
        /// </summary>
        /// <param name="f"></param>
        public void AddFieldByOrder(Field f)
        {
            fields.Add(f);
            fields.Sort((f1, f2) => {
                return string.Compare(f1.Tag, f2.Tag);
            });
        }

        public void RemoveField(Field f)
        {
            int index = fields.IndexOf(f);
            fields.RemoveAt(index);
        }

        
        /// <summary>
        /// 由标准marc生成record对象
        /// </summary>
        /// <param name="strMarc"></param>
        /// <returns></returns>
        public static MarcRecord LoadFromString(string strMarc)
        {
            if (string.IsNullOrEmpty(strMarc))
                return null;
            if (strMarc.Length < LDR.LDRLength)
                return null;
            //if (!strMarc.EndsWith(RecordEnd.ToString()))
            //    return null;

            MarcRecord record = new MarcRecord(strMarc);

            return record;
        }

        /// <summary>
        /// 由xml格式的marc生成record对象，marc的xml格式方案来自于TopEnginge
        /// </summary>
        /// <param name="xmlMarc"></param>
        /// <returns></returns>
        public static MarcRecord LoadFromXMlString(string xmlMarc)
        {
            XElement xe = XElement.Parse(xmlMarc);

            string strldr = xe.Element("header").Value;
            LDR ldr = new LDR(strldr);

            MarcRecord record = new MarcRecord(ldr);

            foreach(XElement fldxml in xe.Elements("field"))
            {
                Field fld;

                string tag = fldxml.Attribute("name").Value;

                if (tag.Equals("-01") || tag.Equals("001") || tag.Equals("005"))
                {
                    fld = new ControlField(tag, fldxml.Value);
                }
                else
                {
                    fld = new DataField(tag);



                    string indicator = fldxml.Attribute("indicator") == null ? string.Empty : fldxml.Attribute("indicator").Value;

                    foreach (XElement sfxml in fldxml.Elements("sf"))
                    {

                        ((DataField)fld).AddSubField(sfxml.Attribute("name").Value[0], sfxml.Value);
                    }

                }

                record.AddField(fld);
            }

            return record;
        }

        /// <summary>
        /// 生成新的Marc的record记录
        /// </summary>
        /// <param name="newLDR"></param>
        /// <returns></returns>
        public static MarcRecord NewRecord(string newLDR)
        {
            if (string.IsNullOrEmpty(newLDR))
                return null;
            if (newLDR.Length < LDR.LDRLength)
                return null;

            LDR ldr = new LDR(newLDR);
            MarcRecord record = new MarcRecord(ldr);

            return record;
        }
    }
}
