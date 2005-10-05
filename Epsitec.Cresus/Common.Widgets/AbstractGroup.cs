//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractGroup sert de base aux autres classes qui
	/// implémentent des groupes de widgets.
	/// </summary>
	
	[Support.SuppressBundleSupport]
	
	public abstract class AbstractGroup : Widget
	{
		public AbstractGroup()
		{
			this.InternalState |= InternalState.PossibleContainer;
			base.TabNavigation  = Widget.TabNavigationMode.ForwardTabPassive;
		}
		
		public AbstractGroup(Widget embedder) : this ()
		{
			this.SetEmbedder(embedder);
		}
		
		public override TabNavigationMode		TabNavigation
		{
			get
			{
				return base.TabNavigation;
			}
			set
			{
				value &= ~ (TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly);
				value |= TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
				
				this.SetTabNavigation (value);
			}
		}
		
		
		public void SetTabNavigation(TabNavigationMode value)
		{
			//	Comme this.TabNavigation, mais sans aucun garde-fou.
			
			base.TabNavigation = value;
		}
	}
}
