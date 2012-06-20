//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

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
			this.columns = new List<ColumnType> ();
			this.Active = true;
		}

		public SearchText SearchText
		{
			get
			{
				return this.searchText;
			}
		}

		public List<ColumnType> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public bool Active
		{
			get;
			set;
		}

		public void Clear()
		{
			this.searchText.Clear ();
			this.columns.Clear ();
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
			if (other.columns.Count != this.columns.Count)
			{
				return false;
			}

			for (int i = 0; i < other.columns.Count; i++)
			{
				if (other.columns[i] != this.columns[i])
				{
					return false;
				}
			}

			return other.Active == this.Active &&
				   this.searchText.CompareTo (other.searchText);
		}

		public void CopyTo(SearchTabData dst)
		{
			dst.columns.Clear ();
			dst.columns.AddRange (this.columns);

			dst.Active = this.Active;

			this.searchText.CopyTo (dst.searchText);
		}


		public FormattedText GetSummary(List<ColumnMapper> columnMappers)
		{
			return this.searchText.GetSummary (this.GetColumnsSummary (columnMappers));
		}

		public FormattedText GetColumnsSummary(List<ColumnMapper> columnMappers)
		{
			var list = new List<string> ();

			foreach (var column in this.columns)
			{
				var mapper = columnMappers.Where (x => x.Column == column).FirstOrDefault ();
				if (mapper != null)
				{
					list.Add (mapper.Description.ToString ());
				}
			}

			return Strings.SentenceConcat ("ou", list);
		}


		private readonly SearchText			searchText;
		private readonly List<ColumnType>	columns;
	}
}