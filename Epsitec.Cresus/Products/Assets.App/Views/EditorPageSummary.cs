﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
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
			const int size = AbstractFieldController.lineHeight;

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


		public static PageType GetPageType(ObjectField field)
		{
			//	Retourne la page permettant d'éditer un champ donné.
			if (field >= ObjectField.GroupGuidRatioFirst &&
				field <= ObjectField.GroupGuidRatioLast)
			{
				return PageType.Groups;
			}

			switch (field)
			{
				case ObjectField.OneShotNumber:
				case ObjectField.OneShotDateOperation:
				case ObjectField.OneShotComment:
				case ObjectField.OneShotDocuments:
					return PageType.OneShot;

				case ObjectField.MainValue:
					return PageType.Values;

				case ObjectField.CategoryName:
				case ObjectField.AmortizationRate:
				case ObjectField.AmortizationType:
				case ObjectField.Periodicity:
				case ObjectField.Prorata:
				case ObjectField.Round:
				case ObjectField.ResidualValue:
				case ObjectField.Compte1:
				case ObjectField.Compte2:
				case ObjectField.Compte3:
				case ObjectField.Compte4:
				case ObjectField.Compte5:
				case ObjectField.Compte6:
				case ObjectField.Compte7:
				case ObjectField.Compte8:
					return PageType.Amortization;

				case ObjectField.Person1:
				case ObjectField.Person2:
				case ObjectField.Person3:
				case ObjectField.Person4:
				case ObjectField.Person5:
					return PageType.Persons;

				default:
					return PageType.Asset;
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
					new ObjectSummaryControllerTile (ObjectField.OneShotNumber),
					new ObjectSummaryControllerTile (ObjectField.OneShotDateOperation),
					new ObjectSummaryControllerTile (ObjectField.OneShotComment),
					new ObjectSummaryControllerTile (ObjectField.OneShotDocuments),

					ObjectSummaryControllerTile.Empty,

					new ObjectSummaryControllerTile ("Général"),
					new ObjectSummaryControllerTile (ObjectField.Number),
					new ObjectSummaryControllerTile (ObjectField.Name),
					new ObjectSummaryControllerTile (ObjectField.Description),

					ObjectSummaryControllerTile.Empty,

					new ObjectSummaryControllerTile ("Personnes"),
					new ObjectSummaryControllerTile (ObjectField.Person1),
					new ObjectSummaryControllerTile (ObjectField.Person2),
					new ObjectSummaryControllerTile (ObjectField.Person3),
					new ObjectSummaryControllerTile (ObjectField.Person4),
					new ObjectSummaryControllerTile (ObjectField.Person5),
				};
				list.Add (c1);

				var c2 = new List<ObjectSummaryControllerTile> ()
				{
					new ObjectSummaryControllerTile ("Regroupements"),
				};
				for (int i=0; i<=ObjectField.GroupGuidRatioLast-ObjectField.GroupGuidRatioFirst; i++)
				{
					c2.Add (new ObjectSummaryControllerTile (ObjectField.GroupGuidRatioFirst+i));
				}
				list.Add (c2);

				var c3 = new List<ObjectSummaryControllerTile> ()
				{
					new ObjectSummaryControllerTile ("Valeurs"),
					new ObjectSummaryControllerTile (ObjectField.MainValue),

					ObjectSummaryControllerTile.Empty,

					new ObjectSummaryControllerTile ("Amortissements"),
					new ObjectSummaryControllerTile (ObjectField.CategoryName),
					new ObjectSummaryControllerTile (ObjectField.AmortizationRate),
					new ObjectSummaryControllerTile (ObjectField.AmortizationType),
					new ObjectSummaryControllerTile (ObjectField.Periodicity),
					new ObjectSummaryControllerTile (ObjectField.Prorata),
					new ObjectSummaryControllerTile (ObjectField.Round),
					new ObjectSummaryControllerTile (ObjectField.ResidualValue),
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
