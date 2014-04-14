﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageSummary : AbstractEditorPage
	{
		public EditorPageSummary(DataAccessor accessor, BaseType baseType, BaseType subBaseType, bool isTimeless)
			: base (accessor, baseType, subBaseType, isTimeless)
		{
			this.summaryController = new ObjectSummaryController (this.accessor, this.baseType);

			this.summaryController.TileClicked += delegate (object sender, int row, int column)
			{
				this.TileClicked (row, column);
			};
		}


		protected internal override void CreateUI(Widget parent)
		{
			this.CreateLockedWidgets (parent);
			this.CreateRightGrey (parent);
			this.summaryController.CreateUI (parent);
			this.CreateCommentaries (parent);
		}

		public override void SetObject(Guid objectGuid, Timestamp timestamp)
		{
			base.SetObject (objectGuid, timestamp);

			this.summaryController.SetTiles (this.SummaryTiles, this.isLocked);
			this.summaryController.UpdateFields (this.objectGuid, this.timestamp);

			this.UpdateCommentaries ();
		}


		private void CreateCommentaries(Widget parent)
		{
			const int h = AbstractFieldController.lineHeight;

			this.commentaries = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Bottom,
				PreferredHeight = h,
				Margins         = new Margins (10),
			};

			this.commentariesDefinable = new FrameBox
			{
				Parent        = this.commentaries,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (h, h),
			};

			{
				var text = "Champ pouvant être défini par cet événement";
				var width = text.GetTextWidth ();

				new StaticText
				{
					Parent        = this.commentaries,
					Text          = text,
					Dock          = DockStyle.Left,
					PreferredSize = new Size (width+20, h),
					Margins       = new Margins (10, 0, 0, 0),
				};
			}

			this.commentariesDefined = new FrameBox
			{
				Parent        = this.commentaries,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (h, h),
			};

			{
				var text = "Champ défini par cet événement";
				var width = text.GetTextWidth ();

				new StaticText
				{
					Parent        = this.commentaries,
					Text          = text,
					Dock          = DockStyle.Left,
					PreferredSize = new Size (width+20, h),
					Margins       = new Margins (10, 0, 0, 0),
				};
			}

			this.UpdateCommentaries ();
		}

		private void UpdateCommentaries()
		{
			this.commentaries.Visibility = this.hasEvent;

			var c1 = AbstractFieldController.GetBackgroundColor (PropertyState.Synthetic, this.isLocked);
			var c2 = AbstractFieldController.GetBackgroundColor (PropertyState.Single,    this.isLocked);

			this.commentariesDefinable.BackColor = c1;
			this.commentariesDefined  .BackColor = c2;
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

			var userField = accessor.GlobalSettings.GetUserField (field);
			if (!userField.IsEmpty)
			{
				var baseType = accessor.GlobalSettings.GetBaseType (userField.Guid);

				if (baseType == BaseType.Assets)
				{
					if (userField.Type == FieldType.ComputedAmount)
					{
						return PageType.Asset;
					}
					else if (userField.Type == FieldType.GuidPerson)
					{
						return PageType.Asset;
					}
					else if (userField.Type == FieldType.GuidAccount)
					{
						return PageType.AmortizationDefinition;
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
					return PageType.AmortizationValue;

				case ObjectField.CategoryName:
				case ObjectField.AmortizationRate:
				case ObjectField.AmortizationType:
				case ObjectField.Periodicity:
				case ObjectField.Prorata:
				case ObjectField.Round:
				case ObjectField.ResidualValue:
				case ObjectField.Account1:
				case ObjectField.Account2:
				case ObjectField.Account3:
				case ObjectField.Account4:
				case ObjectField.Account5:
				case ObjectField.Account6:
				case ObjectField.Account7:
				case ObjectField.Account8:
					return PageType.AmortizationDefinition;

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
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotNumber));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotDateOperation));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotComment));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotDocuments));
				c1.Add (ObjectSummaryControllerTile.Empty);

				var groups = GroupsGuidRatioLogic.GetSortedFields (this.accessor);
				if (groups.Any ())
				{
					c1.Add (new ObjectSummaryControllerTile ("Groupes"));
					foreach (var field in groups)
					{
						c1.Add (new ObjectSummaryControllerTile (field));
					}
				}

				//	Deuxième colonne.
				var c2 = new List<ObjectSummaryControllerTile> ();
				list.Add (c2);

				c2.Add (new ObjectSummaryControllerTile ("Général"));
				foreach (var userField in this.accessor.GlobalSettings.GetUserFields (BaseType.Assets))
				{
					if (userField.TopMargin >= 5)  // limite arbitraire
					{
						c2.Add (ObjectSummaryControllerTile.Empty);
					}

					c2.Add (new ObjectSummaryControllerTile (userField.Field));
				}
				c2.Add (ObjectSummaryControllerTile.Empty);

				//	Troisième colonne.
				var c3 = new List<ObjectSummaryControllerTile> ();
				list.Add (c3);

				c3.Add (new ObjectSummaryControllerTile ("Valeur comptable"));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.MainValue));
				c3.Add (ObjectSummaryControllerTile.Empty);

				c3.Add (new ObjectSummaryControllerTile ("Amortissements"));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.CategoryName));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.AmortizationRate));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.AmortizationType));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Periodicity));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Prorata));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Round));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.ResidualValue));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Account1));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Account2));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Account3));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Account4));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Account5));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Account6));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Account7));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Account8));

				return list;
			}
		}


		private readonly ObjectSummaryController summaryController;

		private FrameBox						commentaries;
		private FrameBox						commentariesDefinable;
		private FrameBox						commentariesDefined;
	}
}
