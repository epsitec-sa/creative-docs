//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget est un conteneur de TileContainer, qui correspond à une bande verticale complète.
	/// Il dessine un cadre qui tient compte des flèches droites. 
	/// </summary>
	public class ContainerTiles : FrameBox
	{
		public ContainerTiles()
		{
		}

		public ContainerTiles(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		/// <summary>
		/// Indique s'il s'agit du conteneur de la dernière colonne de droite.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is right column; otherwise, <c>false</c>.
		/// </value>
		public bool IsRightColumn
		{
			get;
			set;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate (0.5);
			rect = new Rectangle (rect.Left, rect.Bottom, rect.Width-ArrowedTile.ArrowBreadth, rect.Height);
			graphics.AddRectangle (rect);
			graphics.RenderSolid (adorner.ColorBorder);
		}
	}
}
