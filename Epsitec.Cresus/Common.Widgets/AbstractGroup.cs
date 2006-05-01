//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractGroup sert de base aux autres classes qui
	/// implémentent des groupes de widgets.
	/// </summary>
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
				//	Un groupe transmet toujours les pressions de TAB à ses enfants
				//	sans les consommer lui-même; c'est pourquoi on rajoute les deux
				//	bits ci-dessous :
				
				this.SetTabNavigation (value | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly);
			}
		}
		
		
		public void SetTabNavigation(TabNavigationMode value)
		{
			//	Comme this.TabNavigation, mais sans aucun garde-fou.
			
			base.TabNavigation = value;
		}
	}
}
