//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
	[System.Flags]
	public enum WidgetInternalState : uint
	{
		None				= 0,

		Disposing			= 0x00000001,
		Disposed			= 0x00000002,

		WasValid			= 0x00000004,

		Embedded			= 0x00000008,		//	=> widget appartient au parent (widgets composés)

		Focusable			= 0x00000010,
		Selectable			= 0x00000020,
		Engageable			= 0x00000040,		//	=> peut être enfoncé par une pression
		Frozen				= 0x00000080,		//	=> n'accepte aucun événement

		ExecCmdOnPressed	= 0x00001000,		//	=> exécute la commande quand on presse le widget

		AutoMnemonic		= 0x00100000,
		AutoFitWidth		= 0x00200000,

		PossibleContainer	= 0x01000000,		//	widget peut être la cible d'un drag & drop en mode édition
		EditionEnabled		= 0x02000000,		//	widget peut être édité
		Fence				= 0x04000000,		//	widget marqué comme frontière (usages multiples)

		DebugActive			= 0x80000000		//	widget marqué pour le debug
	}
}
