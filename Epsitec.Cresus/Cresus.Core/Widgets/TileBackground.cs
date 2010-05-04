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
	/// Conteneur de base pour toutes les tuiles, qui s'occupe simplement de déterminer la couleur de fond, sans peindre ce dernier.
	/// </summary>
	public class TileBackground : FrameBox
	{
		public TileBackground()
		{
		}

		public TileBackground(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		/// <summary>
		/// Indique si la tuile permet d'éditer une entité.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is editing; otherwise, <c>false</c>.
		/// </value>
		public bool IsEditing
		{
			get;
			set;
		}


		public Color BackgroundColor
		{
			get
			{
				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

				Color backColor = adorner.ColorTextBackground;

				if (this.IsEditing)
				{
					backColor = this.BackgroundEditingColor;
				}

				if (this.IsSelected)
				{
					backColor = this.BackgroundSelectedColor;
				}

				return backColor;
			}
		}

		public Color BackgroundEditingColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("eef6ff");
			}
		}

		public Color BackgroundSelectedColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("d8e8fe");
			}
		}

		public Color BackgroundHilitedColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("ffc83c");  // orange
			}
		}
	}
}
