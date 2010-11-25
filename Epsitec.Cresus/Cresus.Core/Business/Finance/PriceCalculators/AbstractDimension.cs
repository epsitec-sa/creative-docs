using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Xml.Linq;



namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	public abstract class AbstractDimension
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


		public abstract string GetStringData();


		public XElement XmlExport()
		{
			XElement xDimension = new XElement (XmlConstants.DimensionTag);

			xDimension.SetAttributeValue (XmlConstants.NameTag, this.GetXmlName ());
			xDimension.SetAttributeValue (XmlConstants.TypeTag, this.GetXmlTypeName ());
			xDimension.SetAttributeValue (XmlConstants.DataTag, this.GetStringData ());

			return xDimension;
		}


		private string GetXmlName()
		{
			return this.Name;
		}


		private string GetXmlTypeName()
		{
			if (this is CodeDimension)
			{
				return XmlConstants.CodeTypeName;
			}
			else if (this is NumericDimension)
			{
				return XmlConstants.NumericTypeName;
			}
			else
			{
				throw new System.NotImplementedException ();
			}
		}


		public static AbstractDimension XmlImport(XElement xDimension)
		{
			xDimension.ThrowIfNull ("xDimension");

			AbstractDimension.CheckXmlDimension (xDimension);

			string name = AbstractDimension.ExtractXmlName (xDimension);
			string typeName = AbstractDimension.ExtractXmlTypeName (xDimension);
			string data = AbstractDimension.ExtractXmlData (xDimension);

			return AbstractDimension.BuildDimension (name, typeName, data);
		}


		private static void CheckXmlDimension(XElement XDimension)
		{
			if (XDimension.Name != XmlConstants.DimensionTag)
			{
				throw new System.ArgumentException ("Invalid xml data");
			}
		}


		private static string ExtractXmlName(XElement xDimension)
		{
			return xDimension.Attribute (XmlConstants.NameTag).Value;
		}


		private static string ExtractXmlTypeName(XElement xDimension)
		{
			return xDimension.Attribute (XmlConstants.TypeTag).Value;
		}


		private static string ExtractXmlData(XElement xDimension)
		{
			return xDimension.Attribute (XmlConstants.DataTag).Value;
		}


		private static AbstractDimension BuildDimension(string name, string type, string data)
		{
			if (type == XmlConstants.CodeTypeName)
			{
				return CodeDimension.BuildCodeDimension (name, data);
			}
			else if (type == XmlConstants.NumericTypeName)
			{
				return NumericDimension.BuildNumericDimension (name, data);
			}
			else
			{
				throw new System.ArgumentException ("Invalid xml data");
			}
		}


		private static class XmlConstants
		{
			public static readonly string DimensionTag = "dimension";
			public static readonly string NameTag = "name";
			public static readonly string TypeTag = "type";
			public static readonly string DataTag = "data";
			public static readonly string CodeTypeName = "code";
			public static readonly string NumericTypeName = "numeric";
		}
                        

	}


}
