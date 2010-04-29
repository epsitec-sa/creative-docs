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
	public class MailContactAccessor : AbstractContactAccessor
	{
		public MailContactAccessor(object parentEntities, AbstractEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
		}

		public Entities.MailContactEntity MailContact
		{
			get
			{
				return this.Entity as Entities.MailContactEntity;
			}
		}


		public override string IconUri
		{
			get
			{
				return "Data.Mail";
			}
		}

		public override string Title
		{
			get
			{
				if (this.Grouped)
				{
					return "Adresse";
				}
				else
				{
					var builder = new StringBuilder ();

					builder.Append ("Adresse");
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

				if (this.Grouped)
				{
					bool first = true;

					if (this.MailContact.Address != null && this.MailContact.Address.Street != null && !string.IsNullOrEmpty (this.MailContact.Address.Street.StreetName))
					{
						if (!first)
						{
							builder.Append (", ");
						}

						builder.Append (this.MailContact.Address.Street.StreetName);
						first = false;
					}

					if (this.MailContact.Address != null && this.MailContact.Address.Location != null)
					{
						if (!first)
						{
							builder.Append (", ");
						}

						builder.Append (Misc.SpacingAppend (this.MailContact.Address.Location.PostalCode, this.MailContact.Address.Location.Name));
						first = false;
					}

					if (!first)
					{
						builder.Append (" ");
					}

					builder.Append (Misc.Encapsulate ("(", this.Roles, ")"));
					first = false;
				}
				else
				{
					if (this.MailContact.Address != null && this.MailContact.Address.Street != null && !string.IsNullOrEmpty (this.MailContact.Address.Street.StreetName))
					{
						builder.Append (this.MailContact.Address.Street.StreetName);
						builder.Append ("<br/>");
					}

					if (this.MailContact.Address != null && this.MailContact.Address.Location != null)
					{
						builder.Append (Misc.SpacingAppend (this.MailContact.Address.Location.PostalCode, this.MailContact.Address.Location.Name));
						builder.Append ("<br/>");
					}

					if (this.MailContact.Address != null && this.MailContact.Address.Location != null && this.MailContact.Address.Location.Country != null)
					{
						builder.Append (this.MailContact.Address.Location.Country.Name);
						builder.Append ("<br/>");
					}
				}

				return AbstractAccessor.SummaryPostprocess (builder.ToString ());
			}
		}

		public override AbstractEntity Create()
		{
			var newEntity = new Entities.MailContactEntity ();

			foreach (var role in this.MailContact.Roles)
			{
				newEntity.Roles.Add (role);
			}

			int index = this.ParentAbstractContacts.IndexOf (this.MailContact);
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
				initlalizer.Content.Add ("facturation",   "Facturation");
				initlalizer.Content.Add ("privé",         "Privé");

				return initlalizer;
			}
		}


		public string StreetName
		{
			get
			{
				if (this.MailContact.Address != null && this.MailContact.Address.Street != null)
				{
					return this.MailContact.Address.Street.StreetName;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.MailContact.Address == null)
				{
					this.MailContact.Address = new Entities.AddressEntity ();
				}

				if (this.MailContact.Address.Street == null)
				{
					this.MailContact.Address.Street = new Entities.StreetEntity ();
				}

				this.MailContact.Address.Street.StreetName = value;
			}
		}

		public string StreetComplement
		{
			get
			{
				if (this.MailContact.Address != null && this.MailContact.Address.Street != null)
				{
					return this.MailContact.Address.Street.Complement;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.MailContact.Address == null)
				{
					this.MailContact.Address = new Entities.AddressEntity ();
				}

				if (this.MailContact.Address.Street == null)
				{
					this.MailContact.Address.Street = new Entities.StreetEntity ();
				}

				this.MailContact.Address.Street.Complement = value;
			}
		}

		public string PostBoxNumber
		{
			get
			{
				if (this.MailContact.Address != null && this.MailContact.Address.PostBox != null)
				{
					return this.MailContact.Address.PostBox.Number;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.MailContact.Address == null)
				{
					this.MailContact.Address = new Entities.AddressEntity ();
				}

				if (this.MailContact.Address.PostBox == null)
				{
					this.MailContact.Address.PostBox = new Entities.PostBoxEntity ();
				}

				this.MailContact.Address.PostBox.Number = value;
			}
		}

		public string LocationPostalCode
		{
			get
			{
				if (this.MailContact.Address != null && this.MailContact.Address.Location != null)
				{
					return this.MailContact.Address.Location.PostalCode;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.MailContact.Address == null)
				{
					this.MailContact.Address = new Entities.AddressEntity ();
				}

				if (this.MailContact.Address.Location == null)
				{
					this.MailContact.Address.Location = new Entities.LocationEntity ();
				}

				this.MailContact.Address.Location.PostalCode = value;
			}
		}

		public string LocationName
		{
			get
			{
				if (this.MailContact.Address != null && this.MailContact.Address.Location != null)
				{
					return this.MailContact.Address.Location.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.MailContact.Address == null)
				{
					this.MailContact.Address = new Entities.AddressEntity ();
				}

				if (this.MailContact.Address.Location == null)
				{
					this.MailContact.Address.Location = new Entities.LocationEntity ();
				}

				this.MailContact.Address.Location.Name = value;
			}
		}

		public string CountryName
		{
			get
			{
				if (this.MailContact.Address != null && this.MailContact.Address.Location != null && this.MailContact.Address.Location.Country != null)
				{
					return this.MailContact.Address.Location.Country.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.MailContact.Address == null)
				{
					this.MailContact.Address = new Entities.AddressEntity ();
				}

				if (this.MailContact.Address.Location == null)
				{
					this.MailContact.Address.Location = new Entities.LocationEntity ();
				}

				if (this.MailContact.Address.Location.Country == null)
				{
					this.MailContact.Address.Location.Country = new Entities.CountryEntity ();
				}

				this.MailContact.Address.Location.Country.Name = value;
			}
		}

		public string CountryCode
		{
			get
			{
				if (this.MailContact.Address != null && this.MailContact.Address.Location != null && this.MailContact.Address.Location.Country != null)
				{
					return this.MailContact.Address.Location.Country.Code;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.MailContact.Address == null)
				{
					this.MailContact.Address = new Entities.AddressEntity ();
				}

				if (this.MailContact.Address.Location == null)
				{
					this.MailContact.Address.Location = new Entities.LocationEntity ();
				}

				if (this.MailContact.Address.Location.Country == null)
				{
					this.MailContact.Address.Location.Country = new Entities.CountryEntity ();
				}

				this.MailContact.Address.Location.Country.Code = value;
			}
		}

		public string RegionName
		{
			get
			{
				if (this.MailContact.Address != null && this.MailContact.Address.Location != null && this.MailContact.Address.Location.Region != null)
				{
					return this.MailContact.Address.Location.Region.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.MailContact.Address == null)
				{
					this.MailContact.Address = new Entities.AddressEntity ();
				}

				if (this.MailContact.Address.Location == null)
				{
					this.MailContact.Address.Location = new Entities.LocationEntity ();
				}

				if (this.MailContact.Address.Location.Region == null)
				{
					this.MailContact.Address.Location.Region = new Entities.RegionEntity ();
				}

				this.MailContact.Address.Location.Region.Name = value;
			}
		}

		public string RegionCode
		{
			get
			{
				if (this.MailContact.Address != null && this.MailContact.Address.Location != null && this.MailContact.Address.Location.Region != null)
				{
					return this.MailContact.Address.Location.Region.Code;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.MailContact.Address == null)
				{
					this.MailContact.Address = new Entities.AddressEntity ();
				}

				if (this.MailContact.Address.Location == null)
				{
					this.MailContact.Address.Location = new Entities.LocationEntity ();
				}

				if (this.MailContact.Address.Location.Region == null)
				{
					this.MailContact.Address.Location.Region = new Entities.RegionEntity ();
				}

				this.MailContact.Address.Location.Region.Code = value;
			}
		}
	}
}
