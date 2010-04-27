//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.EntitiesAccessors
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
					StringBuilder builder = new StringBuilder ();

					bool first = true;
					foreach (Entities.ContactRoleEntity role in this.AbstractContact.Roles)
					{
						if (!first)
						{
							builder.Append (", ");
						}

						builder.Append (role.Name);
						first = false;
					}

					return builder.ToString ();
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.AbstractContact.Roles == null)
				{
					this.AbstractContact.Roles.Clear ();

					if (!string.IsNullOrEmpty (value))
					{
						value = value.Replace (" ", ",");
						string[] words = value.Split (',');

						foreach (string w in words)
						{
							string word = w.Trim ();
							if (!string.IsNullOrEmpty (word))
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
}
