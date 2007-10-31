using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Le widget ResetBox contient un groupe quelconque de widgets avec un petit bouton 'reset' en haut à droite.
	/// </summary>
	public class ResetBox : FrameBox
	{
		public ResetBox()
		{
			this.groupBox = new FrameBox(this);
			this.groupBox.Dock = DockStyle.Fill;

			this.resetButton = new IconButton(this);
			this.resetButton.IconName = Misc.Icon("Reset");
			this.resetButton.PreferredWidth = 15;
			this.resetButton.Dock = DockStyle.Right;
			this.resetButton.Margins = new Margins(2, 0, 0, 0);
			ToolTip.Default.SetToolTip(this.resetButton, "Reset");
		}
		
		public ResetBox(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.groupBox.Dispose();
				this.groupBox = null;

				this.resetButton.Dispose();
				this.resetButton = null;
			}
			
			base.Dispose(disposing);
		}


		public bool IsPatch
		{
			//	Indique si on est dans un module de patch.
			//	Dans ce cas, le bouton 'reset' est visible.
			get
			{
				return this.resetButton.Visibility;
			}
			set
			{
				this.resetButton.Visibility = value;
			}
		}

		public FrameBox GroupBox
		{
			//	Retourne le groupe à utiliser comme parent pour tous les widgets contenus.
			get
			{
				return this.groupBox;
			}
		}

		public IconButton ResetButton
		{
			//	Retourne le bouton 'reset', pour se connecter sur l'événement 'clicked' par exemple.
			get
			{
				return this.resetButton;
			}
		}


		protected FrameBox groupBox;
		protected IconButton resetButton;
	}
}
