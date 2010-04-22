//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public class SimpleTile : AbstractTile
	{
		public SimpleTile()
		{
			this.staticTextContent = new StaticText
			{
				Parent = this.mainPanel,
				Dock = DockStyle.Fill,
			};
		}

		public SimpleTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		override public double ContentHeight
		{
			get
			{
				string[] lines = this.Content.Split (new string[] { "<br/>" }, System.StringSplitOptions.None);
				double h = 20+lines.Length*16;  // TODO: provisoire
				return System.Math.Max (h, this.PreferredHeight);
			}
		}

		/// <summary>
		/// Contenu multilignes affiché sous le titre.
		/// </summary>
		/// <value>The content.</value>
		public string Content
		{
			get
			{
				return this.staticTextContent.Text;
			}
			set
			{
				this.staticTextContent.Text = value;
			}
		}


		private StaticText staticTextContent;
	}
}
