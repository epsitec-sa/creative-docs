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
		public Tag() : this (null, null)
		{
		}
		
		public Tag(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		public Tag(string command) : this (command, null)
		{
		}
		
		public Tag(string command, string name) : base (command, null, name)
		{
			this.AutoFocus   = false;
			this.ButtonStyle = ButtonStyle.Flat;
			this.ResetDefaultColors ();
		}
		
		
		public override double					DefaultWidth
		{
			get { return 18; }
		}

		public override double					DefaultHeight
		{
			get { return 18; }
		}
		
		
		public Drawing.Color					Color
		{
			get
			{
				return this.color;
			}
			set
			{
				if (this.color != value)
				{
					this.color = value;
					this.Invalidate ();
				}
			}
		}
		
		
		public void ResetDefaultColors()
		{
			this.BackColor = Drawing.Color.Empty;
			this.Color     = Drawing.Color.Empty;
		}
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			IAdorner          adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect    = this.Client.Bounds;
			WidgetState       state   = this.PaintState;
			Drawing.Color     color   = this.Color;
			
			adorner.PaintTagBackground (graphics, rect, state, color);
		}
		
		
		private Drawing.Color					color;
	}
}
