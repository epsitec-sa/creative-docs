//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// The <c>SummaryTile</c> class displays a summary text; this is a read-
	/// only tile.
	/// </summary>
	public class SummaryTile : GenericTile
	{
		public SummaryTile()
		{
			this.IsReadOnly = true;

			this.CreateUI ();
		}

		
		public FormattedText					Summary
		{
			get
			{
				return this.staticTextSummary.FormattedText;
			}
			set
			{
				if (this.Summary != value)
				{
					this.staticTextSummary.FormattedText = value;
					this.UpdatePreferredSize ();
				}
			}
		}


		public override Size GetBestFitSize()
		{
			return new Size (this.PreferredWidth, Misc.GetEstimatedHeight (this.Summary));
		}

		
		private void CreateUI()
		{
			this.staticTextSummary = new StaticText
			{
				Parent           = this,
				PreferredWidth   = 0,
				Dock             = DockStyle.Fill,
				Margins          = this.ContainerPadding + new Margins (2, 0, 0, 0),
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode    = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split,  // TODO: il manque le bon mode !
			};
		}


		private StaticText staticTextSummary;
	}
}
