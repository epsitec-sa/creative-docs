using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Zoom gère le zoom d'affichage.
	/// </summary>
	public class Zoom : Abstract
	{
		public Zoom() : base()
		{
			this.Title = Res.Strings.Action.ZoomMain;
			this.PreferredWidth = 8 + 22*4 + this.separatorWidth + 50;

			this.buttonZoomMin       = this.CreateIconButton("ZoomMin");
			this.buttonZoomPage      = this.CreateIconButton("ZoomPage");
			this.buttonZoomPageWidth = this.CreateIconButton("ZoomPageWidth");
			this.buttonZoomDefault   = this.CreateIconButton("ZoomDefault");
			this.buttonZoomSel       = this.CreateIconButton("ZoomSel");
			this.buttonZoomSelWidth  = this.CreateIconButton("ZoomSelWidth");
			this.buttonZoomPrev      = this.CreateIconButton("ZoomPrev");
			this.separator           = new IconSeparator(this);
			this.CreateFieldZoom(ref this.fieldZoom, Res.Strings.Action.ZoomValue);
			this.buttonOthers = this.CreateMenuButton("", Res.Strings.Action.ZoomMenu, this.HandleOthersPressed);
			
//			this.UpdateClientGeometry();
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
			if ( this.document != null )
			{
				this.document.Notifier.ZoomChanged -= this.HandleZoomChanged;
			}

			base.SetDocument(type, install, debug, gs, document);

			if ( this.document != null )
			{
				this.document.Notifier.ZoomChanged += this.HandleZoomChanged;
			}

			this.AdaptFieldZoom(this.fieldZoom);
			this.buttonOthers.Enable = (this.document != null);
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonZoomMin == null )  return;

			double dx = this.buttonZoomMin.PreferredWidth;
			double dy = this.buttonZoomMin.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*4;
			rect.Width = this.separatorWidth;
			this.separator.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonZoomMin.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonZoomPage.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonZoomPageWidth.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonZoomDefault.SetManualBounds(rect);
			rect.Offset(dx+this.separatorWidth, -dy*0.5-5);
			rect.Width = 50;
			this.fieldZoom.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonZoomSel.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonZoomSelWidth.SetManualBounds(rect);
			rect.Offset(dx*2, 0);
			this.buttonZoomPrev.SetManualBounds(rect);
			rect.Offset(dx+this.separatorWidth, 0);
			rect.Width = 50;
			rect.Height = dy*0.5;
			this.buttonOthers.SetManualBounds(rect);
		}


		protected void CreateFieldZoom(ref TextFieldReal field, string tooltip)
		{
			//	Crée un champ éditable pour le zoom.
			field = new TextFieldReal(this);
			field.PreferredWidth = 50;
			field.TabIndex = this.tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.ValueChanged += this.HandleFieldValueChanged;
			ToolTip.Default.SetToolTip(field, tooltip);
		}

		protected void AdaptFieldZoom(TextFieldReal field)
		{
			//	Adapte un champ éditable pour le zoom.
			if ( this.document == null )
			{
				field.Enable = false;
			}
			else
			{
				field.Enable = true;

				this.ignoreChange = true;
				this.document.Modifier.AdaptTextFieldRealPercent(field);
				field.Step = 5.0M;
				field.Resolution = 1.0M;
				field.InternalMinValue = 0.1M;
				field.InternalMaxValue = 16.0M;
				field.LogarithmicDivisor = 3.0M;
				field.DefaultValue = 1.0M;
				field.InternalValue = (decimal) this.document.Modifier.ActiveViewer.DrawingContext.Zoom;
				this.ignoreChange = false;
			}
		}

		private void HandleFieldValueChanged(object sender)
		{
			if ( this.ignoreChange )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == this.fieldZoom )
			{
				this.document.Modifier.ZoomValue((double) field.InternalValue);
			}
		}

		private void HandleZoomChanged()
		{
			//	Appelé par le document lorsque le zoom a changé.
			this.ignoreChange = true;
			this.fieldZoom.InternalValue = (decimal) this.document.Modifier.ActiveViewer.DrawingContext.Zoom;
			this.ignoreChange = false;
		}

		private void HandleOthersPressed(object sender, MessageEventArgs e)
		{
			//	Bouton pour ouvrir le menu des autres opérations.
			if ( this.document == null )  return;
			GlyphButton button = sender as GlyphButton;
			if ( button == null )  return;
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			VMenu menu = Menus.ZoomMenu.CreateZoomMenu(context.Zoom, context.ZoomPage, null);
			if ( menu == null )  return;
			menu.Host = this;
			menu.MinWidth = button.ActualWidth;
			TextFieldCombo.AdjustComboSize(button, menu, false);
			menu.ShowAsComboList(button, Point.Zero, button);
		}


		protected IconButton				buttonZoomMin;
		protected IconButton				buttonZoomPage;
		protected IconButton				buttonZoomPageWidth;
		protected IconButton				buttonZoomDefault;
		protected IconButton				buttonZoomSel;
		protected IconButton				buttonZoomSelWidth;
		protected IconButton				buttonZoomPrev;
		protected IconSeparator				separator;
		protected TextFieldReal				fieldZoom;
		protected GlyphButton				buttonOthers;
	}
}
