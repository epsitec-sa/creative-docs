//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// La classe AbstractWrapper sert de base aux "wrappers" qui simplifient
	/// l'accès aux réglages internes d'un texte ou d'un style.
	/// </summary>
	public abstract class AbstractWrapper
	{
		protected AbstractWrapper()
		{
		}
		
		
		public void Attach(Text.TextNavigator navigator)
		{
			this.Detach ();
			
			this.navigator = navigator;
			this.context   = this.navigator.TextContext;
			
			//	TODO: attache au navigateur
		}
		
		public void Attach(Text.TextContext context, Text.TextStyle style)
		{
			this.Detach ();
			
			this.context    = context;
			this.style      = style;
			this.style_list = this.context.StyleList;
			
			//	TODO: attache au styliste
		}
		
		public void Detach()
		{
			if (this.navigator != null)
			{
				//	TODO: détache du navigateur
			}
			if (this.style_list != null)
			{
				//	TODO: détache du styliste
			}
			
			this.navigator  = null;
			this.style      = null;
			this.style_list = null;
			this.context    = null;
		}
		
		
		private TextContext						context;
		private TextNavigator					navigator;
		private StyleList						style_list;
		private TextStyle						style;
	}
}
