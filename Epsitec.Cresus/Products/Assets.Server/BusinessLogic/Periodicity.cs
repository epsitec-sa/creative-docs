//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	//	La valeur correspond au nombre de mois. Mais elle n'est jamais déduite
	//	ainsi. Il s'agit simplement d'une aide pour le debug, lorsque le
	//	contrôleur des énumérations EnumFieldController manipule des int.

	public enum Periodicity
	{
		Unknown     = 0,

		Annual      = 12,
		Semestrial  = 6,
		Trimestrial = 3,
		Mensual     = 1,
	}
}
