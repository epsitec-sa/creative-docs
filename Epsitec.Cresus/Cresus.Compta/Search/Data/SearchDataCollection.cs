//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Data
{
	public class SearchDataCollection : ISettingsData
	{
		public SearchDataCollection()
		{
			this.dataList = new List<SearchData> ();
			this.selectedIndex = -1;
		}


		public List<SearchData> DataList
		{
			get
			{
				return this.dataList;
			}
		}

		public int SelectedIndex
		{
			get
			{
				return this.selectedIndex;
			}
			set
			{
				this.selectedIndex = value;
			}
		}


		private readonly List<SearchData>			dataList;
		private int									selectedIndex;
	}
}