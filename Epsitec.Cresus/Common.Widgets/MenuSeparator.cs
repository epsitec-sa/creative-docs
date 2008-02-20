//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
