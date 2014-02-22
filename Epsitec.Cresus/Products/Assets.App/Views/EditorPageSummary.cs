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
				var type = EditorPageSummary.GetPageType (this.accessor, tile.Field);
				this.OnPageOpen (type, tile.Field);
			}
		}


		public static PageType GetPageType(DataAccessor accessor, ObjectField field)
		{
			//	Retourne la page permettant d'éditer un champ donné.
			if (field >= ObjectField.GroupGuidRatioFirst &&
				field <= ObjectField.GroupGuidRatioLast)
			{
				return PageType.Groups;
			}

			var userField = accessor.Settings.GetUserField (field);
			if (!userField.IsEmpty)
			{
				var baseType = accessor.Settings.GetBaseType (userField.Guid);

				if (baseType == BaseType.Assets)
				{
					if (userField.Type == FieldType.ComputedAmount)
					{
						return PageType.Values;
					}
					else if (userField.Type == FieldType.GuidPerson)
					{
						return PageType.Persons;
					}
					else
					{
						return PageType.Asset;
					}
				}
				else if (baseType == BaseType.Persons)
				{
					return PageType.Person;
				}
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

				//	Première colonne.
				var c1 = new List<ObjectSummaryControllerTile> ();
				list.Add (c1);

				c1.Add (new ObjectSummaryControllerTile ("Evénement"));

				c1.Add (new ObjectSummaryControllerTile ("Evénement"));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotNumber));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotDateOperation));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotComment));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotDocuments));

				c1.Add (ObjectSummaryControllerTile.Empty);

				c1.Add (new ObjectSummaryControllerTile ("Général"));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.Number));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.Name));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.Description));

				foreach (var userField in this.accessor.Settings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type != FieldType.ComputedAmount && x.Type != FieldType.GuidPerson))
				{
					c1.Add (new ObjectSummaryControllerTile (userField.Field));
				}

				c1.Add(ObjectSummaryControllerTile.Empty);

				c1.Add(new ObjectSummaryControllerTile ("Personnes"));

				foreach (var userField in this.accessor.Settings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type == FieldType.GuidPerson))
				{
					c1.Add (new ObjectSummaryControllerTile (userField.Field));
				}

				//	Deuxième colonne.
				var c2 = new List<ObjectSummaryControllerTile> ()
				{
					new ObjectSummaryControllerTile ("Regroupements"),
				};
				for (int i=0; i<=ObjectField.GroupGuidRatioLast-ObjectField.GroupGuidRatioFirst; i++)
				{
					c2.Add (new ObjectSummaryControllerTile (ObjectField.GroupGuidRatioFirst+i));
				}
				list.Add (c2);

				//	Troisième colonne.
				var c3 = new List<ObjectSummaryControllerTile> ();
				list.Add (c3);

				c3.Add (new ObjectSummaryControllerTile ("Valeurs"));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.MainValue));

				foreach (var userField in this.accessor.Settings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type == FieldType.ComputedAmount))
				{
					c3.Add (new ObjectSummaryControllerTile (userField.Field));
				}

				c3.Add (ObjectSummaryControllerTile.Empty);

				c3.Add (new ObjectSummaryControllerTile ("Amortissements"));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.CategoryName));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.AmortizationRate));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.AmortizationType));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Periodicity));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Prorata));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Round));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.ResidualValue));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Compte1));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Compte2));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Compte3));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Compte4));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Compte5));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Compte6));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Compte7));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Compte8));

				return list;
			}
		}


		private readonly ObjectSummaryController summaryController;

		private FrameBox commentaries;
		private FrameBox commentariesDefined;
	}
}
