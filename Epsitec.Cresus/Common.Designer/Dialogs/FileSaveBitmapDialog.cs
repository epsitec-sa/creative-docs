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
	public class FileSaveBitmapDialog : AbstractFileDialog
	{
		public FileSaveBitmapDialog(DesignerApplication designerApplication)
		{
			this.designerApplication     = designerApplication;
			this.title                   = Res.Strings.Entities.Action.SaveBitmap;
			this.owner                   = this.designerApplication.Window;
			this.InitialDirectory        = FileSaveBitmapDialog.initialDirectory;
			this.InitialFileName         = FileSaveBitmapDialog.initialFilename;
			this.enableNavigation        = true;
			this.enableMultipleSelection = false;
			this.hasOptions              = true;
			this.fileDialogType          = FileDialogType.Save;

			this.Filters.Add (new FilterItem ("x", "Image", "*.png|*.tif|*.bmp|*.jpg"));
		}


		public Size BitmapSize
		{
			get;
			set;
		}

		public EntitiesEditor.BitmapParameters BitmapParameters
		{
			get
			{
				return FileSaveBitmapDialog.bitmapParameters;
			}
			set
			{
				FileSaveBitmapDialog.bitmapParameters = value;
				this.UpdateZoom ();
			}
		}

		public void PathMemorize()
		{
			FileSaveBitmapDialog.initialDirectory = System.IO.Path.GetDirectoryName (this.FileName);
			FileSaveBitmapDialog.initialFilename  = System.IO.Path.GetFileName (this.FileName);
		}


		protected override Rectangle GetOwnerBounds()
		{
			//	Donne les frontières de l'application.
			var w = this.designerApplication.Window;

			return new Rectangle (w.WindowLocation, w.WindowSize);
		}

		protected override void FavoritesAddApplicationFolders()
		{
		}

		protected override IFavoritesSettings FavoritesSettings
		{
			get
			{
				return FileSaveBitmapDialog.settings;
			}
		}

		protected override void CreateOptionsUserInterface()
		{
			//	Crée le panneau facultatif pour les options d'enregistrement.
			this.optionsContainer = new Widget (this.window.Root);
			this.optionsContainer.Margins = new Margins (0, 0, 8, 0);
			this.optionsContainer.Dock = DockStyle.Bottom;
			this.optionsContainer.TabNavigationMode = TabNavigationMode.None;
			this.optionsContainer.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.optionsContainer.Name = "OptionsContainer";

			//	Options pour le zoom (groupe de droite).
			var groupZoom = new GroupBox (this.optionsContainer);
			groupZoom.Text = "Zoom de l'image à générer";
			groupZoom.PreferredWidth = 200;
			groupZoom.Padding = new Margins (10, 0, 0, 3);
			groupZoom.Dock = DockStyle.StackEnd;
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

			//	Options pour le cartouche (groupe de gauche).
			var groupCartridge = new GroupBox (this.optionsContainer);
			groupCartridge.Text = "Options pour le cartouche en bas à gauche";
			groupCartridge.PreferredWidth = 240;
			groupCartridge.Padding = new Margins (10, 0, 0, 3);
			groupCartridge.Dock = DockStyle.StackEnd;
			groupCartridge.Margins = new Margins (0, 10, 0, 0);
			groupCartridge.Name = "CartridgeOptions";

			this.cartridgeUserButton = new CheckButton (groupCartridge);
			this.cartridgeUserButton.Text = "Met le nom de l'utilisateur";
			this.cartridgeUserButton.AutoToggle = false;
			this.cartridgeUserButton.Dock = DockStyle.Top;
			this.cartridgeUserButton.Clicked += this.HandleOptionsZoomClicked;

			this.cartridgeDateButton = new CheckButton (groupCartridge);
			this.cartridgeDateButton.Text = "Met la date";
			this.cartridgeDateButton.AutoToggle = false;
			this.cartridgeDateButton.Dock = DockStyle.Top;
			this.cartridgeDateButton.Clicked += this.HandleOptionsZoomClicked;

			this.cartridgeSamplesButton = new CheckButton (groupCartridge);
			this.cartridgeSamplesButton.Text = "Met des exemples";
			this.cartridgeSamplesButton.AutoToggle = false;
			this.cartridgeSamplesButton.Dock = DockStyle.Top;
			this.cartridgeSamplesButton.Clicked += this.HandleOptionsZoomClicked;

			this.UpdateZoom ();
		}

		private string GetZoomDescription(double zoom)
		{
			string z = System.Math.Floor (zoom*100).ToString ();
			string x = System.Math.Floor (this.BitmapSize.Width*zoom).ToString ();
			string y = System.Math.Floor (this.BitmapSize.Height*zoom).ToString ();

			return string.Format ("{0}% ({1} × {2} pixels)", z, x, y);
		}

		protected void UpdateZoom()
		{
			//	Met à jour le mode d'inclusion des polices.
			if (this.optionsZoom1 != null)
			{
				this.optionsZoom1.ActiveState = (this.BitmapParameters.Zoom == 1) ? ActiveState.Yes : ActiveState.No;
				this.optionsZoom2.ActiveState = (this.BitmapParameters.Zoom == 2) ? ActiveState.Yes : ActiveState.No;
				this.optionsZoom3.ActiveState = (this.BitmapParameters.Zoom == 3) ? ActiveState.Yes : ActiveState.No;
				this.optionsZoom4.ActiveState = (this.BitmapParameters.Zoom == 4) ? ActiveState.Yes : ActiveState.No;

				this.cartridgeUserButton.ActiveState    = (this.BitmapParameters.GenerateUserCartridge) ? ActiveState.Yes : ActiveState.No;
				this.cartridgeDateButton.ActiveState    = (this.BitmapParameters.GenerateDateCartridge) ? ActiveState.Yes : ActiveState.No;
				this.cartridgeSamplesButton.ActiveState = (this.BitmapParameters.GenerateSamplesCartridge) ? ActiveState.Yes : ActiveState.No;
			}
		}

		private void HandleOptionsZoomClicked(object sender, MessageEventArgs e)
		{
			//	Un bouton radio pour le mode d'inclusion des polices a été cliqué.
			if (sender == this.optionsZoom1)
			{
				this.BitmapParameters.Zoom = 1;
			}

			if (sender == this.optionsZoom2)
			{
				this.BitmapParameters.Zoom = 2;
			}

			if (sender == this.optionsZoom3)
			{
				this.BitmapParameters.Zoom = 3;
			}

			if (sender == this.optionsZoom4)
			{
				this.BitmapParameters.Zoom = 4;
			}

			if (sender == this.cartridgeUserButton)
			{
				this.BitmapParameters.GenerateUserCartridge = !this.BitmapParameters.GenerateUserCartridge;
				this.UpdateZoom ();
			}

			if (sender == this.cartridgeDateButton)
			{
				this.BitmapParameters.GenerateDateCartridge = !this.BitmapParameters.GenerateDateCartridge;
				this.UpdateZoom ();
			}

			if (sender == this.cartridgeSamplesButton)
			{
				this.BitmapParameters.GenerateSamplesCartridge = !this.BitmapParameters.GenerateSamplesCartridge;
				this.UpdateZoom ();
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
		private static EntitiesEditor.BitmapParameters bitmapParameters = new EntitiesEditor.BitmapParameters();

		private readonly DesignerApplication designerApplication;

		private Widget optionsContainer;
		private RadioButton optionsZoom1;
		private RadioButton optionsZoom2;
		private RadioButton optionsZoom3;
		private RadioButton optionsZoom4;
		private CheckButton cartridgeUserButton;
		private CheckButton cartridgeDateButton;
		private CheckButton cartridgeSamplesButton;
	}
}
