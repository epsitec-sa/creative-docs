//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// L'interface IShortcutCollectionHost permet d'offrir le support pour
	/// la classe HostedShortcutCollection.
	/// </summary>
	public interface IShortcutCollectionHost
	{
		void NotifyShortcutsChanged(ShortcutCollection collection);
	}
}
