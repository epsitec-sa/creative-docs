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
			this.fieldFilename.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldFilename, Res.Strings.Panel.Image.Tooltip.Filename);

			this.fieldClipboard = new TextField(this);
			this.fieldClipboard.Text = Res.Strings.Panel.Image.FromClipboard;
			this.fieldClipboard.IsReadOnly = true;

			this.buttonOpen = new Button(this);
			this.buttonOpen.Text = Res.Strings.Panel.Image.Button.Open;
			this.buttonOpen.Clicked += new MessageEventHandler(this.HandleOpenClicked);
			this.buttonOpen.TabIndex = tabIndex++;
			this.buttonOpen.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonOpen, Res.Strings.Panel.Image.Tooltip.Open);

			this.buttonUpdate = new Button(this);
			this.buttonUpdate.Text = Res.Strings.Panel.Image.Button.Update;
			this.buttonUpdate.Clicked += new MessageEventHandler(this.HandleUpdateClicked);
			this.buttonUpdate.TabIndex = tabIndex++;
			this.buttonUpdate.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonUpdate, Res.Strings.Panel.Image.Tooltip.Update);

			this.buttonSave = new Button(this);
			this.buttonSave.Text = Res.Strings.Panel.Image.Button.Save;
			this.buttonSave.Clicked += new MessageEventHandler(this.HandleSaveClicked);
			this.buttonSave.TabIndex = tabIndex++;
			this.buttonSave.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonSave, Res.Strings.Panel.Image.Tooltip.Save);

			this.buttonRotation90 = new IconButton(this);
			this.buttonRotation90.IconName = Misc.Icon("ImageRotation90");
			this.buttonRotation90.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonRotation90.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.buttonRotation90.TabIndex = tabIndex++;
			this.buttonRotation90.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonRotation90, Res.Strings.Panel.Image.Button.Rotation90);

			this.buttonRotation180 = new IconButton(this);
			this.buttonRotation180.IconName = Misc.Icon("ImageRotation180");
			this.buttonRotation180.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonRotation180.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.buttonRotation180.TabIndex = tabIndex++;
			this.buttonRotation180.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonRotation180, Res.Strings.Panel.Image.Button.Rotation180);

			this.buttonRotation270 = new IconButton(this);
			this.buttonRotation270.IconName = Misc.Icon("ImageRotation270");
			this.buttonRotation270.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonRotation270.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.buttonRotation270.TabIndex = tabIndex++;
			this.buttonRotation270.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonRotation270, Res.Strings.Panel.Image.Button.Rotation270);

			this.buttonMirrorH = new IconButton(this);
			this.buttonMirrorH.IconName = Misc.Icon("ImageMirrorH");
			this.buttonMirrorH.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonMirrorH.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.buttonMirrorH.TabIndex = tabIndex++;
			this.buttonMirrorH.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonMirrorH, Res.Strings.Panel.Image.Button.MirrorX);

			this.buttonMirrorV = new IconButton(this);
			this.buttonMirrorV.IconName = Misc.Icon("ImageMirrorV");
			this.buttonMirrorV.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonMirrorV.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.buttonMirrorV.TabIndex = tabIndex++;
			this.buttonMirrorV.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonMirrorV, Res.Strings.Panel.Image.Button.MirrorY);

			this.buttonFilter = new IconButtonCombo(this);
			this.buttonFilter.AutoFocus = false;
			this.buttonFilter.IsLiveUpdateEnabled = false;
			//?this.buttonFilter.IconName = Misc.Icon("ImageFilter");
			this.buttonFilter.ComboClosed += new EventHandler(this.HandleFilterComboClosed);
			this.buttonFilter.TabIndex = tabIndex++;
			this.buttonFilter.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.AddFilterCombo("ImageFilter0", Res.Strings.Panel.Image.Combo.Filter.None);
			this.AddFilterCombo("ImageFilter1", Res.Strings.Panel.Image.Combo.Filter.FilterA);
			this.AddFilterCombo("ImageFilter2", Res.Strings.Panel.Image.Combo.Filter.FilterB);
			ToolTip.Default.SetToolTip(this.buttonFilter, Res.Strings.Panel.Image.Button.Filter);

			this.buttonHomo = new IconButton(this);
			this.buttonHomo.IconName = Misc.Icon("ImageHomo");
			this.buttonHomo.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonHomo.Pressed += new MessageEventHandler(this.HandleButtonPressed);
			this.buttonHomo.TabIndex = tabIndex++;
			this.buttonHomo.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonHomo, Res.Strings.Panel.Image.Button.Homo);

			this.cropper = new Widgets.Cropper(this);
			this.cropper.Document = this.document;
			this.cropper.CropChanged += new EventHandler(this.HandleCropChanged);

			this.buttonInside = new CheckButton(this);
			this.buttonInside.Text = Res.Strings.Panel.Image.Button.Inside;
			this.buttonInside.ActiveStateChanged += new EventHandler(this.HandleButtonActiveStateChanged);
			this.buttonInside.TabIndex = tabIndex++;
			this.buttonInside.TabNavigationMode = TabNavigationMode.ActivateOnTab;

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
				this.fieldClipboard = null;
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
			//	Met � jour apr�s un changement de g�om�trie.
			this.cropper.UpdateField();
		}


		protected void AddFilterCombo(string icon, string text)
		{
			//	Ajoute une ligne au menu du filtre.
			string regularText  = string.Format("{0} {1}", Misc.Image(icon, -5), text);
			string selectedText = string.Format("{0} {1}", Misc.Image(icon, -5), Misc.Bold(text));
			string briefIcon    = Misc.Icon(icon);
			IconButtonCombo.Item item = new IconButtonCombo.Item("", briefIcon, regularText, selectedText);
			this.buttonFilter.Items.Add(item);
		}


		protected override void PropertyToWidgets()
		{
			//	Propri�t� -> widgets.
			base.PropertyToWidgets();

			Properties.Image p = this.property as Properties.Image;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.fieldFilename.Text = TextLayout.ConvertToTaggedText(p.FileName);
			this.fieldFilename.Cursor = p.FileName.Length;

			this.buttonRotation90.ActiveState  = p.RotationMode == Properties.Image.Rotation.Angle90  ? ActiveState.Yes : ActiveState.No;
			this.buttonRotation180.ActiveState = p.RotationMode == Properties.Image.Rotation.Angle180 ? ActiveState.Yes : ActiveState.No;
			this.buttonRotation270.ActiveState = p.RotationMode == Properties.Image.Rotation.Angle270 ? ActiveState.Yes : ActiveState.No;
			
			this.buttonMirrorH.ActiveState = p.MirrorH   ? ActiveState.Yes : ActiveState.No;
			this.buttonMirrorV.ActiveState = p.MirrorV   ? ActiveState.Yes : ActiveState.No;
			this.buttonHomo   .ActiveState = p.Homo      ? ActiveState.Yes : ActiveState.No;
			this.buttonInside .ActiveState = p.InsideDoc ? ActiveState.Yes : ActiveState.No;

			this.buttonFilter.SelectedIndex = p.FilterCategory+1;  // -1=aucun, 0=A, 1=B

			this.cropper.Crop = p.CropMargins;

			this.UpdateWidgets();

			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propri�t�.
			Properties.Image p = this.property as Properties.Image;
			if ( p == null )  return;

			p.FileName  = TextLayout.ConvertToSimpleText(this.fieldFilename.Text);

			     if (this.buttonRotation90.ActiveState  == ActiveState.Yes)  p.RotationMode = Properties.Image.Rotation.Angle90;
			else if (this.buttonRotation180.ActiveState == ActiveState.Yes)  p.RotationMode = Properties.Image.Rotation.Angle180;
			else if (this.buttonRotation270.ActiveState == ActiveState.Yes)  p.RotationMode = Properties.Image.Rotation.Angle270;
			else                                                             p.RotationMode = Properties.Image.Rotation.Angle0;

			p.MirrorH   = ( this.buttonMirrorH.ActiveState == ActiveState.Yes );
			p.MirrorV   = ( this.buttonMirrorV.ActiveState == ActiveState.Yes );
			p.Homo      = ( this.buttonHomo   .ActiveState == ActiveState.Yes );
			p.InsideDoc = ( this.buttonInside .ActiveState == ActiveState.Yes );

			p.FilterCategory = this.buttonFilter.SelectedIndex-1;  // -1=aucun, 0=A, 1=B

			p.CropMargins = this.cropper.Crop;
		}

		protected void UpdateWidgets()
		{
			//	Met � jour les widgets du panneau.
			Properties.Image p = this.property as Properties.Image;

			if (p != null && p.PastedImage == null)  // image provenant d'un fichier ?
			{
				this.fieldFilename.Visibility = true;
				this.fieldClipboard.Visibility = false;
				this.buttonUpdate.Enable = true;
				this.buttonInside.Enable = true;
			}
			else  // image provenant du clipboard ?
			{
				this.fieldFilename.Visibility = false;
				this.fieldClipboard.Visibility = true;
				this.buttonUpdate.Enable = false;
				this.buttonInside.Enable = false;
			}
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.fieldFilename == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			this.fieldFilename.SetManualBounds(r);
			this.fieldClipboard.SetManualBounds(r);

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
			//	Le nom de l'image a �t� chang�.
			if (this.ignoreChanged)
			{
				return;
			}

			this.OnChanged();

			Properties.Image p = this.property as Properties.Image;
			p.FileDate = this.document.ImageCache.LoadFromFile(p.FileName);

			this.UpdateWidgets();
		}

		private void HandleButtonPressed(object sender, MessageEventArgs e)
		{
			//	Un bouton a �t� press�.
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
			//	Le filtre a �t� chang�.
			this.OnChanged();
		}

		private void HandleButtonActiveStateChanged(object sender)
		{
			//	Une valeur a �t� chang�e.
			this.OnChanged();
		}

		private void HandleOpenClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'Importer' pour choisir l'image a �t� cliqu�.
