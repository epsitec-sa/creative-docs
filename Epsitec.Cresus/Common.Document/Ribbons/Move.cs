using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Move permet de déplacer la sélection.
	/// </summary>
	[SuppressBundleSupport]
	public class Move : Abstract
	{
		public Move(Document document) : base(document)
		{
			this.title.Text = Res.Strings.Action.MoveMain;

			this.CreateButton(ref this.buttonMoveHi, "OperMoveHi", Res.Strings.Action.MoveLeft,  new MessageEventHandler(this.HandleButtonMoveHi));
			this.CreateButton(ref this.buttonMoveH,  "OperMoveH",  Res.Strings.Action.MoveRight, new MessageEventHandler(this.HandleButtonMoveH));
			this.CreateButton(ref this.buttonMoveVi, "OperMoveVi", Res.Strings.Action.MoveDown, new MessageEventHandler(this.HandleButtonMoveVi));
			this.CreateButton(ref this.buttonMoveV,  "OperMoveV",  Res.Strings.Action.MoveUp,   new MessageEventHandler(this.HandleButtonMoveV));
			this.CreateFieldMove(ref this.fieldMoveH, Res.Strings.Action.MoveValueX);
			this.CreateFieldMove(ref this.fieldMoveV, Res.Strings.Action.MoveValueY);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		// Retourne la largeur compacte.
		public override double CompactWidth
		{
			get
			{
				return 8+22+22;
			}
		}

		// Retourne la largeur étendue.
		public override double ExtendWidth
		{
			get
			{
				return 8+22+22+50;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonMoveH == null )  return;

			double dx = this.buttonMoveH.DefaultWidth;
			double dy = this.buttonMoveH.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonMoveHi.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonMoveH.Bounds = rect;
			rect.Offset(dx, 0);
			rect.Width = 50;
			this.fieldMoveH.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonMoveVi.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonMoveV.Bounds = rect;
			rect.Offset(dx, 0);
			rect.Width = 50;
			this.fieldMoveV.Bounds = rect;
		}


		// Met à jour les boutons.
		protected override void UpdateButtons()
		{
			base.UpdateButtons();

			if ( this.fieldMoveH == null )  return;

			this.fieldMoveH.SetVisible(this.isExtendedSize);
			this.fieldMoveV.SetVisible(this.isExtendedSize);
		}

		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			bool enabled = (this.document.Modifier.TotalSelected > 0);

			if ( this.document.Modifier.Tool == "Edit" )
			{
				enabled = false;
			}

			this.buttonMoveH.SetEnabled(enabled);
			this.buttonMoveHi.SetEnabled(enabled);
			this.buttonMoveV.SetEnabled(enabled);
			this.buttonMoveVi.SetEnabled(enabled);
		}
		
		// Crée un champ éditable pour un déplacement.
		protected void CreateFieldMove(ref TextFieldReal field, string tooltip)
		{
			field = new TextFieldReal(this);
			this.document.Modifier.AdaptTextFieldRealDimension(field);
			field.Width = 50;
			if ( this.document.Type == DocumentType.Pictogram )
			{
				field.InternalValue = 1.0M;
			}
			else
			{
				field.InternalValue = 100.0M;  // 10mm
			}
			field.TabIndex = this.tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(field, tooltip);
		}


		private void HandleButtonMoveH(object sender, MessageEventArgs e)
		{
			double dx = (double) this.fieldMoveH.InternalValue;
			this.document.Modifier.MoveSelection(new Point(dx,0));
		}

		private void HandleButtonMoveHi(object sender, MessageEventArgs e)
		{
			double dx = (double) this.fieldMoveH.InternalValue;
			this.document.Modifier.MoveSelection(new Point(-dx,0));
		}

		private void HandleButtonMoveV(object sender, MessageEventArgs e)
		{
			double dy = (double) this.fieldMoveV.InternalValue;
			this.document.Modifier.MoveSelection(new Point(0,dy));
		}

		private void HandleButtonMoveVi(object sender, MessageEventArgs e)
		{
			double dy = (double) this.fieldMoveV.InternalValue;
			this.document.Modifier.MoveSelection(new Point(0,-dy));
		}


		protected IconButton				buttonMoveH;
		protected IconButton				buttonMoveHi;
		protected IconButton				buttonMoveV;
		protected IconButton				buttonMoveVi;
		protected TextFieldReal				fieldMoveH;
		protected TextFieldReal				fieldMoveV;
	}
}
