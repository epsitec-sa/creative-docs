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

			this.buttonRotation90 = new IconButton(this);
			this.buttonRotation90.IconName = Misc.Icon("ImageRotation90");
			this.buttonRotation90.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonRotation90.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.buttonRotation90.TabIndex = tabIndex++;
			this.buttonRotation90.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonRotation90, Res.Strings.Panel.Image.Button.Rotation90);

			this.buttonRotation180 = new IconButton(this);
			this.buttonRotation180.IconName = Misc.Icon("ImageRotation180");
			this.buttonRotation180.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonRotation180.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.buttonRotation180.TabIndex = tabIndex++;
			this.buttonRotation180.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonRotation180, Res.Strings.Panel.Image.Button.Rotation180);

			this.buttonRotation270 = new IconButton(this);
			this.buttonRotation270.IconName = Misc.Icon("ImageRotation270");
			this.buttonRotation270.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonRotation270.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.buttonRotation270.TabIndex = tabIndex++;
			this.buttonRotation270.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonRotation270, Res.Strings.Panel.Image.Button.Rotation270);

			this.buttonMirrorH = new IconButton(this);
			this.buttonMirrorH.IconName = Misc.Icon("ImageMirrorH");
			this.buttonMirrorH.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonMirrorH.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.buttonMirrorH.TabIndex = tabIndex++;
			this.buttonMirrorH.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonMirrorH, Res.Strings.Panel.Image.Button.MirrorX);

			this.buttonMirrorV = new IconButton(this);
			this.buttonMirrorV.IconName = Misc.Icon("ImageMirrorV");
			this.buttonMirrorV.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonMirrorV.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.buttonMirrorV.TabIndex = tabIndex++;
			this.buttonMirrorV.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonMirrorV, Res.Strings.Panel.Image.Button.MirrorY);

			this.buttonFilter = new IconButtonCombo(this);
			this.buttonFilter.AutoFocus = false;
			this.buttonFilter.IsLiveUpdateEnabled = false;
			this.buttonFilter.IconName = Misc.Icon("ImageFilter");
			this.buttonFilter.ComboClosed += new EventHandler(this.HandleFilterComboClosed);
			this.buttonFilter.TabIndex = tabIndex++;
			this.buttonFilter.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
#if false
			this.AddFilterCombo("None");
			this.AddFilterCombo("Bilinear");
			this.AddFilterCombo("Bicubic");
			this.AddFilterCombo("Spline16");
			this.AddFilterCombo("Spline36");
			this.AddFilterCombo("Kaiser");
			this.AddFilterCombo("Quadric");
			this.AddFilterCombo("Catrom");
			this.AddFilterCombo("Gaussian");
			this.AddFilterCombo("Bessel");
			this.AddFilterCombo("Mitchell");
			this.AddFilterCombo("Sinc1");
			this.AddFilterCombo("Sinc2");
			this.AddFilterCombo("Lanczos1");
			this.AddFilterCombo("Lanczos2");
			this.AddFilterCombo("Blackman1");
			this.AddFilterCombo("Blackman2");
#else
			this.AddFilterCombo("None");
			this.AddFilterCombo("Bilinear");
			this.AddFilterCombo("Bicubic");
