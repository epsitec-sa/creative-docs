//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	public class SearchTabData
	{
		public SearchTabData()
		{
			this.searchText = new SearchText ();
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


		private readonly SearchText			searchText;
	}
}