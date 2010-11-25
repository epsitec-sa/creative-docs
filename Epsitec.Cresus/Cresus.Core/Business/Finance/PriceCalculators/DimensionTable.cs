using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;

using System.Xml.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	// TODO Add function to create a new "similar" table from a given one, plus or minus a
	// dimension?
	// Marc


	internal sealed class DimensionTable
	{
		
		
		public DimensionTable(params AbstractDimension[] dimensions)
		{
			dimensions.ThrowIfNull ("dimensions");
			dimensions.ThrowIf (d => d.Length < 1, "dimensions must contain at least one element.");
			dimensions.ThrowIf (dims => dims.Any (dim => dim == null), "dimensions cannot contain null elements.");

			this.dimensions = dimensions.OrderBy (d => d.Name).ToList ();

			this.data = new Dictionary<object[], decimal> (new ArrayEqualityComparer ());
		}


		public IEnumerable<AbstractDimension> Dimensions
		{
			get
			{
				return this.dimensions;
			}
		}


		public IEnumerable<object[]> PossibleKeys
		{
			get
			{
				return this.GeneratePossibleKeys (this.dimensions);
			}
		}


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


		public bool IsNearestValueDefined(params object[] key)
		{
			this.CheckNearestKey (key);

			return this.data.ContainsKey (this.GetNearestKey (key));
		}


		public bool IsValueDefined(params object[] key)
		{
			this.CheckKey (key);

			return this.data.ContainsKey (key);
		}


		private IEnumerable<object[]> GeneratePossibleKeys(IList<AbstractDimension> dimensions)
		{
			if (dimensions.Count == 0)
			{
				yield return new object[0];
			}
			else
			{
				var head = dimensions.First ();
				var tail = dimensions.Skip (1).ToList ();

				foreach (object[] tailKey in this.GeneratePossibleKeys (tail))
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


		private void CheckKey(params object[] key)
		{
			this.CheckKey ((d, o) => d.IsValueDefined (o), key);
		}


		private void CheckNearestKey(params object[] key)
		{
			this.CheckKey ((d, o) => d.IsNearestValueDefined (o), key);
		}


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


		private object[] GetNearestKey(params object[] key)
		{
			object[] nearestKey = new object[key.Length];

			for (int i = 0; i < key.Length; i++)
			{
				nearestKey[i] = this.dimensions[i].GetNearestValue (key[i]);
			}

			return nearestKey;
		}


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


		public XElement XmlExport()
		{
			XElement xDimensionTable = new XElement (XmlConstants.DimensionTableTag);

			xDimensionTable.Add (this.GetXmlHeader ());
			xDimensionTable.Add (this.GetXmlDimensions ());
			xDimensionTable.Add (this.GetXmlData ());

			return xDimensionTable;
		}


		private XElement GetXmlHeader()
		{
			XElement xHeader = new XElement (XmlConstants.HeaderTag);

			xHeader.Add (this.GetXmlVersion (XmlConstants.VersionValue));

			return xHeader;
		}


		private XElement GetXmlVersion(string version)
		{
			XElement xVersion = new XElement (XmlConstants.VersionTag);

			xVersion.SetValue (version);

			return xVersion;
		}


		private XElement GetXmlDimensions()
		{
			XElement xDimensions = new XElement (XmlConstants.DimensionsTag);

			foreach (AbstractDimension dimension in this.dimensions)
			{
				xDimensions.Add (dimension.XmlExport ());
			}

			return xDimensions;
		}


		private XElement GetXmlData()
		{
			string values = this.JoinValues ();

			XElement xData = new XElement (XmlConstants.DataTag);
			
			xData.SetAttributeValue (XmlConstants.ValuesTag, values);

			return xData;
		}


		private string JoinValues()
		{
			var values = this.PossibleKeys
				.Select (k => this.GetValue (k))
				.Select (v => (v.HasValue) ? InvariantConverter.ConvertToString (v) : "");

			return string.Join(XmlConstants.ValueSeparator, values);
		}


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


		private static void CheckXmlDimensionTable(XElement XDimensionTable)
		{
			if (XDimensionTable.Name != XmlConstants.DimensionTableTag)
			{
				throw new System.ArgumentException ("Invalid xml data");
			}
		}


		private static void CheckXmlHeader(XElement xHeader)
		{
			XElement xVersion = xHeader.Element (XmlConstants.VersionTag);

			string version = xVersion.Value;

			if (version != XmlConstants.VersionValue)
			{
				throw new System.ArgumentException ("Invalid xml data");
			}
		}


		private static IEnumerable<AbstractDimension> ExtractXmlDimensions(XElement xDimensions)
		{
			foreach (XElement xDimension in xDimensions.Elements ())
			{
				yield return AbstractDimension.XmlImport (xDimension);
			}
		}


		private static IEnumerable<decimal?> ExtractXmlData(XElement xData)
		{
			return xData.Attribute (XmlConstants.ValuesTag).Value
				.Split (XmlConstants.ValueSeparator)
				.Select (v => (v.Length > 0) ? InvariantConverter.ConvertFromString<decimal> (v) : (decimal?) null);
		}


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


		private List<AbstractDimension> dimensions;


		private IDictionary<object[], decimal> data;


	}


}
