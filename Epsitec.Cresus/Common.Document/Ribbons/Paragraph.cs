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
			this.title.Text = Res.Strings.Action.Text.Paragraph.Main;

			this.buttonLeading08    = this.CreateIconButton("ParagraphLeading08",    Misc.Icon("ParagraphLeading08"),    Res.Strings.Action.Text.Paragraph.Leading08, true);
			this.buttonLeading10    = this.CreateIconButton("ParagraphLeading10",    Misc.Icon("ParagraphLeading10"),    Res.Strings.Action.Text.Paragraph.Leading10, true);
			this.buttonLeading15    = this.CreateIconButton("ParagraphLeading15",    Misc.Icon("ParagraphLeading15"),    Res.Strings.Action.Text.Paragraph.Leading15, true);
			this.buttonLeading20    = this.CreateIconButton("ParagraphLeading20",    Misc.Icon("ParagraphLeading20"),    Res.Strings.Action.Text.Paragraph.Leading20, true);
			this.buttonLeading30    = this.CreateIconButton("ParagraphLeading30",    Misc.Icon("ParagraphLeading30"),    Res.Strings.Action.Text.Paragraph.Leading30, true);
			this.buttonLeadingMinus = this.CreateIconButton("ParagraphLeadingMinus", Misc.Icon("ParagraphLeadingMinus"), Res.Strings.TextPanel.Leading.Tooltip.LeadingMinus, false);
			this.buttonLeadingPlus  = this.CreateIconButton("ParagraphLeadingPlus",  Misc.Icon("ParagraphLeadingPlus"),  Res.Strings.TextPanel.Leading.Tooltip.LeadingPlus,  false);

			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 200;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonLeading08 == null )  return;

			double dx = this.buttonLeading08.DefaultWidth;
			double dy = this.buttonLeading08.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width = dx;
			rect.Offset(0, dy+5);
			this.buttonLeading08.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeading10.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeading15.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeading20.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeading30.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeadingMinus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeadingPlus.Bounds = rect;
		}


		protected IconButton				buttonLeading08;
		protected IconButton				buttonLeading10;
		protected IconButton				buttonLeading15;
		protected IconButton				buttonLeading20;
		protected IconButton				buttonLeading30;
		protected IconButton				buttonLeadingMinus;
		protected IconButton				buttonLeadingPlus;
	}
}
