//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Core.Dialogs.SettingsTabPages
{
	internal struct SearchMode
	{
		public bool CaseSensitive;
		public bool SearchInQuery;
		public bool SearchInParameters;
		public bool SearchInResults;
		public bool SearchInCallStack;

		public bool Compare(SearchMode that)
		{
			return (this.CaseSensitive      == that.CaseSensitive      &&
					this.SearchInQuery      == that.SearchInQuery      &&
					this.SearchInParameters == that.SearchInParameters &&
					this.SearchInResults    == that.SearchInResults    &&
					this.SearchInCallStack  == that.SearchInCallStack);
		}

		public void CopyFrom(SearchMode src)
		{
			this.CaseSensitive      = src.CaseSensitive;
			this.SearchInQuery      = src.SearchInQuery;
			this.SearchInParameters = src.SearchInParameters;
			this.SearchInResults    = src.SearchInResults;
			this.SearchInCallStack  = src.SearchInCallStack;
		}
	}
}
