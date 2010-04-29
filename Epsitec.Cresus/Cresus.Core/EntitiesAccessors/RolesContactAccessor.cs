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
	public class RolesContactAccessor : AbstractContactAccessor
	{
		public RolesContactAccessor(object parentEntities, AbstractEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
		}


		public override string IconUri
		{
			get
			{
				return "R";
			}
		}

		public override string Title
		{
			get
			{
				return "Rôles";
			}
		}

		public override string Summary
		{
			get
			{
				var builder = new StringBuilder ();

				builder.Append (this.Roles);
				builder.Append ("<br/>");

				return AbstractAccessor.SummaryPostprocess (builder.ToString ());
			}
		}


		public ComboInitializer RoleInitializer
		{
			get
			{
				ComboInitializer initializer = new ComboInitializer ();

				initializer.Content.Add ("professionnel", "Professionnel");
				initializer.Content.Add ("facturation",   "Facturation");
				initializer.Content.Add ("privé",         "Privé");

				return initializer;
			}
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

					return this.RoleInitializer.ConvertInternalToEdition(builder.ToString ());
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
						value = this.RoleInitializer.ConvertEditionToInternal (value);
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
