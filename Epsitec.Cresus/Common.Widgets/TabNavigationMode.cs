//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	[System.Flags]
	public enum TabNavigationMode
	{
		Passive=0,

		ActivateOnTab=0x00000001,
		ActivateOnCursorX=0x00000002,
		ActivateOnCursorY=0x00000004,
		ActivateOnCursor=ActivateOnCursorX + ActivateOnCursorY,
		ActivateOnPage=0x00000008,

		ForwardToChildren=0x00010000,		//	transmet aux widgets enfants
		ForwardOnly=0x00020000,		//	utilisé avec ForwardToChilden: ne prend pas le focus soi-même
		SkipIfReadOnly=0x00040000,		//	saute si 'read-only'

		ForwardTabActive=ActivateOnTab | ForwardToChildren,
		ForwardTabPassive=ActivateOnTab | ForwardToChildren | ForwardOnly,
	}
}
