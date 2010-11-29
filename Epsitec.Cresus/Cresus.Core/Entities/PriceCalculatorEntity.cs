using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Xml.Linq;


namespace Epsitec.Cresus.Core.Entities
{


	public partial class PriceCalculatorEntity
	{

		
		// TODO Comment this class.
		// Marc


		public decimal? Compute(ArticleDocumentItemEntity articleItem)
		{
			articleItem.ThrowIfNull ("articleItem");
			
			// TODO Add tons of checks in this method and the two methods that it calls to ensure
			// that the table match what we expect (i.e. that its dimensions are correct and that
			// its values are defined) and that the values for the parameters are properly defined
			// and within their bounds?
			// Marc

			decimal? price;

			try
			{
				DimensionTable priceTable = this.GetPriceTable ();
				IDictionary<string, IList<object>> parameterCodesToValues = ArticleDocumentItemHelper.GetParameterCodesToValues (articleItem);

				price = this.ComputePriceForDimensionTable (articleItem, priceTable, parameterCodesToValues);
			}
			catch
			{
				// Something bad happened but we don't want to throw an exception. Instead, we simply
				// return a null value.
				// Marc

				price = null;
			}

			return price;
		}


		private decimal ComputePriceForDimensionTable(ArticleDocumentItemEntity articleItem, DimensionTable priceTable, IDictionary<string, IList<object>> parameterCodesToValues)
		{
			int nbDimensions = priceTable.Dimensions.Count ();

			if (nbDimensions == 1)
			{
				return this.ComputePriceForUniDimensonTable (priceTable, parameterCodesToValues);
			}
			else if (nbDimensions > 1)
			{
				return this.ComputePriceForMultiDimensionTable (articleItem, priceTable, parameterCodesToValues);
			}
			else
			{
				throw new System.NotImplementedException ();
			}
		}


		private decimal ComputePriceForUniDimensonTable(DimensionTable priceTable, IDictionary<string, IList<object>> parameterCodesToValues)
		{
			decimal price = 0;

			string code = priceTable.Dimensions.Single ().Name;

			if (parameterCodesToValues.ContainsKey (code))
			{
				foreach (object key in parameterCodesToValues[code])
				{
					price += priceTable[key].Value;
				}
			}

			return price;
		}


		private decimal ComputePriceForMultiDimensionTable(ArticleDocumentItemEntity articleItem, DimensionTable priceTable, IDictionary<string, IList<object>> parameterCodesToValues)
		{
			if (!this.CheckParametersForMultiDimensionTable (priceTable, articleItem))
			{
				throw new System.NotSupportedException ();
			}

			object[] key = priceTable.Dimensions
			.Select (d => parameterCodesToValues[d.Name].First ())
			.ToArray ();

			return priceTable[key].Value;
		}


		private bool CheckParametersForMultiDimensionTable(DimensionTable priceTable, ArticleDocumentItemEntity articleItem)
		{
			return articleItem.ArticleDefinition.ArticleParameterDefinitions
				.Where (pd => priceTable.Dimensions.Any (d => d.Name == pd.Code))
				.All (pd =>
				{
					var vpd = pd as NumericValueArticleParameterDefinitionEntity;
					var epd = pd as EnumValueArticleParameterDefinitionEntity;

					return (vpd != null) || (epd != null && epd.Cardinality == EnumValueCardinality.ExactlyOne);
				});
		}


		public void SetPriceTable(DimensionTable priceTable)
		{
			priceTable.ThrowIfNull ("priceTable");
			
			// TODO Add a whole lot of checks on what we expect here? Like look for the article
			// parameters and check that the dimensions are properly defined? That all the values are
			// defined, and such...
			// Marc

			this.SerializedData = this.SerializePriceTable (priceTable);
		}


		public DimensionTable GetPriceTable()
		{
			DimensionTable priceTable = null;
			
			if (this.SerializedData != null)
			{
				priceTable = this.DeserializePriceTable (this.SerializedData);
			}

			return priceTable;
		}


		private DimensionTable DeserializePriceTable(byte[] tableDataAsByteArray)
		{
			string tableDataAsXmlString = Encoding.UTF8.GetString (tableDataAsByteArray);
			XElement xTable = XElement.Parse (tableDataAsXmlString);
			DimensionTable table = DimensionTable.XmlImport (xTable);

			return table;
		}


		private byte[] SerializePriceTable(DimensionTable table)
		{
			XElement xTable = table.XmlExport ();
			string tableDataAsXmlString = xTable.ToString (SaveOptions.DisableFormatting);
			byte[] tableDataAsByteArray = Encoding.UTF8.GetBytes (tableDataAsXmlString);

			return tableDataAsByteArray;
		}


		public static NumericDimension CreateDimension(NumericValueArticleParameterDefinitionEntity parameter, RoundingMode roundingMode)
		{
			parameter.ThrowIfNull ("parameter");
			
			string name = parameter.Code;

			var values = AbstractArticleParameterDefinitionEntity.Split (parameter.PreferredValues)
				.Select (v => InvariantConverter.ConvertFromString<decimal> (v));

			return new NumericDimension (name, values, roundingMode);
		}


		public static CodeDimension CreateDimension(EnumValueArticleParameterDefinitionEntity parameter)
		{
			parameter.ThrowIfNull ("parameter");
			parameter.ThrowIf (p => p.Cardinality != EnumValueCardinality.ExactlyOne, "Unsupported cardinality for parameter.");

			string name = parameter.Code;

			string[] values = AbstractArticleParameterDefinitionEntity.Split (parameter.Values);

			return new CodeDimension (name, values);
		}


		public static DimensionTable CreatePriceTable(NumericValueArticleParameterDefinitionEntity parameter, IDictionary<decimal, decimal> parameterCodesToValues, RoundingMode roundingMode)
		{
			parameter.ThrowIfNull ("parameter");
			parameterCodesToValues.ThrowIfNull ("parameterCodesToValues");
			
			string name = parameter.Code;
			
			List<decimal> values = AbstractArticleParameterDefinitionEntity.Split (parameter.PreferredValues)
				.Select (v => InvariantConverter.ConvertFromString<decimal> (v))
				.ToList ();

			if (values.Count != parameterCodesToValues.Count)
			{
				throw new System.ArgumentException ();
			}

			if (values.Any (v => !parameterCodesToValues.ContainsKey (v)))
			{
				throw new System.ArgumentException ();
			}

			NumericDimension dimension = new NumericDimension (name, values, roundingMode);
			DimensionTable table = new DimensionTable (dimension);

			foreach (var item in parameterCodesToValues)
			{
				table[item.Key] = item.Value;
			}

			return table;
		}


		public static DimensionTable CreatePriceTable(EnumValueArticleParameterDefinitionEntity parameter, IDictionary<string, decimal> parameterCodesToValues)
		{
			parameter.ThrowIfNull ("parameter");
			parameterCodesToValues.ThrowIfNull ("parameterCodesToValues");

			string name = parameter.Code;
			string[] values = AbstractArticleParameterDefinitionEntity.Split (parameter.Values);

			if (values.Length != parameterCodesToValues.Count)
			{
				throw new System.ArgumentException ();
			}

			if (values.Any (v => !parameterCodesToValues.ContainsKey (v)))
			{
				throw new System.ArgumentException ();
			}

			CodeDimension dimension = new CodeDimension (name, values);
			DimensionTable table = new DimensionTable (dimension);

			foreach (var item in parameterCodesToValues)
			{
				table[item.Key] = item.Value;
			}

			return table;
		}


	}


}
