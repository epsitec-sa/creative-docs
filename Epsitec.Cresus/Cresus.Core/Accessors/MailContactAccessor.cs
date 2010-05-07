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
	public class MailContactAccessor : AbstractContactAccessor<Entities.MailContactEntity>
	{
		public MailContactAccessor(object parentEntities, Entities.MailContactEntity entity, bool grouped)
			: base (parentEntities, entity, grouped)
		{
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
					builder.Append (Misc.Encapsulate (" (", this.Roles, ")"));

					return Misc.RemoveLastLineBreak (builder.ToString ());
				}
			}
		}

		protected override string GetSummary()
		{
			var builder = new StringBuilder ();

			if (this.Grouped)
			{
				bool first = true;

				if (this.Entity.Address != null && this.Entity.Address.Street != null && !string.IsNullOrEmpty (this.Entity.Address.Street.StreetName))
				{
					if (!first)
					{
						builder.Append (", ");
					}

					builder.Append (this.Entity.Address.Street.StreetName);
					first = false;
				}

				if (this.Entity.Address != null && this.Entity.Address.Location != null)
				{
					if (!first)
					{
						builder.Append (", ");
					}

					builder.Append (Misc.SpacingAppend (this.Entity.Address.Location.PostalCode, this.Entity.Address.Location.Name));
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
				if (this.Entity.Address != null && this.Entity.Address.Street != null && !string.IsNullOrEmpty (this.Entity.Address.Street.StreetName))
				{
					builder.Append (this.Entity.Address.Street.StreetName);
					builder.Append ("<br/>");
				}

				if (this.Entity.Address != null && this.Entity.Address.Location != null)
				{
					builder.Append (Misc.SpacingAppend (this.Entity.Address.Location.PostalCode, this.Entity.Address.Location.Name));
					builder.Append ("<br/>");
				}

				if (this.Entity.Address != null && this.Entity.Address.Location != null && this.Entity.Address.Location.Country != null)
				{
					builder.Append (this.Entity.Address.Location.Country.Name);
					builder.Append ("<br/>");
				}
			}

			return builder.ToString ();
		}

		public override AbstractEntity Create()
		{
			var newEntity = new Entities.MailContactEntity ();

			foreach (var role in this.Entity.Roles)
			{
				newEntity.Roles.Add (role);
			}

			int index = this.ParentAbstractContacts.IndexOf (this.Entity);
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


		public string StreetName
		{
			get
			{
				if (this.Entity.Address != null && this.Entity.Address.Street != null)
				{
					return this.Entity.Address.Street.StreetName;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.Address == null)
				{
					this.Entity.Address = new Entities.AddressEntity ();
				}

				if (this.Entity.Address.Street == null)
				{
					this.Entity.Address.Street = new Entities.StreetEntity ();
				}

				this.Entity.Address.Street.StreetName = value;
			}
		}

		public string StreetComplement
		{
			get
			{
				if (this.Entity.Address != null && this.Entity.Address.Street != null)
				{
					return this.Entity.Address.Street.Complement;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.Address == null)
				{
					this.Entity.Address = new Entities.AddressEntity ();
				}

				if (this.Entity.Address.Street == null)
				{
					this.Entity.Address.Street = new Entities.StreetEntity ();
				}

				this.Entity.Address.Street.Complement = value;
			}
		}

		public string PostBoxNumber
		{
			get
			{
				if (this.Entity.Address != null && this.Entity.Address.PostBox != null)
				{
					return this.Entity.Address.PostBox.Number;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.Address == null)
				{
					this.Entity.Address = new Entities.AddressEntity ();
				}

				if (this.Entity.Address.PostBox == null)
				{
					this.Entity.Address.PostBox = new Entities.PostBoxEntity ();
				}

				this.Entity.Address.PostBox.Number = value;
			}
		}

		public string LocationPostalCode
		{
			get
			{
				if (this.Entity.Address != null && this.Entity.Address.Location != null)
				{
					return this.Entity.Address.Location.PostalCode;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.Address == null)
				{
					this.Entity.Address = new Entities.AddressEntity ();
				}

				if (this.Entity.Address.Location == null)
				{
					this.Entity.Address.Location = new Entities.LocationEntity ();
				}

				this.Entity.Address.Location.PostalCode = value;
			}
		}

		public string LocationName
		{
			get
			{
				if (this.Entity.Address != null && this.Entity.Address.Location != null)
				{
					return this.Entity.Address.Location.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.Address == null)
				{
					this.Entity.Address = new Entities.AddressEntity ();
				}

				if (this.Entity.Address.Location == null)
				{
					this.Entity.Address.Location = new Entities.LocationEntity ();
				}

				this.Entity.Address.Location.Name = value;
			}
		}

		public string CountryName
		{
			get
			{
				if (this.Entity.Address != null && this.Entity.Address.Location != null && this.Entity.Address.Location.Country != null)
				{
					return this.Entity.Address.Location.Country.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.Address == null)
				{
					this.Entity.Address = new Entities.AddressEntity ();
				}

				if (this.Entity.Address.Location == null)
				{
					this.Entity.Address.Location = new Entities.LocationEntity ();
				}

				if (this.Entity.Address.Location.Country == null)
				{
					this.Entity.Address.Location.Country = new Entities.CountryEntity ();
				}

				this.Entity.Address.Location.Country.Name = value;
			}
		}

		public string CountryCode
		{
			get
			{
				if (this.Entity.Address != null && this.Entity.Address.Location != null && this.Entity.Address.Location.Country != null)
				{
					return this.Entity.Address.Location.Country.Code;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.Address == null)
				{
					this.Entity.Address = new Entities.AddressEntity ();
				}

				if (this.Entity.Address.Location == null)
				{
					this.Entity.Address.Location = new Entities.LocationEntity ();
				}

				if (this.Entity.Address.Location.Country == null)
				{
					this.Entity.Address.Location.Country = new Entities.CountryEntity ();
				}

				this.Entity.Address.Location.Country.Code = value;
			}
		}

		public string RegionName
		{
			get
			{
				if (this.Entity.Address != null && this.Entity.Address.Location != null && this.Entity.Address.Location.Region != null)
				{
					return this.Entity.Address.Location.Region.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.Address == null)
				{
					this.Entity.Address = new Entities.AddressEntity ();
				}

				if (this.Entity.Address.Location == null)
				{
					this.Entity.Address.Location = new Entities.LocationEntity ();
				}

				if (this.Entity.Address.Location.Region == null)
				{
					this.Entity.Address.Location.Region = new Entities.RegionEntity ();
				}

				this.Entity.Address.Location.Region.Name = value;
			}
		}

		public string RegionCode
		{
			get
			{
				if (this.Entity.Address != null && this.Entity.Address.Location != null && this.Entity.Address.Location.Region != null)
				{
					return this.Entity.Address.Location.Region.Code;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.Entity.Address == null)
				{
					this.Entity.Address = new Entities.AddressEntity ();
				}

				if (this.Entity.Address.Location == null)
				{
					this.Entity.Address.Location = new Entities.LocationEntity ();
				}

				if (this.Entity.Address.Location.Region == null)
				{
					this.Entity.Address.Location.Region = new Entities.RegionEntity ();
				}

				this.Entity.Address.Location.Region.Code = value;
			}
		}
	}
}
