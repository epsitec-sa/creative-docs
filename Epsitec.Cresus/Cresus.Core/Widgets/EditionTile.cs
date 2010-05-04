//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Cette tuile contient tout ce qu'il faut pour éditer une entité.
	/// Son parent est forcément un TileGrouping.
	/// </summary>
	public class EditionTile : AbstractTile
	{
		public EditionTile()
		{
			this.container = new FrameBox
			{
				Parent = this,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, ContainerTile.ArrowBreadth, 0, 0),
			};
		}

		public EditionTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		/// <summary>
		/// Donne le conteneur dans lequel on va mettre tous les widgets permettant d'éditer l'entité associée à la tuile.
		/// </summary>
		/// <value>The container.</value>
		public FrameBox Container
		{
			get
			{
				return this.container;
			}
		}


		private FrameBox container;
	}
}
