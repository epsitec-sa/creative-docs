using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Xml;



namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	internal abstract class AbstractDimension
	{


		public AbstractDimension(string name)
		{
			name.ThrowIfNullOrEmpty ("name");

			this.Name = name;
		}


		public string Name
		{
			get;
			private set;
		}


		public abstract IEnumerable<object> Values
		{
			get;
		}


		public abstract bool IsValueDefined(object value);


		public abstract bool IsNearestValueDefined(object value);


		public abstract object GetNearestValue(object value);


		public abstract string ConvertToString(object value);


		public void WriteDimension(XmlWriter xmlWriter)
		{
		    this.WriteDimensionStart (xmlWriter);
		    this.WriteName (xmlWriter);
		    this.WriteType (xmlWriter);
			this.WriteAdditionalInfo (xmlWriter);
			this.WriteValues (xmlWriter);
		    this.WriteDimensionEnd (xmlWriter);
		}

		private void WriteDimensionStart(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("dimension");
		}


		private void WriteName(XmlWriter xmlWriter)
		{
			xmlWriter.WriteAttributeString ("name", this.Name);
		}


		private void WriteType(XmlWriter xmlWriter)
		{
			string type;

			if (this is CodeDimension)
			{
				type = "code";
			}
			else if (this is NumericDimension)
			{
				type = "numeric";
			}
			else
			{
				throw new System.NotImplementedException ();
			}
			
			xmlWriter.WriteAttributeString ("type", type);
		}


		private void WriteAdditionalInfo(XmlWriter xmlWriter)
		{
			if (this is NumericDimension)
			{
				NumericDimension thisAsNumericDimension = (NumericDimension) this;

				string mode = System.Enum.GetName (typeof (RoundingMode), thisAsNumericDimension.RoundingMode);

				xmlWriter.WriteAttributeString ("mode", mode);
			}
		}


		private void WriteValues(XmlWriter xmlWriter)
		{
			string values = string.Join (";", this.Values);

			xmlWriter.WriteAttributeString ("values", values);
		}


		private void WriteDimensionEnd(XmlWriter xmlWriter)
		{
			xmlWriter.WriteEndElement ();
		}
                        

	}


}
