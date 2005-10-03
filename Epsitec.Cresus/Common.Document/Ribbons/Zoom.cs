using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Zoom permet de gérer les groupes.
	/// </summary>
	[SuppressBundleSupport]
	public class Zoom : Abstract
	{
		public Zoom() : base()
		{
			this.title.Text = Res.Strings.Action.ZoomMain;

			this.buttonZoomMin = this.CreateIconButton("ZoomMin", Misc.Icon("ZoomMin"), Res.Strings.Action.ZoomMin);
			this.buttonZoomPage = this.CreateIconButton("ZoomPage", Misc.Icon("ZoomPage"), Res.Strings.Action.ZoomPage);
			this.buttonZoomPageWidth = this.CreateIconButton("ZoomPageWidth", Misc.Icon("ZoomPageWidth"), Res.Strings.Action.ZoomPageWidth);
			this.buttonZoomDefault = this.CreateIconButton("ZoomDefault", Misc.Icon("ZoomDefault"), Res.Strings.Action.ZoomDefault);
			this.buttonZoomSel = this.CreateIconButton("ZoomSel", Misc.Icon("ZoomSel"), Res.Strings.Action.ZoomSel);
			this.buttonZoomSelWidth = this.CreateIconButton("ZoomSelWidth", Misc.Icon("ZoomSelWidth"), Res.Strings.Action.ZoomSelWidth);
			this.buttonZoomPrev = this.CreateIconButton("ZoomPrev", Misc.Icon("ZoomPrev"), Res.Strings.Action.ZoomPrev);
			this.separator = new IconSeparator(this);
			this.CreateFieldZoom(ref this.fieldZoom, "Zoom factor");
			this.buttonOthers = this.CreateMenuButton("", Res.Strings.Action.ZoomMenu, new MessageEventHandler(this.HandleOthersClicked));
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
			if ( this.document != null )
			{
				this.document.Notifier.ZoomChanged -= new SimpleEventHandler(this.HandleZoomChanged);
			}

			base.SetDocument(type, install, gs, document);

			if ( this.document != null )
			{
				this.document.Notifier.ZoomChanged += new SimpleEventHandler(this.HandleZoomChanged);
			}

			this.AdaptFieldZoom(this.fieldZoom);
			this.buttonOthers.SetEnabled(this.document != null);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*4 + this.separatorWidth + 50;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonZoomMin == null )  return;

			double dx = this.buttonZoomMin.DefaultWidth;
			double dy = this.buttonZoomMin.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*4;
			rect.Width = this.separatorWidth;
			this.separator.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonZoomMin.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonZoomPage.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonZoomPageWidth.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonZoomDefault.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, -dy*0.5);
			rect.Width = 50;
			this.fieldZoom.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonZoomSel.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonZoomSelWidth.Bounds = rect;
			rect.Offset(dx*2, 0);
			this.buttonZoomPrev.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			rect.Width = 50;
			rect.Height = dy*0.5;
			this.buttonOthers.Bounds = rect;
		}


		// Crée un champ éditable pour le zoom.
		protected void CreateFieldZoom(ref TextFieldReal field, string tooltip)
		{
			field = new TextFieldReal(this);
			field.Width = 50;
			field.TabIndex = tabIndex++;
			field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			field.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(field, tooltip);
		}

		// Adapte un champ éditable pour le zoom.
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
				this.document.Modifier.AdaptTextFieldRealPercent(field);
				field.Step = 5.0M;
				field.Resolution = 1.0M;
				field.InternalMinValue = 0.1M;
				field.InternalMaxValue = 16.0M;
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

		// Appelé par le document lorsque le zoom a changé.
		private void HandleZoomChanged()
		{
			this.ignoreChange = true;
			this.fieldZoom.InternalValue = (decimal) this.document.Modifier.ActiveViewer.DrawingContext.Zoom;
			this.ignoreChange = false;
		}

		// Bouton pour ouvrir le menu des autres opérations.
		private void HandleOthersClicked(object sender, MessageEventArgs e)
		{
			if ( this.document == null )  return;
			GlyphButton button = sender as GlyphButton;
			if ( button == null )  return;
			Point pos = button.MapClientToScreen(new Point(0, 1));
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			VMenu menu = ZoomMenu.CreateZoomMenu(context.Zoom, context.ZoomPage, null);
			if ( menu == null )  return;
			menu.Host = this;
			menu.ShowAsContextMenu(this.Window, pos);
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
