//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	public class MailContactViewController : EntityViewController
	{
		public MailContactViewController(string name)
			: base (name)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.container = container;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			this.mailContact = this.Entity as Entities.MailContactEntity;
			System.Diagnostics.Debug.Assert (this.mailContact != null);

			FrameBox frame = this.CreateEditionTile (this.Entity, ViewControllerMode.None, EntitySummary.GetIcon (this.mailContact), EntitySummary.GetTitle (this.mailContact));
			FrameBox group;

			this.CreateTextField (frame, "Rue", this.StreetName, x => this.StreetName = x, Validators.StringValidator.Validate);
			this.CreateTextFieldMulti (frame, "Complément de l'adresse", 52, this.StreetComplement, x => this.StreetComplement = x, null);
			this.CreateTextField (frame, "Boîte postale", this.PostBoxNumber, x => this.PostBoxNumber = x, Validators.StringValidator.Validate);

			group = this.CreateGroup (frame, "Numéro postal et ville");
			this.CreateTextField (group, 50, this.LocationPostalCode, x => this.LocationPostalCode = x, Validators.StringValidator.Validate);
			this.CreateTextField (group, 0, this.LocationName, x => this.LocationName = x, Validators.StringValidator.Validate);

			group = this.CreateGroup (frame, "Code et nom du pays");
			this.CreateTextField (group, 50, this.CountryCode, x => this.CountryCode = x, Validators.StringValidator.Validate);
			this.CreateTextField (group, 0, this.CountryName, x => this.CountryName = x, Validators.StringValidator.Validate);

			group = this.CreateGroup (frame, "Code et nom de la région");
			this.CreateTextField (group, 50, this.RegionCode, x => this.RegionCode = x, Validators.StringValidator.Validate);
			this.CreateTextField (group, 0, this.RegionName, x => this.RegionName = x, Validators.StringValidator.Validate);
		}



		private string StreetName
		{
			get
			{
				if (this.mailContact.Address != null && this.mailContact.Address.Street != null)
				{
					return this.mailContact.Address.Street.StreetName;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.mailContact.Address == null)
				{
					this.mailContact.Address = new Entities.AddressEntity ();
				}

				if (this.mailContact.Address.Street == null)
				{
					this.mailContact.Address.Street = new Entities.StreetEntity ();
				}

				this.mailContact.Address.Street.StreetName = value;
			}
		}

		private string StreetComplement
		{
			get
			{
				if (this.mailContact.Address != null && this.mailContact.Address.Street != null)
				{
					return this.mailContact.Address.Street.Complement;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.mailContact.Address == null)
				{
					this.mailContact.Address = new Entities.AddressEntity ();
				}

				if (this.mailContact.Address.Street == null)
				{
					this.mailContact.Address.Street = new Entities.StreetEntity ();
				}

				this.mailContact.Address.Street.Complement = value;
			}
		}

		private string PostBoxNumber
		{
			get
			{
				if (this.mailContact.Address != null && this.mailContact.Address.PostBox != null)
				{
					return this.mailContact.Address.PostBox.Number;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.mailContact.Address == null)
				{
					this.mailContact.Address = new Entities.AddressEntity ();
				}

				if (this.mailContact.Address.PostBox == null)
				{
					this.mailContact.Address.PostBox = new Entities.PostBoxEntity ();
				}

				this.mailContact.Address.PostBox.Number = value;
			}
		}

		private string LocationPostalCode
		{
			get
			{
				if (this.mailContact.Address != null && this.mailContact.Address.Location != null)
				{
					return this.mailContact.Address.Location.PostalCode;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.mailContact.Address == null)
				{
					this.mailContact.Address = new Entities.AddressEntity ();
				}

				if (this.mailContact.Address.Location == null)
				{
					this.mailContact.Address.Location = new Entities.LocationEntity ();
				}

				this.mailContact.Address.Location.PostalCode = value;
			}
		}

		private string LocationName
		{
			get
			{
				if (this.mailContact.Address != null && this.mailContact.Address.Location != null)
				{
					return this.mailContact.Address.Location.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.mailContact.Address == null)
				{
					this.mailContact.Address = new Entities.AddressEntity ();
				}

				if (this.mailContact.Address.Location == null)
				{
					this.mailContact.Address.Location = new Entities.LocationEntity ();
				}

				this.mailContact.Address.Location.Name = value;
			}
		}

		private string CountryName
		{
			get
			{
				if (this.mailContact.Address != null && this.mailContact.Address.Location != null && this.mailContact.Address.Location.Country != null)
				{
					return this.mailContact.Address.Location.Country.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.mailContact.Address == null)
				{
					this.mailContact.Address = new Entities.AddressEntity ();
				}

				if (this.mailContact.Address.Location == null)
				{
					this.mailContact.Address.Location = new Entities.LocationEntity ();
				}

				if (this.mailContact.Address.Location.Country == null)
				{
					this.mailContact.Address.Location.Country = new Entities.CountryEntity ();
				}

				this.mailContact.Address.Location.Country.Name = value;
			}
		}

		private string CountryCode
		{
			get
			{
				if (this.mailContact.Address != null && this.mailContact.Address.Location != null && this.mailContact.Address.Location.Country != null)
				{
					return this.mailContact.Address.Location.Country.Code;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.mailContact.Address == null)
				{
					this.mailContact.Address = new Entities.AddressEntity ();
				}

				if (this.mailContact.Address.Location == null)
				{
					this.mailContact.Address.Location = new Entities.LocationEntity ();
				}

				if (this.mailContact.Address.Location.Country == null)
				{
					this.mailContact.Address.Location.Country = new Entities.CountryEntity ();
				}

				this.mailContact.Address.Location.Country.Code = value;
			}
		}

		private string RegionName
		{
			get
			{
				if (this.mailContact.Address != null && this.mailContact.Address.Location != null && this.mailContact.Address.Location.Region != null)
				{
					return this.mailContact.Address.Location.Region.Name;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.mailContact.Address == null)
				{
					this.mailContact.Address = new Entities.AddressEntity ();
				}

				if (this.mailContact.Address.Location == null)
				{
					this.mailContact.Address.Location = new Entities.LocationEntity ();
				}

				if (this.mailContact.Address.Location.Region == null)
				{
					this.mailContact.Address.Location.Region = new Entities.RegionEntity ();
				}

				this.mailContact.Address.Location.Region.Name = value;
			}
		}

		private string RegionCode
		{
			get
			{
				if (this.mailContact.Address != null && this.mailContact.Address.Location != null && this.mailContact.Address.Location.Region != null)
				{
					return this.mailContact.Address.Location.Region.Code;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (this.mailContact.Address == null)
				{
					this.mailContact.Address = new Entities.AddressEntity ();
				}

				if (this.mailContact.Address.Location == null)
				{
					this.mailContact.Address.Location = new Entities.LocationEntity ();
				}

				if (this.mailContact.Address.Location.Region == null)
				{
					this.mailContact.Address.Location.Region = new Entities.RegionEntity ();
				}

				this.mailContact.Address.Location.Region.Code = value;
			}
		}


		private Entities.MailContactEntity mailContact;
	}
}
