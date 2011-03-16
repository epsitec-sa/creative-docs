using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Move permet de déplacer des objets sélectionnés.
	/// </summary>
	public class MoveRibbon : AbstractRibbon
	{
		public MoveRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.Move;
			this.PreferredWidth = 8 + 22*2 + 50;

			this.buttonMoveLeft  = this.CreateIconButton("MoveLeft");
			this.buttonMoveRight = this.CreateIconButton("MoveRight");
			this.buttonMoveDown  = this.CreateIconButton("MoveDown");
			this.buttonMoveUp    = this.CreateIconButton("MoveUp");
			this.CreateFieldMove(ref this.fieldMoveH, Res.Strings.Ribbon.Section.MoveHorizontal);
			this.CreateFieldMove(ref this.fieldMoveV, Res.Strings.Ribbon.Section.MoveVertical);

			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonMoveLeft == null )  return;

			double dx = this.buttonMoveLeft.PreferredWidth;
			double dy = this.buttonMoveLeft.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonMoveLeft.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonMoveRight.SetManualBounds(rect);
			rect.Offset(dx, 0);
			rect.Width = 50;
			this.fieldMoveH.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonMoveDown.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonMoveUp.SetManualBounds(rect);
			rect.Offset(dx, 0);
			rect.Width = 50;
			this.fieldMoveV.SetManualBounds(rect);
		}


		protected void CreateFieldMove(ref TextFieldReal field, string tooltip)
		{
			//	Crée un champ éditable pour un déplacement.
			field = new TextFieldReal(this);
			field.PreferredWidth = 50;
			field.InternalMinValue     =   1.0M;
			field.InternalMaxValue     = 100.0M;
			field.InternalDefaultValue =   5.0M;
			field.Step                 =   1.0M;
			field.Resolution           =   1.0M;
			field.InternalValue = field.InternalDefaultValue;
			field.TabIndex = this.tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.ValueChanged += this.HandleFieldValueChanged;
			ToolTip.Default.SetToolTip(field, tooltip);
		}

		private void HandleFieldValueChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			TextFieldReal field = sender as TextFieldReal;

			if ( field == this.fieldMoveH )
			{
				this.designerApplication.MoveHorizontal= (double) field.InternalValue;
			}
			if ( field == this.fieldMoveV )
			{
				this.designerApplication.MoveVertical = (double) field.InternalValue;
			}
		}


		protected IconButton				buttonMoveLeft;
		protected IconButton				buttonMoveRight;
		protected IconButton				buttonMoveUp;
		protected IconButton				buttonMoveDown;
		protected TextFieldReal				fieldMoveH;
		protected TextFieldReal				fieldMoveV;
	}
}
