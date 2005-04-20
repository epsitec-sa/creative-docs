using Epsitec.Common.Widgets;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Image permet de choisir un fichier image bitmap.
	/// </summary>
	[SuppressBundleSupport]
	public class Image : Abstract
	{
		public Image(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;

			this.fieldFilename = new TextField(this);
			this.fieldFilename.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldFilename.TabIndex = 1;
			this.fieldFilename.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldFilename, Res.Strings.Panel.Image.Tooltip.Filename);

			this.buttonBrowse = new Button(this);
			this.buttonBrowse.Text = Res.Strings.Dialog.Button.Browse;
			this.buttonBrowse.Clicked += new MessageEventHandler(this.HandleBrowseClicked);
			this.buttonBrowse.TabIndex = 2;
			this.buttonBrowse.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonBrowse, Res.Strings.Panel.Image.Tooltip.Browse);

			this.buttonUpdate = new Button(this);
			this.buttonUpdate.Text = Res.Strings.Panel.Image.Button.Update;
			this.buttonUpdate.Clicked += new MessageEventHandler(this.HandleUpdateClicked);
			this.buttonUpdate.TabIndex = 3;
			this.buttonUpdate.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonUpdate, Res.Strings.Panel.Image.Tooltip.Update);

			this.buttonMirrorH = new CheckButton(this);
			this.buttonMirrorH.Text = Res.Strings.Panel.Image.Button.MirrorX;
			this.buttonMirrorH.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonMirrorH.TabIndex = 4;
			this.buttonMirrorH.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonMirrorV = new CheckButton(this);
			this.buttonMirrorV.Text = Res.Strings.Panel.Image.Button.MirrorY;
			this.buttonMirrorV.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonMirrorV.TabIndex = 5;
			this.buttonMirrorV.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonHomo = new CheckButton(this);
			this.buttonHomo.Text = Res.Strings.Panel.Image.Button.Homo;
			this.buttonHomo.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonHomo.TabIndex = 6;
			this.buttonHomo.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldFilename.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.buttonBrowse.Clicked -= new MessageEventHandler(this.HandleBrowseClicked);
				this.buttonUpdate.Clicked -= new MessageEventHandler(this.HandleUpdateClicked);
				this.buttonMirrorH.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonMirrorV.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonHomo.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);

				this.label = null;
				this.fieldFilename = null;
				this.buttonBrowse = null;
				this.buttonUpdate = null;
				this.buttonMirrorH = null;
				this.buttonMirrorV = null;
				this.buttonHomo = null;
			}
			
			base.Dispose(disposing);
		}


		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.isExtendedSize ? 100 : 55 );
			}
		}

		// Propriété -> widgets.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			Properties.Image p = this.property as Properties.Image;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.label.Text = p.TextStyle;

			this.fieldFilename.Text = p.Filename;
			this.fieldFilename.Cursor = p.Filename.Length;
			this.buttonMirrorH.ActiveState = p.MirrorH ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonMirrorV.ActiveState = p.MirrorV ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonHomo   .ActiveState = p.Homo    ? WidgetState.ActiveYes : WidgetState.ActiveNo;

			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			Properties.Image p = this.property as Properties.Image;
			if ( p == null )  return;

			p.Filename = this.fieldFilename.Text;
			p.MirrorH = ( this.buttonMirrorH.ActiveState == WidgetState.ActiveYes );
			p.MirrorV = ( this.buttonMirrorV.ActiveState == WidgetState.ActiveYes );
			p.Homo    = ( this.buttonHomo   .ActiveState == WidgetState.ActiveYes );
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldFilename == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Right = rect.Right-110;
			this.label.Bounds = r;
			r.Left = r.Right;
			r.Right = rect.Right;
			this.fieldFilename.Bounds = r;

			r.Offset(0, -25);
			r.Left = rect.Right-110;
			r.Right = rect.Right-40;
			this.buttonBrowse.Bounds = r;
			r.Left = rect.Right-38;
			r.Right = rect.Right;
			this.buttonUpdate.Bounds = r;

			r.Offset(0, -25);
			r.Left = rect.Left;
			r.Width = 70;
			this.buttonMirrorH.Bounds = r;
			r.Offset(80, 0);
			this.buttonMirrorV.Bounds = r;

			r.Offset(0, -20);
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.buttonHomo.Bounds = r;

			this.buttonMirrorH.SetVisible(this.isExtendedSize);
			this.buttonMirrorV.SetVisible(this.isExtendedSize);
			this.buttonHomo.SetVisible(this.isExtendedSize);
		}
		
		// Une valeur a été changée.
		private void HandleTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Une valeur a été changée.
		private void HandleButtonActiveStateChanged(object sender)
		{
			this.OnChanged();
		}

		private void HandleBrowseClicked(object sender, MessageEventArgs e)
		{
			FileOpen dialog = new FileOpen();
		
			dialog.Title = Res.Strings.Panel.Image.Dialog.Title;
			dialog.FileName = this.fieldFilename.Text;
			dialog.Filters.Add("all", Res.Strings.File.Bitmap.All, "*.bmp; *.tif; *.tiff; *.jpg; *.jpeg; *.gif; *.png");
			dialog.Filters.Add("bmp", Res.Strings.File.Bitmap.BMP, "*.bmp");
			dialog.Filters.Add("tif", Res.Strings.File.Bitmap.TIF, "*.tif; *.tiff");
			dialog.Filters.Add("jpg", Res.Strings.File.Bitmap.JPG, "*.jpg; *.jpeg");
			dialog.Filters.Add("gif", Res.Strings.File.Bitmap.GIF, "*.gif");
			dialog.Filters.Add("png", Res.Strings.File.Bitmap.PNG, "*.png");
			dialog.OpenDialog();

			this.fieldFilename.Text = dialog.FileName;
			this.fieldFilename.Cursor = dialog.FileName.Length;
		}

		private void HandleUpdateClicked(object sender, MessageEventArgs e)
		{
			Properties.Image p = this.property as Properties.Image;
			p.Reload = true;
		}


		protected StaticText				label;
		protected TextField					fieldFilename;
		protected Button					buttonBrowse;
		protected Button					buttonUpdate;
		protected CheckButton				buttonMirrorH;
		protected CheckButton				buttonMirrorV;
		protected CheckButton				buttonHomo;
	}
}
