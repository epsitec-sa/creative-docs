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
	public class SearchingTabData
	{
		public SearchingTabData(ComptaEntity comptaEntity)
		{
			this.comptaEntity = comptaEntity;
			this.searchingText = new SearchingText (this.comptaEntity);
		}

		public SearchingText SearchingText
		{
			get
			{
				return this.searchingText;
			}
		}

		public ColumnType Column
		{
			get;
			set;
		}

		public void Clear()
		{
			this.searchingText.Clear ();
			this.Column = ColumnType.None;
		}

		public bool IsEmpty
		{
			get
			{
				return this.searchingText == null || this.searchingText.IsEmpty;
			}
		}


		private readonly ComptaEntity		comptaEntity;
		private readonly SearchingText		searchingText;
	}
}