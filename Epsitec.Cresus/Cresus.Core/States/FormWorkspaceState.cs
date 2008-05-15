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

namespace Epsitec.Cresus.Core.States
{
	/// <summary>
	/// The <c>FormWorkspaceState</c> class manages the state associated with a
	/// form workspace, as implemented by the <see cref="FormWorkspace"/> class.
	/// </summary>
	public abstract class FormWorkspaceState : AbstractState
	{
		public FormWorkspaceState()
		{
			
		}


		public Workspaces.FormWorkspace			Workspace
		{
			get
			{
				return this.workspace;
			}
			set
			{
				if (this.workspace != value)
				{
					this.workspace = value;
					
					if ((this.serialization != null) &&
						(this.workspace != null))
					{
						XElement data = this.serialization;

						this.serialization = null;
						this.RestoreWorkspace (data);
					}
				}
			}
		}

		
		public override XElement Serialize(XElement element)
		{
			return element;
		}

		public override AbstractState Deserialize(XElement element)
		{
			if (this.workspace != null)
			{
				this.RestoreWorkspace (element);
			}
			else
			{
				this.serialization = element;
			}

			return this;
		}


		protected virtual void StoreWorkspace(XElement element)
		{
			if (this.workspace != null)
			{
				element.Add (new XElement ("workspace",
					new XAttribute ("entityId", this.workspace.EntityId.ToString ()),
					new XAttribute ("formId", this.workspace.FormId.ToString ())));
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
			}
		}


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
						switch (field.Relation)
						{
							case FieldRelation.None:
								element.Add (new XElement ("data",
									new XAttribute ("path", path),
									new XAttribute ("value", converter.Convert (value, typeof (string), null, culture))));
								break;

							case FieldRelation.Reference:
								element.Add (new XElement ("ref",
									new XAttribute ("path", path)));
								//	TODO: how do we serialize the reference ?
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


		private Workspaces.FormWorkspace		workspace;
		private XElement						serialization;
	}
}
