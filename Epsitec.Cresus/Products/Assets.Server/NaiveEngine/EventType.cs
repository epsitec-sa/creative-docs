//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	public enum EventType
	{
		Unknown,
		Entrée,					// entrée dans l'inventaire, acquisition
		AmortissementAuto,		// amortissement ordinaire
		AmortissementExtra,		// amortissement extraordinaire
		Modification,			// modification de diverses informations
		Réorganisation,			// spécificité MCH2
		Augmentation,			// revalorisation
		Diminution,				// réévaluation
		Sortie,					// sortie de l'inventaire, vente, vol, destruction, etc.
	}
}