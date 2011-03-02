//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.Designer.Dialogs
{
	public class FileSaveImageDialog : AbstractFileDialog
	{
		public FileSaveImageDialog(DesignerApplication designerApplication)
		{
			this.designerApplication = designerApplication;

			this.InitialDirectory = FileSaveImageDialog.initialDirectory;
			this.InitialFileName  = FileSaveImageDialog.initialFilename;
			this.FileFilterPattern = "*.png|*.tif|*.bmp|*.jpg";
			this.enableNavigation = true;
			this.enableMultipleSelection = false;
		}


		public Size ImageSize
		{
			get;
			set;
		}

		public double Zoom
		{
			get
			{
				return FileSaveImageDialog.zoom;
			}
			set
			{
				if (FileSaveImageDialog.zoom != value)
				{
					FileSaveImageDialog.zoom = value;
					this.UpdateZoom ();
				}
			}
		}

		public bool GenerateCartridge
		{
			get
			{
				return FileSaveImageDialog.generateCartridge;
			}
			set
			{
				if (FileSaveImageDialog.generateCartridge != value)
				{
					FileSaveImageDialog.generateCartridge = value;
					this.UpdateZoom ();
				}
			}
		}

		public void PathMemorize()
		{
			FileSaveImageDialog.initialDirectory = System.IO.Path.GetDirectoryName (this.FileName);
			FileSaveImageDialog.initialFilename  = System.IO.Path.GetFileName (this.FileName);
		}


		protected override void CreateWindow()
		{
			this.CreateUserInterface ("FileSaveImage", new Size (720, 480), Res.Strings.Entities.Action.SaveBitmap, 20, this.designerApplication.Window);
		}

		protected override FileDialogType FileDialogType
		{
			get
			{
				return Epsitec.Common.Dialogs.FileDialogType.Save;
			}
		}

		protected override Rectangle GetOwnerBounds()
		{
			//	Donne les frontières de l'application.
			var w = this.designerApplication.Window;

			return new Rectangle (w.WindowLocation, w.WindowSize);
		}

		public override void PersistWindowBounds()
		{
			//	Sauve la fenêtre.
		}

		protected override void CreateFileExtensionDescriptions(Epsitec.Common.Dialogs.IFileExtensionDescription settings)
		{
			settings.Add (".png", "Image PNG");
			settings.Add (".tif", "Image TIFF");
			settings.Add (".bmp", "Image BMP");
			settings.Add (".jpg", "Image JPEG");
		}

		protected override void FavoritesAddApplicationFolders()
		{
		}

		protected override IFavoritesSettings FavoritesSettings
		{
			get
			{
				return FileSaveImageDialog.settings;
			}
		}

		protected override void CreateOptionsUserInterface()
		{
			//	Crée le panneau facultatif pour les options d'enregistrement.
			this.optionsContainer = new Widget (this.window.Root);
			this.optionsContainer.Margins = new Margins (0, 0, 8, 0);
			this.optionsContainer.Dock = DockStyle.Bottom;
			this.optionsContainer.TabNavigationMode = TabNavigationMode.None;
			this.optionsContainer.Visibility = FileSaveImageDialog.showOptions;
			this.optionsContainer.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.optionsContainer.Name = "OptionsContainer";

			//	Options pour le zoom.
			var groupZoom = new GroupBox (this.optionsContainer);
			groupZoom.Text = "Zoom de l'image à générer";
			groupZoom.PreferredWidth = 200;
			groupZoom.Padding = new Margins (10, 0, 0, 3);
			groupZoom.Dock = DockStyle.StackEnd;
			groupZoom.Margins = new Margins (0, 182, 0, 0);
			groupZoom.Name = "ZoomOptions";

			this.optionsZoom1 = new RadioButton (groupZoom);
			this.optionsZoom1.Text = this.GetZoomDescription (1);
			this.optionsZoom1.Dock = DockStyle.Top;
			this.optionsZoom1.Clicked += this.HandleOptionsZoomClicked;

			this.optionsZoom2 = new RadioButton (groupZoom);
			this.optionsZoom2.Text = this.GetZoomDescription (2);
			this.optionsZoom2.Dock = DockStyle.Top;
			this.optionsZoom2.Clicked += this.HandleOptionsZoomClicked;

			this.optionsZoom3 = new RadioButton (groupZoom);
			this.optionsZoom3.Text = this.GetZoomDescription (3);
			this.optionsZoom3.Dock = DockStyle.Top;
			this.optionsZoom3.Clicked += this.HandleOptionsZoomClicked;

			this.optionsZoom4 = new RadioButton (groupZoom);
			this.optionsZoom4.Text = this.GetZoomDescription (4);
			this.optionsZoom4.Dock = DockStyle.Top;
			this.optionsZoom4.Clicked += this.HandleOptionsZoomClicked;

			var groupFlags = new GroupBox (this.optionsContainer);
			groupFlags.Text = "Options pour l'image à générer";
			groupFlags.PreferredWidth = 210;
			groupFlags.Padding = new Margins (10, 0, 0, 3);
			groupFlags.Dock = DockStyle.StackEnd;
			groupFlags.Margins = new Margins (0, 10, 0, 0);
			groupFlags.Name = "ZoomOptions";

			this.cartridgeButton = new CheckButton (groupFlags);
			this.cartridgeButton.Text = "Met une légende en bas à gauche";
			this.cartridgeButton.AutoToggle = false;
			this.cartridgeButton.Dock = DockStyle.Top;
			this.cartridgeButton.Clicked += this.HandleOptionsZoomClicked;
		}

		private string GetZoomDescription(double zoom)
		{
			string z = System.Math.Floor (zoom*100).ToString ();
			string x = System.Math.Floor (this.ImageSize.Width*zoom).ToString ();
			string y = System.Math.Floor (this.ImageSize.Height*zoom).ToString ();

			return string.Format ("{0}% ({1} × {2} pixels)", z, x, y);
		}

		protected override void CreateFooterOptions(Widget footer)
		{
			this.optionsExtend = new GlyphButton (footer);
			this.optionsExtend.PreferredWidth = 16;
			this.optionsExtend.ButtonStyle = ButtonStyle.Slider;
			this.optionsExtend.AutoFocus = false;
			this.optionsExtend.TabNavigationMode = TabNavigationMode.None;
			this.optionsExtend.Dock = DockStyle.Left;
			this.optionsExtend.Margins = new Margins (0, 8, 3, 3);
			this.optionsExtend.Clicked += this.HandleOptionsExtendClicked;
			ToolTip.Default.SetToolTip (this.optionsExtend, "Montre ou cache les options");
		}

		protected override void UpdateOptions()
		{
			base.UpdateOptions ();
			
			if (this.optionsExtend != null)
			{
				this.optionsExtend.GlyphShape = this.optionsContainer.Visibility ? GlyphShape.ArrowDown : GlyphShape.ArrowUp;
			}

			this.UpdateZoom ();
		}

		protected void UpdateZoom()
		{
			//	Met à jour le mode d'inclusion des polices.
			if (this.optionsZoom1 != null)
			{
				this.optionsZoom1.ActiveState = (FileSaveImageDialog.zoom == 1) ? ActiveState.Yes : ActiveState.No;
				this.optionsZoom2.ActiveState = (FileSaveImageDialog.zoom == 2) ? ActiveState.Yes : ActiveState.No;
				this.optionsZoom3.ActiveState = (FileSaveImageDialog.zoom == 3) ? ActiveState.Yes : ActiveState.No;
				this.optionsZoom4.ActiveState = (FileSaveImageDialog.zoom == 4) ? ActiveState.Yes : ActiveState.No;

				this.cartridgeButton.ActiveState = (FileSaveImageDialog.generateCartridge) ? ActiveState.Yes : ActiveState.No;
			}
		}

		private void HandleOptionsExtendClicked(object sender, MessageEventArgs e)
		{
			this.optionsContainer.Visibility = !this.optionsContainer.Visibility;
			FileSaveImageDialog.showOptions = this.optionsContainer.Visibility;

			this.UpdateOptions();
		}

		private void HandleOptionsZoomClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton radio pour le mode d'inclusion des polices a été cliqué.
			if (sender == this.optionsZoom1)
			{
				this.Zoom = 1;
			}

			if (sender == this.optionsZoom2)
			{
				this.Zoom = 2;
			}

			if (sender == this.optionsZoom3)
			{
				this.Zoom = 3;
			}

			if (sender == this.optionsZoom4)
			{
				this.Zoom = 4;
			}

			if (sender == this.cartridgeButton)
			{
				this.GenerateCartridge = !this.GenerateCartridge;
			}
		}



		public class Settings : IFavoritesSettings
		{
			public Settings()
			{
				this.favoritesBig = true;
				this.favoritesList = new List<string> ();
			}


			#region IFavoritesSettings Members

			public bool UseLargeIcons
			{
				get
				{
					return this.favoritesBig;
				}
				set
				{
					this.favoritesBig = value;
				}
			}

			public IList<string> Items
			{
				get
				{
					return this.favoritesList;
				}
			}

			#endregion
			
			
			protected bool							favoritesBig;
			protected List<string>					favoritesList;
		}


		//	Tous les réglages sont conservés dans des variables statiques,
		//	ce qui est une solution rapide et provisoire !
		private static Settings settings = new Settings ();
		private static string initialDirectory = null;
		private static string initialFilename = null;
		private static double zoom = 1;
		private static bool generateCartridge = true;
		private static bool showOptions = true;

		private readonly DesignerApplication designerApplication;

		private GlyphButton optionsExtend;
		private Widget optionsContainer;
		private RadioButton optionsZoom1;
		private RadioButton optionsZoom2;
		private RadioButton optionsZoom3;
		private RadioButton optionsZoom4;
		private CheckButton cartridgeButton;
	}
}
