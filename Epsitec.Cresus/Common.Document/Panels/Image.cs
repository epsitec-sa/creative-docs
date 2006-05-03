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

			this.buttonFilter = new CheckButton(this);
			this.buttonFilter.Text = Res.Strings.Panel.Image.Button.Filter;
			this.buttonFilter.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonFilter.TabIndex = 7;
			this.buttonFilter.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

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
				this.buttonFilter.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);

				this.fieldFilename = null;
				this.buttonBrowse = null;
				this.buttonUpdate = null;
				this.buttonMirrorH = null;
				this.buttonMirrorV = null;
				this.buttonHomo = null;
				this.buttonFilter = null;
			}
			
			base.Dispose(disposing);
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return ( this.isExtendedSize ? this.LabelHeight+120 : this.LabelHeight+55 );
			}
		}

		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			Properties.Image p = this.property as Properties.Image;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.fieldFilename.Text = TextLayout.ConvertToTaggedText(p.Filename);
			this.fieldFilename.Cursor = p.Filename.Length;
			this.buttonMirrorH.ActiveState = p.MirrorH ? ActiveState.Yes : ActiveState.No;
			this.buttonMirrorV.ActiveState = p.MirrorV ? ActiveState.Yes : ActiveState.No;
			this.buttonHomo   .ActiveState = p.Homo    ? ActiveState.Yes : ActiveState.No;
			this.buttonFilter .ActiveState = p.Filter  ? ActiveState.Yes : ActiveState.No;

			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Image p = this.property as Properties.Image;
			if ( p == null )  return;

			p.Filename = TextLayout.ConvertToSimpleText(this.fieldFilename.Text);
			p.MirrorH = ( this.buttonMirrorH.ActiveState == ActiveState.Yes );
			p.MirrorV = ( this.buttonMirrorV.ActiveState == ActiveState.Yes );
			p.Homo    = ( this.buttonHomo   .ActiveState == ActiveState.Yes );
			p.Filter  = ( this.buttonFilter .ActiveState == ActiveState.Yes );
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.fieldFilename == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			this.fieldFilename.SetManualBounds(r);

			r.Offset(0, -25);
			r.Right = rect.Right-50-2;
			this.buttonBrowse.SetManualBounds(r);
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.buttonUpdate.SetManualBounds(r);

			r.Offset(0, -25);
			r.Left = rect.Left;
			r.Width = 70;
			this.buttonMirrorH.SetManualBounds(r);
			r.Offset(80, 0);
			this.buttonMirrorV.SetManualBounds(r);

			r.Offset(0, -20);
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.buttonHomo.SetManualBounds(r);

			r.Offset(0, -20);
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.buttonFilter.SetManualBounds(r);

			this.buttonMirrorH.Visibility = (this.isExtendedSize);
			this.buttonMirrorV.Visibility = (this.isExtendedSize);
			this.buttonHomo.Visibility = (this.isExtendedSize);
			this.buttonFilter.Visibility = (this.isExtendedSize);
		}
		
		private void HandleTextChanged(object sender)
		{
			//	Une valeur a été changée.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleButtonActiveStateChanged(object sender)
		{
			//	Une valeur a été changée.
			this.OnChanged();
		}

		private void HandleBrowseClicked(object sender, MessageEventArgs e)
		{
			FileOpen dialog = new FileOpen();
		
			dialog.Title = Res.Strings.Panel.Image.Dialog.Title;
			dialog.FileName = this.fieldFilename.Text;
			dialog.Filters.Add("all", Res.Strings.File.Bitmap.All, "*.bmp; *.tif; *.tiff; *.jpg; *.jpeg; *.gif; *.png; *.wmf; *.emf");
			dialog.Filters.Add("bmp", Res.Strings.File.Bitmap.BMP, "*.bmp");
			dialog.Filters.Add("tif", Res.Strings.File.Bitmap.TIF, "*.tif; *.tiff");
			dialog.Filters.Add("jpg", Res.Strings.File.Bitmap.JPG, "*.jpg; *.jpeg");
			dialog.Filters.Add("gif", Res.Strings.File.Bitmap.GIF, "*.gif");
			dialog.Filters.Add("png", Res.Strings.File.Bitmap.PNG, "*.png");
			dialog.Filters.Add("wmf", Res.Strings.File.Vector.WMF, "*.wmf; *.emf");
			dialog.OpenDialog();

			this.fieldFilename.Text = TextLayout.ConvertToTaggedText(dialog.FileName);
			this.fieldFilename.Cursor = this.fieldFilename.Text.Length;
		}

		private void HandleUpdateClicked(object sender, MessageEventArgs e)
		{
			Properties.Image p = this.property as Properties.Image;
			p.ReloadDo();
		}


		protected TextField					fieldFilename;
		protected Button					buttonBrowse;
		protected Button					buttonUpdate;
		protected CheckButton				buttonMirrorH;
		protected CheckButton				buttonMirrorV;
		protected CheckButton				buttonHomo;
		protected CheckButton				buttonFilter;
	}
}
