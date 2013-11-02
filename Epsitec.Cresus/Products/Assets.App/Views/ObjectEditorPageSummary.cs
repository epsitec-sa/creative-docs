//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditorPageSummary : AbstractObjectEditorPage
	{
		public ObjectEditorPageSummary(DataAccessor accessor)
			: base (accessor)
		{
			this.baseType = BaseType.Objects;

			this.summaryController = new ObjectSummaryController
			(
				this.accessor,
				ObjectEditorPageSummary.SummaryColumns,
				ObjectEditorPageSummary.SummaryFields
			);

			this.summaryController.TileClicked += delegate (object sender, int row, int column)
			{
				this.TileClicked (row, column);
			};
		}


		public override IEnumerable<EditionObjectPageType> ChildrenPageTypes
		{
			get
			{
				yield return EditionObjectPageType.General;
				yield return EditionObjectPageType.Values;
				yield return EditionObjectPageType.Amortissements;
			}
		}


		public override void CreateUI(Widget parent)
		{
			this.summaryController.CreateUI (parent);
			this.CreateCommentaries (parent);
		}

		public override void SetObject(Guid objectGuid, Timestamp timestamp)
		{
			base.SetObject (objectGuid, timestamp);

			this.summaryController.UpdateFields (this.objectGuid, this.timestamp);

			this.UpdateCommentaries ();
		}


		private void CreateCommentaries(Widget parent)
		{
			const int size = 17;

			this.commentaries = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Bottom,
				PreferredHeight = size,
			};

			new FrameBox
			{
				Parent        = this.commentaries,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (size, size),
				BackColor     = ColorManager.NormalFieldColor,
			};

			new StaticText
			{
				Parent        = this.commentaries,
				Text          = "Champ pouvant être défini par cet événement",
				Dock          = DockStyle.Left,
				PreferredSize = new Size (250, size),
				Margins       = new Margins (10, 0, 0, 0),
			};

			this.commentariesDefined = new FrameBox
			{
				Parent        = this.commentaries,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (size, size),
			};

			new StaticText
			{
				Parent        = this.commentaries,
				Text          = "Champ défini par cet événement",
				Dock          = DockStyle.Left,
				PreferredSize = new Size (200, size),
				Margins       = new Margins (10, 0, 0, 0),
			};

			this.UpdateCommentaries ();
		}

		private void UpdateCommentaries()
		{
			this.commentaries.Visibility = this.hasEvent;
			this.commentariesDefined.BackColor = ColorManager.GetEditSinglePropertyColor (DataAccessor.Simulation);
		}


		private void TileClicked(int row, int column)
		{
			int? field = ObjectEditorPageSummary.GetField (column, row);

			if (field.HasValue)
			{
				var type = ObjectEditorPageSummary.GetPageType ((ObjectField) field.Value);
				this.OnPageOpen (type, (ObjectField) field);
			}
		}


		public static EditionObjectPageType GetPageType(ObjectField field)
		{
			//	Retourne la page permettant d'éditer un champ donné.
			switch (field)
			{
				case ObjectField.Valeur1:
				case ObjectField.Valeur2:
				case ObjectField.Valeur3:
					return EditionObjectPageType.Values;

				case ObjectField.NomCatégorie1:
				case ObjectField.NomCatégorie2:
				case ObjectField.NomCatégorie3:
					return EditionObjectPageType.Amortissements;

				default:
					return EditionObjectPageType.General;
			}
		}

		private static int? GetField(int column, int row)
		{
			var fields = ObjectEditorPageSummary.SummaryFields;

			if (column < fields.Count)
			{
				var rows = fields[column];

				if (row < rows.Count)
				{
					return rows[row];
				}
			}

			return null;
		}

		private static List<string> SummaryColumns
		{
			get
			{
				var list = new List<string> ();

				list.Add ("Général");
				list.Add ("Valeurs");
				list.Add ("Amortissements");

				return list;
			}
		}

		private static List<List<int>> SummaryFields
		{
			//	Retourne la liste des champs qui peupleront le tableau.
			get
			{
				var list = new List<List<int>> ();

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
					(int) ObjectField.NomCatégorie1,
					(int) ObjectField.NomCatégorie2,
					(int) ObjectField.NomCatégorie3,
				};
				list.Add (c3);

				return list;
			}
		}

	
		private readonly ObjectSummaryController			summaryController;

		private FrameBox commentaries;
		private FrameBox commentariesDefined;
	}
}
