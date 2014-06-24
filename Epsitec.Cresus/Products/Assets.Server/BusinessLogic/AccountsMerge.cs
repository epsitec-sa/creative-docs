//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class AccountsMerge : System.IDisposable
	{
		public AccountsMerge()
		{
			this.links = new Dictionary<DataObject, DataObject> ();
		}

		public void Dispose()
		{
		}


		public void Merge(GuidList<DataObject> current, GuidList<DataObject> import, AccountsMergeMode mode)
		{
			this.currentData = current;
			this.importData  = import;
			this.mode        = mode;

			if (this.mode == AccountsMergeMode.XferAll ||
				current.Any () == false)
			{
				this.XferAll ();
			}
			else
			{
				this.Merge ();
			}
		}

		private void XferAll()
		{
			this.currentData.Clear ();

			foreach (var account in this.importData)
			{
				this.currentData.Add (account);
			}
		}

		private void Merge()
		{
			this.UpdateLinks ();

			//	On s'occupe d'abord des données brutes.
			foreach (var imported in this.importData)
			{
				DataObject current;
				if (this.links.TryGetValue (imported, out current))
				{
					this.MergeAccount (current, imported);
				}
				else
				{
					this.AddAccount (imported);
				}
			}

			//	On s'occupe ensuite de la parenté.
			foreach (var imported in this.importData)
			{
				var guid = ObjectProperties.GetObjectPropertyGuid (imported, null, ObjectField.GroupParent);
				if (guid.IsEmpty)
				{
					continue;
				}

				var importedParent = this.importData[guid];
				var currentParent = this.links[importedParent];
				var current = this.links[imported];
				var e = current.GetEvent (0);
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, currentParent.Guid));
			}
		}

		private void AddAccount(DataObject imported)
		{
			var o = new DataObject ();
			this.currentData.Add (o);
			{
				var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);
				var e = new DataEvent (start, EventType.Input);
				o.AddEvent (e);

				this.MergeAccount (o, imported);
			}

			this.links.Add (imported, o);
		}

		private void MergeAccount(DataObject current, DataObject imported)
		{
			var e = current.GetEvent (0);

			this.MergeString (e, imported, ObjectField.Number);
			this.MergeString (e, imported, ObjectField.Name);
			this.MergeInt    (e, imported, ObjectField.AccountCategory);
			this.MergeInt    (e, imported, ObjectField.AccountType);
		}

		private void MergeString(DataEvent e, DataObject imported, ObjectField field)
		{
			var value = ObjectProperties.GetObjectPropertyString (imported, null, field);
			e.AddProperty (new DataStringProperty (field, value));
		}

		private void MergeInt(DataEvent e, DataObject imported, ObjectField field)
		{
			var value = ObjectProperties.GetObjectPropertyInt (imported, null, field);
			e.AddProperty (new DataIntProperty (field, value.Value));
		}


		private void UpdateLinks()
		{
			this.links.Clear ();

			foreach (var imported in this.importData)
			{
				var current = this.SearchAccordingCriterion (imported);

				if (current != null)
				{
					this.links.Add (imported, current);
				}
			}
		}


		private DataObject SearchAccordingCriterion(DataObject imported)
		{
			var s = this.GetCriterion (imported);
			return this.currentData.Where (x => this.GetCriterion (x) == s).FirstOrDefault ();
		}

		private string GetCriterion(DataObject account)
		{
			return ObjectProperties.GetObjectPropertyString (account, null, this.CriterionField);
		}

		private ObjectField CriterionField
		{
			get
			{
				switch (this.mode)
				{
					case AccountsMergeMode.PriorityNumber:
						return ObjectField.Number;

					case AccountsMergeMode.PriorityTitle:
						return ObjectField.Name;

					default:
						throw new System.InvalidOperationException (string.Format ("Unsupported mode {0}", this.mode));
				}
			}
		}


		private readonly Dictionary<DataObject, DataObject>	links;

		private GuidList<DataObject>			currentData;
		private GuidList<DataObject>			importData;
		private AccountsMergeMode				mode;
	}
}
