//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
