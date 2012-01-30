//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	public class SearchData : ISettingsData
	{
		public SearchData()
		{
			this.tabsData = new List<SearchTabData> ();
			this.OrMode = true;
		}


		public List<SearchTabData> TabsData
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

		public bool GetIntervalDates(out Date? beginDate, out Date? endDate)
		{
			foreach (var data in this.tabsData)
			{
				if (data.SearchText.GetIntervalDates (out beginDate, out endDate))
				{
					return true;
				}
			}

			beginDate = null;
			endDate   = null;
			return false;
		}


		private readonly List<SearchTabData>		tabsData;
	}
}