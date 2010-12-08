using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Xml.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	public abstract class AbstractDimension
	{


		public AbstractDimension(string code, string name)
		{
			this.Code = code;
			this.Name = name;
			this.DimensionTable = null;
		}


		public string Code
		{
			get
			{
				return this.code;
			}
			set
			{
				this.code = value;
			}
		}


		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}


		public abstract IEnumerable<string> Values
		{
			get;
		}


		public abstract int Count
		{
			get;
		}


		protected DimensionTable DimensionTable
		{
			get;
			private set;
		}


		internal void AddToDimensionTable(DimensionTable dimensionTable)
		{
			this.DimensionTable = dimensionTable;
		}


		internal void RemoveFromDimensionTable(DimensionTable dimensionTable)
		{
			this.DimensionTable = null;
		}


		public abstract void Add(string value);


		public abstract void Remove(string value);


		public abstract bool Contains(string value);


		public abstract bool IsValueRoundable(string value);


		public abstract string GetRoundedValue(string value);


		public abstract int GetIndexOf(string value);


		public abstract string GetValueAt(int index);


		/// <summary>
		/// Gets a <see cref="System.String"/> that contains the data that is necessary to serialize
		/// the current instance and deserialize it later.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that can be used to build a clone of the current instance.
		/// </returns>
		public abstract string GetStringData();


		/// <summary>
		/// Builds an <see cref="XElement"/> that represents the current instance.
		/// </summary>
		/// <returns>The <see cref="XElement"/> that represents the current instance.</returns>
		public XElement XmlExport()
		{
			XElement xDimension = new XElement (XmlConstants.DimensionTag);

			xDimension.SetAttributeValue (XmlConstants.CodeTag, this.Code);
			xDimension.SetAttributeValue (XmlConstants.NameTag, this.Name);
			xDimension.SetAttributeValue (XmlConstants.TypeTag, this.GetXmlTypeName ());
			xDimension.SetAttributeValue (XmlConstants.DataTag, this.GetStringData ());

			return xDimension;
		}


		/// <summary>
		/// Gets the <see cref="System.String"/> value for the concrete type of this instance.
		/// </summary>
		/// <returns>The value for the concrete type of this instance.</returns>
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


		/// <summary>
		/// Creates a new concrete instance of <c>AbstractDimension</c> based on the data that has
		/// been obtained with the <see cref="AbstractDimension.XmlExport"/> method.
		/// </summary>
		/// <param name="xDimension">The xml data.</param>
		/// <returns>The new instance.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="xDimension"/> is <c>null</c> or invalid.</exception>
		public static AbstractDimension XmlImport(XElement xDimension)
		{
			xDimension.ThrowIfNull ("xDimension");

			AbstractDimension.CheckXmlDimension (xDimension);

			string code = AbstractDimension.ExtractXmlCode (xDimension);
			string name = AbstractDimension.ExtractXmlName (xDimension);
			string typeName = AbstractDimension.ExtractXmlTypeName (xDimension);
			string data = AbstractDimension.ExtractXmlData (xDimension);

			return AbstractDimension.BuildDimension (code, name, typeName, data);
		}


		/// <summary>
		/// Checks that the given <see cref="XElement"/> is a valid serialized dimension.
		/// </summary>
		/// <param name="XDimension">The <see cref="XElement"/> to check.</param>
		private static void CheckXmlDimension(XElement XDimension)
		{
			if (XDimension.Name != XmlConstants.DimensionTag)
			{
				throw new System.ArgumentException ("Invalid xml data");
			}
		}


		/// <summary>
		/// Extracts the code of the serialized <c>AbstractDimension</c>.
		/// </summary>
		/// <param name="xDimension">The <see cref="XElement"/> that contains the data.</param>
		/// <returns>The code.</returns>
		private static string ExtractXmlCode(XElement xDimension)
		{
			return xDimension.Attribute (XmlConstants.CodeTag).Value;
		}


		/// <summary>
		/// Extracts the name of the serialized <c>AbstractDimension</c>.
		/// </summary>
		/// <param name="xDimension">The <see cref="XElement"/> that contains the data.</param>
		/// <returns>The name.</returns>
		private static string ExtractXmlName(XElement xDimension)
		{
			return xDimension.Attribute (XmlConstants.NameTag).Value;
		}


		/// <summary>
		/// Extracts the name of the concrete type of the serialized <c>AbstractDimension</c>.
		/// </summary>
		/// <param name="xDimension">The <see cref="XElement"/> that contains the data.</param>
		/// <returns>The name of th concrete type.</returns>
		private static string ExtractXmlTypeName(XElement xDimension)
		{
			return xDimension.Attribute (XmlConstants.TypeTag).Value;
		}


		/// <summary>
		/// Extracts the serialized data of the serialized <c>AbstractDimension</c> that will be used
		/// to restore it.
		/// </summary>
		/// <param name="xDimension">The <see cref="XElement"/> that contains the data.</param>
		/// <returns>The serialized data.</returns>
		private static string ExtractXmlData(XElement xDimension)
		{
			return xDimension.Attribute (XmlConstants.DataTag).Value;
		}


		/// <summary>
		/// Creates a new concrete <see cref="AbstractDimension"/> of the appropriate type.
		/// </summary>
		/// <param name="name">The name of the dimension.</param>
		/// <param name="type">The concrete type of the dimension.</param>
		/// <param name="data">The serialized data of the dimension.</param>
		/// <returns>The new instance of the dimension.</returns>
		private static AbstractDimension BuildDimension(string code, string name, string type, string data)
		{
			if (type == XmlConstants.CodeTypeName)
			{
				return CodeDimension.BuildCodeDimension (code, name, data);
			}
			else if (type == XmlConstants.NumericTypeName)
			{
				return NumericDimension.BuildNumericDimension (code, name, data);
			}
			else
			{
				throw new System.ArgumentException ("Invalid xml data");
			}
		}


		/// <summary>
		/// Private class that contains the xml constants used for the xml serialization of instances
		/// of <see cref="AbstractDimension"/>s.
		/// </summary>
		private static class XmlConstants
		{
			public static readonly string DimensionTag = "dimension";
			public static readonly string CodeTag = "code";
			public static readonly string NameTag = "name";
			public static readonly string TypeTag = "type";
			public static readonly string DataTag = "data";
			public static readonly string CodeTypeName = "code";
			public static readonly string NumericTypeName = "numeric";
		}


		private string code;


		private string name;

       
	}


}
