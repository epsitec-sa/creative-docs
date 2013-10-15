//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditor : AbstractEditor
	{
		public ObjectEditor(DataAccessor accessor)
			: base (accessor)
		{
		}

		public override void CreateUI(Widget parent)
		{
			this.editorContainer = new Scrollable
			{
				Parent                 = parent,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode   = ScrollableScrollerMode.ShowAlways,
				Dock                   = DockStyle.Fill,
			};

			this.editorContainer.Viewport.IsAutoFitting = true;
			this.editorContainer.Viewport.Padding = new Margins (20);

			this.topTitle = new TopTitle
			{
				Parent = parent,
			};
		}


		public void SetObject(Guid objectGuid, Timestamp? timestamp)
		{
			this.objectGuid = objectGuid;
			this.timestamp = timestamp;

			if (this.objectGuid.IsEmpty)
			{
				this.properties = null;
			}
			else
			{
				var ts = this.timestamp.GetValueOrDefault (new Timestamp (System.DateTime.MaxValue, 0));
				this.properties = this.accessor.GetObjectSyntheticProperties (this.objectGuid, ts);
			}

			this.topTitle.SetTitle (this.ObjectTitle);
			this.CreateEditorUI ();
		}

		private void CreateEditorUI()
		{
			this.editorContainer.Viewport.Children.Clear ();

			// brouillon :
			{
				var t = new TextField
				{
					Parent = this.editorContainer.Viewport,
					Dock = DockStyle.Top,
					PreferredWidth = 300,
				};

				if (this.properties != null)
				{
					t.Text = DataAccessor.GetStringProperty (this.properties, (int) ObjectField.Nom);
				}
			}

			for (int i=0; i<100; i++)
			{
				var t = new TextField
				{
					Parent = this.editorContainer.Viewport,
					Dock = DockStyle.Top,
					PreferredWidth = 300,
				};

				if (this.properties != null)
				{
					var m = DataAccessor.GetDecimalProperty (this.properties, (int) ObjectField.Valeur1);
					if (m.HasValue)
					{
						t.Text = m.Value.ToString ("C");
					}
				}
			}
		}


		private string ObjectTitle
		{
			//	Retourne le nom de l'objet sélectionné ainsi que la date de l'événement
			//	définissant ses propriétés.
			get
			{
				var list = new List<string> ();

				if (this.properties != null)
				{
					var nom = DataAccessor.GetStringProperty (this.properties, (int) ObjectField.Nom);
					if (!string.IsNullOrEmpty (nom))
					{
						list.Add (nom);
					}
				}

				if (this.timestamp.HasValue)
				{
					list.Add (this.timestamp.Value.Date.ToString ("dd.MM.yyyy"));
				}

				return string.Join (" — ", list);
			}
		}


		private Scrollable							editorContainer;
		private TopTitle							topTitle;
		private Guid								objectGuid;
		private Timestamp?							timestamp;
		private IEnumerable<AbstractDataProperty>	properties;
	}
}
