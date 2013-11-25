//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class EmptyMandat
	{
		public static DataMandat GetMandat()
		{
			var mandat = new DataMandat ("Vide", new System.DateTime (2000, 1, 1), new System.DateTime (2050, 12, 31));

			EmptyMandat.AddCategories (mandat);
			EmptyMandat.AddGroups (mandat);
			EmptyMandat.AddObjects (mandat);

			return mandat;
		}

		private static void AddObjects(DataMandat mandat)
		{
		}

		private static void AddCategories(DataMandat mandat)
		{
		}

		private static void AddGroups(DataMandat mandat)
		{
			var categories = mandat.GetData (BaseType.Groups);

			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);

			var o0 = new DataObject ();
			categories.Add (o0);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o0.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (EmptyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Groupes"));
			}

			///////////////

			var oImmob = new DataObject ();
			categories.Add (oImmob);
			{
				var e = new DataEvent (start, EventType.Entrée);
				oImmob.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (EmptyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Immobilisations"));
			}

		}

		private static void AddAmortissement1(DataEvent e)
		{
			e.AddProperty (new DataStringProperty  (ObjectField.NomCatégorie1, "Bureaux"));
		}

		private static void AddAmortissement2(DataEvent e)
		{
			e.AddProperty (new DataStringProperty  (ObjectField.NomCatégorie1, "Voiture"));
		}

		private static Guid GetGroup(DataMandat mandat, string text)
		{
			var list = mandat.GetData (BaseType.Groups);

			foreach (var group in list)
			{
				var nom = ObjectCalculator.GetObjectPropertyString (group, null, ObjectField.Nom);
				if (nom == text)
				{
					return group.Guid;
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("Le groupe {0} n'existe pas !", text));
			return Guid.Empty;
		}


		private static int EventNumber = 1;
		private static int CategoryNumber = 1;
		private static int GroupNumber = 1;
	}

}