//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	public class SearchingTabData
	{
		public string SearchingText
		{
			get
			{
				return this.searchingText;
			}
			set
			{
				if (this.searchingText != value)
				{
					this.searchingText = value;
					this.searchingPreparedText = SearchResult.PrepareForSearching (value);

					this.searchingDecimalLow  = null;
					this.searchingDecimalHigh = null;

					if (!string.IsNullOrEmpty (value))
					{
						value = value.Replace ("à", " ");    // autorise "5.35 à 9.95"
						value = value.Replace ("a", " ");    // autorise "5.35 a 9.95"
						value = value.Replace ("...", " ");  // autorise "5.35...9.95"
						value = value.Replace ("..", " ");   // autorise "5.35..9.95"
						value = value.Replace ("  ", " ");   // autorise "5.35 .. 9.95"
						value = value.Replace ("  ", " ");   // autorise "5.35  9.95"

						var words = value.Split (' ');

						if (words.Length <= 2)
						{
							{
								decimal d;
								if (decimal.TryParse (words[0], out d))
								{
									this.searchingDecimalLow = d;
								}
							}

							if (words.Length == 2)
							{
								decimal d;
								if (decimal.TryParse (words[1], out d))
								{
									this.searchingDecimalHigh = d;
								}
							}

							if (!this.searchingDecimalHigh.HasValue)
							{
								this.searchingDecimalHigh = this.searchingDecimalLow;
							}

							if (this.searchingDecimalLow.GetValueOrDefault () > this.searchingDecimalHigh.GetValueOrDefault ())
							{
								var t                     = this.searchingDecimalLow;
								this.searchingDecimalLow  = this.searchingDecimalHigh;
								this.searchingDecimalHigh = t;
							}
						}
					}
				}
			}
		}

		public string SearchingPreparedText
		{
			get
			{
				return this.searchingPreparedText;
			}
		}

		public bool HasSearchingDecimal
		{
			get
			{
				return this.searchingDecimalLow.HasValue;
			}
		}

		public decimal SearchingDecimalLow
		{
			get
			{
				return this.searchingDecimalLow.GetValueOrDefault ();
			}
		}

		public decimal SearchingDecimalHigh
		{
			get
			{
				return this.searchingDecimalHigh.GetValueOrDefault ();
			}
		}

		public ColumnType Column
		{
			get;
			set;
		}

		public void Clear()
		{
			this.SearchingText = null;
			this.Column = ColumnType.None;
		}

		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty (this.searchingText);
			}
		}


		private string				searchingText;
		private string				searchingPreparedText;
		private decimal?			searchingDecimalLow;
		private decimal?			searchingDecimalHigh;
	}
}