//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public enum ExtractionAmount
	{
		StateAt,			// état à la fin d'une période
		LastFiltered,		// état à la fin d'une période pour un événement donné
		DeltaSum,			// somme des variations durant une période
		UserColumn,			// colonne supplémentaire définie à partir de ObjectField.UserFieldMCH2SummaryOrder
	}
}
