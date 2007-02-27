//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Dialogs
{
	public interface IFavoritesSettings
	{
		bool UseLargeIcons
		{
			get;
			set;
		}

		Epsitec.Common.Types.Collections.ObservableList<string> Items
		{
			get;
		}
	}
}
