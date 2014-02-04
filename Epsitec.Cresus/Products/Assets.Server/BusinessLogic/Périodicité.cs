//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	//	La valeur correspond au nombre de mois. Mais elle n'est jamais d�duite
	//	ainsi. Il s'agit simplement d'une aide pour le debug, lorsque le
	//	contr�leur des �num�rations EnumFieldController manipule des int.

	public enum P�riodicit�
	{
		Unknown     = 0,
		Annuel      = 12,
		Semestriel  = 6,
		Trimestriel = 3,
		Mensuel     = 1,
	}
}
