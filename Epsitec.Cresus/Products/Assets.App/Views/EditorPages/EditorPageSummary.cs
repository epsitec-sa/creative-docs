﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public class EditorPageSummary : AbstractEditorPage
	{
		public EditorPageSummary(DataAccessor accessor, CommandContext commandContext, BaseType baseType, bool isTimeless)
			: base (accessor, commandContext, baseType, isTimeless)
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
			this.CreateColorsExplanation (parent);
			this.summaryController.CreateUI (parent);
		}

		public override void SetObject(Guid objectGuid, Timestamp timestamp)
		{
			base.SetObject (objectGuid, timestamp);

			this.summaryController.SetTiles (this.SummaryTiles, this.isLocked);
			this.summaryController.UpdateFields (this.objectGuid, this.timestamp);

			this.UpdateColorsExplanation ();
		}


		protected override void UpdateColorsExplanation()
		{
			this.colorsExplanationController.ClearTypesToShow ();

			if (this.hasEvent)
			{
				this.colorsExplanationController.AddTypesToShow (this.summaryController.FieldColorTypes);
			}

			this.colorsExplanationController.Update ();
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

			if (DataAccessor.AccountFields.Where (x => x == field).Any ())
			{
					return PageType.AmortizationDefinition;
			}

			switch (field)
			{
				case ObjectField.OneShotNumber:
				case ObjectField.OneShotUser:
				case ObjectField.OneShotDateEvent:
				case ObjectField.OneShotDateOperation:
				case ObjectField.OneShotComment:
				case ObjectField.OneShotDocuments:
					return PageType.OneShot;

				case ObjectField.MainValue:
					return PageType.AmortizationValue;

				case ObjectField.CategoryName:
				case ObjectField.MethodGuid:
				case ObjectField.AmortizationRate:
				case ObjectField.AmortizationYearCount:
				case ObjectField.Periodicity:
				case ObjectField.Prorata:
				case ObjectField.Round:
				case ObjectField.ResidualValue:
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

				c1.Add (new ObjectSummaryControllerTile (Res.Strings.EditorPages.Summary.Event.ToString ()));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotNumber));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotUser));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotDateEvent));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotDateOperation));
				c1.Add (new ObjectSummaryControllerTile (ObjectField.OneShotComment));
				c1.Add (ObjectSummaryControllerTile.Empty);

				var groups = GroupsGuidRatioLogic.GetSortedFields (this.accessor);
				if (groups.Any ())
				{
					c1.Add (new ObjectSummaryControllerTile (Res.Strings.EditorPages.Summary.Groups.ToString ()));
					foreach (var field in groups)
					{
						c1.Add (new ObjectSummaryControllerTile (field));
					}
				}

				//	Deuxième colonne.
				var c2 = new List<ObjectSummaryControllerTile> ();
				list.Add (c2);

				c2.Add (new ObjectSummaryControllerTile (Res.Strings.EditorPages.Summary.Main.ToString ()));
				foreach (var userField in this.accessor.UserFieldsAccessor.GetUserFields (BaseType.AssetsUserFields))
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

				c3.Add (new ObjectSummaryControllerTile (Res.Strings.EditorPages.Summary.MainValue.ToString ()));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.MainValue));
				c3.Add (ObjectSummaryControllerTile.Empty);

				c3.Add (new ObjectSummaryControllerTile (Res.Strings.EditorPages.Summary.Amortizations.ToString ()));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.CategoryName));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.MethodGuid));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.AmortizationRate));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.AmortizationYearCount));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Periodicity));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Prorata));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.Round));
				c3.Add (new ObjectSummaryControllerTile (ObjectField.ResidualValue));

				foreach (var field in DataAccessor.AccountFields)
				{
					c3.Add (new ObjectSummaryControllerTile (field));
				}

				return list;
			}
		}


		private readonly ObjectSummaryController	summaryController;
	}
}
