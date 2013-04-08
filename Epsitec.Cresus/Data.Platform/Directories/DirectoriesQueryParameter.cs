using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Epsitec.Data.Platform.Directories
{
    internal class DirectoriesQueryParameter
    {
        public DirectoriesQueryParameter(int SequenceNumber,string ParameterName,string Value)
        {
            this.Sequence = SequenceNumber;

            XElement Parameter = new XElement(ParameterName);
            Parameter.SetAttributeValue("Value", Value);
            this.Element = Parameter;

        }

        public DirectoriesQueryParameter(int SequenceNumber,string ParameterName,string Value,bool UsePhonetic)
        {
            this.Sequence = SequenceNumber;

            XElement Parameter = new XElement(ParameterName);
            Parameter.SetAttributeValue("Value", Value);
            Parameter.SetAttributeValue ("Phonetic", UsePhonetic == true ? "1" : "0");
            this.Element = Parameter;

        }

        public DirectoriesQueryParameter(int SequenceNumber,string ParameterName,string Value,bool UsePhonetic,string DirectoriesPrecisionCode)
        {
            this.Sequence = SequenceNumber;

            XElement Parameter = new XElement(ParameterName);
            Parameter.SetAttributeValue("Value", Value);
            Parameter.SetAttributeValue ("Phonetic", UsePhonetic == true ? "1" : "0");
            Parameter.SetAttributeValue ("PrecisionCode", DirectoriesPrecisionCode);
            this.Element = Parameter;

        }


        public int Sequence;
        private XElement Element; 

        public XElement GetParameter()
        {
            return this.Element;
        }
    }
}
