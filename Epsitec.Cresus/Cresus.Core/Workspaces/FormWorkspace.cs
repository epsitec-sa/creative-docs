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

		public IEnumerable<AbstractEntity>		SelectedEntities
		{
			get
			{
				//	If somebody defined a default suggestion, which has still
				//	not been taken into account by the controller, then we
				//	will simply return it as the only selected entity : the
				//	controller has never had a chance to update the list...

				if ((this.controller != null) &&
					(this.controller.DefaultSuggestion != null))
				{
					return new AbstractEntity[] { this.controller.DefaultSuggestion };
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

		internal DialogData						DialogData
		{
			get
			{
				return this.dialogData;
			}
		}

		internal AbstractEntity					SearchTemplate
		{
			get
			{
				if ((this.editionDialogData != null) ||
					(this.controller == null))
				{
					return null;
				}
				else
				{
					ISearchContext context = this.controller.ActiveSearchContext;

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

		/// <summary>
		/// Gets the path of the focused field.
		/// </summary>
		/// <value>The path of the focused field or <c>null</c>.</value>
		internal EntityFieldPath FocusPath
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
				(this.EntityId.IsValid))
			{
				this.searchContext = new EntityContext (this.Application.ResourceManager, EntityLoopHandlingMode.Skip);
				this.searchContext.ExceptionManager = this.Application.ExceptionManager;
				this.searchContext.PersistanceManagers.Add (this.Application.Data.DataContext);

				this.currentItem = this.searchContext.CreateEntity (this.EntityId);
				this.dialogData = new DialogData (this.currentItem, this.searchContext, DialogDataMode.Search);
				this.dialogData.ExternalDataChanged += this.HandleDialogDataExternalDataChanged;
				this.resolver = this.Application.Data.Resolver;

				this.controller = this.hintListController.SearchController;
			}
		}

		public void SelectEntity(AbstractEntity entity)
		{
			if (this.controller != null)
			{
				this.controller.DefaultSuggestion = entity;
			}
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

			this.searchPanel = UI.LoadPanel (this.FormId, PanelInteractionMode.Search);
			this.searchPanel.Dock = DockStyle.Fill;
			this.searchPanel.SetEmbedder (frame);
			this.searchPanel.Margins = new Margins (4);
			
			this.editionPanel = UI.LoadPanel (this.FormId, PanelInteractionMode.Default);
			this.editionPanel.Dock = DockStyle.Fill;
			this.editionPanel.SetEmbedder (frame);
			this.editionPanel.Visibility = false;
			this.editionPanel.Margins = new Margins (4);

			this.controller.DialogData   = this.dialogData;
			this.controller.DialogPanel  = this.searchPanel;
//-			this.controller.DialogWindow = this.Application.Window;
			this.controller.Resolver     = this.resolver;
			this.controller.AssertReady ();

			this.controller.DialogFocusChanged += this.HandleSearchControllerDialogFocusChanged;
			this.controller.SuggestionChanged  += this.HandleSearchControllerSuggestionChanged;
			this.controller.DialogDataChanged  += this.HandleSearchControllerDialogDataChanged;

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
			System.Diagnostics.Debug.Assert (this.Container != null);
			System.Diagnostics.Debug.Assert (this.Container.Window != null);

			this.Application.CommandDispatcher.RegisterRange (this.GetCommandHandlers ());

			if ((this.controller != null) &&
				(this.controller.DialogPanel != null))
			{
				this.controller.DialogWindow = this.Container.Window;

				if ((this.focusFieldPath == null) ||
					(this.focusFieldPath.IsEmpty))
				{
					this.controller.DialogPanel.SetFocusOnTabWidget ();
				}
				else
				{
					this.controller.SetFocus (this.focusFieldPath);
				}

				this.hintListController.RefreshHintListWidget ();
			}
		}

		protected override void DisableWorkspace()
		{
			this.Application.CommandDispatcher.UnregisterRange (this.GetCommandHandlers ());

			if (this.controller != null)
			{
				this.controller.DialogWindow = null;
			}
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
			if (this.controller != null)
			{
				this.controller.ClearActiveSuggestion ();
			}
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
		
		private void HandleSearchControllerDialogFocusChanged(object sender, DialogFocusEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Focus changed : " + e.ToString ());

			if (e.NewPath != null)
			{
				this.FocusPath = e.NewPath;
				this.SaveState ();
			}
		}
		
		private void HandleSearchControllerSuggestionChanged(object sender, DialogDataEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Suggestion changed : " + e.ToString ());

			AbstractEntity suggestion = e.NewValue as AbstractEntity;
			this.SaveState ();
		}

		private void HandleSearchControllerDialogDataChanged(object sender, DialogDataEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Data changed : " + e.ToString ());
			this.SaveState ();
		}

		private void SaveState()
		{
			this.StateManager.WriteStates (@"S:\states.xml");
			System.Diagnostics.Debug.WriteLine ("Save done.");
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
		private EntityFieldPath					focusFieldPath;
	}
}
