//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities.Helpers
{
	/// <summary>
	/// The <c>AiderPersonAdditionalContactAddressList</c> class gives access to the additional
	/// address fields defined in <see cref="AiderPersonEntity"/> as if they belonged to a list.
	/// </summary>
	public class AiderPersonAdditionalContactAddressList : ObservableList<AiderAddressEntity>, ICollectionModificationCapabilities
	{
		public AiderPersonAdditionalContactAddressList(AiderPersonEntity person)
		{
			this.person = person;
			this.AddRange (this.GetAddresses ());
		}

		
		public override void Insert(int index, AiderAddressEntity item)
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

		public override AiderAddressEntity this[int index]
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


		private IEnumerable<AiderAddressEntity> GetAddresses()
		{
			if (this.person.AdditionalAddress1.IsNotNull ())
			{
				yield return this.person.AdditionalAddress1;
			}

			if (this.person.AdditionalAddress2.IsNotNull ())
			{
				yield return this.person.AdditionalAddress2;
			}

			if (this.person.AdditionalAddress3.IsNotNull ())
			{
				yield return this.person.AdditionalAddress3;
			}

			if (this.person.AdditionalAddress4.IsNotNull ())
			{
				yield return this.person.AdditionalAddress4;
			}
		}

		private void Apply(System.Action<IList<AiderAddressEntity>> action)
		{
			var list = this.GetAddresses ().ToList ();
			action (list);
			this.ReplaceAddresses (list);
		}

		private void ReplaceAddresses(IList<AiderAddressEntity> list)
		{
			int n = list.Count;

			System.Diagnostics.Debug.Assert (n <= AiderPersonAdditionalContactAddressList.NumAddresses);

			//	Should we somehow suspend the events here in order to avoid sending
			//	notifications while the person is in a transient state ?
			
			this.person.AdditionalAddress1 = n > 0 ? list[0] : null;
			this.person.AdditionalAddress2 = n > 1 ? list[1] : null;
			this.person.AdditionalAddress3 = n > 2 ? list[2] : null;
			this.person.AdditionalAddress4 = n > 3 ? list[3] : null;
		}

		#region ICollectionModificationCapabilities Members

		bool ICollectionModificationCapabilities.CanInsert(int index)
		{
			if ((index < 0) || (index >= AiderPersonAdditionalContactAddressList.NumAddresses))
			{
				return false;
			}
			else if (this.GetAddresses ().Count () == AiderPersonAdditionalContactAddressList.NumAddresses)
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
			if ((index < 0) || (index >= this.GetAddresses ().Count ()))
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


		private const int						NumAddresses = 4;

		private readonly AiderPersonEntity		person;
	}
}
