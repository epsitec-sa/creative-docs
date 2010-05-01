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
	public class UriContactAccessor : AbstractContactAccessor
	{
		public UriContactAccessor(object parentEntities, AbstractEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
		}


		public Entities.UriContactEntity UriContact
		{
			get
			{
				return this.Entity as Entities.UriContactEntity;
			}
		}


		public override string IconUri
		{
			get
			{
				return "Data.Uri";
			}
		}

		public override string Title
		{
			get
			{
				if (this.Grouped)
				{
					return "Mail";
				}
				else
				{
					var builder = new StringBuilder ();

					builder.Append ("Mail");
					builder.Append (Misc.Encapsulate (" ", this.Roles, ""));

					return Misc.RemoveLastBreakLine (builder.ToString ());
				}
			}
		}

		public override string Summary
		{
			get
			{
				var builder = new StringBuilder ();

				builder.Append (this.UriContact.Uri);

				if (this.Grouped)
				{
					builder.Append (Misc.Encapsulate (" (", this.Roles, ")"));
				}

				builder.Append ("<br/>");

				return AbstractAccessor.SummaryPostprocess (builder.ToString ());
			}
		}

		public override AbstractEntity Create()
		{
			var newEntity = new Entities.UriContactEntity ();

			foreach (var role in this.UriContact.Roles)
			{
				newEntity.Roles.Add (role);
			}

			newEntity.UriScheme = this.UriContact.UriScheme;

			int index = this.ParentAbstractContacts.IndexOf (this.UriContact);
			if (index == -1)
			{
				this.ParentAbstractContacts.Add (newEntity);
			}
			else
			{
				this.ParentAbstractContacts.Insert (index+1, newEntity);
			}

			return newEntity;
		}
	}
}
