//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	//	La valeur correspond au nombre de mois. Mais elle n'est jamais d�duite
	//	ainsi. Il s'agit simplement d'une aide pour le debug, lorsque le
	//	contr�leur des �num�rations EnumFieldController manipule des int.

	[DesignerVisible]
	public enum Periodicity
	{
		Unknown     = 0,

		Annual      = 12,
		Semestrial  = 6,	// ce n'est pas de l'anglais, mais je me comprends !
		Trimestrial = 3,	//		"
		Mensual     = 1,	//		"
	}
}
