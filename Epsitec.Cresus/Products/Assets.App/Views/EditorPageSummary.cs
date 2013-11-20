//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageSummary : AbstractEditorPage
	{
		public EditorPageSummary(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
			this.summaryController = new ObjectSummaryController (this.accessor, this.baseType);

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

			this.summaryController.SetTiles (this.SummaryTiles);
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
			var tile = this.GetTile (column/2, row);

			if (!tile.IsEmpty)
			{
				var type = EditorPageSummary.GetPageType (tile.Field);
				this.OnPageOpen (type, tile.Field);
			}
		}


		public static EditionObjectPageType GetPageType(ObjectField field)
		{
			//	Retourne la page permettant d'éditer un champ donné.
			switch (field)
			{
				case ObjectField.OneShotNuméro:
				case ObjectField.OneShotDateOpération:
				case ObjectField.OneShotCommentaire:
				case ObjectField.OneShotDocuments:
					return EditionObjectPageType.OneShot;

				case ObjectField.Valeur1:
				case ObjectField.Valeur2:
				case ObjectField.Valeur3:
					return EditionObjectPageType.Values;

				case ObjectField.NomCatégorie1:
				case ObjectField.TauxAmortissement:
				case ObjectField.TypeAmortissement:
				case ObjectField.Périodicité:
				case ObjectField.ValeurRésiduelle:
				case ObjectField.Compte1:
				case ObjectField.Compte2:
				case ObjectField.Compte3:
				case ObjectField.Compte4:
				case ObjectField.Compte5:
				case ObjectField.Compte6:
				case ObjectField.Compte7:
				case ObjectField.Compte8:
					return EditionObjectPageType.Amortissements;

				case ObjectField.GroupGuid+0:
				case ObjectField.GroupGuid+1:
				case ObjectField.GroupGuid+2:
				case ObjectField.GroupGuid+3:
				case ObjectField.GroupGuid+4:
				case ObjectField.GroupGuid+5:
				case ObjectField.GroupGuid+6:
				case ObjectField.GroupGuid+7:
				case ObjectField.GroupGuid+8:
				case ObjectField.GroupGuid+9:
				case ObjectField.GroupRate+0:
				case ObjectField.GroupRate+1:
				case ObjectField.GroupRate+2:
				case ObjectField.GroupRate+3:
				case ObjectField.GroupRate+4:
				case ObjectField.GroupRate+5:
				case ObjectField.GroupRate+6:
				case ObjectField.GroupRate+7:
				case ObjectField.GroupRate+8:
				case ObjectField.GroupRate+9:
					return EditionObjectPageType.Groups;

				default:
					return EditionObjectPageType.Object;
			}
		}


		private ObjectSummaryControllerTile GetTile(int column, int row)
		{
			var fields = this.SummaryTiles;

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


		private List<List<ObjectSummaryControllerTile>> SummaryTiles
		{
			get
			{
				var list = new List<List<ObjectSummaryControllerTile>> ();

				var c1 = new List<ObjectSummaryControllerTile> ()
				{
					new ObjectSummaryControllerTile ("Evénement"),
					new ObjectSummaryControllerTile (ObjectField.OneShotNuméro),
					new ObjectSummaryControllerTile (ObjectField.OneShotDateOpération),
					new ObjectSummaryControllerTile (ObjectField.OneShotCommentaire),
					new ObjectSummaryControllerTile (ObjectField.OneShotDocuments),

					ObjectSummaryControllerTile.Empty,

					new ObjectSummaryControllerTile ("Général"),
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
					new ObjectSummaryControllerTile ("Regroupements"),
					new ObjectSummaryControllerTile (ObjectField.GroupGuid+0),
					new ObjectSummaryControllerTile (ObjectField.GroupRate+0),
					new ObjectSummaryControllerTile (ObjectField.GroupGuid+1),
					new ObjectSummaryControllerTile (ObjectField.GroupRate+1),
					new ObjectSummaryControllerTile (ObjectField.GroupGuid+2),
					new ObjectSummaryControllerTile (ObjectField.GroupRate+2),
					new ObjectSummaryControllerTile (ObjectField.GroupGuid+3),
					new ObjectSummaryControllerTile (ObjectField.GroupRate+3),
					new ObjectSummaryControllerTile (ObjectField.GroupGuid+4),
					new ObjectSummaryControllerTile (ObjectField.GroupRate+4),
					new ObjectSummaryControllerTile (ObjectField.GroupGuid+5),
					new ObjectSummaryControllerTile (ObjectField.GroupRate+5),
				};
				list.Add (c2);

				var c3 = new List<ObjectSummaryControllerTile> ()
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
					new ObjectSummaryControllerTile (ObjectField.Compte1),
					new ObjectSummaryControllerTile (ObjectField.Compte2),
					new ObjectSummaryControllerTile (ObjectField.Compte3),
					new ObjectSummaryControllerTile (ObjectField.Compte4),
					new ObjectSummaryControllerTile (ObjectField.Compte5),
					new ObjectSummaryControllerTile (ObjectField.Compte6),
					new ObjectSummaryControllerTile (ObjectField.Compte7),
					new ObjectSummaryControllerTile (ObjectField.Compte8),
				};
				list.Add (c3);

				return list;
			}
		}


		private readonly ObjectSummaryController summaryController;

		private FrameBox commentaries;
		private FrameBox commentariesDefined;
	}
}
