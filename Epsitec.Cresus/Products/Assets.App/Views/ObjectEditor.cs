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
			parent.BackColor = ColorManager.EditBackgroundColor;

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

			this.tabPageAmortissement = new TabPage
			{
				TabTitle = "Amortissement",
				Padding  = new Margins (10),
			};

			this.tabPageCompta = new TabPage
			{
				TabTitle = "Comptabilisation",
				Padding  = new Margins (10),
			};

			this.tabBook.Items.Add (this.tabPageInfos);
			this.tabBook.Items.Add (this.tabPageValues);
			this.tabBook.Items.Add (this.tabPageAmortissement);
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

			//	Amortissement.
			this.containerAmortissement = new Scrollable
			{
				Parent                 = this.tabPageAmortissement,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode   = ScrollableScrollerMode.ShowAlways,
				Dock                   = DockStyle.Fill,
			};

			this.containerAmortissement.Viewport.IsAutoFitting = true;

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
				this.hasEvent = false;
				this.properties = null;
			}
			else
			{
				var ts = this.timestamp.GetValueOrDefault (new Timestamp (System.DateTime.MaxValue, 0));
				this.hasEvent = this.accessor.HasObjectEvent (this.objectGuid, ts);
				this.properties = this.accessor.GetObjectSyntheticProperties (this.objectGuid, ts);
			}

			this.topTitle.SetTitle (this.ObjectTitle);
			this.CreateEditorUI ();
		}

		private void CreateEditorUI()
		{
			this.CreateEditorInfosUI ();
			this.CreateEditorValuesUI ();
			this.CreateEditorAmortissementUI ();
			this.CreateEditorComptaUI ();
		}

		private void CreateEditorInfosUI()
		{
			this.containerInfos.Viewport.Children.Clear ();
			this.tabIndex = 1;

			if (this.properties != null)
			{
				this.CreateStringController (this.containerInfos.Viewport, ObjectField.Numéro,      "Numéro", editWidth: 100);
				this.CreateStringController (this.containerInfos.Viewport, ObjectField.Nom,         "Nom");
				this.CreateStringController (this.containerInfos.Viewport, ObjectField.Description, "Description", lineCount: 5);
				this.CreateStringController (this.containerInfos.Viewport, ObjectField.Responsable, "Responsable");
				this.CreateStringController (this.containerInfos.Viewport, ObjectField.Couleur,     "Couleur", editWidth: 100);
				this.CreateStringController (this.containerInfos.Viewport, ObjectField.NuméroSérie, "Numéro de série");
			}
		}

		private void CreateEditorValuesUI()
		{
			this.containerValues.Viewport.Children.Clear ();
			this.tabIndex = 1;

			if (this.properties != null)
			{
				this.CreateDecimalController (this.containerValues.Viewport, ObjectField.Valeur1, "Valeur comptable");
				this.CreateDecimalController (this.containerValues.Viewport, ObjectField.Valeur2, "Valeur d'assurance");
				this.CreateDecimalController (this.containerValues.Viewport, ObjectField.Valeur3, "Valeur imposable");
			}
		}

		private void CreateEditorAmortissementUI()
		{
			this.containerAmortissement.Viewport.Children.Clear ();
			this.tabIndex = 1;

			if (this.properties != null)
			{
				this.CreateStringController  (this.containerAmortissement.Viewport, ObjectField.NomCatégorie,           "Nom de la catégorie");
				this.CreateDecimalController (this.containerAmortissement.Viewport, ObjectField.TauxAmortissement,      "Taux", isRate: true);
				this.CreateStringController  (this.containerAmortissement.Viewport, ObjectField.TypeAmortissement,      "Type", editWidth: 100);
				this.CreateDecimalController (this.containerAmortissement.Viewport, ObjectField.FréquenceAmortissement, "Fréquence");
				this.CreateDecimalController (this.containerAmortissement.Viewport, ObjectField.ValeurRésiduelle,       "Valeur résiduelle");
			}
		}

		private void CreateEditorComptaUI()
		{
			this.containerCompta.Viewport.Children.Clear ();
		}

		private void CreateStringController(Widget parent, ObjectField field, string label, int editWidth = 280, int lineCount = 1)
		{
			var controller = new StringFieldController
			{
				Label         = label,
				Value         = DataAccessor.GetStringProperty (this.properties, (int) field),
				PropertyState = this.GetPropertyState (field),
				EditWidth     = editWidth,
				LineCount     = lineCount,
				TabIndex      = this.tabIndex++,
			};

			controller.CreateUI (parent);

			controller.ValueChanged += delegate
			{
			};
		}

		private void CreateDecimalController(Widget parent, ObjectField field, string label, bool isRate = false)
		{
			var controller = new DecimalFieldController
			{
				Label         = label,
				Value         = DataAccessor.GetDecimalProperty (this.properties, (int) field),
				PropertyState = this.GetPropertyState (field),
				IsRate        = isRate,
				TabIndex      = this.tabIndex++,
			};

			controller.CreateUI (parent);

			controller.ValueChanged += delegate
			{
			};
		}

		private PropertyState GetPropertyState(ObjectField field)
		{
			if (this.hasEvent)
			{
				return DataAccessor.GetPropertyState (this.properties, (int) field);
			}
			else
			{
				return PropertyState.Readonly;
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
		private TabPage								tabPageAmortissement;
		private TabPage								tabPageCompta;
		private Scrollable							containerInfos;
		private Scrollable							containerValues;
		private Scrollable							containerAmortissement;
		private Scrollable							containerCompta;
		private TopTitle							topTitle;
		private Guid								objectGuid;
		private Timestamp?							timestamp;
		private bool								hasEvent;
		private IEnumerable<AbstractDataProperty>	properties;
		private int									tabIndex;
	}
}
