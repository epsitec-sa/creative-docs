//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>INotifyChanged</c> interface defines a <c>Changed</c> event.
	/// </summary>
	public interface INotifyChanged
	{
		event Support.EventHandler	Changed;
	}
}
