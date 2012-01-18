//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	public class SearchingData
	{
		public SearchingData()
		{
			this.tabsData = new List<SearchingTabData> ();
			this.OrMode = true;
		}


		public List<SearchingTabData> TabsData
		{
			get
			{
				return this.tabsData;
			}
		}

		public bool OrMode
		{
			get;
			set;
		}

		public bool IsEmpty
		{
			get
			{
				foreach (var tab in this.tabsData)
				{
					if (!tab.IsEmpty)
					{
						return false;
					}
				}

				return true;
			}
		}


		private readonly List<SearchingTabData>		tabsData;
	}
}