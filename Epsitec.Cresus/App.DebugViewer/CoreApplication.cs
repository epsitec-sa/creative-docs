//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DebugViewer.ViewControllers;

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
