//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>CoreApplication</c> class implements the central application
	/// logic.
	/// </summary>
	public class CoreApplication : Application
	{
		public CoreApplication()
		{
			this.data = new CoreData ();
			this.exceptionManager = new CoreLibrary.ExceptionManager ();
		}


		public CoreData Data
		{
			get
			{
				return this.data;
			}
		}

		public IExceptionManager ExceptionManager
		{
			get
			{
				return this.exceptionManager;
			}
		}
		
		public override string ShortWindowTitle
		{
			get
			{
				return Res.Strings.ProductName;
			}
		}

		
		public void SetupInterface()
		{
			Window window = new Window ();

			window.Text = this.ShortWindowTitle;
			window.ClientSize = new Epsitec.Common.Drawing.Size (600, 400);

			this.Window = window;

			this.ribbonBox = new FrameBox (window.Root)
			{
				Dock = DockStyle.Top
			};

			this.workspaceBox = new FrameBox (window.Root)
			{
				Dock = DockStyle.Fill
			};

			this.CreateRibbon ();
			this.CreateWorkspaces ();
			
			this.formWorkspace.SetEnable (true);
		}

		public void SetupData()
		{
			this.data.SetupDatabase ();
		}

		private void CreateRibbon()
		{
			this.ribbonBook = new RibbonBook (this.ribbonBox)
			{
				Dock = DockStyle.Fill
			};
			
			this.ribbonPageHome = new RibbonPage (this.ribbonBook)
			{
				Name = "Home",
				RibbonTitle = "Principal"
			};


			RibbonSection section;
			FrameBox      frame;

			section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Edition",
				Title = "Edition",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow
			};

			frame = new FrameBox (section)
			{
				Dock = DockStyle.Stacked,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow
			};

			section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "Bases",
				Title = "Bases de données",
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow
			};

			section = new RibbonSection (this.ribbonPageHome)
			{
				Name = "States",
				Title = "Etats",
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow
			};

#if false
			subFrame = new FrameBox (frame);
			subFrame.Dock = DockStyle.Stacked;
			subFrame.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			subFrame.Children.Add (new IconButton (ApplicationCommands.New, new Size (47, 36), DockStyle.Stacked));

			Widget groupOpen = new Widget (subFrame);
			groupOpen.Dock = DockStyle.Stacked;
			groupOpen.PreferredSize = new Size (47, 47);
			GlyphButton buttonLastFiles = new GlyphButton (groupOpen);
			IconButton buttonOpen = new IconButton (ApplicationCommands.Open, new Size (47, 36), DockStyle.Top);
#endif
		}

		private void CreateWorkspaces()
		{
			this.formWorkspace = new Workspaces.FormWorkspace ()
			{
				Application = this,
				FormId      = Epsitec.Cresus.Mai2008.FormIds.Facture,
				EntityId    = Epsitec.Cresus.Mai2008.Entities.FactureEntity.EntityStructuredTypeId
			};
#if true
			this.formWorkspace.FormId = Epsitec.Cresus.AddressBook.FormIds.AdressePersonne;
			this.formWorkspace.EntityId = Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity.EntityStructuredTypeId;
#endif

			this.workspaceBox.Children.Add (this.formWorkspace.Container);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.data != null)
				{
					this.data.Dispose ();
					this.data = null;
				}
				if (this.exceptionManager != null)
				{
					this.exceptionManager.Dispose ();
					this.exceptionManager = null;
				}
			}

			base.Dispose (disposing);
		}


		Workspaces.FormWorkspace				formWorkspace;
		CoreData								data;
		CoreLibrary.ExceptionManager			exceptionManager;
		
		private FrameBox						ribbonBox;
		private FrameBox						workspaceBox;

		private RibbonBook						ribbonBook;
		private RibbonPage						ribbonPageHome;
	}
}
