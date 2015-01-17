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
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Dialogs
{
	public class FileSaveImageDialog : AbstractFileDialog
	{
		public FileSaveImageDialog(Widget parent)
		{
			this.parent                  = parent;
			this.title                   = "Enregistrement d'une image bitmap";
			this.owner                   = this.parent.Window;
			this.InitialDirectory        = FileSaveImageDialog.initialDirectory;
			this.InitialFileName         = FileSaveImageDialog.initialFilename;
			this.enableNavigation        = true;
			this.enableMultipleSelection = false;
			this.hasOptions              = true;
			this.fileDialogType          = FileDialogType.Save;

			this.Filters.Add (new FilterItem ("x", "Image", "*.png|*.tif|*.bmp|*.jpg"));
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


		public void PathMemorize()
		{
			FileSaveImageDialog.initialDirectory = System.IO.Path.GetDirectoryName (this.FileName);
			FileSaveImageDialog.initialFilename  = System.IO.Path.GetFileName (this.FileName);
		}


		protected override Rectangle GetOwnerBounds()
		{
			//	Donne les frontières de l'application.
			var w = this.parent.Window;

			return new Rectangle (w.WindowLocation, w.WindowSize);
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
			this.optionsContainer.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.optionsContainer.Name = "OptionsContainer";

			//	Options pour le zoom.
			GroupBox groupZoom = new GroupBox (this.optionsContainer);
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

			this.UpdateZoom ();
		}

		private string GetZoomDescription(double zoom)
		{
			string z = System.Math.Floor (zoom*100).ToString ();
			string x = System.Math.Floor (this.ImageSize.Width*zoom).ToString ();
			string y = System.Math.Floor (this.ImageSize.Height*zoom).ToString ();

			return string.Format ("{0}% ({1} × {2} pixels)", z, x, y);
		}

		protected void UpdateZoom()
		{
			if (this.optionsZoom1 != null)
			{
				this.optionsZoom1.ActiveState = (FileSaveImageDialog.zoom == 1) ? ActiveState.Yes : ActiveState.No;
				this.optionsZoom2.ActiveState = (FileSaveImageDialog.zoom == 2) ? ActiveState.Yes : ActiveState.No;
				this.optionsZoom3.ActiveState = (FileSaveImageDialog.zoom == 3) ? ActiveState.Yes : ActiveState.No;
				this.optionsZoom4.ActiveState = (FileSaveImageDialog.zoom == 4) ? ActiveState.Yes : ActiveState.No;
			}
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
		private static Settings			settings = new Settings ();
		private static string			initialDirectory = null;
		private static string			initialFilename = null;
		private static double			zoom = 1;

		private readonly Widget			parent;

		private Widget					optionsContainer;
		private RadioButton				optionsZoom1;
		private RadioButton				optionsZoom2;
		private RadioButton				optionsZoom3;
		private RadioButton				optionsZoom4;
	}
}
