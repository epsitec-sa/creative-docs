//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public enum EventType
	{
		Unknown,
		Input,					// entrée dans l'inventaire, acquisition
		AmortizationAuto,		// amortissement ordinaire
		AmortizationPreview,	// aperçu d'un amortissement ordinaire
		AmortizationExtra,		// amortissement extraordinaire
		Modification,			// modification de diverses informations
		Reorganization,			// spécificité MCH2
		Increase,				// revalorisation
		Decrease,				// réévaluation
		Output,					// sortie de l'inventaire, vente, vol, destruction, etc.
	}
}