#if false
			FileOpen dialog = new FileOpen();
			dialog.Title = Res.Strings.Panel.Image.Dialog.Open.Title;
			dialog.FileName = this.fieldFilename.Text;
			dialog.Filters.Add("all", TextLayout.ConvertToSimpleText(Res.Strings.File.Bitmap.All), "*.bmp; *.tif; *.tiff; *.jpg; *.jpeg; *.gif; *.png; *.wmf; *.emf");
			dialog.Filters.Add("bmp", TextLayout.ConvertToSimpleText(Res.Strings.File.Bitmap.BMP), "*.bmp");
			dialog.Filters.Add("tif", TextLayout.ConvertToSimpleText(Res.Strings.File.Bitmap.TIF), "*.tif; *.tiff");
			dialog.Filters.Add("jpg", TextLayout.ConvertToSimpleText(Res.Strings.File.Bitmap.JPG), "*.jpg; *.jpeg");
			dialog.Filters.Add("gif", TextLayout.ConvertToSimpleText(Res.Strings.File.Bitmap.GIF), "*.gif");
			dialog.Filters.Add("png", TextLayout.ConvertToSimpleText(Res.Strings.File.Bitmap.PNG), "*.png");
			dialog.Filters.Add("wmf", TextLayout.ConvertToSimpleText(Res.Strings.File.Vector.WMF), "*.wmf; *.emf");
			dialog.OpenDialog();  // demande le nom du fichier...

			this.fieldFilename.Text = TextLayout.ConvertToTaggedText(dialog.FileName);
			this.fieldFilename.Cursor = this.fieldFilename.Text.Length;
