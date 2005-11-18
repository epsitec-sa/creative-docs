//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe MenuSeparator est la variante du MenuItem servant
	/// à peindre les séparations.
	/// </summary>
	public class MenuSeparator : MenuItem
	{
		public MenuSeparator()
		{
		}
		
		public MenuSeparator(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override bool					IsSeparator
		{
			get
			{
				return true;
			}
		}

	}
}
