using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

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


		//public void WriteDimension(XmlWriter xmlWriter)
		//{
		//    this.WriteDimensionStart (xmlWriter);
		//    this.WriteName (xmlWriter);
		//    this.WriteType (xmlWriter);
			
		//    //xmlWriter.WriteStartElement ("type");
		//    //// TODO
		//    //xmlWriter.WriteEndElement ();

		//    //if (true) // TODO
		//    //{
		//    //    xmlWriter.WriteStartElement ("mode");
		//    //    // TODO
		//    //    xmlWriter.WriteEndElement ();
		//    //}

		//    //foreach (object value in Values)
		//    //{
		//    //    xmlWriter.WriteStartElement ("point");
		//    //    xmlWriter.WriteValue (InvariantConverter.ConvertToString (value));
		//    //    xmlWriter.WriteEndElement ();
		//    //}

		//    this.WriteDimensionEnd (xmlWriter);
		//}

		//private void WriteDimensionStart(XmlWriter xmlWriter)
		//{
		//    xmlWriter.WriteStartElement ("dimension");
		//}


		//private void WriteName(XmlWriter xmlWriter)
		//{
		//    xmlWriter.WriteStartElement ("name");
		//    xmlWriter.WriteValue (Name);
		//    xmlWriter.WriteEndElement ();
		//}


		//protected abstract void WriteType(XmlWriter xmlWriter);


		//protected abstract void WriteAdditionalInfo(XmlWriter xmlWriter);


		//protected abstract void WriteValue


		//private void WriteDimensionEnd(XmlWriter xmlWriter)
		//{
		//    xmlWriter.WriteEndElement ();
		//}
                        

	}


}
