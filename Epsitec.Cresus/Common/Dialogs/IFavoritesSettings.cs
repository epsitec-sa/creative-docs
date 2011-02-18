//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		IList<string> Items
		{
			get;
		}
	}
}
