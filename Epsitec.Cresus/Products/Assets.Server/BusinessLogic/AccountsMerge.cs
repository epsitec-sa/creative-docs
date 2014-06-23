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


		public void Merge(GuidList<DataObject> current, GuidList<DataObject> import, System.DateTime beginDate, AccountsMergeMode mode)
		{
			this.currentData = current;
			this.importData  = import;
			this.beginDate   = beginDate;
			this.mode        = mode;

			this.beginTimestamp = new Timestamp (this.beginDate, 0);

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

			foreach (var imported in this.importData)
			{
				DataObject current;
				if (this.links.TryGetValue (imported, out current))
				{
					this.Merge (current, imported);
				}
				else
				{
					// TODO: adapter ObjectField.GroupParent !
					this.currentData.Add (imported);
				}
			}
		}

		private void Merge(DataObject current, DataObject imported)
		{
			var e = current.GetEvent (0);

			//?e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (this.accountNumber++).ToString ()));
			this.MergeString (e, imported, ObjectField.Number);
			this.MergeString (e, imported, ObjectField.Name);
			this.MergeInt    (e, imported, ObjectField.AccountCategory);
			this.MergeInt    (e, imported, ObjectField.AccountType);

			var importedParent = imported;
			while (true)
			{
				var importedParentGuid = ObjectProperties.GetObjectPropertyGuid (importedParent, null, ObjectField.GroupParent);
				if (importedParentGuid.IsEmpty)
				{
					break;
				}

				importedParent = this.importData[importedParentGuid];

				DataObject currentParent;
				if (this.links.TryGetValue (importedParent, out currentParent))
				{
					e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, currentParent.Guid));
					break;
				}
			}

			// TODO: adapter ObjectField.GroupParent !
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
			return ObjectProperties.GetObjectPropertyString (account, this.beginTimestamp, this.CriterionField);
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
		private System.DateTime					beginDate;
		private Timestamp						beginTimestamp;
		private AccountsMergeMode				mode;
	}
}
