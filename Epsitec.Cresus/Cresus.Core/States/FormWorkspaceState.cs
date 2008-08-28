//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.States;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Common.Support.EntityEngine;

[assembly: State (typeof (FormWorkspaceState))]

namespace Epsitec.Cresus.Core.States
{
	/// <summary>
	/// The <c>FormWorkspaceState</c> class manages the state associated with a
	/// form workspace, as implemented by the <see cref="FormWorkspace"/> class.
	/// </summary>
	public class FormWorkspaceState : CoreWorkspaceState
	{
		public FormWorkspaceState(StateManager manager)
			: base (manager)
		{
			this.hintListController = new HintListController ()
			{
				Visibility = HintListVisibilityMode.Visible,
				ContentType = HintListContentType.Catalog
			};

			this.searchController = this.hintListController.SearchController;
		}


		public AbstractEntity					CurrentEntity
		{
			get
			{
				return Collection.GetFirst (this.SelectedEntities, null);
			}
		}

		public Druid FormId
		{
			get;
			set;
		}

		public Druid EntityId
		{
			get;
			set;
		}

		public FormWorkspaceMode Mode
		{
			get;
			set;
		}

		public IEnumerable<AbstractEntity> SelectedEntities
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

		public DialogData DialogData
		{
			get
			{
				return this.dialogData;
			}
		}

		public AbstractEntity SearchTemplate
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

		public EntityContext SearchContext
		{
			get
			{
				return this.searchContext;
			}
		}

		public AbstractEntity CurrentItem
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

