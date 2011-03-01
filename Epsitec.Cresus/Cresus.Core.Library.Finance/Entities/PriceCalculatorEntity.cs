//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace Epsitec.Cresus.Core.Entities
{
	public partial class PriceCalculatorEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name, "(", this.Informations, ")");
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Name.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Code.GetEntityStatus ());
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
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
		/// <param name="priceTable">The new price table.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="priceTable"/> is <c>null</c>.</exception>
		public void SetPriceTable(DimensionTable priceTable)
		{
			priceTable.ThrowIfNull ("priceTable");
		
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
		/// Deserializes a <see cref="DimensionTable"/> out of a byte array.
		/// </summary>
		/// <param name="tableDataAsByteArray">The data of the <see cref="DimensionTable"/>.</param>
		/// <returns>The <see cref="DimensionTable"/>.</returns>
		private DimensionTable DeserializePriceTable(byte[] tableDataAsByteArray)
		{
			string tableDataAsXmlString = System.Text.Encoding.UTF8.GetString (tableDataAsByteArray);
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
			byte[] tableDataAsByteArray = System.Text.Encoding.UTF8.GetBytes (tableDataAsXmlString);

			return tableDataAsByteArray;
		}


	}


}
