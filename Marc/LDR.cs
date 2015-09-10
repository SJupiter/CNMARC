using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marc
{
    public class LDR
    {
        public const int LDRLength = 24;

        private int iRecordLength;     

        private char cRecordState;
        private char cRecordType;
        private char catalogLevel;
        private char cHierarchyLevel;
        private char cUndefine1;

        private char cFldTagLength;
        private char cSfIdLength;

        private int iBaseAddress;

        private char cCatalogingLevel;
        private char cRecordFormat;
        private char cUndefined2;

        private string sAddressStructure;


        public int BaseAddress
        {
            get
            {
                return iBaseAddress;
            }
        }

        public LDR(string strLDR)
        {
            if (strLDR.Length < 24)
                return;
            this.iRecordLength = Convert.ToInt32(strLDR.Substring(0, 5));

            this.cRecordState = strLDR[5];
            this.cRecordType = strLDR[6];
            this.catalogLevel = strLDR[7];
            this.cHierarchyLevel = strLDR[8];
            this.cUndefine1 = strLDR[9];

            this.cFldTagLength = strLDR[10];
            this.cSfIdLength = strLDR[11];

            this.iBaseAddress = Convert.ToInt32(strLDR.Substring(12, 5));

            this.cCatalogingLevel = strLDR[17];
            this.cRecordFormat = strLDR[18];
            this.cUndefined2 = strLDR[19];

            this.sAddressStructure = strLDR.Substring(20);

        }

        public void SetRecordLength(int recordLength)
        {
            this.iRecordLength = recordLength;
        }

        public void SetBaseAddress(int baseAddress)
        {
            this.iBaseAddress = baseAddress;
        }

        public override string ToString()
        {
            string ldr = string.Empty;

            ldr += iRecordLength.ToString("D5");

            ldr += cRecordState;
            ldr += cRecordType;
            ldr += catalogLevel;
            ldr += cHierarchyLevel;
            ldr += cUndefine1;

            ldr += cFldTagLength;
            ldr += cSfIdLength;

            ldr += iBaseAddress.ToString("D5");

            ldr += cCatalogingLevel;
            ldr += cRecordFormat;
            ldr += cUndefined2;
            ldr += sAddressStructure;

            return ldr;
        }
    }
}