#else
			Button button = sender as Button;
			Dialogs.FileOpenImage dlg = new Dialogs.FileOpenImage(this.document, button.Window);

			if (string.IsNullOrEmpty(this.fieldFilename.Text))
			{
				dlg.InitialDirectory = this.document.GlobalSettings.InitialDirectory;
				dlg.InitialFileName = "";
			}
			else
			{
				dlg.InitialDirectory = System.IO.Path.GetDirectoryName(this.fieldFilename.Text);
				dlg.InitialFileName = System.IO.Path.GetFileName(this.fieldFilename.Text);
			}

			dlg.ShowDialog();  // choix d'un fichier image...

			if (dlg.Result == Common.Dialogs.DialogResult.Accept)
			{
				this.fieldFilename.Text = dlg.FileName;
				this.fieldFilename.Cursor = this.fieldFilename.Text.Length;
				this.UpdateWidgets();
			}
#endif
		}

		private void HandleUpdateClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton 'M�j' pour relire l'image a �t� cliqu�.
			Properties.Image p = this.property as Properties.Image;
			ImageCache.Item item = this.document.ImageCache.Find(p.FileName, p.FileDate);

			if (item != null)
			{
				if (item.ReloadImage())  // relit l'image sur disque
				{
					p.FileDate = this.document.ImageCache.LoadFromFile (p.FileName);
					return;  // tout c'est bien pass�
				}
			}

			//	Indique que la mise � jour n'est pas possible.
			this.document.Modifier.ActiveViewer.DialogError(Res.Strings.Error.FileDoesNoetExist);
		}

		private void HandleSaveClicked(object sender, MessageEventArgs e)
		{
#if false
			//	Le bouton 'Exporter' pour choisir l'image a �t� cliqu�.
			Properties.Image p = this.property as Properties.Image;
			ImageCache.Item item = this.document.ImageCache.Find(p.FileName, p.FileDate);
			if (item == null)
			{
				return;
			}

			string ext = System.IO.Path.GetExtension(this.fieldFilename.Text);
			if (ext.StartsWith("."))
			{
				ext = ext.Substring(1);  // enl�ve le point
			}

			FileSave dialog = new FileSave();
			dialog.Title = Res.Strings.Panel.Image.Dialog.Save.Title;
			dialog.FileName = this.fieldFilename.Text;
			dialog.PromptForOverwriting = true;
			dialog.Filters.Add(ext, "Image", "*."+ext);
			dialog.OpenDialog();  // demande le nom du fichier...
			if (dialog.Result != Common.Dialogs.DialogResult.Accept)
			{
				return;
			}

			item.ExportImage(dialog.FileName);  // �crit le fichier sur disque
#else
			Button button = sender as Button;
			Dialogs.FileSaveImage dlg = new Dialogs.FileSaveImage(this.document, button.Window);

			Properties.Image p = this.property as Properties.Image;

			if (p.PastedImage == null)  // image import�e normalement ?
			{
				dlg.FileExtension = System.IO.Path.GetExtension(this.fieldFilename.Text);
				dlg.InitialDirectory = System.IO.Path.GetDirectoryName(this.fieldFilename.Text);
				dlg.InitialFileName = System.IO.Path.GetFileName(this.fieldFilename.Text);
			}
			else  // donn�e d'une image coll�e directement ?
			{
				if (!string.IsNullOrEmpty(this.document.Filename))
				{
					dlg.InitialDirectory = System.IO.Path.GetDirectoryName(this.document.Filename);
				}
				if (string.IsNullOrEmpty(dlg.InitialDirectory))
				{
					dlg.InitialDirectory = this.document.GlobalSettings.InitialDirectory;
				}
				dlg.InitialFileName = "";
				dlg.FileExtension = ".png";
			}

			dlg.ShowDialog();  // choix d'un fichier image...

			if (dlg.Result == Common.Dialogs.DialogResult.Accept)
			{
				ImageCache.Item item = this.document.ImageCache.Find(p.FileName, p.FileDate);
				if (item == null)
				{
					if (p.PastedImage != null)
					{
						byte[] data = p.PastedImage.BitmapImage.Save(ImageFormat.Png);
						System.IO.File.WriteAllBytes(dlg.FileName, data);  // �crit le fichier sur disque
					}
				}
				else
				{
					item.ExportImage(dlg.FileName);  // �crit le fichier sur disque
				}
			}
#endif
		}

		void HandleCropChanged(object sender)
		{
			this.OnChanged();
		}


		protected TextField					fieldFilename;
		protected TextField					fieldClipboard;
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
