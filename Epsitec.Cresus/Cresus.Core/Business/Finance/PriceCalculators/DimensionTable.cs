using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;

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

				decimal value;

				bool found = this.data.TryGetValue (this.GetNearestKey (key), out value);

				if (found)
				{
					return value;
				}
				else
				{
					return null;
				}
			}
			set
			{
				this.CheckKey (key);

				if (value.HasValue)
				{
					this.data[key.ToArray()] = value.Value;
				}
				else
				{
					this.data.Remove (key.ToArray ()); 
				}
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


		//public void Export(XmlWriter xmlWriter)
		//{
		//    this.WritePriceCalculatorStart (xmlWriter);
		//    this.WriteHeader (xmlWriter);
		//    this.WriteDefinition (xmlWriter);
		//    this.WriteData (xmlWriter);
		//    this.WritePriceCalculatorEnd (xmlWriter);
		//}


		//private void WritePriceCalculatorStart(XmlWriter xmlWriter)
		//{
		//    xmlWriter.WriteStartElement ("priceCalculatorTable");
		//}

		
		//private void WriteHeader(XmlWriter xmlWriter)
		//{
		//    this.WriteHeaderStart (xmlWriter);
		//    this.WriteHeaderVersion(xmlWriter, "1.0.0");
		//    this.WriteHeaderEnd (xmlWriter);
		//}


		//private void WriteHeaderStart(XmlWriter xmlWriter)
		//{
		//    xmlWriter.WriteStartElement ("header");
		//}


		//private void WriteHeaderVersion(XmlWriter xmlWriter, string version)
		//{
		//    xmlWriter.WriteStartElement ("version");
		//    xmlWriter.WriteValue (version);
		//    xmlWriter.WriteEndElement ();
		//}

		//private void WriteHeaderEnd(XmlWriter xmlWriter)
		//{
		//    xmlWriter.WriteEndElement ();
		//}


		//private void WriteDefinition(XmlWriter xmlWriter)
		//{
		//    this.WriteDefinitionStart(xmlWriter);

		//    foreach (PriceCalculatorDimension dimension in this.dimensions)
		//    {
		//        dimension.WriteDimension (xmlWriter);
		//    }

		//    this.WriteDefinitionEnd(xmlWriter);
		//}


		//private void WriteDefinitionStart(XmlWriter xmlWriter)
		//{
		//    xmlWriter.WriteStartElement ("dimensions");
		//}


		//private void WriteDefinitionEnd(XmlWriter xmlWriter)
		//{
		//    xmlWriter.WriteEndElement ();
		//}


		//private void WriteData(XmlWriter xmlWriter)
		//{
		//    this.WriteDataStart (xmlWriter);

		//    foreach (object[] key in this.ExactKeys)
		//    {
		//        this.WritePoint (xmlWriter, key);
		//    }

		//    this.WriteDataEnd (xmlWriter);
		//}


		//private void WriteDataStart(XmlWriter xmlWriter)
		//{
		//    xmlWriter.WriteStartElement ("data");
		//}


		//private void WritePoint(XmlWriter xmlWriter, object[] key)
		//{
		//    decimal value = this[key];

		//    xmlWriter.WriteStartElement ("point");
		//    xmlWriter.WriteValue (InvariantConverter.ConvertToString (value));
		//    xmlWriter.WriteEndElement ();
		//}


		//private void WriteDataEnd(XmlWriter xmlWriter)
		//{
		//    xmlWriter.WriteEndElement ();
		//}


		//private void WritePriceCalculatorEnd(XmlWriter xmlWriter)
		//{
		//    xmlWriter.WriteEndElement ();
		//}


		//public static PriceCalculatorTable Import(XmlWriter xmlReader)
		//{
		//    throw new System.NotImplementedException ();
		//}


		private List<AbstractDimension> dimensions;


		private IDictionary<object[], decimal> data;


	}


}
