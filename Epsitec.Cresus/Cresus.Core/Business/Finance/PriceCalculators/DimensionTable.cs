using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Xml;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	// TODO Add argument check and test argument check behavior.
	// Marc


	internal sealed class DimensionTable
	{
		
		
		public DimensionTable(params AbstractDimension[] dimensions)
		{
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


		private void CheckKey(params object[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				if (!this.dimensions[i].IsValueDefined (keys[i]))
				{
					throw new System.ArgumentException ("Invalid value for key " + i);
				}
			}
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


		private void CheckNearestKey(params object[] keys)
		{
			for (int i = 0; i < keys.Length; i++)
			{
				if (!this.dimensions[i].IsNearestValueDefined (keys[i]))
				{
					throw new System.ArgumentException ("Invalid value for key " + i);
				}
			}
		}


		private object[] GetNearestKey(params object[] key)
		{
			object[] nearestKeys = new object[key.Length];

			for (int i = 0; i < key.Length; i++)
			{
				nearestKeys[i] = this.dimensions[i].GetNearestValue (key[i]);
			}

			return nearestKeys;
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


		public void Export(XmlWriter xmlWriter)
		{
			this.WriteDimensionStart (xmlWriter);
			this.WriteHeader (xmlWriter);
			this.WriteDimensions (xmlWriter);
			this.WriteData (xmlWriter);
			this.WriteDimensionTableEnd (xmlWriter);
		}


		private void WriteDimensionStart(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("dimensionTable");
		}


		private void WriteHeader(XmlWriter xmlWriter)
		{
			this.WriteHeaderStart (xmlWriter);
			this.WriteHeaderVersion (xmlWriter, "1.0.0");
			this.WriteHeaderEnd (xmlWriter);
		}


		private void WriteHeaderStart(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("header");
		}


		private void WriteHeaderVersion(XmlWriter xmlWriter, string version)
		{
			xmlWriter.WriteStartElement ("version");
			xmlWriter.WriteValue (version);
			xmlWriter.WriteEndElement ();
		}

		private void WriteHeaderEnd(XmlWriter xmlWriter)
		{
			xmlWriter.WriteEndElement ();
		}


		private void WriteDimensions(XmlWriter xmlWriter)
		{
			this.WriteDimensionsStart (xmlWriter);

			foreach (AbstractDimension dimension in this.dimensions)
			{
				dimension.WriteDimension (xmlWriter);
			}

			this.WriteDimensionsEnd (xmlWriter);
		}


		private void WriteDimensionsStart(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("dimensions");
		}


		private void WriteDimensionsEnd(XmlWriter xmlWriter)
		{
			xmlWriter.WriteEndElement ();
		}


		private void WriteData(XmlWriter xmlWriter)
		{
			string values = this.JoinValues ();

			xmlWriter.WriteStartElement ("data");
			xmlWriter.WriteAttributeString ("values", values);
			xmlWriter.WriteEndElement ();
		}


		private string JoinValues()
		{
			var values = this.PossibleKeys
				.Select (k => this.GetValue (k))
				.Select (v => (v.HasValue) ? InvariantConverter.ConvertToString (v) : "");

			return string.Join(";", values);
		}


		private void WriteDimensionTableEnd(XmlWriter xmlWriter)
		{
			xmlWriter.WriteEndElement ();
		}


		public static DimensionTable Import(XmlWriter xmlReader)
		{
			throw new System.NotImplementedException ();
		}


		private List<AbstractDimension> dimensions;


		private IDictionary<object[], decimal> data;


	}


}
