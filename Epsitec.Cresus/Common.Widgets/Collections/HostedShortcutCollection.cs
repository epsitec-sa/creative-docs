//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// La classe HostedShortcutCollection est une variante de ShortcutCollection
	/// qui avertit son "host" des changements, s'il y en a.
	/// </summary>
	public class HostedShortcutCollection : ShortcutCollection
	{
		public HostedShortcutCollection(IShortcutCollectionHost host)
		{
			this.host = host;
		}
		
		
		protected override void  NotifyInsertion(Shortcut item)
		{
			if (this.host != null)
			{
				this.host.NotifyShortcutsChanged (this);
			}
			
			base.NotifyInsertion (item);
		}
		
		protected override void  NotifyRemoval(Shortcut item)
		{
			if (this.host != null)
			{
				this.host.NotifyShortcutsChanged (this);
			}
			
			base.NotifyRemoval (item);
		}

		
		private IShortcutCollectionHost			host;
	}
}
