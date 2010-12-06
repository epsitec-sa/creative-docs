using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;

using System.Xml.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	// TODO Add function to create a new "similar" table from a given one, plus or minus a
	// dimension? And to clone it?
	// Marc


	/// <summary>
	/// The <see cref="DimensionTable"/> class represents a table holding <see cref="System.Decimal"/>
	/// values, which can have an arbitrary number of user defined dimensions. The definition of the
	/// object, that is its dimensions, are immutable but its values are mutable.
	/// </summary>
	public sealed class DimensionTable
	{
		
		
		/// <summary>
		/// Builds a new <see cref="DimensionTable"/> with the given <see cref="AbstractDimension"/>
		/// as dimensions.
		/// </summary>
		/// <remarks>
		/// Note that the given dimensions will be re ordered using their name as sorting criterion.
		/// </remarks>
		/// <param name="dimensions">The <see cref="AbstractDimension"/> of the new instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dimensions"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="dimensions"/> is empty or contains <c>null</c> elements.</exception>
		public DimensionTable(params AbstractDimension[] dimensions)
		{
			dimensions.ThrowIfNull ("dimensions");
			dimensions.ThrowIf (d => d.Length < 1, "dimensions must contain at least one element.");
			dimensions.ThrowIf (dims => dims.Any (dim => dim == null), "dimensions cannot contain null elements.");

			this.dimensions = dimensions.OrderBy (d => d.Name).ToList ();

			this.data = new Dictionary<object[], decimal> (new ArrayEqualityComparer ());
		}


		/// <summary>
		/// Gets the sequence of <see cref="AbstractDimension"/> which are the dimensions that defines
		/// this instance, sorted in the same order as the order required by the getter/setter.
		/// </summary>
		public IEnumerable<AbstractDimension> Dimensions
		{
			get
			{
				return this.dimensions;
			}
		}


		/// <summary>
		/// Gets the sequence of all the possible keys that might be used to set a value to this
		/// instance. This is basically the Carthesian Product of the values of the dimensions of this
		/// instance.
		/// </summary>
		public IEnumerable<object[]> PossibleKeys
		{
			get
			{
				return this.GenerateCarthesianProduct (this.dimensions);
			}
		}


		/// <summary>
		/// Gets or sets the value associated with the given key.
		/// </summary>
		/// <remarks>
		/// When setting a value, the key must correspond to an exact key in the table, but when
		/// getting a value, the key will be rounded for each dimension according to the strategy
		/// defined by the dimensions, so the key can be somewhat different from an exact value.
		/// </remarks>
		/// <param name="key">The object used to address elements.</param>
		/// <returns>The value for the given key.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> is invalid.</exception>
		public decimal? this[params object[] key]
		{
			get
			{
				this.CheckNearestKey (key);

				return this.GetValue (this.GetNearestKey (key));
			}
			set
			{
				this.CheckKey (key);

				this.SetValue (key.ToArray (), value);
			}
		}


		/// <summary>
		/// Tells whether the value defined by the given key (possibly after rounding) is defined.
		/// </summary>
		/// <param name="key">The key whose value definition to check.</param>
		/// <returns><c>true</c> if there is a value defined for the given key after rounding.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> is invalid.</exception>
		public bool IsNearestValueDefined(params object[] key)
		{
			this.CheckNearestKey (key);

			return this.data.ContainsKey (this.GetNearestKey (key));
		}


		/// <summary>
		/// Tells whether the value defined by the given key (without rounding) is defined.
		/// </summary>
		/// <param name="key">The key whose value definition to check.</param>
		/// <returns><c>true</c> if there is a value defined for the given key without rounding.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> is invalid.</exception>
		public bool IsValueDefined(params object[] key)
		{
			this.CheckKey (key);

			return this.data.ContainsKey (key);
		}


		/// <summary>
		/// Generates the Carthesian Product of the given sequence of <see cref="AbstractDimension"/>.
		/// </summary>
		/// <param name="dimensions">The sequence of <see cref="AbstractDimension"/> whose Carthesian Product to compute.</param>
		/// <returns>The Carthesian Product of the given <see cref="AbstractDimension"/>.</returns>
		private IEnumerable<object[]> GenerateCarthesianProduct(IList<AbstractDimension> dimensions)
		{
			if (dimensions.Count == 0)
			{
				yield return new object[0];
			}
			else
			{
				var head = dimensions.First ();
				var tail = dimensions.Skip (1).ToList ();

				foreach (object[] tailKey in this.GenerateCarthesianProduct (tail))
				{
					foreach (object headKey in head.Values)
					{
						object[] key = new object[dimensions.Count];

						key[0] = headKey;
						tailKey.CopyTo (key, 1);

						yield return key;
					}
				}
			}
		}


		/// <summary>
		/// Checks if the given key corresponds to an exact key for the current instance.
		/// </summary>
		/// <param name="key">The key to check.</param>
		private void CheckKey(params object[] key)
		{
			this.CheckKey ((d, o) => d.IsValueDefined (o), key);
		}


		/// <summary>
		/// Checks if the given key corresponds to an key for the current instance, after rounding.
		/// </summary>
		/// <param name="key">The key to check.</param>
		private void CheckNearestKey(params object[] key)
		{
			this.CheckKey ((d, o) => d.IsNearestValueDefined (o), key);
		}


		/// <summary>
		/// Checks if the given key is valid, given a function that checks it for a given dimension.
		/// </summary>
		/// <param name="check">The function that given part of the key and a dimension, checks if it is valid or not.</param>
		/// <param name="key">The key to check.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="key"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> contains <c>null</c> elements.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> does not contains the required number of elements.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> contains an invalid element.</exception>
		private void CheckKey(System.Func<AbstractDimension, object, bool> check, params object[] key)
		{
			key.ThrowIfNull ("key");
			key.ThrowIf (k => k.Any (e => e == null), "Null element in key");

			if (key.Length != this.dimensions.Count)
			{
				throw new System.ArgumentException ("Invalid number of element in key");
			}

			for (int i = 0; i < key.Length; i++)
			{
				if (!check (this.dimensions[i], key[i]))
				{
					throw new System.ArgumentException ("Invalid element in key at position " + i);
				}
			}
		}


		/// <summary>
		/// Rounds the given key for each of its dimensions.
		/// </summary>
		/// <param name="key">The key to round.</param>
		/// <returns>The rounded key.</returns>
		private object[] GetNearestKey(params object[] key)
		{
			object[] nearestKey = new object[key.Length];

			for (int i = 0; i < key.Length; i++)
			{
				nearestKey[i] = this.dimensions[i].GetNearestValue (key[i]);
			}

			return nearestKey;
		}


		/// <summary>
		/// Gets the value for the given key or <c>null</c> if it is undefined.
		/// </summary>
		/// <param name="key">The key whose value to get.</param>
		/// <returns>The value for the given key.</returns>
		private decimal? GetValue(object[] key)
		{
			decimal value;

			bool found = this.data.TryGetValue (key, out value);

			if (found)
			{
				return value;
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// Sets the value for the given key.
		/// </summary>
		/// <param name="key">The key whose value to set.</param>
		/// <param name="value">The value to set.</param>
		private void SetValue(object[] key, decimal? value)
		{
			if (value.HasValue)
			{
				this.data[key] = value.Value;
			}
			else
			{
				this.data.Remove (key);
			}
		}


		/// <summary>
		/// Builds an <see cref="XElement"/> that describes the current instance and might be used
		/// to rebuild a similar one with the <see cref="DimensionTable.XmlImport"/> method.
		/// </summary>
		/// <returns>The <see cref="XElement"/> that describes the current instance.</returns>
		public XElement XmlExport()
		{
			XElement xDimensionTable = new XElement (XmlConstants.DimensionTableTag);

			xDimensionTable.Add (this.GetXmlHeader ());
			xDimensionTable.Add (this.GetXmlDimensions ());
			xDimensionTable.Add (this.GetXmlData ());

			return xDimensionTable;
		}


		/// <summary>
		/// that is the header of the XML definition of this instance.
		/// </summary>
		/// <returns>The <see cref="XElement"/> for the header of the XML definition of this instance.</returns>
		private XElement GetXmlHeader()
		{
			XElement xHeader = new XElement (XmlConstants.HeaderTag);

			xHeader.Add (this.GetXmlVersion (XmlConstants.VersionValue));

			return xHeader;
		}


		/// <summary>
		/// Gets the <see cref="XElement"/> that contains the version of the XML description.
		/// </summary>
		/// <param name="version">The version number to use.</param>
		/// <returns>The <see cref="XElement"/> for the version.</returns>
		private XElement GetXmlVersion(string version)
		{
			XElement xVersion = new XElement (XmlConstants.VersionTag);

			xVersion.SetValue (version);

			return xVersion;
		}


		/// <summary>
		/// Gets the <see cref="XElement"/> that contains the definitions of the dimensions of this
		/// instance.
		/// </summary>
		/// <returns>The <see cref="XElement"/> for the dimensions.</returns>
		private XElement GetXmlDimensions()
		{
			XElement xDimensions = new XElement (XmlConstants.DimensionsTag);

			foreach (AbstractDimension dimension in this.dimensions)
			{
				xDimensions.Add (dimension.XmlExport ());
			}

			return xDimensions;
		}


		/// <summary>
		/// Gets the <see cref="XElement"/> that contains the data of this instance.
		/// </summary>
		/// <returns>The <see cref="XElement"/> that contains the data.</returns>
		private XElement GetXmlData()
		{
			string values = this.JoinValues ();

			XElement xData = new XElement (XmlConstants.DataTag);
			
			xData.SetAttributeValue (XmlConstants.ValuesTag, values);

			return xData;
		}


		/// <summary>
		/// Joins the values of this instance as a <see cref="System.String"/> of values delimited by a separator.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the values of this instance.</returns>
		private string JoinValues()
		{
			var values = this.PossibleKeys
				.Select (k => this.GetValue (k))
				.Select (v => (v.HasValue) ? InvariantConverter.ConvertToString (v) : "");

			return string.Join(XmlConstants.ValueSeparator, values);
		}


		/// <summary>
		/// Builds a new instance of <see cref="DimensionTable"/> based on an <see cref="XElement"/>
		/// that has been generated by the <see cref="DimensionTable.XmlExport"/> method.
		/// </summary>
		/// <param name="xDimensionTable">The XML data.</param>
		/// <returns>The <see cref="DimensionTable"/>.</returns>
		/// <exception cref="System.ArgumentException">If the given xml data is invalid.</exception>
		public static DimensionTable XmlImport(XElement xDimensionTable)
		{
			xDimensionTable.ThrowIfNull ("xDimensionTable");

			DimensionTable.CheckXmlDimensionTable (xDimensionTable);

			XElement xHeader = xDimensionTable.Element (XmlConstants.HeaderTag);
			DimensionTable.CheckXmlHeader (xHeader);

			XElement xDimensions = xDimensionTable.Element (XmlConstants.DimensionsTag);
			var dimensions = DimensionTable.ExtractXmlDimensions (xDimensions);

			XElement xData = xDimensionTable.Element (XmlConstants.DataTag);
			List<decimal?> values = DimensionTable.ExtractXmlData (xData).ToList();

			DimensionTable table = new DimensionTable (dimensions.ToArray ());

			List<object[]> keys = table.PossibleKeys.ToList ();

			if (values.Count != keys.Count)
			{
				throw new System.ArgumentException ("Invalid xml data");
			}

			for (int i = 0; i < keys.Count; i++)
			{
				table.SetValue (keys[i], values[i]);
			}

			return table;
		}


		/// <summary>
		/// Checks that the tag of the XML data defining an <see cref="DimensionTable"/> is valid.
		/// </summary>
		/// <param name="XDimensionTable">The <see cref="XElement"/> to check.</param>
		/// <exception cref="System.ArgumentException">If the tag is not valid.</exception>
		private static void CheckXmlDimensionTable(XElement XDimensionTable)
		{
			if (XDimensionTable.Name != XmlConstants.DimensionTableTag)
			{
				throw new System.ArgumentException ("Invalid xml data");
			}
		}


		/// <summary>
		/// Checks the header of the XML data defining an <see cref="DimensionTable"/>.
		/// </summary>
		/// <param name="xHeader">The <see cref="XElement"/> to check.</param>
		/// <exception cref="System.ArgumentException">If the header is invalid.</exception>
		private static void CheckXmlHeader(XElement xHeader)
		{
			XElement xVersion = xHeader.Element (XmlConstants.VersionTag);

			string version = xVersion.Value;

			if (version != XmlConstants.VersionValue)
			{
				throw new System.ArgumentException ("Invalid xml data");
			}
		}


		/// <summary>
		/// Extracts the definition of the dimensions out of the XML data.
		/// </summary>
		/// <param name="xDimensions">The <see cref="XElement"/> that defines the dimensions.</param>
		/// <returns>The extracted sequence of <see cref="AbstractDimension"/>.</returns>
		private static IEnumerable<AbstractDimension> ExtractXmlDimensions(XElement xDimensions)
		{
			foreach (XElement xDimension in xDimensions.Elements ())
			{
				yield return AbstractDimension.XmlImport (xDimension);
			}
		}


		/// <summary>
		/// Extracts the values of the <see cref="DimensionTable"/> out of the XML data.
		/// </summary>
		/// <param name="xData">The <see cref="XElement"/> that defines the data.</param>
		/// <returns>The extracted sequence of values.</returns>
		private static IEnumerable<decimal?> ExtractXmlData(XElement xData)
		{
			return xData.Attribute (XmlConstants.ValuesTag).Value
				.Split (XmlConstants.ValueSeparator)
				.Select (v => (v.Length > 0) ? InvariantConverter.ConvertFromString<decimal> (v) : (decimal?) null);
		}


		/// <summary>
		/// The constants used to defined XML elements in the XMl serialization of a
		/// <see cref="DimensionTable"/>.
		/// </summary>
		private static class XmlConstants
		{
			public static readonly string DimensionTableTag = "dimensionTable";
			public static readonly string HeaderTag = "header";
			public static readonly string DimensionsTag = "dimensions";
			public static readonly string DataTag = "data";
			public static readonly string VersionTag = "dimensionTable";
			public static readonly string ValuesTag = "values";
			public static readonly string VersionValue = "1.0.0";
			public static readonly string ValueSeparator = ";";
		}


		/// <summary>
		/// The sequence of <see cref="AbstractDimension"/> that are the dimensions of this instance,
		/// ordered by their name.
		/// </summary>
		private List<AbstractDimension> dimensions;


		/// <summary>
		/// The values of the current instance.
		/// </summary>
		private IDictionary<object[], decimal> data;


	}


}
