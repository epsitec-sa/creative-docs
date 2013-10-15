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
			parent.BackColor = Color.FromBrightness (0.5);

			this.tabBook = new TabBook
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Arrows  = TabBookArrows.Stretch,
				Margins = new Margins (10),
			};

			this.tabPageInfos = new TabPage
			{
				TabTitle = "Général",
				Padding  = new Margins (10),
			};

			this.tabPageValues = new TabPage
			{
				TabTitle = "Valeurs",
				Padding  = new Margins (10),
			};

			this.tabPageCompta = new TabPage
			{
				TabTitle = "Comptabilisation",
				Padding  = new Margins (10),
			};

			this.tabBook.Items.Add (this.tabPageInfos);
			this.tabBook.Items.Add (this.tabPageValues);
			this.tabBook.Items.Add (this.tabPageCompta);

			this.tabBook.ActivePage = this.tabPageInfos;

			//	Infos.
			this.containerInfos = new Scrollable
			{
				Parent                 = this.tabPageInfos,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode   = ScrollableScrollerMode.ShowAlways,
				Dock                   = DockStyle.Fill,
			};

			this.containerInfos.Viewport.IsAutoFitting = true;

			//	Values.
			this.containerValues = new Scrollable
			{
				Parent                 = this.tabPageValues,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode   = ScrollableScrollerMode.ShowAlways,
				Dock                   = DockStyle.Fill,
			};

			this.containerValues.Viewport.IsAutoFitting = true;

			//	Compta.
			this.containerCompta = new Scrollable
			{
				Parent                 = this.tabPageCompta,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode   = ScrollableScrollerMode.ShowAlways,
				Dock                   = DockStyle.Fill,
			};

			this.containerCompta.Viewport.IsAutoFitting = true;

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
			this.containerInfos.Viewport.Children.Clear ();

			// brouillon :
			{
				var t = new TextField
				{
					Parent = this.containerInfos.Viewport,
					Dock = DockStyle.Top,
					PreferredWidth = 300,
				};

				if (this.properties != null)
				{
					t.Text = DataAccessor.GetStringProperty (this.properties, (int) ObjectField.Nom);
				}
			}

			for (int i=0; i<1; i++)
			{
				var t = new TextField
				{
					Parent = this.containerInfos.Viewport,
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


		private TabBook								tabBook;
		private TabPage								tabPageInfos;
		private TabPage								tabPageValues;
		private TabPage								tabPageCompta;
		private Scrollable							containerInfos;
		private Scrollable							containerValues;
		private Scrollable							containerCompta;
		private TopTitle							topTitle;
		private Guid								objectGuid;
		private Timestamp?							timestamp;
		private IEnumerable<AbstractDataProperty>	properties;
	}
}
