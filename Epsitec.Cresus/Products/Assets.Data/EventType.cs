//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Assets.Data
{
	[DesignerVisible]
	public enum EventType
	{
		Unknown,
		Input,					// entrée dans l'inventaire, acquisition
		Modification,			// modification de diverses informations (sauf valeur comptable)
		Increase,				// augmentation de la valeur comptable (revalorisation)
		Decrease,				// diminution de la valeur comptable (réévaluation)
		MainValue,				// modification de la valeur comptable (obsolète ?)
		AmortizationAuto,		// amortissement ordinaire
		AmortizationPreview,	// aperçu d'un amortissement ordinaire
		AmortizationExtra,		// amortissement extraordinaire
		Locked,					// verrouille les événements antérieurs
		Output,					// sortie de l'inventaire, vente, vol, destruction, etc.
	}
}