namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VScroller implémente l'ascenceur vertical.
	/// </summary>
	public class VScroller : Scroller
	{
		public VScroller() : base (true)
		{
		}
		
		public override double				DefaultWidth
		{
			get { return Scroller.defaultBreadth; }
		}
		
		public override Drawing.Size		DefaultMinSize
		{
			get { return new Drawing.Size (Scroller.defaultBreadth, Scroller.minimalThumb+6); }
		}
	}
}
