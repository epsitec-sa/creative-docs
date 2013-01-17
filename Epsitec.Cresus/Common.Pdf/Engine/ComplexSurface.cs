//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.Engine
{
	/// <summary>
	/// La classe ComplexSurface enregistre les informations d'une surface complexe.
	/// </summary>
	public class ComplexSurface
	{
		public ComplexSurface(int page, RichColor color, int id)
		{
			//	Crée une surface complexe correspondant à une couleur transparente unie.
			this.page  = page;
			this.type  = ComplexSurfaceType.TransparencyRegular;
			this.color = color;
			this.id    = id;
		}

		public int Page
		{
			//	Numéro de la page (1..n).
			get
			{
				return this.page;
			}
		}

		public ComplexSurfaceType Type
		{
			//	Type de la surface.
			get
			{
				return this.type;
			}
		}

		public RichColor Color
		{
			//	Couleur transparente (A < 1.0).
			get
			{
				return this.color;
			}
		}

		public int Id
		{
			//	Identificateur unique (1..n).
			get
			{
				return this.id;
			}
		}


		private readonly int					page;  // 1..n
		private readonly ComplexSurfaceType		type;
		private readonly RichColor				color;
		private readonly int					id;
	}
}
