namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe GraphicsFactory instancie le contexte graphique.
	/// </summary>
	public class GraphicsFactory
	{
		private GraphicsFactory()
		{
		}
		
		public static Graphics NewGraphics()
		{
			return new Agg.Graphics ();
		}
	}
}
