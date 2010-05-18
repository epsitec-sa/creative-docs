//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// Cette tuile contient un résumé non éditable d'une entité.
	/// Son parent est forcément un TileGrouping.
	/// </summary>
	public class SummaryTile : GenericTile
	{
		public SummaryTile()
		{
			this.staticTextSummary = new StaticText
			{
				Parent = this,
				PreferredWidth = 0,
				Dock = DockStyle.Fill,
				Margins = new Margins (2, TileArrow.Breadth, 0, 0),
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split,  // TODO: il manque le bon mode !
			};
		}

		public double ContentHeight
		{
			get
			{
				// TODO: faire cela mieux !
				string[] lines = this.Summary.Split (new string[] { "<br/>" }, System.StringSplitOptions.None);
				return lines.Length*16;
			}
		}

		/// <summary>
		/// Résumé multilignes affiché sous le titre.
		/// </summary>
		/// <value>The content.</value>
		public string Summary
		{
			get
			{
				return this.staticTextSummary.Text;
			}
			set
			{
				this.staticTextSummary.Text = value;
			}
		}


		private readonly StaticText staticTextSummary;
	}
}