		public HintListController HintListController
		{
			get
			{
				return this.hintListController;
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
					"« ", Epsitec.Common.Types.Converters.TextConverter.ConvertToTaggedText (this.Title ?? "<null>"), " »",
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
			System.Diagnostics.Debug.Assert (this.RootWidget != null);
			System.Diagnostics.Debug.Assert (this.RootWidget.Window != null);

			this.Application.CommandDispatcher.RegisterRange (this.GetCommandHandlers ());

			if (this.searchController.DialogPanel != null)
			{
				this.searchController.DialogWindow = this.RootWidget.Window;
				this.searchController.SetFocus (EntityFieldPath.Parse (this.FocusPath));

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
				this.FocusPath = e.NewPath.ToString ();
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





		protected override void StoreWorkspace(XElement workspaceElement, StateSerializationContext context)
		{
			System.Diagnostics.Debug.Assert (workspaceElement != null);

			string currentEntityId = null;
			XElement savedDialogData = null;
			XElement[] emptyElement = new XElement[0];

			if ((this.DialogData != null) &&
				(this.DialogData.EntityContext != null))
			{
				AbstractEntity template = this.SearchTemplate;

				currentEntityId = this.DialogData.EntityContext.GetPersistedId (this.CurrentEntity);
				savedDialogData = template == null ? FormWorkspaceState.SaveDialogData (this.DialogData) : FormWorkspaceState.SaveTemplate (template);
			}

			workspaceElement.Add (new XAttribute ("entityId", this.EntityId.ToString ()));
			workspaceElement.Add (new XAttribute ("formId", this.FormId.ToString ()));
			workspaceElement.Add (new XAttribute ("mode", this.Mode.ToString ()));
			workspaceElement.Add (new XAttribute ("focusPath", this.FocusPath == null ? "" : this.FocusPath.ToString ()));
			
			if (currentEntityId != null)
			{
				workspaceElement.Add (new XAttribute ("currentEntityId", currentEntityId));
			}

			if (savedDialogData != null)
			{
				workspaceElement.Add (savedDialogData);
			}
		}

		protected override void RestoreWorkspace(XElement workspaceElement)
		{
			System.Diagnostics.Debug.Assert (workspaceElement != null);

			string entityId        = (string) workspaceElement.Attribute ("entityId");
			string formId          = (string) workspaceElement.Attribute ("formId");
			string mode            = (string) workspaceElement.Attribute ("mode");
			string currentEntityId = (string) workspaceElement.Attribute ("currentEntityId");
			string focusPath       = (string) workspaceElement.Attribute ("focusPath");
			
			XElement dialogDataXml = workspaceElement.Element ("dialogData");

			this.EntityId = Druid.Parse (entityId);
			this.FormId   = Druid.Parse (formId);
			this.Mode     = mode.ToEnum<FormWorkspaceMode> (FormWorkspaceMode.None);

			AbstractEntity item = this.ResolvePersistedEntity (currentEntityId);

			switch (this.Mode)
			{
				case FormWorkspaceMode.Edition:
					this.CurrentItem = item;
					break;

				case FormWorkspaceMode.Creation:
					this.CurrentItem = this.CreateEntity (this.EntityId);
					break;

				case FormWorkspaceMode.Search:
					this.RegisterFixup (() => this.SelectEntity (item));
					break;
			}
			
			if (string.IsNullOrEmpty (focusPath) == false)
			{
				this.FocusPath = focusPath;
			}

			this.Initialize ();

			if (dialogDataXml != null)
			{
				FormWorkspaceState.RestoreDialogData (this.DialogData, dialogDataXml);
			}
		}
		
		#region Private Strings

		/// <summary>
		/// The <c>Strings</c> class defines the constants used for XML serialization
		/// of the state.
		/// </summary>
		private static class Strings
		{
			public static readonly string XmlEntityId = "entityId";
			public static readonly string XmlFormId = "formId";
			public static readonly string XmlMode = "mode";
			public static readonly string XmlCurrentEntityId = "currentEntityId";
			public static readonly string XmlFocusPath = "focusPath";
			public static readonly string XmlDialogData = "dialogData";
		}

		#endregion

		private AbstractEntity ResolvePersistedEntity(string id)
		{
			return this.StateManager.Application.Data.DataContext.GetPeristedEntity (id);
		}

		private AbstractEntity CreateEntity(Druid id)
		{
			return this.StateManager.Application.Data.DataContext.CreateEntity (id);
		}

		/// <summary>
		/// Saves the dialog data as an XML chunk.
		/// </summary>
		/// <param name="data">The dialog data.</param>
		/// <returns>The XML chunk of the saved dialog data.</returns>
		public static XElement SaveDialogData(DialogData data)
		{
			IValueConverter converter = Epsitec.Common.Types.Converters.AutomaticValueConverter.Instance;
			XElement        element   = new XElement ("dialogData");

			if (data == null)
			{
				return element;
			}

			System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;
			
			data.ForEachChange (
				change =>
				{
					object value = change.NewValue;
					string path  = change.Path.ToString ();
					StructuredTypeField field = change.Path.NavigateReadField (data.Data);

					if (value == null)
					{
						element.Add (new XElement ("null",
							new XAttribute ("path", path)));
					}
					else
					{
						string id;

						switch (field.Relation)
						{
							case FieldRelation.None:
								element.Add (new XElement ("data",
									new XAttribute ("path", path),
									new XAttribute ("value", converter.Convert (value, typeof (string), null, culture))));
								break;

							case FieldRelation.Reference:
								
								id = data.EntityContext.GetPersistedId (value as AbstractEntity);

								if (id == null)
								{
									//	TODO : ...
								}
								else
								{
									element.Add (new XElement ("ref",
										new XAttribute ("path", path),
										new XAttribute ("id", id)));
								}
								break;

							case FieldRelation.Collection:
								element.Add (new XElement ("collection",
									new XAttribute ("path", path)));
								//	TODO: how do we serialize the collection ?
								break;

							default:
								throw new System.NotSupportedException (string.Format ("Relation {0} not supported for field {1}", field.Relation, field.Id));
						}
					}
					
					return true;
				});

			return element;
		}

		/// <summary>
		/// Saves the search template as an XML chunk.
		/// </summary>
		/// <param name="template">The template.</param>
		/// <returns>The XML chunk of the saved search template.</returns>
		public static XElement SaveTemplate(AbstractEntity template)
		{
			IValueConverter converter = Epsitec.Common.Types.Converters.AutomaticValueConverter.Instance;
			XElement        element   = new XElement ("dialogData");

			if (template == null)
			{
				return element;
			}

			System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;

			template.ForEachField (EntityDataVersion.Modified,
				(fieldPath, field, value) =>
				{
					string path = fieldPath.ToString ();
					
					if (value == null)
					{
						element.Add (new XElement ("null",
							new XAttribute ("path", path)));
					}
					else
					{
						switch (field.Relation)
						{
							case FieldRelation.None:
								value = converter.Convert (value, typeof (string), null, culture);

								if (value != null)
								{
									element.Add (new XElement ("data",
										new XAttribute ("path", path),
										new XAttribute ("value", value)));
								}
								break;

							case FieldRelation.Reference:

								//	skip references; a template has just data in its fields

								break;

							case FieldRelation.Collection:
								
								//	TODO: how do we serialize the collection ?
								
								break;

							default:
								throw new System.NotSupportedException (string.Format ("Relation {0} not supported for field {1}", field.Relation, field.Id));
						}
					}
				});

			return element;
		}

		/// <summary>
		/// Restores the dialog data based on an XML chunk.
		/// </summary>
		/// <param name="data">The dialog data.</param>
		/// <param name="element">The XML chunk used to restore the dialog data.</param>
		public static void RestoreDialogData(DialogData data, XElement element)
		{
			System.Diagnostics.Debug.Assert (element.Name == "dialogData");

			if (data == null)
			{
				return;
			}
			
			IValueConverter converter = Epsitec.Common.Types.Converters.AutomaticValueConverter.Instance;
			System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;

			foreach (XElement dataElement in element.Descendants ())
			{
				EntityFieldPath     path = EntityFieldPath.Parse ((string) dataElement.Attribute ("path"));
				StructuredTypeField field;
				
				object value;
				
				switch (dataElement.Name.LocalName)
				{
					case "null":
						value = null;
						break;

					case "data":
						path.CreateMissingNodes (data.Data);
						field = path.NavigateReadField (data.Data);
						value = (string) dataElement.Attribute ("value");
						value = converter.ConvertBack (value, field.Type.SystemType, null, culture);
						break;
					
					case "ref":
						path.CreateMissingNodes (data.Data);
						value = data.EntityContext.GetPeristedEntity ((string) dataElement.Attribute ("id"));
						break;
					
					case "collection":
						//	TODO: ...
						continue;
					
					default:
						throw new System.NotSupportedException (string.Format ("Unsupported XML element {0} found", dataElement.Name));
				}

				path.NavigateWrite (data.Data, value);
			}
		}
		
		private readonly HintListController		hintListController;
		private readonly DialogSearchController	searchController;
		private Panel							searchPanel;
		private Panel							editionPanel;
		private DialogData						dialogData;
		private AbstractEntity					currentItem;
		private EntityContext					searchContext;
		private IEntityResolver					resolver;
	}
}
