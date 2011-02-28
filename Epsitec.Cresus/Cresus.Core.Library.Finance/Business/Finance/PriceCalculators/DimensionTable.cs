//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;

using System.Xml.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	// TODO Comment this class.
	// Marc


	/// <summary>
	/// The <see cref="DimensionTable"/> class represents a table holding <see cref="System.Decimal"/>
	/// values, which can have an arbitrary number of user defined dimensions.
	/// </summary>
	public sealed class DimensionTable
	{


		public DimensionTable()
			: this (new AbstractDimension[0])
		{
		}


		/// <summary>
		/// Builds a new <see cref="DimensionTable"/> with the given <see cref="AbstractDimension"/>
		/// as dimensions.
		/// </summary>
		/// <remarks>
		/// Note that the given dimensions will be re ordered using their name as sorting criterion.
		/// </remarks>
		/// <param name="dimensions">The <see cref="AbstractDimension"/> of the new instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dimensions"/> is <c>null</c>.</exception>
		public DimensionTable(params AbstractDimension[] dimensions)
		{
			dimensions.ThrowIfNull ("dimensions");

			this.dimensions = new List<AbstractDimension> ();
			this.values = new Dictionary<string[], decimal> (new ArrayEqualityComparer<string> ());

			foreach (AbstractDimension dimension in dimensions)
			{
				this.AddDimension (dimension);
			}
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


		public int DimensionCount
		{
			get
			{
				return this.dimensions.Count;
			}
		}


		/// <summary>
		/// Gets the sequence of all the possible keys that might be used to set a value to this
		/// instance. This is basically the Carthesian Product of the values of the dimensions of this
		/// instance.
		/// </summary>
		public IEnumerable<string[]> Keys
		{
			get
			{
				return this.GenerateCarthesianProduct (this.dimensions);
			}
		}


		public IEnumerable<KeyValuePair<string[], decimal>> DefinedEntries
		{
			get
			{
				return this.values.Select (kvp => new KeyValuePair<string[], decimal> (this.GetExternalKey (kvp.Key), kvp.Value));
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
		public decimal? this[params string[] key]
		{
			get
			{
				this.CheckKey (key);

				return this.GetValue (key);
			}
			set
			{
				this.CheckKey (key);

				this.SetValue (key.ToArray (), value);
			}
		}


		public void AddDimension(AbstractDimension dimension)
		{
			this.InsertDimension (this.dimensions.Count, dimension);
		}


		public void InsertDimension(int index, AbstractDimension dimension)
		{
			dimension.ThrowIfNull ("dimension");
			dimension.ThrowIf (d => this.dimensions.Contains (d), "dimension is already within this instance.");

			this.dimensions.Insert (index, dimension);

			this.internalIndexes = Enumerable.Range (0, this.dimensions.Count).ToDictionary (i => this.dimensions[i], i => i);
			this.values.Clear ();

			dimension.AddToDimensionTable (this);
		}


		public void RemoveDimension(AbstractDimension dimension)
		{
			int index = this.GetIndexOfDimension (dimension);

			this.RemoveDimensionAt (index);
		}


		public void RemoveDimensionAt(int index)
		{
			AbstractDimension dimension = this.GetDimensionAt (index);
			
			this.dimensions.RemoveAt (index);

			this.internalIndexes = Enumerable.Range (0, this.dimensions.Count).ToDictionary (i => this.dimensions[i], i => i);
			this.values.Clear ();

			dimension.RemoveFromDimensionTable (this);
		}


		public void SwapDimensions(AbstractDimension dimension1, AbstractDimension dimension2)
		{
			int index1 = this.GetIndexOfDimension (dimension1);
			int index2 = this.GetIndexOfDimension (dimension2);

			this.SwapDimensionsAt (index1, index2);
		}


		public void SwapDimensionsAt(int index1, int index2)
		{
			AbstractDimension dimension1 = this.GetDimensionAt (index1);
			AbstractDimension dimension2 = this.GetDimensionAt (index2);

			this.dimensions[index1] = dimension2;
			this.dimensions[index2] = dimension1;
		}


		public bool ContainsDimension(AbstractDimension dimension)
		{
			dimension.ThrowIfNull ("dimension");

			return this.dimensions.Contains (dimension);
		}


		public int GetIndexOfDimension(AbstractDimension dimension)
		{
			dimension.ThrowIfNull ("dimension");
			dimension.ThrowIf (d => !this.dimensions.Contains (d), "dimension is not within this instance.");

			return this.dimensions.IndexOf (dimension);
		}


		public AbstractDimension GetDimensionAt(int index)
		{
			index.ThrowIf (i => i < 0 || i >= this.dimensions.Count, "Index is out of range.");

			return this.dimensions[index];
		}


		/// <summary>
		/// Tells whether the value defined by the given key (without rounding) is defined.
		/// </summary>
		/// <param name="key">The key whose value definition to check.</param>
		/// <returns><c>true</c> if there is a value defined for the given key without rounding.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> is invalid.</exception>
		public bool IsValueDefined(params string[] key)
		{
			this.CheckKey (key);

			return this.values.ContainsKey (key);
		}


		public bool IsKeyValid(params string[] key)
		{
			key.ThrowIfNull ("key");

			bool valid = (key.Length == this.dimensions.Count);

			for (int i = 0; i < key.Length && valid; i++)
			{
				valid = this.dimensions[i].Contains (key[i]);
			}

			return valid;
		}


		public bool IsKeyRoundable(params string[] key)
		{
			key.ThrowIfNull ("key");
			key.ThrowIf (k => k.Length != this.dimensions.Count, "Invalid key length");

			bool roundable = true;

			for (int i = 0; i < key.Length && roundable; i++)
			{
				roundable = this.dimensions[i].IsValueRoundable (key[i]);
			}

			return roundable;
		}


		/// <summary>
		/// Rounds the given key for each of its dimensions.
		/// </summary>
		/// <param name="key">The key to round.</param>
		/// <returns>The rounded key.</returns>
		public string[] GetRoundedKey(params string[] key)
		{
			key.ThrowIfNull ("key");
			key.ThrowIf (k => k.Length != this.dimensions.Count, "Invalid key length");

			string[] nearestKey = new string[key.Length];

			for (int i = 0; i < key.Length; i++)
			{
				nearestKey[i] = this.dimensions[i].GetRoundedValue (key[i]);
			}

			return nearestKey;
		}


		public string[] GetKeyFromIndexes(params int[] indexes)
		{
			indexes.ThrowIfNull ("indexes");
			indexes.ThrowIf (k => k.Length != this.dimensions.Count, "Invalid indexes length");

			string[] key = new string[indexes.Length];

			for (int i = 0; i < indexes.Length; i++)
			{
				key[i] = this.dimensions[i].GetValueAt (indexes[i]);
			}

			return key;
		}


		public int[] GetIndexesFromKey(params string[] key)
		{
			key.ThrowIfNull ("key");
			key.ThrowIf (k => k.Length != this.dimensions.Count, "Invalid key length");

			int[] indexes = new int[key.Length];

			for (int i = 0; i < key.Length; i++)
			{
				indexes[i] = this.dimensions[i].GetIndexOf (key[i]);
			}

			return indexes;
		}


		internal void NotifyDimensionValueAdded(AbstractDimension dimension, string value)
		{
			// Nothing to do here.
		}


		internal void NotifyDimensionValueRemoved(AbstractDimension dimension, string value)
		{
			int internalIndexOfDimension = this.internalIndexes[dimension];

			List<string[]> keysToRemove = this.values.Keys
				.Where (k => k[internalIndexOfDimension] == value)
				.ToList ();

			foreach (string[] key in keysToRemove)
			{
				this.values.Remove (key);
			}
		}


		/// <summary>
		/// Generates the Carthesian Product of the given sequence of <see cref="AbstractDimension"/>.
		/// </summary>
		/// <param name="dimensions">The sequence of <see cref="AbstractDimension"/> whose Carthesian Product to compute.</param>
		/// <returns>The Carthesian Product of the given <see cref="AbstractDimension"/>.</returns>
		private IEnumerable<string[]> GenerateCarthesianProduct(IList<AbstractDimension> dimensions)
		{
			if (dimensions.Count == 0)
			{
				yield return new string[0];
			}
			else
			{
				var head = dimensions.First ();
				var tail = dimensions.Skip (1).ToList ();

				foreach (string[] tailKey in this.GenerateCarthesianProduct (tail))
				{
					foreach (string headKey in head.Values)
					{
						string[] key = new string[dimensions.Count];

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
		private void CheckKey(params string[] key)
		{
			key.ThrowIfNull ("key");

			if (key.Length != this.dimensions.Count)
			{
				throw new System.ArgumentException ("Invalid number of element in key");
			}

			for (int i = 0; i < key.Length; i++)
			{
				if (!this.dimensions[i].Contains (key[i]))
				{
					throw new System.ArgumentException ("Invalid element in key at position " + i);
				}
			}
		}


		private string[] GetInternalKey(params string[] externalKey)
		{
			string[] internalKey = new string[externalKey.Length];

			for (int externalIndex = 0; externalIndex < externalKey.Length; externalIndex++)
			{
				AbstractDimension dimension = this.dimensions[externalIndex];

				int internalIndex = this.internalIndexes[dimension];

				internalKey[internalIndex] = externalKey[externalIndex];
			}

			return internalKey;
		}


		private string[] GetExternalKey(params string[] internalKey)
		{
			string[] externalKey = new string[internalKey.Length];

			for (int internalIndex = 0; internalIndex < internalKey.Length; internalIndex++)
			{
				AbstractDimension dimension = this.internalIndexes.Single (kvp => kvp.Value == internalIndex).Key;

				int externalIndexindex = this.dimensions.IndexOf (dimension);

				externalKey[externalIndexindex] = internalKey[internalIndex];
			}

			return externalKey;
		}


		/// <summary>
		/// Gets the value for the given key or <c>null</c> if it is undefined.
		/// </summary>
		/// <param name="key">The key whose value to get.</param>
		/// <returns>The value for the given key.</returns>
		private decimal? GetValue(string[] key)
		{
			decimal value;

			string[] internalKey = this.GetInternalKey (key);

			bool found = this.values.TryGetValue (internalKey, out value);

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
		private void SetValue(string[] key, decimal? value)
		{
			string[] internalKey = this.GetInternalKey (key);
			
			if (value.HasValue)
			{
				this.values[internalKey] = value.Value;
			}
			else
			{
				this.values.Remove (internalKey);
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
			var values = this.Keys
				.Select (k => this.GetValue (k))
				.Select (v => (v.HasValue) ? InvariantConverter.ConvertToString (v) : "");

			return string.Join (XmlConstants.ValueSeparator, values);
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
			List<decimal?> values = DimensionTable.ExtractXmlData (xData).ToList ();

			DimensionTable table = new DimensionTable (dimensions.ToArray ());

			List<string[]> keys = table.Keys.ToList ();

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


		private readonly List<AbstractDimension> dimensions;
		private readonly IDictionary<string[], decimal> values;
		private IDictionary<AbstractDimension, int> internalIndexes;
	}
}
