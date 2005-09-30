using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Rotate permet de tourner la sélection.
	/// </summary>
	[SuppressBundleSupport]
	public class Rotate : Abstract
	{
		public Rotate() : base()
		{
			this.title.Text = Res.Strings.Action.RotateMain;

			this.CreateButton(ref this.buttonRotate90,  "OperRot90",  Res.Strings.Action.Rotate90,  new MessageEventHandler(this.HandleButtonRotate90));
			this.CreateButton(ref this.buttonRotate270, "OperRot270", Res.Strings.Action.Rotate270, new MessageEventHandler(this.HandleButtonRotate270));
			this.CreateButton(ref this.buttonRotate180, "OperRot180", Res.Strings.Action.Rotate180, new MessageEventHandler(this.HandleButtonRotate180));
			this.CreateSeparator(ref this.separator);
			this.CreateButton(ref this.buttonRotate,  "OperRot",  Res.Strings.Action.RotateFreeCCW, new MessageEventHandler(this.HandleButtonRotate));
			this.CreateButton(ref this.buttonRotatei, "OperRoti", Res.Strings.Action.RotateFreeCW,  new MessageEventHandler(this.HandleButtonRotatei));
			this.CreateFieldRot(ref this.fieldRotate, Res.Strings.Action.RotateValue);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override void SetDocument(DocumentType type, Settings.GlobalSettings gs, Document document)
		{
			base.SetDocument(type, gs, document);

			this.AdaptFieldRot(this.fieldRotate);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*2 + this.separatorWidth + 50;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonRotate == null )  return;

			double dx = this.buttonRotate.DefaultWidth;
			double dy = this.buttonRotate.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*2;
			rect.Width = this.separatorWidth;
			this.separator.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonRotate90.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonRotate270.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonRotate.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonRotatei.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*0.5, 0);
			this.buttonRotate180.Bounds = rect;
			rect.Offset(dx*1.5+this.separatorWidth, 0);
			rect.Width = 50;
			this.fieldRotate.Bounds = rect;
		}


		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			bool enabled = false;

			if ( this.document != null )
			{
				enabled = (this.document.Modifier.TotalSelected > 0);

				if ( this.document.Modifier.Tool == "Edit" )
				{
					enabled = false;
				}
			}

			this.buttonRotate90.SetEnabled(enabled);
			this.buttonRotate180.SetEnabled(enabled);
			this.buttonRotate270.SetEnabled(enabled);
			this.buttonRotate.SetEnabled(enabled);
			this.buttonRotatei.SetEnabled(enabled);
		}
		
		// Crée un champ éditable pour une rotation.
		protected void CreateFieldRot(ref TextFieldReal field, string tooltip)
		{
			field = new TextFieldReal(this);
			field.Width = 50;
			field.TabIndex = tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(field, tooltip);
		}

		// Adapte un champ éditable pour une rotation.
		protected void AdaptFieldRot(TextFieldReal field)
		{
			if ( this.document == null )
			{
				field.SetEnabled(false);
			}
			else
			{
				field.SetEnabled(true);

				this.ignoreChange = true;
				this.document.Modifier.AdaptTextFieldRealAngle(field);
				field.InternalValue = (decimal) this.document.Modifier.RotateAngle;
				this.ignoreChange = false;
			}
		}

		private void HandleFieldValueChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == this.fieldRotate )
			{
				this.document.Modifier.RotateAngle = (double) field.InternalValue;
			}
		}

		private void HandleButtonRotate90(object sender, MessageEventArgs e)
		{
			this.document.Modifier.RotateSelection(90.0);
		}

		private void HandleButtonRotate180(object sender, MessageEventArgs e)
		{
			this.document.Modifier.RotateSelection(180.0);
		}

		private void HandleButtonRotate270(object sender, MessageEventArgs e)
		{
			this.document.Modifier.RotateSelection(270.0);
		}

		private void HandleButtonRotate(object sender, MessageEventArgs e)
		{
			double angle = this.document.Modifier.RotateAngle;
			this.document.Modifier.RotateSelection(angle);
		}

		private void HandleButtonRotatei(object sender, MessageEventArgs e)
		{
			double angle = this.document.Modifier.RotateAngle;
			this.document.Modifier.RotateSelection(-angle);
		}


		protected IconButton				buttonRotate90;
		protected IconButton				buttonRotate180;
		protected IconButton				buttonRotate270;
		protected IconSeparator				separator;
		protected IconButton				buttonRotate;
		protected IconButton				buttonRotatei;
		protected TextFieldReal				fieldRotate;
	}
}
