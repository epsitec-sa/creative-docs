//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views
{
	public enum DateRangeCategory
	{
		Mandat,			// date seulement à partir de la date de début du mandat
		Free,			// date libre (par exemple pour une date de naissance)
	}
}
