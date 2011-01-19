//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		
		public string Summary
		{
			get
			{
				return this.staticTextSummary.Text;
			}
			set
			{
				if (this.Summary != value)
				{
					this.staticTextSummary.Text = value;
					this.UpdatePreferredSize ();
				}
			}
		}


		public override Size GetBestFitSize()
		{
			// TODO: faire cela mieux !
			var lines = this.Summary.Split (new string[] { FormattedText.HtmlBreak }, System.StringSplitOptions.None);
			var height = lines.Length*16;

			return new Size (this.PreferredWidth, height);
		}

		private void CreateUI()
		{
			this.staticTextSummary = new StaticText
			{
				Parent = this,
				PreferredWidth = 0,
				Dock = DockStyle.Fill,
				Margins = this.ContainerPadding + new Margins (2, 0, 0, 0),
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split,  // TODO: il manque le bon mode !
			};
		}


		private StaticText staticTextSummary;
	}
}
