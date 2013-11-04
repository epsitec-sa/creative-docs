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
		public ObjectEditorPageSummary(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.summaryController = new ObjectSummaryController
			(
				this.accessor,
				ObjectEditorPageSummary.SummaryTiles
			);

			this.summaryController.TileClicked += delegate (object sender, int row, int column)
			{
				this.TileClicked (row, column);
			};
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
			var tile = ObjectEditorPageSummary.GetTile (column, row);

			if (!tile.IsEmpty)
			{
				var type = ObjectEditorPageSummary.GetPageType (tile.Field);
				this.OnPageOpen (type, tile.Field);
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
				case ObjectField.TauxAmortissement:
				case ObjectField.TypeAmortissement:
				case ObjectField.Périodicité:
				case ObjectField.ValeurRésiduelle:
					return EditionObjectPageType.Amortissements;

				default:
					return EditionObjectPageType.General;
			}
		}

		private static ObjectSummaryControllerTile GetTile(int column, int row)
		{
			var fields = ObjectEditorPageSummary.SummaryTiles;

			if (column < fields.Count)
			{
				var rows = fields[column];

				if (row < rows.Count)
				{
					return rows[row];
				}
			}

			return ObjectSummaryControllerTile.Empty;
		}

		private static List<List<ObjectSummaryControllerTile>> SummaryTiles
		{
			//	Retourne la liste des tuiles qui peupleront le tableau.
			get
			{
				var list = new List<List<ObjectSummaryControllerTile>> ();

				var c1 = new List<ObjectSummaryControllerTile> ()
				{
					new ObjectSummaryControllerTile ("Général"),
					new ObjectSummaryControllerTile (ObjectField.Level),
					new ObjectSummaryControllerTile (ObjectField.Numéro),
					new ObjectSummaryControllerTile (ObjectField.Nom),
					new ObjectSummaryControllerTile (ObjectField.Description),
					new ObjectSummaryControllerTile (ObjectField.Responsable),
					new ObjectSummaryControllerTile (ObjectField.Couleur),
					new ObjectSummaryControllerTile (ObjectField.NuméroSérie),
				};
				list.Add (c1);

				var c2 = new List<ObjectSummaryControllerTile> ()
				{
					new ObjectSummaryControllerTile ("Valeurs"),
					new ObjectSummaryControllerTile (ObjectField.Valeur1),
					new ObjectSummaryControllerTile (ObjectField.Valeur2),
					new ObjectSummaryControllerTile (ObjectField.Valeur3),

					ObjectSummaryControllerTile.Empty,

					new ObjectSummaryControllerTile ("Amortissements"),
					new ObjectSummaryControllerTile (ObjectField.NomCatégorie1),
					new ObjectSummaryControllerTile (ObjectField.TauxAmortissement),
					new ObjectSummaryControllerTile (ObjectField.TypeAmortissement),
					new ObjectSummaryControllerTile (ObjectField.Périodicité),
					new ObjectSummaryControllerTile (ObjectField.ValeurRésiduelle),
				};
				list.Add (c2);

				return list;
			}
		}

	
		private readonly ObjectSummaryController			summaryController;

		private FrameBox commentaries;
		private FrameBox commentariesDefined;
	}
}
