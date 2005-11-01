//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// La classe ParagraphLayoutWrapper simplifie l'accès aux réglages liés à
	/// la disposition d'un paragraphe (justification, marges, etc.)
	/// </summary>
	public class ParagraphLayoutWrapper : AbstractWrapper
	{
		public ParagraphLayoutWrapper()
		{
		}
		
		
		public State								Active
		{
			get
			{
				return this.active_state;
			}
		}
		
		public State								Defined
		{
			get
			{
				return this.defined_state;
			}
		}
		
		
		public class State
		{
			public JustificationMode				JustificationMode
			{
				get
				{
					return this.justification_mode;
				}
				set
				{
					if (this.justification_mode != value)
					{
						this.justification_mode = value;
						this.NotifyChanged ();
					}
				}
			}
			
			
			public State Clone()
			{
				State clone = new State ();
				
				clone.justification_mode = this.justification_mode;
				
				return clone;
			}
			
			
			private void NotifyChanged()
			{
			}
			
			
			public event Common.Support.EventHandler	Changed;
			
			private JustificationMode				justification_mode;
		}
		
		private State								active_state;
		private State								defined_state;
	}
}