#endif
			ToolTip.Default.SetToolTip(this.buttonFilter, Res.Strings.Panel.Image.Button.Filter);

			this.buttonHomo = new IconButton(this);
			this.buttonHomo.IconName = Misc.Icon("ImageHomo");
			this.buttonHomo.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonHomo.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.buttonHomo.TabIndex = tabIndex++;
			this.buttonHomo.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonHomo, Res.Strings.Panel.Image.Button.Homo);

			this.cropper = new Widgets.Cropper(this);
			this.cropper.Document = this.document;
			this.cropper.CropChanged += new EventHandler(this.HandleCropChanged);

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
				this.buttonRotation90.Pressed -= new MessageEventHandler(this.HandleButtonPressed);
				this.buttonRotation180.Pressed -= new MessageEventHandler(this.HandleButtonPressed);
				this.buttonRotation270.Pressed -= new MessageEventHandler(this.HandleButtonPressed);
				this.buttonMirrorH.Pressed -= new MessageEventHandler(this.HandleButtonPressed);
				this.buttonMirrorV.Pressed -= new MessageEventHandler(this.HandleButtonPressed);
				this.buttonHomo.Pressed -= new MessageEventHandler(this.HandleButtonPressed);
				this.buttonFilter.ComboClosed -= new EventHandler(this.HandleFilterComboClosed);
				this.buttonInside.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);

				this.fieldFilename = null;
				this.buttonOpen = null;
				this.buttonUpdate = null;
				this.buttonSave = null;
				this.cropper = null;
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
				return ( this.isExtendedSize ? this.LabelHeight+245 : this.LabelHeight+55 );
			}
		}

		public override void UpdateGeometry()
		{
			//	Met à jour après un changement de géométrie.
			this.cropper.UpdateField();
		}


		protected void AddFilterCombo(string name)
		{
			//	Ajoute une ligne au menu du filtre.
			string text = Image.NameToText(name);
			string icon = name == "None" ? "DeleteItem" : "ImageFilter";
			string regularText  = text;
			string selectedText = Misc.Bold(text);
			string briefIcon    = Misc.Icon(icon);
			IconButtonCombo.Item item = new IconButtonCombo.Item(name, briefIcon, regularText, selectedText);
			this.buttonFilter.Items.Add(item);
		}

		protected static ImageFilter NameToFilter(string name)
		{
			switch (name)
			{
				case "None":
					return new ImageFilter(ImageFilteringMode.None);

				case "Bilinear":
					return new ImageFilter(ImageFilteringMode.Bilinear);

				case "Bicubic":
					return new ImageFilter(ImageFilteringMode.Bicubic);

				case "Spline16":
					return new ImageFilter(ImageFilteringMode.Spline16);

				case "Spline36":
					return new ImageFilter(ImageFilteringMode.Spline36);

				case "Kaiser":
					return new ImageFilter(ImageFilteringMode.Kaiser);

				case "Quadric":
					return new ImageFilter(ImageFilteringMode.Quadric);

				case "Catrom":
					return new ImageFilter(ImageFilteringMode.Catrom);

				case "Gaussian":
					return new ImageFilter(ImageFilteringMode.Gaussian);

				case "Bessel":
					return new ImageFilter(ImageFilteringMode.Bessel);

				case "Mitchell":
					return new ImageFilter(ImageFilteringMode.Mitchell);

				case "Sinc1":
					return new ImageFilter(ImageFilteringMode.Sinc, Image.normRadius);

				case "Sinc2":
					return new ImageFilter(ImageFilteringMode.Sinc, Image.softRadius);

				case "Lanczos1":
					return new ImageFilter(ImageFilteringMode.Lanczos, Image.normRadius);

				case "Lanczos2":
					return new ImageFilter(ImageFilteringMode.Lanczos, Image.softRadius);

				case "Blackman1":
					return new ImageFilter(ImageFilteringMode.Blackman, Image.normRadius);

				case "Blackman2":
					return new ImageFilter(ImageFilteringMode.Blackman, Image.softRadius);
			}

			return new ImageFilter(ImageFilteringMode.None);
		}

		protected static string NameToText(string name)
		{
			switch (name)
			{
				case "None":
					return Res.Strings.Panel.Image.Filter.None;

				case "Bilinear":
					return Res.Strings.Panel.Image.Filter.Bilinear;

				case "Bicubic":
					return Res.Strings.Panel.Image.Filter.Bicubic;

				default:
					return string.Format(Res.Strings.Panel.Image.Filter.Other, name);
			}
		}

		protected static string FilterToName(ImageFilter filter)
		{
			if (filter.Mode == ImageFilteringMode.None)
			{
				return "None";
			}

			if (filter.Mode == ImageFilteringMode.Bilinear)
			{
				return "Bilinear";
			}

			if (filter.Mode == ImageFilteringMode.Bicubic)
			{
				return "Bicubic";
			}

			if (filter.Mode == ImageFilteringMode.Spline16)
			{
				return "Spline16";
			}

			if (filter.Mode == ImageFilteringMode.Spline36)
			{
				return "Spline36";
			}

			if (filter.Mode == ImageFilteringMode.Kaiser)
			{
				return "Kaiser";
			}

			if (filter.Mode == ImageFilteringMode.Quadric)
			{
				return "Quadric";
			}

			if (filter.Mode == ImageFilteringMode.Catrom)
			{
				return "Catrom";
			}

			if (filter.Mode == ImageFilteringMode.Gaussian)
			{
				return "Gaussian";
			}

			if (filter.Mode == ImageFilteringMode.Bessel)
			{
				return "Bessel";
			}

			if (filter.Mode == ImageFilteringMode.Mitchell)
			{
				return "Mitchell";
			}

			if (filter.Mode == ImageFilteringMode.Sinc)
			{
				if (filter.Radius == Image.normRadius)
				{
					return "Sinc1";
				}
				else
				{
					return "Sinc2";
				}
			}

			if (filter.Mode == ImageFilteringMode.Lanczos)
			{
				if (filter.Radius == Image.normRadius)
				{
					return "Lanczos1";
				}
				else
				{
					return "Lanczos2";
				}
			}

			if (filter.Mode == ImageFilteringMode.Blackman)
			{
				if (filter.Radius == Image.normRadius)
				{
					return "Blackman1";
				}
				else
				{
					return "Blackman2";
				}
			}

			return null;
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

			this.buttonRotation90.ActiveState  = p.RotationMode == Properties.Image.Rotation.Angle90  ? ActiveState.Yes : ActiveState.No;
			this.buttonRotation180.ActiveState = p.RotationMode == Properties.Image.Rotation.Angle180 ? ActiveState.Yes : ActiveState.No;
			this.buttonRotation270.ActiveState = p.RotationMode == Properties.Image.Rotation.Angle270 ? ActiveState.Yes : ActiveState.No;
			
			this.buttonMirrorH.ActiveState = p.MirrorH   ? ActiveState.Yes : ActiveState.No;
			this.buttonMirrorV.ActiveState = p.MirrorV   ? ActiveState.Yes : ActiveState.No;
			this.buttonHomo   .ActiveState = p.Homo      ? ActiveState.Yes : ActiveState.No;
			this.buttonInside .ActiveState = p.InsideDoc ? ActiveState.Yes : ActiveState.No;

			this.buttonFilter.SelectedName = Image.FilterToName(p.ImageFilter);

			this.cropper.Crop = p.CropMargins;

			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Image p = this.property as Properties.Image;
			if ( p == null )  return;

			p.Filename  = TextLayout.ConvertToSimpleText(this.fieldFilename.Text);

			     if (this.buttonRotation90.ActiveState  == ActiveState.Yes)  p.RotationMode = Properties.Image.Rotation.Angle90;
			else if (this.buttonRotation180.ActiveState == ActiveState.Yes)  p.RotationMode = Properties.Image.Rotation.Angle180;
			else if (this.buttonRotation270.ActiveState == ActiveState.Yes)  p.RotationMode = Properties.Image.Rotation.Angle270;
			else                                                             p.RotationMode = Properties.Image.Rotation.Angle0;

			p.MirrorH   = ( this.buttonMirrorH.ActiveState == ActiveState.Yes );
			p.MirrorV   = ( this.buttonMirrorV.ActiveState == ActiveState.Yes );
			p.Homo      = ( this.buttonHomo   .ActiveState == ActiveState.Yes );
			p.InsideDoc = ( this.buttonInside .ActiveState == ActiveState.Yes );

			p.ImageFilter = Image.NameToFilter(this.buttonFilter.SelectedName);

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

			r.Offset(0, -30);
			Rectangle rr = r;
			rr.Height = 22;
			rr.Width = 22;
			this.buttonRotation90.SetManualBounds(rr);
			rr.Offset(22, 0);
			this.buttonRotation180.SetManualBounds(rr);
			rr.Offset(22, 0);
			this.buttonRotation270.SetManualBounds(rr);
			rr.Offset(22+4, 0);
			this.buttonMirrorH.SetManualBounds(rr);
			rr.Offset(22, 0);
			this.buttonMirrorV.SetManualBounds(rr);
			rr.Offset(22+4, 0);
			rr.Width += 12;
			this.buttonFilter.SetManualBounds(rr);
			rr.Offset(22+12+4, 0);
			rr.Width -= 12;
			this.buttonHomo.SetManualBounds(rr);

			r.Offset(0, -110);
			rr = r;
			rr.Top = rr.Bottom+105;
			rr.Left = rect.Left;
			rr.Right = rect.Right;
			this.cropper.SetManualBounds(rr);

			r.Offset(0, -25);
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.buttonInside.SetManualBounds(r);

			this.buttonSave.Visibility = (this.isExtendedSize);
			this.cropper.Visibility = (this.isExtendedSize);
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

		private void HandleButtonPressed(object sender, MessageEventArgs e)
		{
			//	Un bouton a été pressé.
			IconButton button = sender as IconButton;

			if (button == this.buttonRotation90)
			{
				this.buttonRotation180.ActiveState = ActiveState.No;
				this.buttonRotation270.ActiveState = ActiveState.No;
			}

			if (button == this.buttonRotation180)
			{
				this.buttonRotation90.ActiveState = ActiveState.No;
				this.buttonRotation270.ActiveState = ActiveState.No;
			}

			if (button == this.buttonRotation270)
			{
				this.buttonRotation90.ActiveState = ActiveState.No;
				this.buttonRotation180.ActiveState = ActiveState.No;
			}

			button.ActiveState = (button.ActiveState == ActiveState.Yes) ? ActiveState.No : ActiveState.Yes;

			this.OnChanged();
			this.document.Notifier.NotifyGeometryChanged();
		}

		private void HandleFilterComboClosed(object sender)
		{
			//	Le filtre a été changé.
			this.OnChanged();
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


		protected static readonly double normRadius = 1.0;
		protected static readonly double softRadius = 2.0;

		protected TextField					fieldFilename;
		protected Button					buttonOpen;
		protected Button					buttonUpdate;
		protected Button					buttonSave;
		protected IconButton				buttonRotation90;
		protected IconButton				buttonRotation180;
		protected IconButton				buttonRotation270;
		protected IconButton				buttonMirrorH;
		protected IconButton				buttonMirrorV;
		protected IconButtonCombo			buttonFilter;
		protected IconButton				buttonHomo;
		protected Widgets.Cropper			cropper;
		protected CheckButton				buttonInside;

		protected Margins					initialCrop;
	}
}

