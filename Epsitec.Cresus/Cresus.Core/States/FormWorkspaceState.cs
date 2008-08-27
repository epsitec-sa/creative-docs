//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.States;
using Epsitec.Cresus.Core.Workspaces;

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
	public class FormWorkspaceState : CoreWorkspaceState<Workspaces.FormWorkspace>
	{
		public FormWorkspaceState(StateManager manager)
			: base (manager)
		{
		}


		public AbstractEntity					CurrentEntity
		{
			get
			{
				Workspaces.FormWorkspace workspace = this.Workspace;
				
				if (workspace == null)
				{
					return null;
				}
				else
				{
					return Collection.GetFirst (workspace.SelectedEntities, null);
				}
			}
		}


		protected override void StoreWorkspace(XElement element, StateSerializationContext context)
		{
			base.StoreWorkspace (element, context);
			
			Workspaces.FormWorkspace workspace = this.Workspace;

			if (workspace != null)
			{
				string currentEntityId = null;
				XElement savedDialogData = null;
				XElement[] emptyElement = new XElement[0];

				if ((workspace.DialogData != null) &&
					(workspace.DialogData.EntityContext != null))
				{
					AbstractEntity template = workspace.SearchTemplate;

					currentEntityId = workspace.DialogData.EntityContext.GetPersistedId (this.CurrentEntity);
					savedDialogData = template == null ? FormWorkspaceState.SaveDialogData (workspace.DialogData) : FormWorkspaceState.SaveTemplate (template);
				}

				element.Add (new XAttribute ("entityId", workspace.EntityId.ToString ()));
				element.Add (new XAttribute ("formId", workspace.FormId.ToString ()));
				element.Add (new XAttribute ("mode", workspace.Mode.ToString ()));
				element.Add (new XAttribute ("focusPath", workspace.FocusPath == null ? "" : workspace.FocusPath.ToString ()));
				
				if (currentEntityId != null)
				{
					element.Add (new XAttribute ("currentEntityId", currentEntityId));
				}

				if (savedDialogData != null)
				{
					element.Add (savedDialogData);
				}
			}
		}

		protected override void RestoreWorkspace(XElement workspaceElement)
		{
			base.RestoreWorkspace (workspaceElement);

			Workspaces.FormWorkspace workspace = this.Workspace;
			
			if (workspaceElement != null)
			{
				string entityId        = (string) workspaceElement.Attribute ("entityId");
				string formId          = (string) workspaceElement.Attribute ("formId");
				string mode            = (string) workspaceElement.Attribute ("mode");
				string currentEntityId = (string) workspaceElement.Attribute ("currentEntityId");
				string focusPath       = (string) workspaceElement.Attribute ("focusPath");
				
				XElement dialogDataXml = workspaceElement.Element ("dialogData");
				
				workspace.EntityId = Druid.Parse (entityId);
				workspace.FormId   = Druid.Parse (formId);
				workspace.Mode     = mode.ToEnum<FormWorkspaceMode> (FormWorkspaceMode.None);

				AbstractEntity item = this.ResolvePersistedEntity (currentEntityId);

				switch (workspace.Mode)
				{
					case FormWorkspaceMode.Edition:
						workspace.CurrentItem = item;
						break;

					case FormWorkspaceMode.Creation:
						workspace.CurrentItem = this.StateManager.Application.Data.DataContext.CreateEntity (workspace.EntityId);
						break;

					case FormWorkspaceMode.Search:
						this.RegisterFixup (() => workspace.SelectEntity (item));
						break;
				}
				
				if (string.IsNullOrEmpty (focusPath) == false)
				{
					workspace.FocusPath = EntityFieldPath.Parse (focusPath);
				}

				workspace.Initialize ();

				if (dialogDataXml != null)
				{
					FormWorkspaceState.RestoreDialogData (workspace.DialogData, dialogDataXml);
				}
			}
		}

		private AbstractEntity ResolvePersistedEntity(string id)
		{
			return this.StateManager.Application.Data.DataContext.GetPeristedEntity (id);
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
						string id;

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
	}
}
