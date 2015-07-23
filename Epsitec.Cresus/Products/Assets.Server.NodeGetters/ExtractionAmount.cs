//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public enum ExtractionAmount
	{
		StateAt,			// �tat � la fin d'une p�riode
		DeltaFiltered,		// variation durant une p�riode (d�but - fin)
		LastFiltered,		// �tat � la fin d'une p�riode pour un �v�nement donn�
		Amortizations,		// amortissements durant une p�riode
		UserColumn,			// colonne suppl�mentaire d�finie � partir de ObjectField.UserFieldMCH2SummaryOrder
	}
}
