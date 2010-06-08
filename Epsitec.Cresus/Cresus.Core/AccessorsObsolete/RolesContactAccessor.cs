//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Accessors
{
	public class RolesContactAccessor : AbstractContactAccessor<Entities.AbstractContactEntity>
	{
		public RolesContactAccessor(object parentEntities, Entities.AbstractContactEntity entity, bool grouped)
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

		protected override string GetSummary()
		{
			var builder = new StringBuilder ();

			builder.Append (this.Roles);
			builder.Append ("<br/>");

			return builder.ToString ();
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
				if (this.Entity.Roles != null)
				{
					var words = new List<string> ();

					foreach (Entities.ContactRoleEntity role in this.Entity.Roles)
					{
						words.Add (role.Name);
					}

					return Misc.Join (", ", words);
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


		public override void WidgetInitialize(Widget widget, object unspecifiedEntitie)
		{
			var editor = widget as Widgets.ItemPicker;
			var selectedRoles = unspecifiedEntitie as IList<Entities.ContactRoleEntity>;
			var possibleRoles = Controllers.MainViewController.roles;

			foreach (var role in possibleRoles)
			{
				editor.Items.Add (null, role);
			}

			editor.ValueToDescriptionConverter = RolesContactAccessor.DetailedValueToDescriptionConverter;
			editor.CreateUI ();

			foreach (var role in selectedRoles)
			{
				int index = editor.Items.FindIndexByValue (role);

				if (index != -1)
				{
					editor.AddSelection (Enumerable.Range (index, 1));
				}
			}
		}

		private static FormattedText DetailedValueToDescriptionConverter(object value)
		{
			var entity = value as Entities.ContactRoleEntity;

			return FormattedText.FromSimpleText (entity.Name);
		}
	}
}
