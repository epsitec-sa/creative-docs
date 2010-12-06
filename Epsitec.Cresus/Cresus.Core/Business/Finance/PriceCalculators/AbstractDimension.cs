using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Xml.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	/// <summary>
	/// The <c>AbstractDimension</c> class represents a dimension for a <see cref="DimensionTable"/>.
	/// It has a name and defines a set of points that represents the points of the dimension. In
	/// addition, it defines mechanisms that can be used to obtain the nearest point defined on the
	/// dimension, given a value which is not a defined point.
	/// </summary>
	public abstract class AbstractDimension
	{


		/// <summary>
		/// Creates a new <c>AbstractDimension</c>.
		/// </summary>
		/// <param name="name">The name of the dimension.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		public AbstractDimension(string name)
		{
			name.ThrowIfNullOrEmpty ("name");

			this.Name = name;
		}


		/// <summary>
		/// Gets the name of the current instance.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the set of points that defined the current instance.
		/// </summary>
		public abstract IEnumerable<object> Values
		{
			get;
		}


		/// <summary>
		/// Tells whether the given value is a point which is exactly defined in the current instance.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><c>true</c> if the point is exactly defined, <c>false</c> if it is not.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="value"/> is <c>null</c> or invalid.</exception>
		public abstract bool IsValueDefined(object value);


		/// <summary>
		/// Tells whether there exist a nearest point in the current instance of the given value.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><c>true</c> if there is a nearest point defined, <c>false</c> if there is not.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="value"/> is <c>null</c> or invalid.</exception>
		public abstract bool IsNearestValueDefined(object value);


		/// <summary>
		/// Gets the nearest point defined in this current instance for the given value.
		/// </summary>
		/// <param name="value">The value whose nearest point to get.</param>
		/// <returns>The nearest point defined, if there is any.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="value"/> is <c>null</c> or invalid.</exception>
		public abstract object GetNearestValue(object value);


		/// <summary>
		/// Gets a <see cref="System.String"/> that contains the data that is necessary to serialize
		/// the current instance and deserialize it later.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that can be used to build a clone of the current instance.</returns>
		public abstract string GetStringData();


		/// <summary>
		/// Builds an <see cref="XElement"/> that represents the current instance.
		/// </summary>
		/// <returns>The <see cref="XElement"/> that represents the current instance.</returns>
		public XElement XmlExport()
		{
			XElement xDimension = new XElement (XmlConstants.DimensionTag);

			xDimension.SetAttributeValue (XmlConstants.NameTag, this.GetXmlName ());
			xDimension.SetAttributeValue (XmlConstants.TypeTag, this.GetXmlTypeName ());
			xDimension.SetAttributeValue (XmlConstants.DataTag, this.GetStringData ());

			return xDimension;
		}


		/// <summary>
		/// Gets the value for the name of this instance.
		/// </summary>
		/// <returns>The name of this instance.</returns>
		private string GetXmlName()
		{
			return this.Name;
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

			string name = AbstractDimension.ExtractXmlName (xDimension);
			string typeName = AbstractDimension.ExtractXmlTypeName (xDimension);
			string data = AbstractDimension.ExtractXmlData (xDimension);

			return AbstractDimension.BuildDimension (name, typeName, data);
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


		/// <summary>
		/// Private class that contains the xml constants used for the xml serialization of instances
		/// of <see cref="AbstractDimension"/>s.
		/// </summary>
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
