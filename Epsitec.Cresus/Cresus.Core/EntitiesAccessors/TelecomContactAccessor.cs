﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.EntitiesAccessors
{
	public class TelecomContactAccessor : AbstractContactAccessor
	{
		public TelecomContactAccessor(object parentEntities, AbstractEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
		}


		public Entities.TelecomContactEntity TelecomContact
		{
			get
			{
				return this.Entity as Entities.TelecomContactEntity;
			}
		}


		public override string IconUri
		{
			get
			{
				return "Data.Telecom";
			}
		}

		public override string Title
		{
			get
			{
				if (this.Grouped)
				{
					return "Téléphone";
				}
				else
				{
					var builder = new StringBuilder ();

					builder.Append ("Téléphone");
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

				builder.Append (this.TelecomContact.Number);
				
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
			var newEntity = new Entities.TelecomContactEntity ();

			foreach (var role in this.TelecomContact.Roles)
			{
				newEntity.Roles.Add (role);
			}

			newEntity.TelecomType = this.TelecomContact.TelecomType;

			int index = this.ParentAbstractContacts.IndexOf (this.TelecomContact);
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


		public ComboInitializer RoleInitializer
		{
			get
			{
				ComboInitializer initlalizer = new ComboInitializer ();

				initlalizer.Content.Add ("professionnel", "Professionnel");
				initlalizer.Content.Add ("privé",         "Privé");

				return initlalizer;
			}
		}

		public ComboInitializer TelecomTypeInitializer
		{
			get
			{
				ComboInitializer initlalizer = new ComboInitializer ();

				initlalizer.Content.Add ("fixnet", "Fixe");
				initlalizer.Content.Add ("mobile", "Mobile");
				initlalizer.Content.Add ("fax",    "Fax");

				return initlalizer;
			}
		}


		public string TelecomType
		{
			get
			{
				if (this.TelecomContact.TelecomType != null)
				{
					return this.TelecomContact.TelecomType.Code;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.TelecomContact.TelecomType == null)
				{
					this.TelecomContact.TelecomType = new Entities.TelecomTypeEntity ();
				}

				this.TelecomContact.TelecomType.Code = value;
			}
		}
	}
}
