//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	public class FormWorkspace : CoreWorkspace
	{
		public FormWorkspace()
		{
			this.hintListController = new HintListController ()
			{
				Visibility = HintListVisibilityMode.Visible,
				ContentType = HintListContentType.Catalog
			};
		}

		
		public override AbstractGroup CreateUserInterface()
		{
			FrameBox frame = new FrameBox ();

			this.hintListController.DefineContainer (frame);
			this.panel = UI.LoadPanel (Epsitec.Cresus.AddressBook.FormIds.AdressePersonne);
			this.panel.Dock = DockStyle.Fill;
			this.panel.SetEmbedder (frame);

			this.currentData = EntityContext.Current.CreateEntity<AddressBook.Entities.AdressePersonneEntity> ();
			this.dialogData = new DialogData (this.currentData, DialogDataMode.Search);
			this.resolver = this.Application.Data.Resolver;
			
			this.controller = new DialogSearchController ();
			this.controller.DialogData = this.dialogData;
			this.controller.DialogPanel = this.panel;
			this.controller.DialogWindow = this.Application.Window;
			this.controller.Resolver = this.resolver;

			this.dialogData.BindToUserInterface (this.panel);

#if false
			if (this.dialogSearchController != null)
			{
				this.dialogSearchController.DialogData = this.dialogData;
				this.dialogSearchController.DialogWindow = this.DialogWindow;
				this.dialogSearchController.DialogPanel = this.panel;
			}

			ValidationContext.SetContext (this.panel, this.validationContext);

			this.validationContext.CommandContext = Widgets.Helpers.VisualTree.GetCommandContext (this.panel);
			this.validationContext.Refresh (this.panel);
#endif
			
			return frame;
		}


		private readonly HintListController hintListController;
		private Panel panel;
		private DialogData dialogData;
		private AbstractEntity currentData;
		private DialogSearchController controller;
		private IEntityResolver resolver;
	}
}
