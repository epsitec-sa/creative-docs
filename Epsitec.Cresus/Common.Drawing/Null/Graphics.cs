namespace Epsitec.Common.Drawing.Null
{
	/// <summary>
	/// Impl�mentation de la classe Graphics, sans effet.
	/// </summary>
	public class Graphics : Epsitec.Common.Drawing.Graphics
	{
		public Graphics()
		{
			this.solid_renderer = new Common.Drawing.Null.SolidRenderer ();
			this.rasterizer     = new Common.Drawing.Null.Rasterizer ();
		}
		
		
		public override Epsitec.Common.Drawing.Rasterizer	Rasterizer
		{
			get { return this.rasterizer; }
		}
		
		public override Renderers.Solid						SolidRenderer
		{
			get { return this.solid_renderer; }
		}
		
		
		private Renderers.Solid			solid_renderer;
		private Rasterizer				rasterizer;
	}
}
