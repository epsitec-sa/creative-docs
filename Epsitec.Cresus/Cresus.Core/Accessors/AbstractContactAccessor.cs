//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Accessors
{
	public class AbstractContactAccessor : AbstractAccessor
	{
		public AbstractContactAccessor(object parentEntities, AbstractEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
		}


		public IList<Entities.AbstractContactEntity> ParentAbstractContacts
		{
			get
			{
				return this.ParentEntities as IList<Entities.AbstractContactEntity>;
			}
		}

		public Entities.AbstractContactEntity AbstractContact
		{
			get
			{
				return this.Entity as Entities.AbstractContactEntity;
			}
		}


		public override void Remove()
		{
			this.ParentAbstractContacts.Remove (this.AbstractContact);
		}



		public string Roles
		{
			get
			{
				if (this.AbstractContact.Roles != null)
				{
					var words = new List<string> ();

					foreach (Entities.ContactRoleEntity role in this.AbstractContact.Roles)
					{
						words.Add (role.Name);
					}

					return Misc.Join(words, ", ");
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.AbstractContact.Roles != null)
				{
					this.AbstractContact.Roles.Clear ();

					if (!string.IsNullOrEmpty (value))
					{
						var words = Misc.Split (value.Replace (",", " "), " ");

						foreach (string word in words)
						{
							var role = new Entities.ContactRoleEntity ();
							role.Name = word;
							this.AbstractContact.Roles.Add (role);
						}
					}
				}
			}
		}
	}
}
