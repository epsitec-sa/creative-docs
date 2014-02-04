//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	//	La valeur correspond au nombre de mois. Mais elle n'est jamais déduite
	//	ainsi. Il s'agit simplement d'une aide pour le debug, lorsque le
	//	contrôleur des énumérations EnumFieldController manipule des int.

	public enum Périodicité
	{
		Unknown     = 0,
		Annuel      = 12,
		Semestriel  = 6,
		Trimestriel = 3,
		Mensuel     = 1,
	}
}
