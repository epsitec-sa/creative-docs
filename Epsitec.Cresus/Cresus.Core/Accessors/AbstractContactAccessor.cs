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
	public abstract class AbstractContactAccessor<T> : AbstractEntityAccessor<T> where T : Entities.AbstractContactEntity
	{
		protected AbstractContactAccessor(object parentEntities, T entity, bool grouped)
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


		public override void Remove()
		{
			this.ParentAbstractContacts.Remove (this.Entity);
		}



		public string Roles
		{
			get
			{
				if (this.Entity.Roles != null)
				{
					var words = new List<string> ();

					foreach (Entities.ContactRoleEntity role in this.Entity.Roles)
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
				if (this.Entity.Roles != null)
				{
					this.Entity.Roles.Clear ();

					if (!string.IsNullOrEmpty (value))
					{
						var words = Misc.Split (value.Replace (",", " "), " ");

						foreach (string word in words)
						{
							var role = new Entities.ContactRoleEntity ();
							role.Name = word;
							this.Entity.Roles.Add (role);
						}
					}
				}
			}
		}
	}
}
