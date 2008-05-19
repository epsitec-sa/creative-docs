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
	public class FormWorkspaceState : CoreState
	{
		public FormWorkspaceState(StateManager manager)
			: base (manager)
		{
		}


		public Workspaces.FormWorkspace			Workspace
		{
			get
			{
				return this.workspace;
			}
			internal set
			{
				if (this.workspace != value)
				{
					System.Diagnostics.Debug.Assert (this.workspace == null);
					
					this.workspace = value;
					this.workspace.State = this;
				}
			}
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

		
		public override XElement Serialize(XElement element)
		{
			this.StoreCoreState (element);
			this.StoreWorkspace (element);

			return element;
		}

		public override CoreState Deserialize(XElement element)
		{
			System.Diagnostics.Debug.Assert (this.workspace == null);

			this.workspace = new FormWorkspace ()
			{
				State = this
			};

			this.RestoreWorkspace (element);
			this.RestoreCoreState (element);

			return this;
		}


		protected virtual void StoreWorkspace(XElement element)
		{
			if (this.workspace != null)
			{
				element.Add (new XElement ("workspace",
					new XAttribute ("entityId", this.workspace.EntityId.ToString ()),
					new XAttribute ("formId", this.workspace.FormId.ToString ()),
					new XAttribute ("focusPath", this.workspace.FocusPath == null ? "" : this.workspace.FocusPath.ToString ()),
					FormWorkspaceState.SaveDialogData (this.workspace.DialogData)));
			}
		}

		protected virtual void RestoreWorkspace(XElement element)
		{
			System.Diagnostics.Debug.Assert (this.workspace != null);

			XElement workspaceElement = element.Element ("workspace");

			if (workspaceElement != null)
			{
				this.workspace.EntityId = Druid.Parse ((string) workspaceElement.Attribute ("entityId"));
				this.workspace.FormId   = Druid.Parse ((string) workspaceElement.Attribute ("formId"));

				string focusPath = (string) workspaceElement.Attribute ("focusPath");

				if (focusPath.Length > 0)
				{
					this.workspace.FocusPath = EntityFieldPath.Parse (focusPath);
				}
			}
		}


		protected override void SoftAttachState()
		{
			if (this.workspace != null)
			{
				this.workspace.Container.SetParent (this.Container);
				this.workspace.SetEnable (true);
			}
		}

		protected override void SoftDetachState()
		{
			if (this.workspace != null)
			{
				this.workspace.SetEnable (false);
				this.workspace.Container.SetParent (null);
			}
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
		/// Restores the dialog data based on an XML chunk.
		/// </summary>
		/// <param name="data">The dialog data.</param>
		/// <param name="element">The XML chunk used to restore the dialog data.</param>
		public static void RestoreDialogData(DialogData data, XElement element)
		{
			System.Diagnostics.Debug.Assert (element.Name == "dialogData");
			
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
						field = path.NavigateReadField (data.Data);
						value = (string) dataElement.Attribute ("value");
						value = converter.ConvertBack (value, field.Type.SystemType, null, culture);
						break;
					
					case "ref":
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

		
		private Workspaces.FormWorkspace		workspace;
	}
}
