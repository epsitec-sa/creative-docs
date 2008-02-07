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
			
			this.searchPanel = UI.LoadPanel (Epsitec.Cresus.AddressBook.FormIds.AdressePersonne, PanelInteractionMode.Search);
			this.searchPanel.Dock = DockStyle.Fill;
			this.searchPanel.SetEmbedder (frame);
			
			this.editionPanel = UI.LoadPanel (Epsitec.Cresus.AddressBook.FormIds.AdressePersonne, PanelInteractionMode.Default);
			this.editionPanel.Dock = DockStyle.Fill;
			this.editionPanel.SetEmbedder (frame);
			this.editionPanel.Visibility = false;

			this.searchContext = new EntityContext (this.Application.ResourceManager, EntityLoopHandlingMode.Skip);
			this.currentData = this.searchContext.CreateEntity<AddressBook.Entities.AdressePersonneEntity> ();
			this.dialogData = new DialogData (this.currentData, this.searchContext, DialogDataMode.Search);
			this.dialogData.ExternalDataChanged += this.HandleDialogDataExternalDataChanged;
			this.resolver = this.Application.Data.Resolver;

			this.controller = this.hintListController.SearchController;
			this.controller.DialogData = this.dialogData;
			this.controller.DialogPanel = this.searchPanel;
			this.controller.DialogWindow = this.Application.Window;
			this.controller.Resolver = this.resolver;

			this.dialogData.BindToUserInterface (this.searchPanel);

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

			this.hintListController.HintListWidget.Header.ToolBar.Items.Add (new Button ()
			{
				CommandObject = Epsitec.Common.Dialogs.Res.Commands.HintList.StartItemEdition,
				PreferredWidth = 40
			});
			
			this.hintListController.HintListWidget.Header.ToolBar.Items.Add (new Button ()
			{
				CommandObject = Epsitec.Common.Dialogs.Res.Commands.HintList.ClearSearch,
				PreferredWidth = 40
			});

			
			return frame;
		}

		protected override void EnableWorkspace()
		{
			CommandDispatcher dispatcher = this.Application.CommandDispatcher;

			dispatcher.Register (Epsitec.Common.Dialogs.Res.Commands.HintList.ClearSearch, this.ExecuteClearSearchCommand);
			dispatcher.Register (Epsitec.Common.Dialogs.Res.Commands.HintList.StartItemEdition, this.ExecuteStartItemEditionCommand);
		}

		protected override void DisableWorkspace()
		{
			CommandDispatcher dispatcher = this.Application.CommandDispatcher;

			dispatcher.Unregister (Epsitec.Common.Dialogs.Res.Commands.HintList.ClearSearch, this.ExecuteClearSearchCommand);
			dispatcher.Unregister (Epsitec.Common.Dialogs.Res.Commands.HintList.StartItemEdition, this.ExecuteStartItemEditionCommand);
		}


		private void ExecuteClearSearchCommand(object sender, CommandEventArgs e)
		{
			this.controller.ClearSuggestions ();
		}

		private void ExecuteStartItemEditionCommand(object sender, CommandEventArgs e)
		{
			AbstractEntity data = this.dialogData.ExternalData;

			if (data != null)
			{
				this.controller.ClearSuggestions ();
				this.searchPanel.Visibility = false;
				this.editionPanel.Visibility = true;
				this.editionDialogData = new DialogData (data, this.searchContext, DialogDataMode.Isolated);
				this.controller.DialogData = this.editionDialogData;
				this.editionDialogData.BindToUserInterface (this.editionPanel);
			}
		}


		private void HandleDialogDataExternalDataChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			AbstractEntity value = e.NewValue as AbstractEntity;

			if (value != null)
			{
				System.Diagnostics.Debug.WriteLine (value.Dump ());
			}
		}


		private readonly HintListController hintListController;
		private Panel searchPanel;
		private Panel editionPanel;
		private DialogData dialogData;
		private DialogData editionDialogData;
		private AbstractEntity currentData;
		private EntityContext searchContext;
		private DialogSearchController controller;
		private IEntityResolver resolver;
	}
}
