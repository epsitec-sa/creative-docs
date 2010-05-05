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
				return "Data.Roles";
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
				initializer.Content.Add ("commande",      "Commande");
				initializer.Content.Add ("facturation",   "Facturation");
				initializer.Content.Add ("livraison",     "Livraison");
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
					var words = new List<string> ();

					foreach (Entities.ContactRoleEntity role in this.AbstractContact.Roles)
					{
						words.Add (role.Name);
					}

					return Misc.Combine (words, ", ");
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
