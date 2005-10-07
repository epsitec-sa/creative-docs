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

			this.buttonBullet1 = this.CreateIconButton("o",  Res.Strings.Action.Text.Paragraph.BulletRound,   new MessageEventHandler(this.HandleButtonClicked));
			this.buttonBullet2 = this.CreateIconButton("1.", Res.Strings.Action.Text.Paragraph.BulletNumeric, new MessageEventHandler(this.HandleButtonClicked));
			this.buttonBullet3 = this.CreateIconButton("A)", Res.Strings.Action.Text.Paragraph.BulletAlpha,   new MessageEventHandler(this.HandleButtonClicked));

			this.buttonAlignLeft   = this.CreateIconButton(Misc.Icon("JustifHLeft"),   Res.Strings.Action.Text.Paragraph.AlignLeft,   new MessageEventHandler(this.HandleButtonClicked));
			this.buttonAlignCenter = this.CreateIconButton(Misc.Icon("JustifHCenter"), Res.Strings.Action.Text.Paragraph.AlignCenter, new MessageEventHandler(this.HandleButtonClicked));
			this.buttonAlignRight  = this.CreateIconButton(Misc.Icon("JustifHRight"),  Res.Strings.Action.Text.Paragraph.AlignRight,  new MessageEventHandler(this.HandleButtonClicked));
			this.buttonAlignJustif = this.CreateIconButton(Misc.Icon("JustifHJustif"), Res.Strings.Action.Text.Paragraph.AlignJustif, new MessageEventHandler(this.HandleButtonClicked));
			
			this.buttonLeadingNorm = this.CreateIconButton("x1", Res.Strings.Action.Text.Paragraph.LeadingNorm, new MessageEventHandler(this.HandleButtonClicked));
			this.buttonLeadingPlus = this.CreateIconButton("x2", Res.Strings.Action.Text.Paragraph.LeadingPlus, new MessageEventHandler(this.HandleButtonClicked));
			this.buttonLeadingAuto = this.CreateIconButton("A",  Res.Strings.Action.Text.Paragraph.LeadingAuto, new MessageEventHandler(this.HandleButtonClicked));
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override void SetDocument(DocumentType type, InstallType install, Settings.GlobalSettings gs, Document document)
		{
			base.SetDocument(type, install, gs, document);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*3 + 5 + 22*3;
			}
		}


		// Effectue la mise à jour du contenu d'édition.
		protected override void DoUpdateText()
		{
			Objects.Abstract editObject = this.EditObject;

			this.buttonBullet1.SetEnabled(editObject != null);
			this.buttonBullet2.SetEnabled(editObject != null);
			this.buttonBullet3.SetEnabled(editObject != null);

			this.buttonAlignLeft.SetEnabled(editObject != null);
			this.buttonAlignCenter.SetEnabled(editObject != null);
			this.buttonAlignRight.SetEnabled(editObject != null);
			this.buttonAlignJustif.SetEnabled(editObject != null);

			this.buttonLeadingNorm.SetEnabled(editObject != null);
			this.buttonLeadingPlus.SetEnabled(editObject != null);
			this.buttonLeadingAuto.SetEnabled(editObject != null);

			bool bullet1 = false;
			bool bullet2 = false;
			bool bullet3 = false;

			bool alignLeft = false;
			bool alignCenter = false;
			bool alignRight = false;
			bool alignJustif = false;

			bool leadingNorm = false;
			bool leadingPlus = false;
			bool leadingAuto = false;

			if ( editObject != null )
			{
				bullet1 = editObject.GetTextStyle("Bullet1");
				bullet2 = editObject.GetTextStyle("Bullet2");
				bullet3 = editObject.GetTextStyle("Bullet3");

				alignLeft   = editObject.GetTextStyle("AlignLeft");
				alignCenter = editObject.GetTextStyle("AlignCenter");
				alignRight  = editObject.GetTextStyle("AlignRight");
				alignJustif = editObject.GetTextStyle("AlignJustif");

				leadingNorm = editObject.GetTextStyle("LeadingNorm");
				leadingPlus = editObject.GetTextStyle("LeadingPlus");
				leadingAuto = editObject.GetTextStyle("LeadingAuto");
			}

			this.buttonBullet1.ActiveState = bullet1 ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonBullet2.ActiveState = bullet2 ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonBullet3.ActiveState = bullet3 ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.buttonAlignLeft.ActiveState   = alignLeft   ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonAlignCenter.ActiveState = alignCenter ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonAlignRight.ActiveState  = alignRight  ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonAlignJustif.ActiveState = alignJustif ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.buttonLeadingNorm.ActiveState = leadingNorm ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonLeadingPlus.ActiveState = leadingPlus ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonLeadingAuto.ActiveState = leadingAuto ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonBullet1 == null )  return;

			double dx = this.buttonBullet1.DefaultWidth;
			double dy = this.buttonBullet1.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonBullet1.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonBullet2.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonBullet3.Bounds = rect;
			rect.Offset(dx+5, 0);
			this.buttonLeadingNorm.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeadingPlus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonLeadingAuto.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonAlignLeft.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAlignCenter.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAlignRight.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonAlignJustif.Bounds = rect;
		}


		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			Objects.Abstract editObject = this.EditObject;
			if ( editObject == null )  return;

			if ( sender == this.buttonBullet1 )  this.InvertStyle(editObject, "Bullet1", "Bullet");
			if ( sender == this.buttonBullet2 )  this.InvertStyle(editObject, "Bullet2", "Bullet");
			if ( sender == this.buttonBullet3 )  this.InvertStyle(editObject, "Bullet3", "Bullet");
			
			if ( sender == this.buttonAlignLeft   )  this.InvertStyle(editObject, "AlignLeft",   "Align");
			if ( sender == this.buttonAlignCenter )  this.InvertStyle(editObject, "AlignCenter", "Align");
			if ( sender == this.buttonAlignRight  )  this.InvertStyle(editObject, "AlignRight",  "Align");
			if ( sender == this.buttonAlignJustif )  this.InvertStyle(editObject, "AlignJustif", "Align");
			
			if ( sender == this.buttonLeadingNorm )  this.InvertStyle(editObject, "LeadingNorm", "Leading");
			if ( sender == this.buttonLeadingPlus )  this.InvertStyle(editObject, "LeadingPlus", "Leading");
			if ( sender == this.buttonLeadingAuto )  this.InvertStyle(editObject, "LeadingAuto", "Leading");
		}

		protected void InvertStyle(Objects.Abstract editObject, string name, string exclude)
		{
			bool state = editObject.GetTextStyle(name);
			editObject.SetTextStyle(name, exclude, !state);
		}


		protected IconButton				buttonBullet1;
		protected IconButton				buttonBullet2;
		protected IconButton				buttonBullet3;
		protected IconButton				buttonAlignLeft;
		protected IconButton				buttonAlignCenter;
		protected IconButton				buttonAlignRight;
		protected IconButton				buttonAlignJustif;
		protected IconButton				buttonLeadingNorm;
		protected IconButton				buttonLeadingPlus;
		protected IconButton				buttonLeadingAuto;
	}
}
