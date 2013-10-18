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


		public override IEnumerable<ObjectPageType> ChildrenPageTypes
		{
			get
			{
				yield return ObjectPageType.Infos;
				yield return ObjectPageType.Values;
				yield return ObjectPageType.Amortissements;
				yield return ObjectPageType.Compta;
			}
		}


		protected override void CreateUI(Widget parent)
		{
			this.summaryController.CreateUI (parent);
			this.summaryController.UpdateFields (this.objectGuid, this.timestamp);
		}


		private void TileClicked(int row, int column)
		{
			switch (column)
			{
				case 0:
					this.OnPageOpen (ObjectPageType.Infos);
					break;

				case 1:
					this.OnPageOpen (ObjectPageType.Values);
					break;

				case 2:
					this.OnPageOpen (ObjectPageType.Amortissements);
					break;
			}
		}


		private static List<List<int>> SummaryFields
		{
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
					(int) ObjectField.TauxAmortissement,
					(int) ObjectField.TypeAmortissement,
					(int) ObjectField.FréquenceAmortissement,
					(int) ObjectField.ValeurRésiduelle,
				};
				list.Add (c3);

				return list;
			}
		}

	
		private readonly ObjectSummaryController			summaryController;
	}
}
