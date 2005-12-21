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

			this.buttonRotate90      = this.CreateIconButton("Rotate90");
			this.buttonRotate270     = this.CreateIconButton("Rotate270");
			this.buttonRotate180     = this.CreateIconButton("Rotate180");
			this.separator           = new IconSeparator(this);
			this.buttonRotateFreeCCW = this.CreateIconButton("RotateFreeCCW");
			this.buttonRotateFreeCW  = this.CreateIconButton("RotateFreeCW");
			this.fieldRotate         = this.CreateFieldRot(Res.Strings.Action.RotateValue);
			
			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override void SetDocument(DocumentType type, InstallType install, DebugMode debug, Settings.GlobalSettings gs, Document document)
		{
			base.SetDocument(type, install, debug, gs, document);

			this.AdaptFieldRot(this.fieldRotate);
		}

		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 8 + 22*2 + this.separatorWidth + 50;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonRotate90 == null )  return;

			double dx = this.buttonRotate90.DefaultWidth;
			double dy = this.buttonRotate90.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*2;
			rect.Width = this.separatorWidth;
			this.separator.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonRotate90.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonRotate270.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			rect.Width = 25;
			this.buttonRotateFreeCCW.Bounds = rect;
			rect.Offset(25, 0);
			this.buttonRotateFreeCW.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*0.5, 0);
			this.buttonRotate180.Bounds = rect;
			rect.Offset(dx*1.5+this.separatorWidth, 0);
			rect.Width = 50;
			this.fieldRotate.Bounds = rect;
		}


		protected TextFieldReal CreateFieldRot(string tooltip)
		{
			//	Crée un champ éditable pour une rotation.
			TextFieldReal field = new TextFieldReal(this);
			field.Width = 50;
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(field, tooltip);
			return field;
		}

		protected void AdaptFieldRot(TextFieldReal field)
		{
			//	Adapte un champ éditable pour une rotation.
			if ( this.document == null )
			{
				field.Enable = false;
			}
			else
			{
				field.Enable = true;

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


		protected IconButton				buttonRotate90;
		protected IconButton				buttonRotate180;
		protected IconButton				buttonRotate270;
		protected IconSeparator				separator;
		protected IconButton				buttonRotateFreeCCW;
		protected IconButton				buttonRotateFreeCW;
		protected TextFieldReal				fieldRotate;
	}
}
