//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public enum BaseType
	{
		Unknown    = 0,
		Assets     = 1,
		Categories = 2,
		Groups     = 3,
		Persons    = 4,
		UserFields = 5,
		Entries    = 6,
		Accounts   = 100,	// +n pour les différentes périodes
	}
}