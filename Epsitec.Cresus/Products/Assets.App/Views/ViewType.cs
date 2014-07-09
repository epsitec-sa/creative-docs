//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views
{
	public enum ViewType
	{
		Unknown         = 0,

		Assets          = 1,
		Amortizations   = 2,
		Entries         = 3,
		Categories      = 4,
		Groups          = 5,
		Persons         = 6,
		Reports         = 7,
		AssetsSettings  = 8,
		PersonsSettings = 9,
		Accounts        = 100,	// +n pour les différentes périodes
	}
}
