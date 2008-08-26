//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
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
			
			this.searchController = this.hintListController.SearchController;
		}


		public Druid							FormId
		{
			get;
			set;
		}

		public Druid							EntityId
		{
			get;
			set;
		}

		public FormWorkspaceMode				Mode
		{
			get;
			set;
		}

		public IEnumerable<AbstractEntity>		SelectedEntities
		{
			get
			{
				//	If somebody defined a default suggestion, which has still
				//	not been taken into account by the controller, then we
				//	will simply return it as the only selected entity : the
				//	controller has never had a chance to update the list...

				if ((this.searchController != null) &&
					(this.searchController.DefaultSuggestion != null))
				{
					return new AbstractEntity[] { this.searchController.DefaultSuggestion };
				}

				AbstractEntity data = this.dialogData.ExternalData;

				if (data == null)
				{
					return EmptyEnumerable<AbstractEntity>.Instance;
				}
				else
				{
					//	TODO: handle multiple selection

					return new AbstractEntity[] { data };
				}
			}
		}

		public DialogData						DialogData
		{
			get
			{
				return this.dialogData;
			}
		}

		public AbstractEntity					SearchTemplate
		{
			get
			{
				if (this.Mode != FormWorkspaceMode.Search)
				{
					return null;
				}
				else
				{
					ISearchContext context = this.searchController.ActiveSearchContext;

					if (context == null)
					{
						return null;
					}
					else
					{
						return context.SearchTemplate;
					}
				}
			}
		}

		public EntityContext					SearchContext
		{
			get
			{
				return this.searchContext;
			}
		}

		public AbstractEntity					CurrentItem
		{
			get
			{
				return this.currentItem;
			}
			set
			{
				this.currentItem = value;
			}
		}

		public HintListController				HintListController
		{
			get
			{
				return this.hintListController;
			}
		}

		/// <summary>
		/// Gets the path of the focused field.
		/// </summary>
		/// <value>The path of the focused field or <c>null</c>.</value>
		public EntityFieldPath					FocusPath
		{
			get
			{
				return this.focusFieldPath;
			}
			set
			{
				if (this.focusFieldPath != value)
				{
					this.focusFieldPath = value;

					//	TODO: generate event...
				}
			}
		}


		internal void Initialize()
		{
			if ((this.searchContext == null) &&
				(this.FormId.IsValid) &&
				(this.EntityId.IsValid) &&
				(this.Mode != FormWorkspaceMode.None))
			{
				this.searchContext = new EntityContext (this.Application.ResourceManager, EntityLoopHandlingMode.Skip);
				this.searchContext.ExceptionManager = this.Application.ExceptionManager;
				this.searchContext.PersistenceManagers.Add (this.Application.Data.DataContext);

				if (this.currentItem == null)
				{
					this.currentItem = this.searchContext.CreateEntity (this.EntityId);
				}

				switch (this.Mode)
				{
					case FormWorkspaceMode.Creation:
					case FormWorkspaceMode.Edition:
						this.dialogData = new DialogData (this.currentItem, this.searchContext, DialogDataMode.Isolated);
						break;
					
					case FormWorkspaceMode.Search:
						this.dialogData = new DialogData (this.currentItem, this.searchContext, DialogDataMode.Search);
						break;
				}

				
				this.dialogData.ExternalDataChanged += this.HandleDialogDataExternalDataChanged;
				this.resolver = this.Application.Data.Resolver;
			}
		}

		internal void AcceptEdition()
		{
			switch (this.Mode)
			{
				case FormWorkspaceMode.Creation:
				case FormWorkspaceMode.Edition:
					this.dialogData.ApplyChanges ();
					break;
			}
		}

		public void SelectEntity(AbstractEntity entity)
		{
			this.searchController.DefaultSuggestion = entity;
		}
		
		public override AbstractGroup CreateUserInterface()
		{
			FrameBox frame = new FrameBox ();

			if ((this.FormId.IsEmpty) ||
				(this.EntityId.IsEmpty))
			{
				string title = string.Concat (
					@"<font size=""141%"">",
					"« ", Epsitec.Common.Types.Converters.TextConverter.ConvertToTaggedText (this.State.Title ?? "<null>"), " »",
					@"</font><br/>",
					@"<i>Invalid form workspace</i>");

				frame.Children.Add (
					new StaticText ()
					{
						Text = title,
						ContentAlignment = ContentAlignment.MiddleCenter,
						Dock = DockStyle.Fill
					});

				return frame;
			}

			this.Initialize ();

			System.Diagnostics.Debug.Assert (this.FormId.IsValid);
			System.Diagnostics.Debug.Assert (this.EntityId.IsValid);

			this.hintListController.DefineContainer (frame);
			
			this.searchController.DialogFocusChanged += this.HandleSearchControllerDialogFocusChanged;
			this.searchController.SuggestionChanged  += this.HandleSearchControllerSuggestionChanged;
			this.searchController.DialogDataChanged  += this.HandleSearchControllerDialogDataChanged;
			
			switch (this.Mode)
			{
				case FormWorkspaceMode.Search:
					this.searchPanel = UI.LoadPanel (this.FormId, PanelInteractionMode.Search);
					this.searchPanel.Dock = DockStyle.Fill;
					this.searchPanel.SetEmbedder (frame);
					this.searchPanel.Margins = new Margins (4);

					this.hintListController.HintListWidget.Header.ContentType = HintListContentType.Catalog;
					
					this.searchController.DialogData   = this.dialogData;
					this.searchController.DialogPanel  = this.searchPanel;
					this.searchController.Resolver     = this.resolver;
					this.searchController.AssertReady ();

					this.dialogData.BindToUserInterface (this.searchPanel);
					break;

				case FormWorkspaceMode.Creation:
				case FormWorkspaceMode.Edition:
					this.editionPanel = UI.LoadPanel (this.FormId, PanelInteractionMode.Default);
					this.editionPanel.Dock = DockStyle.Fill;
					this.editionPanel.SetEmbedder (frame);
					this.editionPanel.Margins = new Margins (4);

					this.hintListController.HintListWidget.Header.ContentType = HintListContentType.Suggestions;
					
					this.searchController.DialogData   = this.dialogData;
					this.searchController.DialogPanel  = this.editionPanel;
					this.searchController.Resolver     = this.resolver;
					
					this.dialogData.BindToUserInterface (this.editionPanel);
					break;

				default:
					throw new System.ArgumentException ();
			}


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

#if false
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
#endif
			
			return frame;
		}

		protected override void EnableWorkspace()
		{
			System.Diagnostics.Debug.Assert (this.Container != null);
			System.Diagnostics.Debug.Assert (this.Container.Window != null);

			this.Application.CommandDispatcher.RegisterRange (this.GetCommandHandlers ());

			if (this.searchController.DialogPanel != null)
			{
				this.searchController.DialogWindow = this.Container.Window;
				this.searchController.SetFocus (this.focusFieldPath);

				this.hintListController.RefreshHintList ();
			}
		}

		protected override void DisableWorkspace()
		{
			this.Application.CommandDispatcher.UnregisterRange (this.GetCommandHandlers ());

			this.searchController.DialogWindow = null;
			this.hintListController.SetEmptyHintList ();
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
			this.searchController.ClearActiveSuggestion ();
		}

		private void ExecuteStartItemEditionCommand(object sender, CommandEventArgs e)
		{
#if false
			AbstractEntity data = this.dialogData.ExternalData;

			if ((data != null) &&
				(this.editionDialogData == null))
			{
				this.searchController.ResetSuggestions ();
				this.searchPanel.Visibility = false;
				this.editionPanel.Visibility = true;
				this.editionDialogData = new DialogData (data, this.searchContext, DialogDataMode.Isolated);
				this.searchController.DialogData = this.editionDialogData;
				this.editionDialogData.BindToUserInterface (this.editionPanel);
				this.dialogData.UnbindFromUserInterface (this.searchPanel);
				this.editionPanel.SetFocusOnTabWidget ();
			}
#endif
		}

		private void ExecuteValidateItemEditionCommand(object sender, CommandEventArgs e)
		{
#if false
			AbstractEntity data = this.dialogData.ExternalData;

			if ((data != null) &&
				(this.editionDialogData != null))
			{
				this.editionDialogData.ApplyChanges ();
				this.searchController.ResetSuggestions ();
				this.editionPanel.Visibility = false;
				this.editionDialogData.UnbindFromUserInterface (this.editionPanel);
				this.editionDialogData = null;
				this.searchPanel.Visibility = true;
				this.searchController.DialogData = this.dialogData;
				this.dialogData.BindToUserInterface (this.searchPanel);
				this.searchPanel.SetFocusOnTabWidget ();
			}
#endif
		}


		private void HandleDialogDataExternalDataChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			AbstractEntity value = e.NewValue as AbstractEntity;

			if (value != null)
			{
				System.Diagnostics.Debug.WriteLine (value.Dump ());
			}
		}
		
		private void HandleSearchControllerDialogFocusChanged(object sender, DialogFocusEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Focus changed : " + e.ToString ());

			if (e.NewPath != null)
			{
				this.FocusPath = e.NewPath;
				this.Application.AsyncSaveApplicationState ();
			}
		}
		
		private void HandleSearchControllerSuggestionChanged(object sender, DialogDataEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Suggestion changed : " + e.ToString ());

			AbstractEntity suggestion = e.NewValue as AbstractEntity;
			this.Application.AsyncSaveApplicationState ();
		}

		private void HandleSearchControllerDialogDataChanged(object sender, DialogDataEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Data changed : " + e.ToString ());
			this.Application.AsyncSaveApplicationState ();
		}




		private readonly HintListController		hintListController;
		private readonly DialogSearchController	searchController;
		private Panel							searchPanel;
		private Panel							editionPanel;
		private DialogData						dialogData;
		private AbstractEntity					currentItem;
		private EntityContext					searchContext;
		private IEntityResolver					resolver;
		private EntityFieldPath					focusFieldPath;
	}
}
