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

			this.buttonBulletRound   = this.CreateIconButton(Misc.Icon("BulletRound"),   Res.Strings.Action.Text.Paragraph.BulletRound,   new MessageEventHandler(this.HandleButtonClicked));
			this.buttonBulletNumeric = this.CreateIconButton(Misc.Icon("BulletNumeric"), Res.Strings.Action.Text.Paragraph.BulletNumeric, new MessageEventHandler(this.HandleButtonClicked));
			this.buttonBulletAlpha   = this.CreateIconButton(Misc.Icon("BulletAlpha"),   Res.Strings.Action.Text.Paragraph.BulletAlpha,   new MessageEventHandler(this.HandleButtonClicked));

			this.buttonIndentMinus = this.CreateIconButton(Misc.Icon("ParagraphIndentMinus"), Res.Strings.Action.Text.Paragraph.IndentMinus, new MessageEventHandler(this.HandleButtonClicked), false);
			this.buttonIndentPlus  = this.CreateIconButton(Misc.Icon("ParagraphIndentPlus"),  Res.Strings.Action.Text.Paragraph.IndentPlus,  new MessageEventHandler(this.HandleButtonClicked), false);

			this.buttonAlignLeft   = this.CreateIconButton(Misc.Icon("JustifHLeft"),   Res.Strings.Action.Text.Paragraph.AlignLeft,   new MessageEventHandler(this.HandleButtonClicked));
			this.buttonAlignCenter = this.CreateIconButton(Misc.Icon("JustifHCenter"), Res.Strings.Action.Text.Paragraph.AlignCenter, new MessageEventHandler(this.HandleButtonClicked));
			this.buttonAlignRight  = this.CreateIconButton(Misc.Icon("JustifHRight"),  Res.Strings.Action.Text.Paragraph.AlignRight,  new MessageEventHandler(this.HandleButtonClicked));
			this.buttonAlignJustif = this.CreateIconButton(Misc.Icon("JustifHJustif"), Res.Strings.Action.Text.Paragraph.AlignJustif, new MessageEventHandler(this.HandleButtonClicked));
			
			this.fieldLeading = this.CreateFieldLeading("Interligne");
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

			this.AdaptFieldLeading(this.fieldLeading);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*4 + 5 + 50;
			}
		}


		// Effectue la mise à jour du contenu d'édition.
		protected override void DoUpdateText()
		{
			this.AdaptFieldLeading(this.fieldLeading);

			Objects.Abstract editObject = this.EditObject;

			this.buttonBulletRound.SetEnabled(editObject != null);
			this.buttonBulletNumeric.SetEnabled(editObject != null);
			this.buttonBulletAlpha.SetEnabled(editObject != null);

			this.buttonIndentMinus.SetEnabled(editObject != null);
			this.buttonIndentPlus.SetEnabled(editObject != null);

			this.buttonAlignLeft.SetEnabled(editObject != null);
			this.buttonAlignCenter.SetEnabled(editObject != null);
			this.buttonAlignRight.SetEnabled(editObject != null);
			this.buttonAlignJustif.SetEnabled(editObject != null);

			bool bulletRound = false;
			bool bulletNumeric = false;
			bool bulletAlpha = false;

			bool alignLeft = false;
			bool alignCenter = false;
			bool alignRight = false;
			bool alignJustif = false;

			if ( editObject != null )
			{
				bulletRound   = editObject.GetTextStyle("BulletRound");
				bulletNumeric = editObject.GetTextStyle("BulletNumeric");
				bulletAlpha   = editObject.GetTextStyle("BulletAlpha");

				alignLeft   = editObject.GetTextStyle("AlignLeft");
				alignCenter = editObject.GetTextStyle("AlignCenter");
				alignRight  = editObject.GetTextStyle("AlignRight");
				alignJustif = editObject.GetTextStyle("AlignJustif");
			}

			this.buttonBulletRound.ActiveState   = bulletRound   ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonBulletNumeric.ActiveState = bulletNumeric ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonBulletAlpha.ActiveState   = bulletAlpha   ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.buttonAlignLeft.ActiveState   = alignLeft   ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonAlignCenter.ActiveState = alignCenter ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonAlignRight.ActiveState  = alignRight  ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonAlignJustif.ActiveState = alignJustif ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonBulletRound == null )  return;

			double dx = this.buttonBulletRound.DefaultWidth;
			double dy = this.buttonBulletRound.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonBulletRound.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonBulletNumeric.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonBulletAlpha.Bounds = rect;
			rect.Offset(dx+5, 0);
			this.buttonIndentMinus.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonIndentPlus.Bounds = rect;

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
			rect.Offset(dx+5, 0);
			rect.Width = 50;
			this.fieldLeading.Bounds = rect;
		}


		// Crée un champ éditable pour l'interligne.
		protected TextFieldCombo CreateFieldLeading(string tooltip)
		{
			TextFieldCombo field = new TextFieldCombo(this);
			field.TextChanged += new EventHandler(this.HandleFieldComboChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);
			return field;
		}

		// Adapte un champ éditable pour l'interligne.
		protected void AdaptFieldLeading(TextFieldCombo field)
		{
			Objects.Abstract editObject = this.EditObject;

			if ( editObject == null )
			{
				field.SetEnabled(false);
			}
			else
			{
				field.SetEnabled(true);

				this.ignoreChange = true;
				
				this.UpdateFieldLeadingList(field);

				double size;
				Text.Properties.SizeUnits units;
				editObject.GetTextLeading(out size, out units, false);
				if ( units == Common.Text.Properties.SizeUnits.None )
				{
					field.Text = Res.Strings.Action.Text.Font.Default;
				}
				else
				{
					if ( units == Common.Text.Properties.SizeUnits.Points )
					{
						size /= Modifier.fontSizeScale;
					}
					field.Text = Misc.ConvertDoubleToString(size, units, 0);
				}

				this.ignoreChange = false;
			}
		}

		// Met à jour la liste d'un champ éditable pour l'interligne.
		protected void UpdateFieldLeadingList(TextFieldCombo field)
		{
			if ( field.Items.Count == 0 )
			{
				field.Items.Add(Res.Strings.Action.Text.Font.Default);  // par défaut
				field.Items.Add("\u2015\u2015\u2015\u2015");
				field.Items.Add("80%");
				field.Items.Add("125%");
				field.Items.Add("150%");
				field.Items.Add("200%");
				field.Items.Add("\u2015\u2015\u2015\u2015");
				field.Items.Add("8");
				field.Items.Add("9");
				field.Items.Add("10");
				field.Items.Add("11");
				field.Items.Add("12");
				field.Items.Add("14");
				field.Items.Add("16");
				field.Items.Add("18");
				field.Items.Add("20");
				field.Items.Add("24");
				field.Items.Add("36");
				field.Items.Add("48");
				field.Items.Add("72");
			}
		}

		// Un champ combo a été changé.
		private void HandleFieldComboChanged(object sender)
		{
			if ( this.ignoreChange )  return;

			TextFieldCombo field = sender as TextFieldCombo;
			if ( field == null )  return;

			Objects.Abstract editObject = this.EditObject;
			if ( editObject == null )  return;

			if ( field == this.fieldLeading )
			{
				if ( field.Text.StartsWith("\u2015") )  // sur un "séparateur" ?
				{
					this.ignoreChange = true;
					field.Text = "";
					this.ignoreChange = false;
					return;
				}

				double size = 0;
				Text.Properties.SizeUnits units = Common.Text.Properties.SizeUnits.None;
				if ( field.Text != Res.Strings.Action.Text.Font.Default )
				{
					Misc.ConvertStringToDouble(out size, out units, field.Text, 0, 1000, 0);
					if ( units == Common.Text.Properties.SizeUnits.Points )
					{
						size *= Modifier.fontSizeScale;
					}
				}
				editObject.SetTextLeading(size, units);
			}
		}

		
		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			Objects.Abstract editObject = this.EditObject;
			if ( editObject == null )  return;

			if ( sender == this.buttonBulletRound   )  this.InvertStyle(editObject, "BulletRound",   "Bullet");
			if ( sender == this.buttonBulletNumeric )  this.InvertStyle(editObject, "BulletNumeric", "Bullet");
			if ( sender == this.buttonBulletAlpha   )  this.InvertStyle(editObject, "BulletAlpha",   "Bullet");

			if ( sender == this.buttonIndentMinus )  this.IndentStyle(editObject, -100);
			if ( sender == this.buttonIndentPlus  )  this.IndentStyle(editObject,  100);
			
			if ( sender == this.buttonAlignLeft   )  this.InvertStyle(editObject, "AlignLeft",   "Align");
			if ( sender == this.buttonAlignCenter )  this.InvertStyle(editObject, "AlignCenter", "Align");
			if ( sender == this.buttonAlignRight  )  this.InvertStyle(editObject, "AlignRight",  "Align");
			if ( sender == this.buttonAlignJustif )  this.InvertStyle(editObject, "AlignJustif", "Align");
		}

		protected void IndentStyle(Objects.Abstract editObject, double distance)
		{
			double leftFirst, leftBody;
			Text.Properties.SizeUnits units;
			editObject.GetTextLeftMargins(out leftFirst, out leftBody, out units, true);

			leftFirst = System.Math.Max(leftFirst+distance, 0);
			leftBody  = System.Math.Max(leftBody+distance, 0);
			units = Common.Text.Properties.SizeUnits.Points;

			if ( leftFirst == 0 && leftBody == 0 )
			{
				leftFirst = 0;
				units = Common.Text.Properties.SizeUnits.None;
			}

			editObject.SetTextLeftMargins(leftFirst, leftBody, units);
		}

		protected void InvertStyle(Objects.Abstract editObject, string name, string exclude)
		{
			bool state = editObject.GetTextStyle(name);
			editObject.SetTextStyle(name, exclude, !state);
		}


		protected IconButton				buttonBulletRound;
		protected IconButton				buttonBulletNumeric;
		protected IconButton				buttonBulletAlpha;
		protected IconButton				buttonIndentMinus;
		protected IconButton				buttonIndentPlus;
		protected IconButton				buttonAlignLeft;
		protected IconButton				buttonAlignCenter;
		protected IconButton				buttonAlignRight;
		protected IconButton				buttonAlignJustif;
		protected TextFieldCombo			fieldLeading;
	}
}
