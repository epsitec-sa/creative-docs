namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe HScroller implémente l'ascenceur horizontal.
	/// </summary>
	public class HScroller : Scroller
	{
		public HScroller() : base (false)
		{
		}
		
		public override double				DefaultHeight
		{
			get { return Scroller.defaultBreadth; }
		}
		
		public override Drawing.Size		DefaultMinSize
		{
			get { return new Drawing.Size (Scroller.minimalThumb+6, Scroller.defaultBreadth); }
		}
	}
}
