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
			this.summaryController = new SummaryController (this.accessor, ObjectEditor.SummaryFields);
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

			this.tabPageSummary = new TabPage
			{
				TabTitle = "Résumé",
				Padding  = new Margins (10),
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

			this.tabBook.Items.Add (this.tabPageSummary);
			this.tabBook.Items.Add (this.tabPageInfos);
			this.tabBook.Items.Add (this.tabPageValues);
			this.tabBook.Items.Add (this.tabPageAmortissement);
			this.tabBook.Items.Add (this.tabPageCompta);

			this.tabBook.ActivePage = this.tabPageSummary;

			//	Summary.
			this.containerSummary = new Scrollable
			{
				Parent                 = this.tabPageSummary,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode   = ScrollableScrollerMode.ShowAlways,
				Dock                   = DockStyle.Fill,
			};

			this.containerSummary.Viewport.IsAutoFitting = true;

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
				this.eventType = EventType.Unknown;
				this.properties = null;
			}
			else
			{
				var ts = this.timestamp.GetValueOrDefault (new Timestamp (System.DateTime.MaxValue, 0));
				this.hasEvent = this.accessor.HasObjectEvent (this.objectGuid, ts);
				this.eventType = this.accessor.GetObjectEventType (this.objectGuid, ts).GetValueOrDefault (EventType.Unknown);
				this.properties = this.accessor.GetObjectSyntheticProperties (this.objectGuid, ts);
			}

			this.topTitle.SetTitle (this.ObjectTitle);
			this.CreateEditorUI ();
		}

		private void CreateEditorUI()
		{
			this.CreateEditorSummaryUI ();
			this.CreateEditorInfosUI ();
			this.CreateEditorValuesUI ();
			this.CreateEditorAmortissementUI ();
			this.CreateEditorComptaUI ();
		}

		private void CreateEditorSummaryUI()
		{
			this.containerSummary.Viewport.Children.Clear ();

			this.summaryController.CreateUI (this.containerSummary.Viewport);
			this.summaryController.UpdateFields (this.objectGuid, this.timestamp);
		}

		private void CreateEditorInfosUI()
		{
			this.containerInfos.Viewport.Children.Clear ();
			this.tabIndex = 1;

			if (this.properties != null)
			{
				this.CreateStringController (this.containerInfos.Viewport, ObjectField.Numéro, editWidth: 100);
				this.CreateStringController (this.containerInfos.Viewport, ObjectField.Nom);
				this.CreateStringController (this.containerInfos.Viewport, ObjectField.Description, lineCount: 5);
				this.CreateStringController (this.containerInfos.Viewport, ObjectField.Responsable);
				this.CreateStringController (this.containerInfos.Viewport, ObjectField.Couleur, editWidth: 100);
				this.CreateStringController (this.containerInfos.Viewport, ObjectField.NuméroSérie);
			}
		}

		private void CreateEditorValuesUI()
		{
			this.containerValues.Viewport.Children.Clear ();
			this.tabIndex = 1;

			if (this.properties != null)
			{
				this.CreateDecimalController (this.containerValues.Viewport, ObjectField.Valeur1);
				this.CreateDecimalController (this.containerValues.Viewport, ObjectField.Valeur2);
				this.CreateDecimalController (this.containerValues.Viewport, ObjectField.Valeur3);
			}
		}

		private void CreateEditorAmortissementUI()
		{
			this.containerAmortissement.Viewport.Children.Clear ();
			this.tabIndex = 1;

			if (this.properties != null)
			{
				this.CreateStringController  (this.containerAmortissement.Viewport, ObjectField.NomCatégorie);
				this.CreateDecimalController (this.containerAmortissement.Viewport, ObjectField.TauxAmortissement, isRate: true);
				this.CreateStringController  (this.containerAmortissement.Viewport, ObjectField.TypeAmortissement, editWidth: 100);
				this.CreateDecimalController (this.containerAmortissement.Viewport, ObjectField.FréquenceAmortissement);
				this.CreateDecimalController (this.containerAmortissement.Viewport, ObjectField.ValeurRésiduelle);
			}
		}

		private void CreateEditorComptaUI()
		{
			this.containerCompta.Viewport.Children.Clear ();
		}

		private void CreateStringController(Widget parent, ObjectField field, int editWidth = 280, int lineCount = 1)
		{
			var controller = new StringFieldController
			{
				Label         = StaticDescriptions.GetObjectFieldDescription (field),
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

		private void CreateDecimalController(Widget parent, ObjectField field, bool isRate = false)
		{
			var controller = new DecimalFieldController
			{
				Label         = StaticDescriptions.GetObjectFieldDescription (field),
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

#if false
				if (this.properties != null)
				{
					var nom = DataAccessor.GetStringProperty (this.properties, (int) ObjectField.Nom);
					if (!string.IsNullOrEmpty (nom))
					{
						list.Add (nom);
					}
				}
#endif

				if (this.timestamp.HasValue)
				{
					list.Add (this.timestamp.Value.Date.ToString ("dd.MM.yyyy"));
				}

				var ed = StaticDescriptions.GetEventDescription (this.eventType);
				if (!string.IsNullOrEmpty (ed))
				{
					list.Add (ed);
				}

				return string.Join (" — ", list);
			}
		}

		private static List<List<int>> SummaryFields
		{
			get
			{
				var list = new List<List<int>>();

				var c1 = new List<int> ()
				{
					(int) ObjectField.Level,
					(int) ObjectField.Numéro,
					(int) ObjectField.Nom,
					(int) ObjectField.Description,
					(int) ObjectField.Responsable,
					(int) ObjectField.Couleur,
					(int) ObjectField.NuméroSérie,
				};
				list.Add (c1);

				var c2 = new List<int> ()
				{
					(int) ObjectField.Valeur1,
					(int) ObjectField.Valeur2,
					(int) ObjectField.Valeur3,
				};
				list.Add (c2);

				var c3 = new List<int> ()
				{
					(int) ObjectField.NomCatégorie,
					(int) ObjectField.TauxAmortissement,
					(int) ObjectField.TypeAmortissement,
					(int) ObjectField.FréquenceAmortissement,
					(int) ObjectField.ValeurRésiduelle,
				};
				list.Add (c3);

				return list;
			}
		}


		private readonly SummaryController			summaryController;

		private TabBook								tabBook;

		private TabPage								tabPageSummary;
		private TabPage								tabPageInfos;
		private TabPage								tabPageValues;
		private TabPage								tabPageAmortissement;
		private TabPage								tabPageCompta;

		private Scrollable							containerSummary;
		private Scrollable							containerInfos;
		private Scrollable							containerValues;
		private Scrollable							containerAmortissement;
		private Scrollable							containerCompta;

		private TopTitle							topTitle;
		private Guid								objectGuid;
		private Timestamp?							timestamp;
		private bool								hasEvent;
		private EventType							eventType;
		private IEnumerable<AbstractDataProperty>	properties;
		private int									tabIndex;
	}
}
