//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DebugViewer.ViewControllers;
using Epsitec.Common.Widgets.Behaviors;

namespace Epsitec.Cresus.DebugViewer
{
	public class CoreApplication : CoreInteractiveApp
	{
		public CoreApplication()
		{
		}

		public override string					ShortWindowTitle
		{
			get
			{
				return "Crésus Debug Viewer";
			}
		}

		public override string					ApplicationIdentifier
		{
			get
			{
				return "Cr.DebugViewer";
			}
		}


		public override bool StartupLogin()
		{
			return true;
		}


		protected override void CreateManualComponents(IList<System.Action> initializers)
		{
			initializers.Add (this.InitializeApplication);
		}

		protected override void InitializeEmptyDatabase()
		{
		}

		protected override System.Xml.Linq.XDocument LoadApplicationState()
		{
			return null;
		}

		protected override void SaveApplicationState(System.Xml.Linq.XDocument doc)
		{
		}

		private void InitializeApplication()
		{
			Window dialog = new Window ();

#if false
			var frame1 = new FrameBox ()
			{
				Parent = dialog.Root,
				Dock = DockStyle.Top,
				Margins = new Margins (4, 4, 4, 4),
				BackColor = Color.FromBrightness (1),
				DrawFrameWidth = 1.0,
				DrawFrameEdges = FrameEdges.Top,
				PreferredHeight = 60,
			};
			
			var tab = new OptionTab ()
			{
				Parent = frame1,
				Margins = new Margins (10, 10, 0, 0),
				Dock = DockStyle.Top,
				PreferredHeight = 32,
			};

			var searchBox = new SearchBox ()
			{
				Parent = tab,
				Dock = DockStyle.Fill,
				PreferredHeight = 40,
				Margins = new Margins (1, 1, 1, 1),
			};

			searchBox.Policy.DisplayNavigationButtons = true;
			searchBox.Policy.DisplaySearchButton = true;
#endif

			var frame2 = new FrameBox ()
			{
				Parent = dialog.Root,
				Dock = DockStyle.Fill,
				Padding = new Margins (24, 4, 12, 4),
			};

			var slim1 = new SlimField ()
			{
				Parent = frame2,
				FieldLabel = "Titre",
				FieldText = "Monsieur",
				Dock = DockStyle.Top,
				HorizontalAlignment = HorizontalAlignment.Left,
				DisplayMode = SlimFieldDisplayMode.Text,
				Margins = new Margins (0, 0, 0, 1),
				TabIndex = 1,
			};

			slim1.MenuItems.Add (new SlimFieldMenuItemCommand (SlimFieldMenuItemCommandCode.Clear));
			slim1.MenuItems.Add (new SlimFieldMenuItem ("Monsieur", active: ActiveState.Yes));
			slim1.MenuItems.Add (new SlimFieldMenuItem ("Madame"));
			slim1.MenuItems.Add (new SlimFieldMenuItem ("Mademoiselle"));
			slim1.MenuItems.Add (new SlimFieldMenuItemCommand (SlimFieldMenuItemCommandCode.Extra));

			var frame3 = new FrameBox ()
			{
				Parent = frame2,
				Margins = new Margins (0, 0, 0, 1),
				Dock = DockStyle.Top,
				PreferredHeight = 17,
				TabIndex = 2,
			};

			var slim2 = new SlimField ()
			{
				Parent = frame3,
				FieldLabel = "Prénom",
				FieldText = "Hans",
				Dock = DockStyle.Left,
				HorizontalAlignment = HorizontalAlignment.Left,
				DisplayMode = SlimFieldDisplayMode.Text,
				Margins = new Margins (0, 1, 0, 0),
				TabIndex = 3,
			};

			var slim3 = new SlimField ()
			{
				Parent = frame3,
				FieldLabel = "Nom",
				FieldText = "Meier-Müller",
				Dock = DockStyle.Left,
				HorizontalAlignment = HorizontalAlignment.Left,
				DisplayMode = SlimFieldDisplayMode.Text,
				TabIndex = 4,
			};

			var slim4 = new SlimField ()
			{
				Parent = frame2,
				Dock = DockStyle.Top,
				HorizontalAlignment = HorizontalAlignment.Left,
				FieldLabel = "Langue de correspondance",
				FieldPrefix = "Langue de correspondance : ",
				FieldText = "français",
				DisplayMode = SlimFieldDisplayMode.Text,
				Margins = new Margins (0, 0, 0, 1),
				TabIndex = 5,
			};

			slim4.MenuItems.Add (new SlimFieldMenuItemCommand (SlimFieldMenuItemCommandCode.Clear));
			slim4.MenuItems.Add (new SlimFieldMenuItem ("français", active: ActiveState.Yes));
			slim4.MenuItems.Add (new SlimFieldMenuItem ("allemand"));
			slim4.MenuItems.Add (new SlimFieldMenuItem ("italien", enable: EnableState.Disabled));
			slim4.MenuItems.Add (new SlimFieldMenuItem ("anglais"));
			slim4.MenuItems.Add (new SlimFieldMenuItemCommand (SlimFieldMenuItemCommandCode.Extra));

			var slim5 = new SlimField ()
			{
				Parent = frame2,
				Dock = DockStyle.Top,
				HorizontalAlignment = HorizontalAlignment.Left,
				FieldPrefix = "Né le ",
				FieldText = "10.03.1965",
				DisplayMode = SlimFieldDisplayMode.Text,
				Margins = new Margins (0, 0, 0, 1),
				TabIndex = 6,
			};

			var b1 = new SlimFieldMenuBehavior (slim1);
			var b2 = new SlimFieldTextBehavior (slim2);
			var b3 = new SlimFieldTextBehavior (slim3);
			var b4 = new SlimFieldMenuBehavior (slim4);
			var b5 = new SlimFieldTextBehavior (slim5);
			
			dialog.ShowDialog ();
			System.Environment.Exit (0);

			this.businessContext = new BusinessContext (this.Data);

			var window = this.Window;

			this.mainController = new MainViewController (this);
			this.mainController.CreateUI (window.Root);

			this.folderAccessor = new Accessors.LogFolderDataAccessor (@"Q:\Store");
//			this.mainAccessor = new Accessors.LogDataAccessor (@"Q:\Store\elite23@poste08-63467137422993.1376");
//			this.mainController.DefineHistoryAccessor (this.mainAccessor);
			this.mainController.DefineFolderAccessor (this.folderAccessor);

			window.Root.BackColor = Common.Drawing.Color.FromName ("White");
		}

		private BusinessContext					businessContext;
		private MainViewController				mainController;
		private Accessors.LogDataAccessor		mainAccessor;
		private Accessors.LogFolderDataAccessor	folderAccessor;
	}
}
