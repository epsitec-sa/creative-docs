//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 12/02/2004

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Tag implémente une petite étiquette (pastille) qui peut servir
	/// à l'implémentation de "smart tags".
	/// </summary>
	public class Tag : IconButton
	{
		public Tag()
		{
			this.ButtonStyle = ButtonStyle.Flat;
		}
		
		public Tag(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		public Tag(string icon) : base (icon)
		{
		}
		
		public Tag(string command, string icon) : base (command, icon)
		{
		}
		
		public Tag(string command, string icon, string name) : base (command, icon, name)
		{
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			base.PaintBackgroundImplementation (graphics, clip_rect);
		}
	}
}
