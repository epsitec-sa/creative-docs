//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public enum ExtractionAmount
	{
		StateAt,			// �tat � la fin d'une p�riode
		LastFiltered,		// �tat � la fin d'une p�riode pour un �v�nement donn�
		DeltaSum,			// somme des variations durant une p�riode
		UserColumn,			// colonne suppl�mentaire d�finie � partir de ObjectField.UserFieldMCH2SummaryOrder
	}
}
