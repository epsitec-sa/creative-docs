//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 21/03/2004

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// La classe PrintPort permet d'imprimer des éléments graphiques simples.
	/// </summary>
	public class PrintPort
	{
		internal PrintPort(System.Drawing.Graphics graphics)
		{
			this.graphics = graphics;
		}
		
		public void DrawPath(Drawing.Path path)
		{
			graphics.DrawPath (this.pen, path.CreateSystemPath ());
		}
		
		
		
		
		
		System.Drawing.Graphics					graphics;
		System.Drawing.Pen						pen;
	}
}
