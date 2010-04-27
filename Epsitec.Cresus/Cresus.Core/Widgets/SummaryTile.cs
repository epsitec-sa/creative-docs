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
	public class SummaryTile : AbstractTile
	{
		public SummaryTile()
		{
			this.staticTextSummary = new StaticText
			{
				Parent = this.mainPanel,
				PreferredWidth = 0,
				Dock = DockStyle.Fill,
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split,  // TODO: il manque le bon mode !
			};
		}

		public SummaryTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		override public double ContentHeight
		{
			get
			{
				if (Mode == Controllers.ViewControllerMode.Compact)
				{
					if (this.CompactFollower)
					{
						return 16;
					}
					else
					{
						string[] lines = this.Summary.Split (new string[] { "<br/>" }, System.StringSplitOptions.None);
						double h = 20+lines.Length*16;  // TODO: provisoire
						return System.Math.Max (h, this.PreferredHeight);
					}
				}
				else
				{
					return 200;
				}
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


		protected override void UpdateCompactFollower()
		{
			base.UpdateCompactFollower ();

			if (this.compactFollower)
			{
				this.staticTextSummary.ContentAlignment = ContentAlignment.MiddleLeft;
			}
			else
			{
				this.staticTextSummary.ContentAlignment = ContentAlignment.TopLeft;
			}
		}

	
		private StaticText staticTextSummary;
	}
}
