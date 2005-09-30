using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Scale permet de mettre à l'échelle la sélection.
	/// </summary>
	[SuppressBundleSupport]
	public class Scale : Abstract
	{
		public Scale() : base()
		{
			this.title.Text = Res.Strings.Action.ZoomMain;

			this.CreateButton(ref this.buttonZoomDiv2, "OperZoomDiv2", Res.Strings.Action.ZoomDiv2, new MessageEventHandler(this.HandleButtonZoomDiv2));
			this.CreateButton(ref this.buttonZoomMul2, "OperZoomMul2", Res.Strings.Action.ZoomMul2, new MessageEventHandler(this.HandleButtonZoomMul2));
			this.CreateButton(ref this.buttonMirrorH, "OperMirrorH", Res.Strings.Action.MirrorH, new MessageEventHandler(this.HandleButtonMirrorH));
			this.CreateButton(ref this.buttonMirrorV, "OperMirrorV", Res.Strings.Action.MirrorV, new MessageEventHandler(this.HandleButtonMirrorV));
			this.CreateSeparator(ref this.separator);
			this.CreateButton(ref this.buttonZoomi, "OperZoomi", Res.Strings.Action.ZoomFreeM, new MessageEventHandler(this.HandleButtonZoomi));
			this.CreateButton(ref this.buttonZoom,  "OperZoom",  Res.Strings.Action.ZoomFreeP, new MessageEventHandler(this.HandleButtonZoom));
			this.CreateFieldZoom(ref this.fieldZoom, Res.Strings.Action.ZoomValue);
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

			this.AdaptFieldZoom(this.fieldZoom);
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

			if ( this.buttonZoom == null )  return;

			double dx = this.buttonZoom.DefaultWidth;
			double dy = this.buttonZoom.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*2;
			rect.Width = this.separatorWidth;
			this.separator.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonZoomDiv2.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonZoomMul2.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonZoomi.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonZoom.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonMirrorH.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonMirrorV.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			rect.Width = 50;
			this.fieldZoom.Bounds = rect;
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

			this.buttonZoomDiv2.SetEnabled(enabled);
			this.buttonZoomMul2.SetEnabled(enabled);
			this.buttonMirrorH.SetEnabled(enabled);
			this.buttonMirrorV.SetEnabled(enabled);
			this.buttonZoomi.SetEnabled(enabled);
			this.buttonZoom.SetEnabled(enabled);
		}
		
		// Crée un champ éditable pour un zoom.
		protected void CreateFieldZoom(ref TextFieldReal field, string tooltip)
		{
			field = new TextFieldReal(this);
			field.Width = 50;
			field.TabIndex = tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(field, tooltip);
		}

		// Adapte un champ éditable pour un zoom.
		protected void AdaptFieldZoom(TextFieldReal field)
		{
			if ( this.document == null )
			{
				field.SetEnabled(false);
			}
			else
			{
				field.SetEnabled(true);

				this.ignoreChange = true;
				this.document.Modifier.AdaptTextFieldRealScalar(field);
				field.InternalMinValue = 1.0M;
				field.InternalMaxValue = 2.0M;
				field.DefaultValue = 1.0M;
				field.Step = 0.1M;
				field.Resolution = 0.01M;
				field.InternalValue = (decimal) this.document.Modifier.ZoomFactor;
				this.ignoreChange = false;
			}
		}

		private void HandleFieldValueChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == this.fieldZoom )
			{
				this.document.Modifier.ZoomFactor = (double) field.InternalValue;
			}
		}

		private void HandleButtonMirrorH(object sender, MessageEventArgs e)
		{
			this.document.Modifier.MirrorSelection(true);
		}

		private void HandleButtonMirrorV(object sender, MessageEventArgs e)
		{
			this.document.Modifier.MirrorSelection(false);
		}

		private void HandleButtonZoomMul2(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ZoomSelection(2.0);
		}

		private void HandleButtonZoomDiv2(object sender, MessageEventArgs e)
		{
			this.document.Modifier.ZoomSelection(0.5);
		}

		private void HandleButtonZoom(object sender, MessageEventArgs e)
		{
			double scale = this.document.Modifier.ZoomFactor;
			this.document.Modifier.ZoomSelection(scale);
		}

		private void HandleButtonZoomi(object sender, MessageEventArgs e)
		{
			double scale = this.document.Modifier.ZoomFactor;
			this.document.Modifier.ZoomSelection(1.0/scale);
		}


		protected IconButton				buttonZoomDiv2;
		protected IconButton				buttonZoomMul2;
		protected IconButton				buttonMirrorH;
		protected IconButton				buttonMirrorV;
		protected IconSeparator				separator;
		protected IconButton				buttonZoomi;
		protected IconButton				buttonZoom;
		protected TextFieldReal				fieldZoom;
	}
}
