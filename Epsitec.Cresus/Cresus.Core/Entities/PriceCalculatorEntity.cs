using Epsitec.Common.Types;

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


		// TODO What to do in the case where an enum parameter can have no value and when an enum
		// parameter can have multiple values? These case are not implemented and the application
		// will horribly die in that case.
		// Marc


		public decimal Compute(ArticleDocumentItemEntity articleItem)
		{
			// TODO Add tons of checks in this method and the two methods that it calls to ensure
			// that the table match what we expect (i.e. that its dimensions are correct and that
			// its values are defined) and that the values for the parameters are properly defined
			// and within their bounds?
			// Marc

			DimensionTable priceTable = this.GetPriceTable ();
			var parameterCodesToValues = GetParameterCodesToValues (articleItem);

			return this.Compute (priceTable, parameterCodesToValues);
		}


		private Dictionary<string, object> GetParameterCodesToValues(ArticleDocumentItemEntity articleItem)
		{
			// TODO This method might be more efficient if we only get the parameter codes and values
			// of parameter that will actually be used in the computation of the price.

			var parameterDefinitions = articleItem.ArticleDefinition.ArticleParameterDefinitions;
			var parameterStringValues = ArticleParameterHelper.GetArticleParametersValues (articleItem);

			var parameterCodesToValues = new Dictionary<string, object> ();

			foreach (var parameterDefinition in parameterDefinitions)
			{
				string parameterCode = parameterDefinition.Code;
				string parameterStringValue = parameterStringValues[parameterCode];
				object parameterObjectValue;

				if (parameterDefinition is NumericValueArticleParameterDefinitionEntity)
				{
					parameterObjectValue = InvariantConverter.ConvertFromString<decimal> (parameterStringValue);
				}
				else if (parameterDefinition is EnumValueArticleParameterDefinitionEntity)
				{
					parameterObjectValue = parameterStringValue;
				}
				else
				{
					throw new System.NotImplementedException ();
				}

				parameterCodesToValues[parameterCode] = parameterObjectValue;
			}

			return parameterCodesToValues;
		}


		private decimal Compute(DimensionTable priceTable, Dictionary<string, object> parameterCodesToValues)
		{
			object[] key = priceTable.Dimensions
				.Select (d => parameterCodesToValues[d.Name])
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


		public static CodeDimension CreateDimension(EnumValueArticleParameterDefinitionEntity parameter)
		{
			string name = parameter.Code;

			string[] values = AbstractArticleParameterDefinitionEntity.Split (parameter.Values);

			return new CodeDimension (name, values);
		}


		public static NumericDimension CreateDimension(NumericValueArticleParameterDefinitionEntity parameter, RoundingMode roundingMode)
		{
			string name = parameter.Code;

			var values = AbstractArticleParameterDefinitionEntity.Split (parameter.PreferredValues)
				.Select (v => InvariantConverter.ConvertFromString<decimal> (v));

			return new NumericDimension (name, values, roundingMode);
		}


	}


}
