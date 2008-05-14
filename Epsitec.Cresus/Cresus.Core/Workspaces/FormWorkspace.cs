//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Workspaces
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

			Druid formId   = Epsitec.Cresus.AddressBook.FormIds.AdressePersonne;
			Druid entityId = Epsitec.Cresus.AddressBook.Entities.AdressePersonneEntity.EntityStructuredTypeId;

			formId   = Epsitec.Cresus.Mai2008.FormIds.Facture;
			entityId = Epsitec.Cresus.Mai2008.Entities.FactureEntity.EntityStructuredTypeId;

			
			this.searchPanel = UI.LoadPanel (formId, PanelInteractionMode.Search);
			this.searchPanel.Dock = DockStyle.Fill;
			this.searchPanel.SetEmbedder (frame);
			
			this.editionPanel = UI.LoadPanel (formId, PanelInteractionMode.Default);
			this.editionPanel.Dock = DockStyle.Fill;
			this.editionPanel.SetEmbedder (frame);
			this.editionPanel.Visibility = false;

			this.searchContext = new EntityContext (this.Application.ResourceManager, EntityLoopHandlingMode.Skip);
			this.searchContext.ExceptionManager = this.Application.ExceptionManager;
			
			this.currentItem = this.searchContext.CreateEntity (entityId);
			this.dialogData = new DialogData (this.currentItem, this.searchContext, DialogDataMode.Search);
			this.dialogData.ExternalDataChanged += this.HandleDialogDataExternalDataChanged;
			this.resolver = this.Application.Data.Resolver;

			this.controller = this.hintListController.SearchController;
			this.controller.DialogData   = this.dialogData;
			this.controller.DialogPanel  = this.searchPanel;
			this.controller.DialogWindow = this.Application.Window;
			this.controller.Resolver     = this.resolver;
			this.controller.AssertReady ();

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

			this.hintListController.HintListWidget.Header.ToolBar.Items.Add (new Button ()
			{
				CommandObject = Epsitec.Common.Dialogs.Res.Commands.HintList.ValidateItemEdition,
				PreferredWidth = 40
			});

			
			return frame;
		}

		protected override void EnableWorkspace()
		{
			this.Application.CommandDispatcher.RegisterRange (this.GetCommandHandlers ());
		}

		protected override void DisableWorkspace()
		{
			this.Application.CommandDispatcher.UnregisterRange (this.GetCommandHandlers ());
		}

		
		private IEnumerable<CommandHandlerPair> GetCommandHandlers()
		{
			return new CommandHandlerPair[]
			{
				new CommandHandlerPair (Epsitec.Common.Dialogs.Res.Commands.HintList.ClearSearch, this.ExecuteClearSearchCommand),
				new CommandHandlerPair (Epsitec.Common.Dialogs.Res.Commands.HintList.StartItemEdition, this.ExecuteStartItemEditionCommand),
				new CommandHandlerPair (Epsitec.Common.Dialogs.Res.Commands.HintList.ValidateItemEdition, this.ExecuteValidateItemEditionCommand)
			};
		}

		private void ExecuteClearSearchCommand(object sender, CommandEventArgs e)
		{
			this.controller.ClearActiveSuggestion ();
		}

		private void ExecuteStartItemEditionCommand(object sender, CommandEventArgs e)
		{
			AbstractEntity data = this.dialogData.ExternalData;

			if ((data != null) &&
				(this.editionDialogData == null))
			{
				this.controller.ResetSuggestions ();
				this.searchPanel.Visibility = false;
				this.editionPanel.Visibility = true;
				this.editionDialogData = new DialogData (data, this.searchContext, DialogDataMode.Isolated);
				this.controller.DialogData = this.editionDialogData;
				this.editionDialogData.BindToUserInterface (this.editionPanel);
				this.dialogData.UnbindFromUserInterface (this.searchPanel);
				this.editionPanel.SetFocusOnTabWidget ();
			}
		}

		private void ExecuteValidateItemEditionCommand(object sender, CommandEventArgs e)
		{
			AbstractEntity data = this.dialogData.ExternalData;

			if ((data != null) &&
				(this.editionDialogData != null))
			{
				this.editionDialogData.ApplyChanges ();
				this.controller.ResetSuggestions ();
				this.editionPanel.Visibility = false;
				this.editionDialogData.UnbindFromUserInterface (this.editionPanel);
				this.editionDialogData = null;
				this.searchPanel.Visibility = true;
				this.controller.DialogData = this.dialogData;
				this.dialogData.BindToUserInterface (this.searchPanel);
				this.searchPanel.SetFocusOnTabWidget ();
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


		private readonly HintListController		hintListController;
		private Panel							searchPanel;
		private Panel							editionPanel;
		private DialogData						dialogData;
		private DialogData						editionDialogData;
		private AbstractEntity					currentItem;
		private EntityContext					searchContext;
		private DialogSearchController			controller;
		private IEntityResolver					resolver;
	}
}
