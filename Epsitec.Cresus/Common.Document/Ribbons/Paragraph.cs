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

			this.buttonJustif = this.CreateIconButtonCombo("ParagraphJustif");
			this.AddIconButtonCombo(this.buttonJustif, "ParagraphJustifLeft",   "JustifHLeft",   Res.Strings.Property.Justif.JustifHLeft);
			this.AddIconButtonCombo(this.buttonJustif, "ParagraphJustifCenter", "JustifHCenter", Res.Strings.Property.Justif.JustifHCenter);
			this.AddIconButtonCombo(this.buttonJustif, "ParagraphJustifRight",  "JustifHRight",  Res.Strings.Property.Justif.JustifHRight);
			this.AddIconButtonCombo(this.buttonJustif, "ParagraphJustifJustif", "JustifHJustif", Res.Strings.Property.Justif.JustifHJustif);
			this.AddIconButtonCombo(this.buttonJustif, "ParagraphJustifAll",    "JustifHAll",    Res.Strings.Property.Justif.JustifHAll);

			this.buttonIndentMinus = this.CreateIconButton("ParagraphIndentMinus");
			this.buttonIndentPlus  = this.CreateIconButton("ParagraphIndentPlus");

			this.buttonLeading = this.CreateIconButtonCombo("ParagraphLeading");
			this.AddIconButtonCombo(this.buttonLeading, "ParagraphLeading08", "ParagraphLeading08", Res.Strings.Property.Leading.Leading08);
			this.AddIconButtonCombo(this.buttonLeading, "ParagraphLeading10", "ParagraphLeading10", Res.Strings.Property.Leading.Leading10);
			this.AddIconButtonCombo(this.buttonLeading, "ParagraphLeading15", "ParagraphLeading15", Res.Strings.Property.Leading.Leading15);
			this.AddIconButtonCombo(this.buttonLeading, "ParagraphLeading20", "ParagraphLeading20", Res.Strings.Property.Leading.Leading20);
			this.AddIconButtonCombo(this.buttonLeading, "ParagraphLeading30", "ParagraphLeading30", Res.Strings.Property.Leading.Leading30);

			this.buttonLeadingMinus = this.CreateIconButton("ParagraphLeadingMinus");
			this.buttonLeadingPlus  = this.CreateIconButton("ParagraphLeadingPlus");

			this.buttonClear = this.CreateIconButton("ParagraphClear");

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
				return 123;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.buttonClear == null )  return;

			double dx = this.buttonClear.DefaultWidth;
			double dy = this.buttonClear.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Offset(0, dy+5);
			rect.Height = dy;
			rect.Width = dx*1.5;
			this.buttonJustif.Bounds = rect;
			rect.Offset(dx*1.5+5, 0);
			rect.Width = dx;
			this.buttonIndentMinus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonIndentPlus.Bounds = rect;

			rect = this.UsefulZone;
			rect.Height = dy;
			rect.Width = dx*1.5;
			this.buttonLeading.Bounds = rect;
			rect.Offset(dx*1.5+5, 0);
			rect.Width = dx;
			this.buttonLeadingMinus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeadingPlus.Bounds = rect;
			rect.Offset(dx+10, 0);
			this.buttonClear.Bounds = rect;
		}


		protected IconButtonCombo			buttonJustif;
		protected IconButton				buttonIndentMinus;
		protected IconButton				buttonIndentPlus;
		protected IconButtonCombo			buttonLeading;
		protected IconButton				buttonLeadingMinus;
		protected IconButton				buttonLeadingPlus;
		protected IconButton				buttonClear;
	}
}
