//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditorPageSummary : AbstractObjectEditorPage
	{
		public ObjectEditorPageSummary(DataAccessor accessor)
			: base (accessor)
		{
			this.summaryController = new ObjectSummaryController (this.accessor, ObjectEditorPageSummary.SummaryFields);

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
				yield return EditionObjectPageType.Compta;
			}
		}


		protected override void CreateUI(Widget parent)
		{
			this.summaryController.CreateUI (parent);
			this.summaryController.UpdateFields (this.objectGuid, this.timestamp);
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

				case ObjectField.NomCatégorie:
				case ObjectField.DateAmortissement1:
				case ObjectField.DateAmortissement2:
				case ObjectField.TauxAmortissement:
				case ObjectField.TypeAmortissement:
				case ObjectField.FréquenceAmortissement:
				case ObjectField.ValeurRésiduelle:
					return EditionObjectPageType.Amortissements;

				case ObjectField.Compte1:
				case ObjectField.Compte2:
				case ObjectField.Compte3:
				case ObjectField.Compte4:
				case ObjectField.Compte5:
				case ObjectField.Compte6:
				case ObjectField.Compte7:
				case ObjectField.Compte8:
					return EditionObjectPageType.Compta;

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

		private static List<List<int>> SummaryFields
		{
			//	Retourne la liste des champs qui peupleront la tableau.
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
					(int) ObjectField.NomCatégorie,
					(int) ObjectField.DateAmortissement1,
					(int) ObjectField.DateAmortissement2,
					(int) ObjectField.TauxAmortissement,
					(int) ObjectField.TypeAmortissement,
					(int) ObjectField.FréquenceAmortissement,
					(int) ObjectField.ValeurRésiduelle,
				};
				list.Add (c3);

				var c4 = new List<int> ()
				{
					(int) ObjectField.Compte1,
					(int) ObjectField.Compte2,
					(int) ObjectField.Compte3,
					(int) ObjectField.Compte4,
					(int) ObjectField.Compte5,
					(int) ObjectField.Compte6,
					(int) ObjectField.Compte7,
					(int) ObjectField.Compte8,
				};
				list.Add (c4);

				return list;
			}
		}

	
		private readonly ObjectSummaryController			summaryController;
	}
}
