using Epsitec.Common.Widgets;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Image permet de choisir un fichier image bitmap.
	/// </summary>
	public class Image : Abstract
	{
		public Image(Document document) : base(document)
		{
			int tabIndex = 0;

			this.fieldFilename = new TextField(this);
			this.fieldFilename.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldFilename.TabIndex = tabIndex++;
			this.fieldFilename.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldFilename, Res.Strings.Panel.Image.Tooltip.Filename);

			this.buttonOpen = new Button(this);
			this.buttonOpen.Text = Res.Strings.Panel.Image.Button.Open;
			this.buttonOpen.Clicked += new MessageEventHandler(this.HandleOpenClicked);
			this.buttonOpen.TabIndex = tabIndex++;
			this.buttonOpen.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonOpen, Res.Strings.Panel.Image.Tooltip.Open);

			this.buttonUpdate = new Button(this);
			this.buttonUpdate.Text = Res.Strings.Panel.Image.Button.Update;
			this.buttonUpdate.Clicked += new MessageEventHandler(this.HandleUpdateClicked);
			this.buttonUpdate.TabIndex = tabIndex++;
			this.buttonUpdate.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonUpdate, Res.Strings.Panel.Image.Tooltip.Update);

			this.buttonSave = new Button(this);
			this.buttonSave.Text = Res.Strings.Panel.Image.Button.Save;
			this.buttonSave.Clicked += new MessageEventHandler(this.HandleSaveClicked);
			this.buttonSave.TabIndex = tabIndex++;
			this.buttonSave.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonSave, Res.Strings.Panel.Image.Tooltip.Save);

			this.cropper = new Widgets.Cropper(this);
			this.cropper.Document = this.document;
			this.cropper.CropChanged += new EventHandler(this.HandleCropChanged);

			this.buttonRotation0 = new RadioButton(this);
			this.buttonRotation0.Text = "0°";
			this.buttonRotation0.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonRotation0.TabIndex = tabIndex++;
			this.buttonRotation0.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonRotation90 = new RadioButton(this);
			this.buttonRotation90.Text = "90°";
			this.buttonRotation90.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonRotation90.TabIndex = tabIndex++;
			this.buttonRotation90.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonRotation180 = new RadioButton(this);
			this.buttonRotation180.Text = "180°";
			this.buttonRotation180.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonRotation180.TabIndex = tabIndex++;
			this.buttonRotation180.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonRotation270 = new RadioButton(this);
			this.buttonRotation270.Text = "270°";
			this.buttonRotation270.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonRotation270.TabIndex = tabIndex++;
			this.buttonRotation270.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonMirrorH = new CheckButton(this);
			this.buttonMirrorH.Text = Res.Strings.Panel.Image.Button.MirrorX;
			this.buttonMirrorH.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonMirrorH.TabIndex = tabIndex++;
			this.buttonMirrorH.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonMirrorV = new CheckButton(this);
			this.buttonMirrorV.Text = Res.Strings.Panel.Image.Button.MirrorY;
			this.buttonMirrorV.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonMirrorV.TabIndex = tabIndex++;
			this.buttonMirrorV.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonHomo = new CheckButton(this);
			this.buttonHomo.Text = Res.Strings.Panel.Image.Button.Homo;
			this.buttonHomo.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonHomo.TabIndex = tabIndex++;
			this.buttonHomo.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonFilter = new CheckButton(this);
			this.buttonFilter.Text = Res.Strings.Panel.Image.Button.Filter;
			this.buttonFilter.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonFilter.TabIndex = tabIndex++;
			this.buttonFilter.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.buttonInside = new CheckButton(this);
			this.buttonInside.Text = Res.Strings.Panel.Image.Button.Inside;
			this.buttonInside.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonInside.TabIndex = tabIndex++;
			this.buttonInside.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.isNormalAndExtended = true;
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldFilename.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.buttonOpen.Clicked -= new MessageEventHandler(this.HandleOpenClicked);
				this.buttonUpdate.Clicked -= new MessageEventHandler(this.HandleUpdateClicked);
				this.buttonSave.Clicked -= new MessageEventHandler(this.HandleSaveClicked);
				this.cropper.CropChanged -= new EventHandler(this.HandleCropChanged);
				this.buttonRotation0.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonRotation90.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonRotation180.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonRotation270.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonMirrorH.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonMirrorV.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonHomo.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonFilter.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonInside.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);

				this.fieldFilename = null;
				this.buttonOpen = null;
				this.buttonUpdate = null;
				this.buttonSave = null;
				this.cropper = null;
				this.buttonRotation0 = null;
				this.buttonRotation90 = null;
				this.buttonRotation180 = null;
				this.buttonRotation270 = null;
				this.buttonMirrorH = null;
				this.buttonMirrorV = null;
				this.buttonHomo = null;
				this.buttonFilter = null;
				this.buttonInside = null;
			}
			
			base.Dispose(disposing);
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return ( this.isExtendedSize ? this.LabelHeight+295 : this.LabelHeight+55 );
			}
		}

		public override void UpdateGeometry()
		{
			//	Met à jour après un changement de géométrie.
			this.cropper.UpdateField();
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

			this.buttonRotation0.ActiveState   = p.RotationMode == Properties.Image.Rotation.Angle0   ? ActiveState.Yes : ActiveState.No;
			this.buttonRotation90.ActiveState  = p.RotationMode == Properties.Image.Rotation.Angle90  ? ActiveState.Yes : ActiveState.No;
			this.buttonRotation180.ActiveState = p.RotationMode == Properties.Image.Rotation.Angle180 ? ActiveState.Yes : ActiveState.No;
			this.buttonRotation270.ActiveState = p.RotationMode == Properties.Image.Rotation.Angle270 ? ActiveState.Yes : ActiveState.No;
			
			this.buttonMirrorH.ActiveState = p.MirrorH   ? ActiveState.Yes : ActiveState.No;
			this.buttonMirrorV.ActiveState = p.MirrorV   ? ActiveState.Yes : ActiveState.No;
			this.buttonHomo   .ActiveState = p.Homo      ? ActiveState.Yes : ActiveState.No;
			this.buttonFilter .ActiveState = p.Filter    ? ActiveState.Yes : ActiveState.No;
			this.buttonInside .ActiveState = p.InsideDoc ? ActiveState.Yes : ActiveState.No;

			this.cropper.Crop = p.CropMargins;

			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Image p = this.property as Properties.Image;
			if ( p == null )  return;

			p.Filename  = TextLayout.ConvertToSimpleText(this.fieldFilename.Text);

			if (this.buttonRotation0.ActiveState   == ActiveState.Yes)  p.RotationMode = Properties.Image.Rotation.Angle0;
			if (this.buttonRotation90.ActiveState  == ActiveState.Yes)  p.RotationMode = Properties.Image.Rotation.Angle90;
			if (this.buttonRotation180.ActiveState == ActiveState.Yes)  p.RotationMode = Properties.Image.Rotation.Angle180;
			if (this.buttonRotation270.ActiveState == ActiveState.Yes)  p.RotationMode = Properties.Image.Rotation.Angle270;

			p.MirrorH   = ( this.buttonMirrorH.ActiveState == ActiveState.Yes );
			p.MirrorV   = ( this.buttonMirrorV.ActiveState == ActiveState.Yes );
			p.Homo      = ( this.buttonHomo   .ActiveState == ActiveState.Yes );
			p.Filter    = ( this.buttonFilter .ActiveState == ActiveState.Yes );
			p.InsideDoc = ( this.buttonInside .ActiveState == ActiveState.Yes );

			p.CropMargins = this.cropper.Crop;
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
			r.Left = rect.Left;
			r.Right = rect.Right-50-2;
			this.buttonOpen.SetManualBounds(r);
			r.Left = rect.Right-50;
			r.Right = rect.Right;
			this.buttonUpdate.SetManualBounds(r);

			r.Offset(0, -25);
			r.Left = rect.Left;
			r.Right = rect.Right-50-2;
			this.buttonSave.SetManualBounds(r);

			r.Offset(0, -110);
			Rectangle rr = r;
			rr.Top = rr.Bottom+105;
			rr.Left = rect.Left;
			rr.Right = rect.Right;
			this.cropper.SetManualBounds(rr);

			r.Offset(0, -25);
			r.Left = rect.Left;
			r.Width = 40;
			this.buttonRotation0.SetManualBounds(r);
			r.Offset(r.Width, 0);
			r.Width = 45;
			this.buttonRotation90.SetManualBounds(r);
			r.Offset(r.Width, 0);
			r.Width = 50;
			this.buttonRotation180.SetManualBounds(r);
			r.Offset(r.Width, 0);
			r.Width = 45;
			this.buttonRotation270.SetManualBounds(r);

			r.Offset(0, -20);
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

			r.Offset(0, -20);
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.buttonInside.SetManualBounds(r);

			this.buttonSave.Visibility = (this.isExtendedSize);
			this.cropper.Visibility = (this.isExtendedSize);
			this.buttonRotation0.Visibility = (this.isExtendedSize);
			this.buttonRotation90.Visibility = (this.isExtendedSize);
			this.buttonRotation180.Visibility = (this.isExtendedSize);
			this.buttonRotation270.Visibility = (this.isExtendedSize);
			this.buttonMirrorH.Visibility = (this.isExtendedSize);
			this.buttonMirrorV.Visibility = (this.isExtendedSize);
			this.buttonHomo.Visibility = (this.isExtendedSize);
			this.buttonFilter.Visibility = (this.isExtendedSize);
			this.buttonInside.Visibility = (this.isExtendedSize);
		}
		
		private void HandleTextChanged(object sender)
		{
			//	Le nom de l'image a été changé.
			if (this.ignoreChanged)
			{
				return;
			}

			this.OnChanged();

			Properties.Image p = this.property as Properties.Image;
			string filename = p.Filename;

			if (!string.IsNullOrEmpty(filename))
			{
				ImageCache.Item item = this.document.ImageCache.Get(filename);

				if (item == null)
				{
					item = this.document.ImageCache.Add(filename, null);

					if (item.Image == null)
					{
						this.document.ImageCache.Remove(filename);
					}
				}
			}
		}

		private void HandleButtonActiveStateChanged(object sender)
		{
			//	Une valeur a été changée.
			this.OnChanged();
		}

		private void HandleOpenClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'Importer' pour choisir l'image a été cliqué.
			FileOpen dialog = new FileOpen();
			dialog.Title = Res.Strings.Panel.Image.Dialog.Open.Title;
			dialog.FileName = this.fieldFilename.Text;
			dialog.Filters.Add("all", Res.Strings.File.Bitmap.All, "*.bmp; *.tif; *.tiff; *.jpg; *.jpeg; *.gif; *.png; *.wmf; *.emf");
			dialog.Filters.Add("bmp", Res.Strings.File.Bitmap.BMP, "*.bmp");
			dialog.Filters.Add("tif", Res.Strings.File.Bitmap.TIF, "*.tif; *.tiff");
			dialog.Filters.Add("jpg", Res.Strings.File.Bitmap.JPG, "*.jpg; *.jpeg");
			dialog.Filters.Add("gif", Res.Strings.File.Bitmap.GIF, "*.gif");
			dialog.Filters.Add("png", Res.Strings.File.Bitmap.PNG, "*.png");
			dialog.Filters.Add("wmf", Res.Strings.File.Vector.WMF, "*.wmf; *.emf");
			dialog.OpenDialog();  // demande le nom du fichier...

			this.fieldFilename.Text = TextLayout.ConvertToTaggedText(dialog.FileName);
			this.fieldFilename.Cursor = this.fieldFilename.Text.Length;
		}

		private void HandleUpdateClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'Màj' pour relire l'image a été cliqué.
			Properties.Image p = this.property as Properties.Image;
			ImageCache.Item item = this.document.ImageCache.Get(p.Filename);

			if (item != null)
			{
				if (item.Reload())  // relit l'image sur disque
				{
					return;  // tout c'est bien passé
				}
			}

			//	Indique que la mise à jour n'est pas possible.
			this.document.Modifier.ActiveViewer.DialogError(Res.Strings.Error.FileDoesNoetExist);
		}

		private void HandleSaveClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'Exporter' pour choisir l'image a été cliqué.
			Properties.Image p = this.property as Properties.Image;
			ImageCache.Item item = this.document.ImageCache.Get(p.Filename);
			if (item == null)
			{
				return;
			}

			string ext = System.IO.Path.GetExtension(this.fieldFilename.Text);
			if (ext.StartsWith("."))
			{
				ext = ext.Substring(1);  // enlève le point
			}

			FileSave dialog = new FileSave();
			dialog.Title = Res.Strings.Panel.Image.Dialog.Save.Title;
			dialog.FileName = this.fieldFilename.Text;
			dialog.PromptForOverwriting = true;
			dialog.Filters.Add(ext, Res.Strings.File.Bitmap.All, "*."+ext);
			dialog.OpenDialog();  // demande le nom du fichier...
			if (dialog.Result != Common.Dialogs.DialogResult.Accept)
			{
				return;
			}

			item.Write(dialog.FileName);  // écrit le fichier sur disque
		}

		void HandleCropChanged(object sender)
		{
			this.OnChanged();
		}



		protected TextField					fieldFilename;
		protected Button					buttonOpen;
		protected Button					buttonUpdate;
		protected Button					buttonSave;
		protected Widgets.Cropper			cropper;
		protected RadioButton				buttonRotation0;
		protected RadioButton				buttonRotation90;
		protected RadioButton				buttonRotation180;
		protected RadioButton				buttonRotation270;
		protected CheckButton				buttonMirrorH;
		protected CheckButton				buttonMirrorV;
		protected CheckButton				buttonHomo;
		protected CheckButton				buttonFilter;
		protected CheckButton				buttonInside;

		protected Margins					initialCrop;
	}
}

