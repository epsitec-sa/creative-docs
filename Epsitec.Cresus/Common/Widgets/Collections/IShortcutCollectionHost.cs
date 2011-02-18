//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
