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

			this.fieldCropLeft = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldCropLeft.LabelShortText = Res.Strings.Panel.Image.Short.CropLeft;
			this.fieldCropLeft.LabelLongText  = Res.Strings.Panel.Image.Long.CropLeft;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldCropLeft.TextFieldReal);
			this.fieldCropLeft.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldCropLeft.TabIndex = tabIndex++;
			this.fieldCropLeft.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropLeft, Res.Strings.Panel.Image.Tooltip.CropLeft);

			this.fieldCropRight = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldCropRight.LabelShortText = Res.Strings.Panel.Image.Short.CropRight;
			this.fieldCropRight.LabelLongText  = Res.Strings.Panel.Image.Long.CropRight;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldCropRight.TextFieldReal);
			this.fieldCropRight.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldCropRight.TabIndex = tabIndex++;
			this.fieldCropRight.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropRight, Res.Strings.Panel.Image.Tooltip.CropRight);

			this.fieldCropBottom = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldCropBottom.LabelShortText = Res.Strings.Panel.Image.Short.CropBottom;
			this.fieldCropBottom.LabelLongText  = Res.Strings.Panel.Image.Long.CropBottom;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldCropBottom.TextFieldReal);
			this.fieldCropBottom.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldCropBottom.TabIndex = tabIndex++;
			this.fieldCropBottom.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropBottom, Res.Strings.Panel.Image.Tooltip.CropBottom);

			this.fieldCropTop = new Widgets.TextFieldLabel(this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldCropTop.LabelShortText = Res.Strings.Panel.Image.Short.CropTop;
			this.fieldCropTop.LabelLongText  = Res.Strings.Panel.Image.Long.CropTop;
			this.document.Modifier.AdaptTextFieldRealScalar(this.fieldCropTop.TextFieldReal);
			this.fieldCropTop.TextFieldReal.EditionAccepted += new EventHandler(this.HandleFieldChanged);
			this.fieldCropTop.TabIndex = tabIndex++;
			this.fieldCropTop.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.fieldCropTop, Res.Strings.Panel.Image.Tooltip.CropTop);

			this.offset = new Offset(this);
			this.offset.OffsetValue = Point.Zero;
			this.offset.OffsetValueChanged += new EventHandler(this.HandleOffsetValueChanged);

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
				this.fieldCropLeft.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldCropRight.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldCropBottom.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.fieldCropTop.TextFieldReal.EditionAccepted -= new EventHandler(this.HandleFieldChanged);
				this.offset.OffsetValueChanged -= new EventHandler(this.HandleOffsetValueChanged);
				this.buttonMirrorH.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonMirrorV.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonHomo.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonFilter.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);
				this.buttonInside.ActiveStateChanged -= new EventHandler(this.HandleButtonActiveStateChanged);

				this.fieldFilename = null;
				this.buttonOpen = null;
				this.buttonUpdate = null;
				this.buttonSave = null;
				this.fieldCropLeft = null;
				this.fieldCropRight = null;
				this.fieldCropBottom = null;
				this.fieldCropTop = null;
				this.offset = null;
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
				return ( this.isExtendedSize ? this.LabelHeight+215 : this.LabelHeight+55 );
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
			this.buttonMirrorH.ActiveState = p.MirrorH   ? ActiveState.Yes : ActiveState.No;
			this.buttonMirrorV.ActiveState = p.MirrorV   ? ActiveState.Yes : ActiveState.No;
			this.buttonHomo   .ActiveState = p.Homo      ? ActiveState.Yes : ActiveState.No;
			this.buttonFilter .ActiveState = p.Filter    ? ActiveState.Yes : ActiveState.No;
			this.buttonInside .ActiveState = p.InsideDoc ? ActiveState.Yes : ActiveState.No;

			Margins crop = p.CropMargins;
			this.fieldCropLeft.TextFieldReal.InternalValue = (decimal) crop.Left;
			this.fieldCropRight.TextFieldReal.InternalValue = (decimal) crop.Right;
			this.fieldCropBottom.TextFieldReal.InternalValue = (decimal) crop.Bottom;
			this.fieldCropTop.TextFieldReal.InternalValue = (decimal) crop.Top;

			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			Properties.Image p = this.property as Properties.Image;
			if ( p == null )  return;

			p.Filename  = TextLayout.ConvertToSimpleText(this.fieldFilename.Text);
			p.MirrorH   = ( this.buttonMirrorH.ActiveState == ActiveState.Yes );
			p.MirrorV   = ( this.buttonMirrorV.ActiveState == ActiveState.Yes );
			p.Homo      = ( this.buttonHomo   .ActiveState == ActiveState.Yes );
			p.Filter    = ( this.buttonFilter .ActiveState == ActiveState.Yes );
			p.InsideDoc = ( this.buttonInside .ActiveState == ActiveState.Yes );

			Margins crop = Margins.Zero;
			crop.Left = (double) this.fieldCropLeft.TextFieldReal.InternalValue;
			crop.Right = (double) this.fieldCropRight.TextFieldReal.InternalValue;
			crop.Bottom = (double) this.fieldCropBottom.TextFieldReal.InternalValue;
			crop.Top = (double) this.fieldCropTop.TextFieldReal.InternalValue;
			p.CropMargins = crop;
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

			r.Offset(0, -25);
			r.Left = rect.Left;
			r.Right = rect.Left+62;
			this.fieldCropLeft.SetManualBounds(r);
			r.Offset(62+5, 0);
			this.fieldCropRight.SetManualBounds(r);

			r.Offset(0, -25);
			r.Left = rect.Left;
			r.Right = rect.Left+62;
			this.fieldCropBottom.SetManualBounds(r);
			r.Offset(62+5, 0);
			this.fieldCropTop.SetManualBounds(r);

			Rectangle rr = r;
			rr.Top = rr.Bottom+45;
			rr.Left = rect.Right-45;
			rr.Right = rect.Right;
			this.offset.SetManualBounds(rr);

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

			r.Offset(0, -20);
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.buttonInside.SetManualBounds(r);

			this.buttonSave.Visibility = (this.isExtendedSize);
			this.fieldCropLeft.Visibility = (this.isExtendedSize);
			this.fieldCropRight.Visibility = (this.isExtendedSize);
			this.fieldCropBottom.Visibility = (this.isExtendedSize);
			this.fieldCropTop.Visibility = (this.isExtendedSize);
			this.offset.Visibility = (this.isExtendedSize);
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

		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if (this.ignoreChanged)
			{
				return;
			}

			this.OnChanged();
		}

		void HandleOffsetValueChanged(object sender)
		{
			if (this.ignoreChanged)
			{
				return;
			}

			Point move = this.offset.OffsetValue;

			this.ignoreChanged = true;
			this.offset.OffsetValue = Point.Zero;
			this.ignoreChanged = false;

			Margins crop = Margins.Zero;
			crop.Left = (double) this.fieldCropLeft.TextFieldReal.InternalValue;
			crop.Right = (double) this.fieldCropRight.TextFieldReal.InternalValue;
			crop.Bottom = (double) this.fieldCropBottom.TextFieldReal.InternalValue;
			crop.Top = (double) this.fieldCropTop.TextFieldReal.InternalValue;

			if (move.X > 0)
			{
				double m = System.Math.Min(move.X, crop.Right);
				crop.Right -= m;
				crop.Left  += m;
			}

			if (move.X < 0)
			{
				double m = System.Math.Min(-move.X, crop.Left);
				crop.Left  -= m;
				crop.Right += m;
			}

			if (move.Y > 0)
			{
				double m = System.Math.Min(move.Y, crop.Top);
				crop.Top    -= m;
				crop.Bottom += m;
			}

			if (move.Y < 0)
			{
				double m = System.Math.Min(-move.Y, crop.Bottom);
				crop.Bottom -= m;
				crop.Top    += m;
			}

			this.fieldCropLeft.TextFieldReal.InternalValue = (decimal) crop.Left;
			this.fieldCropRight.TextFieldReal.InternalValue = (decimal) crop.Right;
			this.fieldCropBottom.TextFieldReal.InternalValue = (decimal) crop.Bottom;
			this.fieldCropTop.TextFieldReal.InternalValue = (decimal) crop.Top;
		}
		


		protected TextField					fieldFilename;
		protected Button					buttonOpen;
		protected Button					buttonUpdate;
		protected Button					buttonSave;
		protected Widgets.TextFieldLabel	fieldCropLeft;
		protected Widgets.TextFieldLabel	fieldCropRight;
		protected Widgets.TextFieldLabel	fieldCropBottom;
		protected Widgets.TextFieldLabel	fieldCropTop;
		protected Offset					offset;
		protected CheckButton				buttonMirrorH;
		protected CheckButton				buttonMirrorV;
		protected CheckButton				buttonHomo;
		protected CheckButton				buttonFilter;
		protected CheckButton				buttonInside;
	}
}

