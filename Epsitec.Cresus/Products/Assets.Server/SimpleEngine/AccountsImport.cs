//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class AccountsImport : System.IDisposable
	{
		public void Dispose()
		{
		}


		public void Import(GuidList<DataObject> accounts, string filename)
		{
			//	Importe un plan comptable de Crésus Comptabilité (fichier .crp) et
			//	retourne la liste des comptes sous forme d'objets avec des propriétés.
			this.accounts = accounts;
			this.accounts.Clear ();

			this.lines = AccountsImport.ReadLines (filename);

			this.accountNumber = 1;
			this.AddAccount ("0", "Plan comptable", AccountCategory.Unknown, AccountType.Groupe);
			this.Import ();
		}


		private void Import()
		{
			//	Importe tous les comptes.
			int indexCompte = this.IndexOfLine ("BEGIN=COMPTES");

			var groups  = new Dictionary<string, string> ();
			var boucles = new Dictionary<string, string> ();

			while (++indexCompte < this.lines.Length)
			{
				var line = this.lines[indexCompte];

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
					var numéro = this.GetEntryContentText (indexCompte, "NUM");
					var titre  = this.GetEntryContentText (indexCompte, "NAME");

					if (string.IsNullOrEmpty (numéro) || numéro.Contains ("/") || string.IsNullOrEmpty (titre))
					{
						continue;
					}

					var category = this.GetEntryContentCatégorie (indexCompte, "CAT");
					var type     = this.GetEntryContentType (indexCompte, "STATUS");

					//	Il ne semble pas y avoir d'autre moyen pour savoir s'il s'agit d'un compte de TVA !
					if (type == AccountType.Normal && titre.Contains ("TVA"))
					{
						type = AccountType.TVA;
					}

					//-var niveau = this.GetEntryContentInt (lines, indexCompte, "LEVEL");
					//-if (niveau.HasValue)
					//-{
					//-	compte.Niveau = niveau.Value;
					//-}
					//-
					//-var ordre = this.GetEntryContentInt (lines, indexCompte, "ORDER");
					//-if (ordre.HasValue)
					//-{
					//-	compte.IndexOuvBoucl = ordre.Value;
					//-}

					var group = this.GetEntryContentText (indexCompte, "GROUP");
					if (!string.IsNullOrEmpty (group))
					{
						groups.Add (numéro, group);
					}

					var boucle = this.GetEntryContentText (indexCompte, "BOUCLE");
					if (!string.IsNullOrEmpty (boucle))
					{
						boucles.Add (numéro, boucle);
					}

					this.AddAccount (numéro, titre, category, type);
				}
			}

			//	Met après-coup les champs qui pointent sur des comptes.
			foreach (var item in groups)
			{
				var a1 = this.GetAccount (item.Key);
				var a2 = this.GetAccount (item.Value);

				if (a1 != null && a2 != null)
				{
					this.AddGroup (a1, a2);
				}
			}

			var root = this.GetAccount ("0");
			foreach (var a in this.accounts)
			{
				this.AddGroup (a, root);
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

		private DataObject AddAccount(string number, string name, AccountCategory category, AccountType type)
		{
			var o = new DataObject ();
			this.accounts.Add (o);
			{
				var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);
				var e = new DataEvent (start, EventType.Input);
				o.AddEvent (e);

				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (this.accountNumber++).ToString ()));
				e.AddProperty (new DataStringProperty (ObjectField.Number, number));
				e.AddProperty (new DataStringProperty (ObjectField.Name, name));
				e.AddProperty (new DataIntProperty    (ObjectField.AccountCategory, (int) category));
				e.AddProperty (new DataIntProperty    (ObjectField.AccountType, (int) type));
			}

			//?System.Console.WriteLine (number);
			return o;
		}

		private void AddGroup(DataObject account, DataObject group)
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

		private DataObject GetAccount(string number)
		{
			foreach (var account in this.accounts)
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

		private AccountCategory GetEntryContentCatégorie(int index, string key)
		{
			var value = this.GetEntryContentInt (index, key);

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

		private AccountType GetEntryContentType(int index, string key)
		{
			var value = this.GetEntryContentInt (index, key);

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

		private int? GetEntryContentInt(int index, string key)
		{
			var text = this.GetEntryContentText (index, key);

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

		private string GetEntryContentText(int index, string key)
		{
			key = key+"=";

			while (++index < this.lines.Length)
			{
				if (this.lines[index].StartsWith (key))
				{
					return this.lines[index].Substring (key.Length).Trim ();
				}

				if (this.lines[index].StartsWith ("ENTRY") ||  // est-on sur l'entrée suivante ?
					this.lines[index].StartsWith ("END="))     // fin du bloc ?
				{
					break;
				}
			}

			return null;
		}

		private int IndexOfLine(string key)
		{
			for (int i = 0; i < this.lines.Length; i++)
			{
				if (this.lines[i].StartsWith (key))
				{
					return i;
				}
			}

			return -1;
		}

		private static string[] ReadLines(string filename)
		{
			return System.IO.File.ReadAllLines (filename, System.Text.Encoding.Default);
		}


		private string[]						lines;
		private GuidList<DataObject>			accounts;
		private int								accountNumber;
	}

}