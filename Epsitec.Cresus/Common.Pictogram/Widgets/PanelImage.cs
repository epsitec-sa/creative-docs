using Epsitec.Common.Widgets;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelImage permet de choisir un fichier image bitmap.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelImage : AbstractPanel
	{
		public PanelImage(Drawer drawer) : base(drawer)
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.fieldFilename = new TextField(this);
			this.fieldFilename.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldFilename.TabIndex = 1;
			this.fieldFilename.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldFilename, "Nom du fichier de l'image");

			this.buttonBrowse = new Button(this);
			this.buttonBrowse.Text = "Parcourir...";
			this.buttonBrowse.Clicked += new MessageEventHandler(this.HandleBrowseClicked);
			this.buttonBrowse.TabIndex = 2;
			this.buttonBrowse.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonMirrorH = new CheckButton(this);
			this.buttonMirrorH.Text = "Miroir X";
			this.buttonMirrorH.ActiveStateChanged += new EventHandler(this.ButtonActiveStateChanged);
			this.buttonMirrorH.TabIndex = 3;
			this.buttonMirrorH.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonMirrorV = new CheckButton(this);
			this.buttonMirrorV.Text = "Miroir Y";
			this.buttonMirrorV.ActiveStateChanged += new EventHandler(this.ButtonActiveStateChanged);
			this.buttonMirrorV.TabIndex = 4;
			this.buttonMirrorV.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonHomo = new CheckButton(this);
			this.buttonHomo.Text = "Conserver proportions";
			this.buttonHomo.ActiveStateChanged += new EventHandler(this.ButtonActiveStateChanged);
			this.buttonHomo.TabIndex = 5;
			this.buttonHomo.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldFilename.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.buttonBrowse.Clicked -= new MessageEventHandler(this.HandleBrowseClicked);
				this.buttonMirrorH.ActiveStateChanged -= new EventHandler(this.ButtonActiveStateChanged);
				this.buttonMirrorV.ActiveStateChanged -= new EventHandler(this.ButtonActiveStateChanged);
				this.buttonHomo.ActiveStateChanged -= new EventHandler(this.ButtonActiveStateChanged);
			}
			
			base.Dispose(disposing);
		}


		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return ( this.extendedSize ? 100 : 55 );
			}
		}

		// Propriété -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyImage p = property as PropertyImage;
			if ( p == null )  return;

			this.fieldFilename.Text = p.Filename;
			this.fieldFilename.Cursor = p.Filename.Length;
			this.buttonMirrorH.ActiveState = p.MirrorH ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonMirrorV.ActiveState = p.MirrorV ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			this.buttonHomo   .ActiveState = p.Homo    ? WidgetState.ActiveYes : WidgetState.ActiveNo;
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyImage p = new PropertyImage();
			base.GetProperty(p);

			p.Filename = this.fieldFilename.Text;
			p.MirrorH = ( this.buttonMirrorH.ActiveState == WidgetState.ActiveYes );
			p.MirrorV = ( this.buttonMirrorV.ActiveState == WidgetState.ActiveYes );
			p.Homo    = ( this.buttonHomo   .ActiveState == WidgetState.ActiveYes );
			return p;
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.fieldFilename == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Right = rect.Right-110;
			this.label.Bounds = r;
			r.Left = r.Right;
			r.Right = rect.Right;
			this.fieldFilename.Bounds = r;

			r.Offset(0, -25);
			r.Left = rect.Right-110;
			r.Right = rect.Right;
			this.buttonBrowse.Bounds = r;

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

			this.buttonMirrorH.SetVisible(this.extendedSize);
			this.buttonMirrorV.SetVisible(this.extendedSize);
			this.buttonHomo.SetVisible(this.extendedSize);
		}
		
		// Une valeur a été changée.
		private void HandleTextChanged(object sender)
		{
			this.OnChanged();
		}

		// Une valeur a été changée.
		private void ButtonActiveStateChanged(object sender)
		{
			this.OnChanged();
		}

		private void HandleBrowseClicked(object sender, MessageEventArgs e)
		{
			FileOpen dialog = new FileOpen();
		
			dialog.Title = "Ouvrir une image";
			dialog.FileName = this.fieldFilename.Text;
			dialog.Filters.Add("tiff", "Images TIFF", "*.tif");
			dialog.Filters.Add("jpg", "Images JPG", "*.jpg");
			dialog.Show();

			this.fieldFilename.Text = dialog.FileName;
			this.fieldFilename.Cursor = dialog.FileName.Length;
		}


		protected StaticText				label;
		protected TextField					fieldFilename;
		protected Button					buttonBrowse;
		protected CheckButton				buttonMirrorH;
		protected CheckButton				buttonMirrorV;
		protected CheckButton				buttonHomo;
	}
}
