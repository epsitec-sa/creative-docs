using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Paragraph permet de choisir le style de paragraphe du texte.
	/// </summary>
	[SuppressBundleSupport]
	public class Paragraph : Abstract
	{
		public Paragraph() : base()
		{
			this.title.Text = Res.Strings.Action.ParagraphMain;

			this.buttonJustifHLeft   = this.CreateIconButton("JustifHLeft");
			this.buttonJustifHCenter = this.CreateIconButton("JustifHCenter");
			this.buttonJustifHRight  = this.CreateIconButton("JustifHRight");
			this.buttonJustifHJustif = this.CreateIconButton("JustifHJustif");
			this.buttonJustifHAll    = this.CreateIconButton("JustifHAll");

			this.buttonIndentMinus   = this.CreateIconButton("ParagraphIndentMinus");
			this.buttonIndentPlus    = this.CreateIconButton("ParagraphIndentPlus");

			this.buttonLeading08     = this.CreateIconButton("ParagraphLeading08");
			this.buttonLeading10     = this.CreateIconButton("ParagraphLeading10");
			this.buttonLeading15     = this.CreateIconButton("ParagraphLeading15");
			this.buttonLeading20     = this.CreateIconButton("ParagraphLeading20");
			this.buttonLeading30     = this.CreateIconButton("ParagraphLeading30");
			this.buttonLeadingMinus  = this.CreateIconButton("ParagraphLeadingMinus");
			this.buttonLeadingPlus   = this.CreateIconButton("ParagraphLeadingPlus");

			this.buttonClear         = this.CreateIconButton("ParagraphClear");

			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 200;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonLeading08 == null )  return;

			double dx = this.buttonLeading08.DefaultWidth;
			double dy = this.buttonLeading08.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width = dx;
			rect.Offset(0, dy+5);
			this.buttonJustifHLeft.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonJustifHCenter.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonJustifHRight.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonJustifHJustif.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonJustifHAll.Bounds = rect;
			rect.Offset(dx+5, 0);
			this.buttonIndentMinus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonIndentPlus.Bounds = rect;

			rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width = dx;
			this.buttonLeading08.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeading10.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeading15.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeading20.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeading30.Bounds = rect;
			rect.Offset(dx+5, 0);
			this.buttonLeadingMinus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeadingPlus.Bounds = rect;
			rect.Offset(dx+10, 0);
			this.buttonClear.Bounds = rect;
		}


		protected IconButton				buttonJustifHLeft;
		protected IconButton				buttonJustifHCenter;
		protected IconButton				buttonJustifHRight;
		protected IconButton				buttonJustifHJustif;
		protected IconButton				buttonJustifHAll;
		protected IconButton				buttonLeading08;
		protected IconButton				buttonLeading10;
		protected IconButton				buttonLeading15;
		protected IconButton				buttonLeading20;
		protected IconButton				buttonLeading30;
		protected IconButton				buttonLeadingMinus;
		protected IconButton				buttonLeadingPlus;
		protected IconButton				buttonIndentMinus;
		protected IconButton				buttonIndentPlus;
		protected IconButton				buttonClear;
	}
}
