using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Xml.Linq;
using System.Collections;


namespace Epsitec.Cresus.Core.Entities
{


	public partial class PriceCalculatorEntity
	{

		
		// TODO Comment this class.
		// Marc


		public decimal? Compute(ArticleDocumentItemEntity articleItem)
		{
			// TODO Add tons of checks in this method and the two methods that it calls to ensure
			// that the table match what we expect (i.e. that its dimensions are correct and that
			// its values are defined) and that the values for the parameters are properly defined
			// and within their bounds?
			// Marc

			decimal? price;

			try
			{
				DimensionTable priceTable = this.GetPriceTable ();
				IDictionary<string, IList<object>> parameterCodesToValues = GetParameterCodesToValues (articleItem);

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


		private IDictionary<string, IList<object>> GetParameterCodesToValues(ArticleDocumentItemEntity articleItem)
		{
			// TODO Move this method elsewhere, because it is more generic than just here. It simply
			// gets the values of the parameters of an article document item.
			// Marc

			var parameterDefinitions = articleItem.ArticleDefinition.ArticleParameterDefinitions;
			var parameterStringValues = ArticleParameterHelper.GetArticleParametersValues (articleItem);
			var parameterCodesToValues = new Dictionary<string, IList<object>> ();

			foreach (var parameterDefinition in parameterDefinitions)
			{
				string parameterCode = parameterDefinition.Code;

				if (!parameterStringValues.ContainsKey (parameterCode))
				{
					parameterStringValues[parameterCode] = "";
				}

				string parameterStringValue= parameterStringValues[parameterCode];

				IList<object> parameterObjectValues = this.GetParameterObjectValues (parameterDefinition, parameterStringValue);

				if (parameterObjectValues.Any ())
				{
					parameterCodesToValues[parameterCode] = parameterObjectValues;
				}
			}

			return parameterCodesToValues;
		}


		private IList<object> GetParameterObjectValues(AbstractArticleParameterDefinitionEntity parameterDefinition, string parameterStringValue)
		{
			var numericParameterDefinition = parameterDefinition as NumericValueArticleParameterDefinitionEntity;
			var enumParameterDefinition = parameterDefinition as EnumValueArticleParameterDefinitionEntity;

			if (numericParameterDefinition != null)
			{
				return this.GetNumericParameterObjectValues (numericParameterDefinition, parameterStringValue);
			}
			else if (enumParameterDefinition != null)
			{
				return this.GetEnumParameterObjectValues (enumParameterDefinition, parameterStringValue);
			}
			else
			{
				throw new System.NotImplementedException ();
			}
		}


		private IList<object> GetNumericParameterObjectValues(NumericValueArticleParameterDefinitionEntity parameterDefinition, string parameterStringValue)
		{
			decimal value = InvariantConverter.ConvertFromString<decimal> (parameterStringValue);

			return new List<object> { value };
		}


		private IList<object> GetEnumParameterObjectValues(EnumValueArticleParameterDefinitionEntity parameterDefinition, string parameterStringValue)
		{
			List<object> parameterObjectValues = AbstractArticleParameterDefinitionEntity.Split (parameterStringValue).ToList<object> ();

			if (!this.CheckNbValuesForCardinality (parameterDefinition.Cardinality, parameterObjectValues.Count))
			{
				throw new System.Exception ("Invalid number of values for parameter");
			}

			return parameterObjectValues;
		}


		private bool CheckNbValuesForCardinality(EnumValueCardinality cardinality, int nbValues)
		{
			switch (cardinality)
			{
				case EnumValueCardinality.ExactlyOne:
					return (nbValues == 1);

				case EnumValueCardinality.ZeroOrOne:
					return (nbValues <= 1);

				case EnumValueCardinality.Any:
					return true;

				case EnumValueCardinality.AtLeastOne:
					return (nbValues >= 1);

				default:
					throw new System.NotImplementedException ();
			}
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
			var parameterDefinitions = articleItem.ArticleDefinition.ArticleParameterDefinitions
							.Where (p => priceTable.Dimensions.Any (d => d.Name == p.Code))
							.ToList ();

			foreach (var parameterDefinition in parameterDefinitions)
			{
				var valueParameterDefinition = parameterDefinition as NumericValueArticleParameterDefinitionEntity;
				var enumParameterDefinition = parameterDefinition as EnumValueArticleParameterDefinitionEntity;

				if (valueParameterDefinition == null && (enumParameterDefinition == null || enumParameterDefinition.Cardinality != EnumValueCardinality.ExactlyOne))
				{
					throw new System.NotSupportedException ();
				}
			}

			object[] key = priceTable.Dimensions
			.Select (d => parameterCodesToValues[d.Name].First ())
			.ToArray ();

			return priceTable[key].Value;
		}


		public void SetPriceTable(DimensionTable priceTable)
		{
			// TODO Add a whole lot of checks on what we expect here? Like look for the article
			// parameters and check that the dimensions are properly defined? That all the values are
			// defined, and such...
			// Marc

			this.SerializedData = this.SerializePriceTable (priceTable);
		}


		public DimensionTable GetPriceTable()
		{
			return this.DeserializePriceTable (this.SerializedData);
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
			string name = parameter.Code;

			var values = AbstractArticleParameterDefinitionEntity.Split (parameter.PreferredValues)
				.Select (v => InvariantConverter.ConvertFromString<decimal> (v));

			return new NumericDimension (name, values, roundingMode);
		}


		public static CodeDimension CreateDimension(EnumValueArticleParameterDefinitionEntity parameter)
		{
			if (parameter.Cardinality != EnumValueCardinality.ExactlyOne)
			{
				throw new System.ArgumentException ("This method must be called with an enum parameter with cardinality exactly one.");
			}

			string name = parameter.Code;

			string[] values = AbstractArticleParameterDefinitionEntity.Split (parameter.Values);

			return new CodeDimension (name, values);
		}


		public static DimensionTable CreatePriceTable(NumericValueArticleParameterDefinitionEntity parameter, IDictionary<decimal, decimal> parameterCodesToValues, RoundingMode roundingMode)
		{
			// TODO Check that the given values match the parameter values.
			
			string name = parameter.Code;
			List<decimal> values = parameterCodesToValues.Keys.ToList ();

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
			// TODO Check that the given values match the parameter values.
			
			string name = parameter.Code;
			List<string> values = parameterCodesToValues.Keys.ToList ();

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
