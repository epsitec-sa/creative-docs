namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe HScroller impl�mente l'ascenceur horizontal.
	/// </summary>
	public class HScroller : AbstractScroller
	{
		public HScroller() : base(false)
		{
			this.ArrowUp.Name   = "Right";
			this.ArrowDown.Name = "Left";
		}
		
		public HScroller(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override double				DefaultHeight
		{
			get
			{
				return AbstractScroller.defaultBreadth;
			}
		}
		
		public override Drawing.Size		DefaultMinSize
		{
			get
			{
				return new Drawing.Size (AbstractScroller.minimalThumb+6, AbstractScroller.defaultBreadth);
			}
		}
	}
}
