//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	public interface ISearchBox
	{
		SearchBoxPolicy Policy
		{
			get;
		}

		void NotifySearchClicked();
		void NotifyShowNextClicked();
		void NotifyShowPrevClicked();
	}
}
