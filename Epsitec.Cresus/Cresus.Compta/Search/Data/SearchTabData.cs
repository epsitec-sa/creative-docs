//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Data
{
	/// <summary>
	/// Contient les données d'une ligne pour une recherche ou un filtre. Le texte n'est pas directement
	/// inclu, mais contenu dans SearchText.
	/// </summary>
	public class SearchTabData
	{
		public SearchTabData()
		{
			this.searchText = new SearchText ();
			this.Active = true;
		}

		public SearchText SearchText
		{
			get
			{
				return this.searchText;
			}
		}

		public ColumnType Column
		{
			get;
			set;
		}

		public bool Active
		{
			get;
			set;
		}

		public void Clear()
		{
			this.searchText.Clear ();
			this.Column = ColumnType.None;
		}

		public bool IsEmpty
		{
			get
			{
				return this.searchText == null || this.searchText.IsEmpty;
			}
		}


		public bool CompareTo(SearchTabData other)
		{
			return other.Column == this.Column &&
				   this.searchText.CompareTo (other.searchText);
		}

		public void CopyTo(SearchTabData dst)
		{
			dst.Column = this.Column;
			this.searchText.CopyTo (dst.searchText);
		}


		public FormattedText GetSummary(List<ColumnMapper> columnMappers)
		{
			FormattedText columnName = FormattedText.Empty;

			if (this.Column != ColumnType.None)
			{
				var mapper = columnMappers.Where (x => x.Column == this.Column).FirstOrDefault ();
				if (mapper != null)
				{
					columnName = mapper.Description;
				}
			}

			return this.searchText.GetSummary (columnName);
		}


		private readonly SearchText			searchText;
	}
}