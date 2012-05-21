//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Tableau de données général. De façon interne, les cellules sont des FormattedText.
	/// L'interface permet de les voir sous forme de booléens, de montants ou de pourcentages.
	/// </summary>
	public class DataArray
	{
		public DataArray()
		{
			this.data = new List<Dictionary<ColumnType, FormattedText>> ();
		}


		public void Clear()
		{
			this.data.Clear ();
		}

		public int RowCount
		{
			get
			{
				return this.data.Count;
			}
		}

		public void AddRow()
		{
			this.data.Add (new Dictionary<ColumnType, FormattedText> ());
		}

		public void InsertRow(int row)
		{
			this.data.Insert (row, new Dictionary<ColumnType, FormattedText> ());
		}

		public void RemoveRow(int row)
		{
			this.data.RemoveAt (row);
		}


		public bool GetBool(int row, ColumnType column)
		{
			var text = this.GetText (row, column);

			return !text.IsNullOrEmpty && text == "1";
		}

		public void SetBool(int row, ColumnType column, bool value)
		{
			this.SetText (row, column, value ? "1" : "0");
		}


		public Date? GetDate(int row, ColumnType column)
		{
			return Converters.ParseDate (this.GetText (row, column));
		}

		public void SetDate(int row, ColumnType column, Date value)
		{
			this.SetText (row, column, Converters.DateToString (value));
		}


#if false
		public decimal? GetMontant(int row, ColumnType column)
		{
			return Converters.ParseMontant (this.GetText (row, column));
		}

		public void SetMontant(int row, ColumnType column, decimal value)
		{
			this.SetText (row, column, Converters.MontantToString (value));
		}
#endif


		public decimal? GetPercent(int row, ColumnType column)
		{
			return Converters.ParsePercent (this.GetText (row, column));
		}

		public void SetPercent(int row, ColumnType column, decimal value)
		{
			this.SetText (row, column, Converters.PercentToString (value));
		}


		public FormattedText GetText(int row, ColumnType column)
		{
			if (row >= 0 && row < this.data.Count)
			{
				var dict = this.data[row];
				if (dict.ContainsKey (column))
				{
					return dict[column];
				}
			}

			return null;
		}

		public void SetText(int row, ColumnType column, string text)
		{
			if (row >= 0)
			{
				while (row >= this.data.Count)
				{
					this.data.Add (new Dictionary<ColumnType, FormattedText> ());
				}

				var dict = this.data[row];
				dict[column] = text;
			}
		}


		private readonly List<Dictionary<ColumnType, FormattedText>>	data;
	}
}