namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VScroller implémente l'ascenceur vertical.
	/// </summary>
	public class VScroller : AbstractScroller
	{
		public VScroller() : base(true)
		{
		}
		
		public override double DefaultWidth
		{
			get
			{
				return AbstractScroller.defaultBreadth;
			}
		}
		
		public override Drawing.Size DefaultMinSize
		{
			get
			{
				return new Drawing.Size (AbstractScroller.defaultBreadth, AbstractScroller.minimalThumb+6);
			}
		}
	}
}
