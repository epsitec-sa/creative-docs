//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class DummyAccounts
	{
		public static void AddAccounts(DataMandat mandat)
		{
			//	On lit un plan comptable placé dans "S:\Epsitec.Cresus\App.CresusAssets\External\Data".
			//	C'est n'importe quoi, mais ça marche.
			//	TODO: Hack à supprimer dès que possible !

			var exeRootPath = Globals.Directories.ExecutableRoot;
			var filename = System.IO.Path.Combine (exeRootPath, "External", "Data", "pme 2011.crp");
			var lines = DummyAccounts.ReadLines (filename);

			if (lines != null)
			{
				var accounts = mandat.GetData (BaseType.Accounts);

#if false
				var o = DummyAccounts.AddAccount (accounts, "0", "Plan comptable", AccountCategory.Unknown, AccountType.Groupe);
				var a = DummyAccounts.AddAccount (accounts, "1", "Blupi", AccountCategory.Unknown, AccountType.Groupe);
				var b = DummyAccounts.AddAccount (accounts, "2", "Toto", AccountCategory.Unknown, AccountType.Groupe);

				DummyAccounts.AddGroup (a, o);
				DummyAccounts.AddGroup (b, o);
#else
				DummyAccounts.AddAccount (accounts, "0", "Plan comptable", AccountCategory.Unknown, AccountType.Groupe);
				DummyAccounts.Import (lines, accounts);
#endif
			}
		}

		private static void Import(string[] lines, GuidList<DataObject> accounts)
		{
			//	Importe tous les comptes.
			int indexCompte = DummyAccounts.IndexOfLine (lines, "BEGIN=COMPTES");

			var groups  = new Dictionary<string, string> ();
			var boucles = new Dictionary<string, string> ();

			while (++indexCompte < lines.Length)
			{
				var line = lines[indexCompte];

				if (string.IsNullOrEmpty (line))
				{
					continue;
				}

				if (line.StartsWith ("END=COMPTES"))
				{
					break;
				}

				if (line.StartsWith ("ENTRY"))
				{
					var numéro = DummyAccounts.GetEntryContentText (lines, indexCompte, "NUM");
					var titre  = DummyAccounts.GetEntryContentText (lines, indexCompte, "NAME");

					if (string.IsNullOrEmpty (numéro) || numéro.Contains ("/") || string.IsNullOrEmpty (titre))
					{
						continue;
					}

					var category = DummyAccounts.GetEntryContentCatégorie (lines, indexCompte, "CAT");
					var type     = DummyAccounts.GetEntryContentType (lines, indexCompte, "STATUS");

					//	Il ne samble pas y avoir d'autre moyen pour savoir s'il s'agit d'un compte de TVA !
					if (type == AccountType.Normal && titre.Contains ("TVA"))
					{
						type = AccountType.TVA;
					}

					//-var niveau = DummyAccounts.GetEntryContentInt (lines, indexCompte, "LEVEL");
					//-if (niveau.HasValue)
					//-{
					//-	compte.Niveau = niveau.Value;
					//-}
					//-
					//-var ordre = DummyAccounts.GetEntryContentInt (lines, indexCompte, "ORDER");
					//-if (ordre.HasValue)
					//-{
					//-	compte.IndexOuvBoucl = ordre.Value;
					//-}

					var group = DummyAccounts.GetEntryContentText (lines, indexCompte, "GROUP");
					if (!string.IsNullOrEmpty (group))
					{
						groups.Add (numéro, group);
					}

					var boucle = DummyAccounts.GetEntryContentText (lines, indexCompte, "BOUCLE");
					if (!string.IsNullOrEmpty (boucle))
					{
						boucles.Add (numéro, boucle);
					}

					DummyAccounts.AddAccount (accounts, numéro, titre, category, type);
				}
			}

			//	Met après-coup les champs qui pointent sur des comptes.
			foreach (var item in groups)
			{
				var a1 = DummyAccounts.GetAccount (accounts, item.Key);
				var a2 = DummyAccounts.GetAccount (accounts, item.Value);

				if (a1 != null && a2 != null)
				{
					DummyAccounts.AddGroup (a1, a2);
				}
			}

			var root = DummyAccounts.GetAccount (accounts, "0");
			foreach (var a in accounts)
			{
				DummyAccounts.AddGroup (a, root);
			}

			foreach (var item in boucles)
			{
				//-var c1 = this.compta.PlanComptable.Where (x => x.Numéro == item.Key).FirstOrDefault ();
				//-var c2 = this.compta.PlanComptable.Where (x => x.Numéro == item.Value).FirstOrDefault ();
				//-
				//-if (c1 != null && c2 != null)
				//-{
				//-	c1.CompteOuvBoucl = c2;
				//-}
			}
		}

		private static DataObject AddAccount(GuidList<DataObject> accounts, string number, string name, AccountCategory category, AccountType type)
		{
			var o = new DataObject ();
			accounts.Add (o);
			{
				var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);
				var e = new DataEvent (start, EventType.Input);
				o.AddEvent (e);

				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyAccounts.AccountNumber++).ToString ()));
				e.AddProperty (new DataStringProperty (ObjectField.Number, number));
				e.AddProperty (new DataStringProperty (ObjectField.Name, name));
				e.AddProperty (new DataIntProperty    (ObjectField.AccountCategory, (int) category));
				e.AddProperty (new DataIntProperty    (ObjectField.AccountType, (int) type));
			}

			//?System.Console.WriteLine (number);
			return o;
		}

		private static void AddGroup(DataObject account, DataObject group)
		{
			if (account == group)
			{
				return;
			}

			var e = account.GetEvent (0);

			if (e.GetProperty (ObjectField.GroupParent) == null)
			{
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, group.Guid));
			}
		}

		private static DataObject GetAccount(GuidList<DataObject> accounts, string number)
		{
			foreach (var account in accounts)
			{
				var e = account.GetEvent (0);
				var p = e.GetProperty (ObjectField.Number) as DataStringProperty;
				if (p.Value == number)
				{
					return account;
				}
			}

			return null;
		}

		private static AccountCategory GetEntryContentCatégorie(string[] lines, int index, string key)
		{
			var value = DummyAccounts.GetEntryContentInt (lines, index, key);

			if (value.HasValue)
			{
				switch (value.Value)
				{
					case 0x02:
						return AccountCategory.Actif;
					case 0x04:
						return AccountCategory.Passif;
					case 0x08:
						return AccountCategory.Charge;
					case 0x10:
						return AccountCategory.Produit;
					case 0x20:
						return AccountCategory.Exploitation;
				}
			}

			return AccountCategory.Unknown;
		}

		private static AccountType GetEntryContentType(string[] lines, int index, string key)
		{
			var value = DummyAccounts.GetEntryContentInt (lines, index, key);

			if (value.HasValue)
			{
				int v = value.Value & 0x18;

				if (v != 0x18)
				{
					switch (v)
					{
						case 0x00:
							return AccountType.Normal;
						case 0x10:
							return AccountType.Groupe;
					}
				}
			}

			return AccountType.Normal;
		}

		private static int? GetEntryContentInt(string[] lines, int index, string key)
		{
			var text = DummyAccounts.GetEntryContentText (lines, index, key);

			if (!string.IsNullOrEmpty (text))
			{
				int value;
				if (int.TryParse (text, out value))
				{
					return value;
				}
			}

			return null;
		}

		private static string GetEntryContentText(string[] lines, int index, string key)
		{
			key = key+"=";

			while (++index < lines.Length)
			{
				if (lines[index].StartsWith (key))
				{
					return lines[index].Substring (key.Length).Trim ();
				}

				if (lines[index].StartsWith ("ENTRY") ||  // est-on sur l'entrée suivante ?
					lines[index].StartsWith ("END="))     // fin du bloc ?
				{
					break;
				}
			}

			return null;
		}

		private static int IndexOfLine(string[] lines, string key)
		{
			for (int i = 0; i < lines.Length; i++)
			{
				if (lines[i].StartsWith (key))
				{
					return i;
				}
			}

			return -1;
		}

		private static string[] ReadLines(string filename)
		{
			try
			{
				return System.IO.File.ReadAllLines (filename, System.Text.Encoding.Default);
			}
			catch
			{
				return null;
			}
		}


		private static int AccountNumber = 1;
	}

}