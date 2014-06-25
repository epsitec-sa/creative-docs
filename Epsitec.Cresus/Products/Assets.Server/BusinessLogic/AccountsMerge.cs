//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	/// <summary>
	/// Importation non interactive d'un plan comptable, supportant les modes Replace et Merge.
	/// </summary>
	public class AccountsMerge : System.IDisposable
	{
		public AccountsMerge(GuidList<DataObject> currentAccounts, GuidList<DataObject> importedAccounts, AccountsMergeMode mode)
		{
			this.currentAccounts  = currentAccounts;
			this.importedAccounts = importedAccounts;
			this.mode             = mode;

			this.todo  = new List<AccountMergeTodo> ();
			this.links = new Dictionary<DataObject, DataObject> ();

			this.UpdateLinks ();
		}

		public void Dispose()
		{
		}


		public bool								HasCurrentAccounts
		{
			//	Retourne false s'il n'y a actuellement aucun compte dans le plan comptable.
			get
			{
				return this.currentAccounts.Any ();
			}
		}

		public List<AccountMergeTodo>			Todo
		{
			//	Retourne la liste des comptes à fusionner.
			get
			{
				return this.todo;
			}
		}

		public void Do()
		{
			//	Effectue l'importation, selon la liste Todo.
			if (this.mode == AccountsMergeMode.Replace ||
				this.currentAccounts.Any () == false)
			{
				this.DoReplace ();
			}
			else
			{
				this.DoMerge ();
			}
		}


		private void DoReplace()
		{
			this.currentAccounts.Clear ();

			foreach (var account in this.importedAccounts)
			{
				this.currentAccounts.Add (account);
			}
		}

		private void DoMerge()
		{
			//	On s'occupe d'abord des données brutes.
			foreach (var todo in this.todo)
			{
				if (todo.IsAdd)
				{
					this.AddAccount (todo.ImportedAccount);
				}
				else
				{
					this.MergeAccount (todo.MergeWithAccount, todo.ImportedAccount);
				}
			}

			//	On s'occupe ensuite de la parenté.
			foreach (var todo in this.todo)
			{
				var guid = ObjectProperties.GetObjectPropertyGuid (todo.ImportedAccount, null, ObjectField.GroupParent);
				if (guid.IsEmpty)
				{
					continue;
				}

				var importedParent = this.importedAccounts[guid];
				var currentParent = this.links[importedParent];
				var current = this.links[todo.ImportedAccount];
				var e = current.GetEvent (0);
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, currentParent.Guid));
			}
		}

		private void AddAccount(DataObject imported)
		{
			var o = new DataObject ();
			this.currentAccounts.Add (o);
			{
				var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);
				var e = new DataEvent (start, EventType.Input);
				o.AddEvent (e);

				this.MergeAccount (o, imported);
			}
		}

		private void MergeAccount(DataObject current, DataObject imported)
		{
			var e = current.GetEvent (0);

			this.SetString (e, imported, ObjectField.Number);
			this.SetString (e, imported, ObjectField.Name);
			this.SetInt    (e, imported, ObjectField.AccountCategory);
			this.SetInt    (e, imported, ObjectField.AccountType);

			this.links[imported] = current;
		}

		private void SetString(DataEvent e, DataObject imported, ObjectField field)
		{
			var value = ObjectProperties.GetObjectPropertyString (imported, null, field);
			e.AddProperty (new DataStringProperty (field, value));
		}

		private void SetInt(DataEvent e, DataObject imported, ObjectField field)
		{
			var value = ObjectProperties.GetObjectPropertyInt (imported, null, field);
			e.AddProperty (new DataIntProperty (field, value.Value));
		}


		private void UpdateLinks()
		{
			this.todo.Clear ();
			this.links.Clear ();

			foreach (var imported in this.importedAccounts)
			{
				var current = this.SearchAccordingCriterion (imported);

				if (current == null)
				{
					this.todo.Add (AccountMergeTodo.NewAdd (imported));  // compte à ajouter
				}
				else
				{
					if (!this.IsEqual (current, imported))
					{
						this.todo.Add (AccountMergeTodo.NewMerge (imported, current, true));  // compte à fusionner
					}

					this.links.Add (imported, current);
				}
			}
		}

		private DataObject SearchAccordingCriterion(DataObject imported)
		{
			var s = this.GetCriterion (imported);
			return this.currentAccounts.Where (x => this.GetCriterion (x) == s).FirstOrDefault ();
		}

		private string GetCriterion(DataObject account)
		{
			return ObjectProperties.GetObjectPropertyString (account, null, ObjectField.Number);
		}


		private bool IsEqual(DataObject current, DataObject imported)
		{
			return this.IsEqualParent (current, imported)
				&& this.IsEqualString (current, imported, ObjectField.Number)
				&& this.IsEqualString (current, imported, ObjectField.Name)
				&& this.IsEqualInt    (current, imported, ObjectField.AccountCategory)
				&& this.IsEqualInt    (current, imported, ObjectField.AccountType);
		}

		private bool IsEqualParent(DataObject current, DataObject imported)
		{
			//	Vérifie si les comptes parents ont le même numéro.
			var currentGuid = ObjectProperties.GetObjectPropertyGuid (current, null, ObjectField.GroupParent);
			var currentParent = currentGuid.IsEmpty ? null : this.currentAccounts[currentGuid];

			var importedGuid = ObjectProperties.GetObjectPropertyGuid (imported, null, ObjectField.GroupParent);
			var importedParent = importedGuid.IsEmpty ? null : this.importedAccounts[importedGuid];

			return this.IsEqualString (currentParent, importedParent, ObjectField.Number);
		}

		private bool IsEqualString(DataObject current, DataObject imported, ObjectField field)
		{
			var value1 = ObjectProperties.GetObjectPropertyString (current,  null, field);
			var value2 = ObjectProperties.GetObjectPropertyString (imported, null, field);
			return value1 == value2;
		}

		private bool IsEqualInt(DataObject current, DataObject imported, ObjectField field)
		{
			var value1 = ObjectProperties.GetObjectPropertyInt (current,  null, field);
			var value2 = ObjectProperties.GetObjectPropertyInt (imported, null, field);
			return value1 == value2;
		}


		private readonly GuidList<DataObject>				currentAccounts;
		private readonly GuidList<DataObject>				importedAccounts;
		private readonly AccountsMergeMode					mode;
		private readonly List<AccountMergeTodo>				todo;
		private readonly Dictionary<DataObject, DataObject>	links;
	}
}
