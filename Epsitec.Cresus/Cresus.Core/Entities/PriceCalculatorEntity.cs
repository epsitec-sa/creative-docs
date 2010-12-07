//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace Epsitec.Cresus.Core.Entities
{
	/// <summary>
	/// The <see cref="PriceCalculatorEntity"/> provides tools to compute the price of an
	/// <see cref="ArticleDocumentItemEntity"/> based on the values of its parameters. Basically an
	/// instance of this class contains <see cref="DimensionTable"/> which define prices for some
	/// combinations of some of its parameter values.
	/// </summary>
	/// <remarks>
	/// There are many remarks to do with respect to this class.
	/// - Each dimension of the price table corresponds to a parameter of the article definition. For
	///   numeric parameters, the dimension points must correspond to the parameter preferred values.
	///   For enum parameters, the dimension points must correspond to the parameter values.
	/// - The dimensions in the price table are ordered by their name, and their name is equal to the
	///   code of the dimension they match.
	/// - Enum parameters are considered as options. So if one has more than one value, then the
	///   result of the price is the sum of the price for each of its values.
	/// - The field <see cref="PriceCalculatorEntity.SerializedData"/> should never be accessed
	///   directly. So don't touch it, otherwise you'll risk to mess everything up.
	/// - The price tables can have one or more dimension. There are no restrictions on the dimensions
	///   of single dimension tables. But for multi dimension tables, the dimensions must correspond
	///   to a numeric parameter or to an enum parameter with cardinality "exactly one".
	/// </remarks>
	public partial class PriceCalculatorEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Code, this.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Code, this.Name);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Code, this.Name.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus()
		{
			//	We consider a location to be empty if it has neither postal code, nor
			//	location name; a location with just a coutry or region is still empty.
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Code.GetEntityStatus ());
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}

		
		/// <summary>
		/// Computes the contribution made to the price of the given <see cref="ArticleDocumentItemEntity"/>
		/// defined by this instance.
		/// </summary>
		/// <param name="articleItem">The article whose price to compute.</param>
		/// <returns>The price, or <c>null</c> if it is not defined.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="articleItem"/> is <c>null</c>.</exception>
		/// <exception cref="System.NotSupportedException">If the price table of this instance is invalid.</exception>
		public decimal? Compute(ArticleDocumentItemEntity articleItem)
		{
			articleItem.ThrowIfNull ("articleItem");

			DimensionTable priceTable = this.GetPriceTable ();

			this.CheckPriceTable (articleItem.ArticleDefinition, priceTable);

			IDictionary<string, IList<object>> parameterCodesToValues = ArticleDocumentItemHelper.GetParameterCodesToValues (articleItem);

			return this.ComputePriceForDimensionTable (priceTable, parameterCodesToValues);
		}


		/// <summary>
		/// Use the given <see cref="DimensionTable"/>, the <see cref="ArticleDocumentItem"/> and
		/// the values of the parameters to compute the price.
		/// </summary>
		/// <param name="priceTable">The table defining the prices for the different combinations of parameters.</param>
		/// <param name="parameterCodesToValues">The mapping between parameter codes and values.</param>
		/// <returns>The computed price, or <c>null</c> if it is not defined.</returns>
		private decimal? ComputePriceForDimensionTable(DimensionTable priceTable, IDictionary<string, IList<object>> parameterCodesToValues)
		{
			int nbDimensions = priceTable.Dimensions.Count ();

			if (nbDimensions == 1)
			{
				return this.ComputePriceForUniDimensonTable (priceTable, parameterCodesToValues);
			}
			else if (nbDimensions > 1)
			{
				return this.ComputePriceForMultiDimensionTable (priceTable, parameterCodesToValues);
			}
			else
			{
				throw new System.NotImplementedException ();
			}
		}


		/// <summary>
		/// Computes the price when the price table contains a single dimension.
		/// </summary>
		/// <param name="priceTable">The price table used for the computation.</param>
		/// <param name="parameterCodesToValues">The mapping between parameter codes and values.</param>
		/// <returns>The computed price or <c>null</c> if it is not defined.</returns>
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


		/// <summary>
		/// Computes the price when the price table contains more than one dimension.
		/// </summary>
		/// <param name="priceTable">The price table used for the computation.</param>
		/// <param name="parameterCodesToValues">The mapping between parameter codes and values.</param>
		/// <returns>The computed price, or <c>null</c> if it is not defined.</returns>
		private decimal? ComputePriceForMultiDimensionTable(DimensionTable priceTable, IDictionary<string, IList<object>> parameterCodesToValues)
		{
			object[] key = priceTable.Dimensions
			.Select (d => parameterCodesToValues[d.Name].Single ())
			.ToArray ();

			decimal? price = null;

			if (priceTable.IsNearestValueDefined (key))
			{
				price = priceTable[key];
			}

			return price;
		}


		/// <summary>
		/// Sets the given <see cref="DimensionTable"/> as the price table used to compute the values
		/// of this instance.
		/// </summary>
		/// <remarks>
		/// There are two things to note. Firstly, you need to call the <see cref="DataContext.SaveChanges"/>
		/// method in order to persist this modification to the database. Secondly, this method works
		/// by value and does not keep a reference to <paramref name="priceTable"/>, therefore any
		/// modification done to <paramref name="priceTable"/> after the call of this method will not
		/// be reported in this instance, you'll need to call this method again in order to do that.
		/// </remarks>
		/// <param name="articleDefinition">The definition of the article targeted by this instance.</param>
		/// <param name="priceTable">The new price table.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="articleDefinition"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="priceTable"/> is <c>null</c>.</exception>
		/// <exception cref="System.NotSupportedException">If <paramref name="priceTable"/> is not valid with respect to <paramref name="articleDefinition"/>.</exception>
		public void SetPriceTable(ArticleDefinitionEntity articleDefinition, DimensionTable priceTable)
		{
			articleDefinition.ThrowIfNull ("articleDefinition");
			priceTable.ThrowIfNull ("priceTable");

			this.CheckPriceTable (articleDefinition, priceTable);

			this.SerializedData = this.SerializePriceTable (priceTable);
		}

		
		/// <summary>
		/// Gets the <see cref="DimensionTable"/> associated with this instance.
		/// </summary>
		/// <remarks>
		/// This method returns a copy of the price table, so feel free to do whatever you want with
		/// it. But that means that you cannot use it directly to modify the price table associated
		/// with this instance.
		/// </remarks>
		/// <returns>The <see cref="DimensionTable"/> used to compute the values of this instance.</returns>
		public DimensionTable GetPriceTable()
		{
			DimensionTable priceTable = null;
			
			if (this.SerializedData != null)
			{
				priceTable = this.DeserializePriceTable (this.SerializedData);
			}

			return priceTable;
		}


		/// <summary>
		/// Checks that the given <see cref="DimensionTable"/> is a valid price table for the given
		/// <see cref="ArticleDefinitionEntity"/>.
		/// </summary>
		/// <param name="articleDefinition">The definition of the article.</param>
		/// <param name="priceTable">The price table to check.</param>
		private void CheckPriceTable(ArticleDefinitionEntity articleDefinition, DimensionTable priceTable)
		{
			var parameterDefinitions = articleDefinition.ArticleParameterDefinitions.ToDictionary (pd => pd.Code, pd => pd);

			int nbDimensions = priceTable.Dimensions.Count ();

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
						throw new System.NotSupportedException ();
					}
				}
				else if (epd != null && cd != null)
				{
					if (!PriceCalculatorEntity.CheckValuesForEnumParameter (epd, cd.Values.Cast<string> ()))
					{
						throw new System.NotSupportedException ();
					}

					if (nbDimensions > 1 && epd.Cardinality != EnumValueCardinality.ExactlyOne)
					{
						throw new System.NotSupportedException ();
					}
				}
				else
				{
					throw new System.NotSupportedException ();
				}
			}
		}


		/// <summary>
		/// Deserializes a <see cref="DimensionTable"/> out of a byte array.
		/// </summary>
		/// <param name="tableDataAsByteArray">The data of the <see cref="DimensionTable"/>.</param>
		/// <returns>The <see cref="DimensionTable"/>.</returns>
		private DimensionTable DeserializePriceTable(byte[] tableDataAsByteArray)
		{
			string tableDataAsXmlString = Encoding.UTF8.GetString (tableDataAsByteArray);
			XElement xTable = XElement.Parse (tableDataAsXmlString);
			DimensionTable table = DimensionTable.XmlImport (xTable);

			return table;
		}


		/// <summary>
		/// Serializes a <see cref="DimensionTable"/> to a byte array.
		/// </summary>
		/// <param name="table">The <see cref="DimensionTable"/> to serialize.</param>
		/// <returns>The byte data.</returns>
		private byte[] SerializePriceTable(DimensionTable table)
		{
			XElement xTable = table.XmlExport ();
			string tableDataAsXmlString = xTable.ToString (SaveOptions.DisableFormatting);
			byte[] tableDataAsByteArray = Encoding.UTF8.GetBytes (tableDataAsXmlString);

			return tableDataAsByteArray;
		}


		/// <summary>
		/// Creates a <see cref="NumericDimension"/> based on the given parameter. This method uses
		/// the preferred values as points on the dimension.
		/// </summary>
		/// <param name="parameter">The parameter on which to base the new dimension.</param>
		/// <param name="roundingMode">The rounding strategy for the dimension.</param>
		/// <returns>The new <see cref="NumericDimension"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="parameter"/> is <c>null</c>.</exception>
		public static NumericDimension CreateDimension(NumericValueArticleParameterDefinitionEntity parameter, RoundingMode roundingMode)
		{
			parameter.ThrowIfNull ("parameter");
			
			string name = parameter.Code;

			var values = AbstractArticleParameterDefinitionEntity.Split (parameter.PreferredValues)
				.Select (v => InvariantConverter.ConvertFromString<decimal> (v));

			return new NumericDimension (name, values, roundingMode);
		}


		/// <summary>
		/// Creates a <see cref="CodeDimension"/> based on the given parameter. This method uses the
		/// defined values as points on the dimension. Only parameters with the cardinality
		/// <see cref="EnumValueCardinality.ExactlyOne"/> can be used with this method.
		/// </summary>
		/// <param name="parameter">The parameter on which to base the new dimension.</param>
		/// <returns>The new <see cref="CodeDimension"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="parameter"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="parameter"/> is not of the appropriate cardinality.</exception>
		public static CodeDimension CreateDimension(EnumValueArticleParameterDefinitionEntity parameter)
		{
			parameter.ThrowIfNull ("parameter");
			parameter.ThrowIf (p => p.Cardinality != EnumValueCardinality.ExactlyOne, "Unsupported cardinality for parameter.");

			string name = parameter.Code;

			string[] values = AbstractArticleParameterDefinitionEntity.Split (parameter.Values);

			return new CodeDimension (name, values);
		}


		/// <summary>
		/// Creates a new single dimension <see cref="DimensionTable"/> which will be the price table,
		/// based on the given parameter and with the given strategy for rounding and the given values.
		/// </summary>
		/// <param name="parameter">The parameter on which to base the single dimension of the new table.</param>
		/// <param name="parameterCodesToValues">The mapping between the parameter values to the values in the table.</param>
		/// <param name="roundingMode">The rounding strategy.</param>
		/// <returns>The new <see cref="DimensionTable"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="parameter"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="parameterCodesToValues"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="parameterCodesToValues"/> does not contains the proper values.</exception>
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


		/// <summary>
		/// Creates a new single dimension <see cref="DimensionTable"/> which will be the price table,
		/// base on the given parameter.
		/// </summary>
		/// <param name="parameter">The parameter on which to base the single dimension of the new table.</param>
		/// <param name="parameterCodesToValues">The mapping between the parameter codes to the values in the table.</param>
		/// <returns>The new <see cref="DimensionTable"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="parameter"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="parameterCodesToValues"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="parameterCodesToValues"/> does not contains the proper values.</exception>
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


		/// <summary>
		/// Builds the key used to access elements in a price table.
		/// </summary>
		/// <param name="parametersToValues">The mapping from the parameter definitions to their actual value.</param>
		/// <returns>The key corresponding to the given mapping.</returns>
		public static object[] CreateKey(IDictionary<AbstractArticleParameterDefinitionEntity, object> parametersToValues)
		{
			parametersToValues.ThrowIfNull ("parametersToValues");

			return parametersToValues
				.OrderBy (kvp => kvp.Key.Code)
				.Select (kvp => kvp.Value)
				.ToArray ();
		}


		/// <summary>
		/// Checks that the given values correspond to the preferred values of the given parameter.
		/// </summary>
		/// <param name="parameterDefinition">The parameter to check.</param>
		/// <param name="values">The set of values to check.</param>
		/// <returns><c>true</c> if the given values correspond to the preferred values of the given parameter, <c>false</c> if they don't.</returns>
		private static bool CheckValuesForNumericParameter(NumericValueArticleParameterDefinitionEntity parameterDefinition, IEnumerable<decimal> values)
		{
			return AbstractArticleParameterDefinitionEntity.Split (parameterDefinition.PreferredValues)
				.Select (v => InvariantConverter.ConvertFromString<decimal> (v))
				.SetEquals (values);
		}


		/// <summary>
		/// Checks that the given values correspond to the values of the given parameter.
		/// </summary>
		/// <param name="parameterDefinition">The parameter to check.</param>
		/// <param name="values">The set of values to check.</param>
		/// <returns><c>true</c> if the given values correspond to the values of the given parameter, <c>false</c> if they don't.</returns>
		private static bool CheckValuesForEnumParameter(EnumValueArticleParameterDefinitionEntity parameterDefinition, IEnumerable<string> values)
		{
			return AbstractArticleParameterDefinitionEntity.Split (parameterDefinition.Values)
				.SetEquals (values);
		}


	}


}
