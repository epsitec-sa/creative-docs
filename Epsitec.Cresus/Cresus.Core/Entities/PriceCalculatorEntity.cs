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

			DimensionTable priceTable = this.GetPriceTable ();

			this.CheckPriceTable (articleItem.ArticleDefinition, priceTable);

			IDictionary<string, IList<object>> parameterCodesToValues = ArticleDocumentItemHelper.GetParameterCodesToValues (articleItem);

			return this.ComputePriceForDimensionTable (articleItem, priceTable, parameterCodesToValues);
		}


		private decimal? ComputePriceForDimensionTable(ArticleDocumentItemEntity articleItem, DimensionTable priceTable, IDictionary<string, IList<object>> parameterCodesToValues)
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


		private decimal? ComputePriceForUniDimensonTable(DimensionTable priceTable, IDictionary<string, IList<object>> parameterCodesToValues)
		{
			decimal? price = 0;

			string code = priceTable.Dimensions.Single ().Name;

			if (parameterCodesToValues.ContainsKey (code))
			{
				foreach (object key in parameterCodesToValues[code])
				{
					if (priceTable.IsNearestValueDefined (key))
					{
						price += priceTable[key];
					}
					else
					{
						price = null;
					}
				}
			}

			return price;
		}


		private decimal? ComputePriceForMultiDimensionTable(ArticleDocumentItemEntity articleItem, DimensionTable priceTable, IDictionary<string, IList<object>> parameterCodesToValues)
		{
			if (!this.CheckParametersForMultiDimensionTable (priceTable, articleItem))
			{
				throw new System.NotSupportedException ();
			}

			object[] key = priceTable.Dimensions
			.Select (d => parameterCodesToValues[d.Name].First ())
			.ToArray ();

			decimal? price = null;

			if (priceTable.IsNearestValueDefined (key))
			{
				price = priceTable[key];
			}

			return price;
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


		public void SetPriceTable(ArticleDefinitionEntity articleDefinition, DimensionTable priceTable)
		{
			articleDefinition.ThrowIfNull ("articleDefinition");
			priceTable.ThrowIfNull ("priceTable");

			this.CheckPriceTable (articleDefinition, priceTable);

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


		private void CheckPriceTable(ArticleDefinitionEntity articleDefinition, DimensionTable priceTable)
		{
			var parameterDefinitions = articleDefinition.ArticleParameterDefinitions.ToDictionary (pd => pd.Code, pd => pd);

			foreach (AbstractDimension dimension in priceTable.Dimensions)
			{
				if (!parameterDefinitions.ContainsKey (dimension.Name))
				{
					throw new System.Exception ();
				}

				var pd = parameterDefinitions[dimension.Name];
				var npd = pd as NumericValueArticleParameterDefinitionEntity;
				var epd = pd as EnumValueArticleParameterDefinitionEntity;
				var nd = dimension as NumericDimension;
				var cd = dimension as CodeDimension;

				if (npd != null && nd != null)
				{
					if (!PriceCalculatorEntity.CheckValuesForNumericParameter (npd, nd.Values.Cast<decimal> ()))
					{
						throw new System.Exception ();
					}
				}
				else if (epd != null && cd != null)
				{
					if (!PriceCalculatorEntity.CheckValuesForEnumParameter (epd, cd.Values.Cast<string> ()))
					{
						throw new System.Exception ();
					}
				}
				else
				{
					throw new System.Exception ();
				}
			}
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

			if (!PriceCalculatorEntity.CheckValuesForNumericParameter (parameter, parameterCodesToValues.Keys))
			{
				throw new System.ArgumentException ();
			}

			NumericDimension dimension = new NumericDimension (parameter.Code, parameterCodesToValues.Keys, roundingMode);
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

			if (!PriceCalculatorEntity.CheckValuesForEnumParameter (parameter, parameterCodesToValues.Keys))
			{
				throw new System.ArgumentException ();
			}

			CodeDimension dimension = new CodeDimension (parameter.Code, parameterCodesToValues.Keys);
			DimensionTable table = new DimensionTable (dimension);

			foreach (var item in parameterCodesToValues)
			{
				table[item.Key] = item.Value;
			}

			return table;
		}


		private static bool CheckValuesForNumericParameter(NumericValueArticleParameterDefinitionEntity parameterDefinition, IEnumerable<decimal> values)
		{
			return AbstractArticleParameterDefinitionEntity.Split (parameterDefinition.PreferredValues)
				.Select (v => InvariantConverter.ConvertFromString<decimal> (v))
				.SetEquals (values);
		}


		private static bool CheckValuesForEnumParameter(EnumValueArticleParameterDefinitionEntity parameterDefinition, IEnumerable<string> values)
		{
			return AbstractArticleParameterDefinitionEntity.Split (parameterDefinition.Values)
				.SetEquals (values);
		}


	}


}
