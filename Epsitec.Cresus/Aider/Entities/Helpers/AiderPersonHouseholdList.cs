//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities.Helpers
{
	public class AiderPersonHouseholdList : ObservableList<AiderHouseholdEntity>, ICollectionModificationCapabilities
	{
		public AiderPersonHouseholdList(AiderPersonEntity person)
		{
			this.person = person;
			this.AddRange (this.GetHouseholds ());
		}


		public override void Insert(int index, AiderHouseholdEntity item)
		{
			if (this.Contains (item))
			{
				throw new System.InvalidOperationException ("Duplicate address");
			}

			this.Apply (list => list.Insert (index, item));
			base.Insert (index, item);
		}

		public override void RemoveAt(int index)
		{
			this.Apply (list => list.RemoveAt (index));
			base.RemoveAt (index);
		}

		public override AiderHouseholdEntity this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				this.Apply (list => list[index] = value);
				base[index] = value;
			}
		}

		public override void Clear()
		{
			this.Apply (list => list.Clear ());
			base.Clear ();
		}


		private IEnumerable<AiderHouseholdEntity> GetHouseholds()
		{
			if (this.person.Household1.IsNotNull ())
			{
				yield return this.person.Household1;
			}

			if (this.person.Household2.IsNotNull ())
			{
				yield return this.person.Household2;
			}
		}

		private void Apply(System.Action<IList<AiderHouseholdEntity>> action)
		{
			var list = this.GetHouseholds ().ToList ();
			action (list);
			this.ReplaceHouseholds (list);
		}

		private void ReplaceHouseholds(IList<AiderHouseholdEntity> list)
		{
			int n = list.Count;

			System.Diagnostics.Debug.Assert (n <= AiderPersonHouseholdList.NumHouseholds);

			//	Should we somehow suspend the events here in order to avoid sending
			//	notifications while the person is in a transient state ?

			this.person.Household1 = n > 0 ? list[0] : null;
			this.person.Household2 = n > 1 ? list[1] : null;
		}

		#region ICollectionModificationCapabilities Members

		bool ICollectionModificationCapabilities.CanInsert(int index)
		{
			if ((index < 0) || (index >= AiderPersonHouseholdList.NumHouseholds))
			{
				return false;
			}
			else if (this.GetHouseholds ().Count () == AiderPersonHouseholdList.NumHouseholds)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		bool ICollectionModificationCapabilities.CanRemove(int index)
		{
			if ((index < 0) || (index >= this.GetHouseholds ().Count ()))
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		bool ICollectionModificationCapabilities.CanBeReordered
		{
			get
			{
				return true;
			}
		}

		#endregion


		private const int						NumHouseholds = 2;

		private readonly AiderPersonEntity		person;
	}
}
