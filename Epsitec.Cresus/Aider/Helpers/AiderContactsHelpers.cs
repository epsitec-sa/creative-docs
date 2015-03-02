//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Enumerations;

namespace Epsitec.Aider.Helpers
{
	public sealed class AiderContactsHelpers
	{
		public static string GetMainPhone(IEnumerable<AiderContactEntity> contacts)
		{
			if ((contacts == null) || (contacts.IsEmpty ()))
			{
				return "";
			}

			var professionalAddress = contacts.Where (c => c.AddressType == AddressType.Professional).FirstOrDefault ();
			if (professionalAddress != null)
			{
				if (!professionalAddress.Address.Mobile.IsNullOrWhiteSpace ())
				{
					return professionalAddress.Address.Mobile;
				}

				if (!professionalAddress.Address.Phone1.IsNullOrWhiteSpace ())
				{
					return professionalAddress.Address.Phone1;
				}
			}

			var otherAddress = contacts.Where (c => c.AddressType == AddressType.Other).FirstOrDefault ();
			if (otherAddress != null)
			{
				if (!otherAddress.Address.Mobile.IsNullOrWhiteSpace ())
				{
					return otherAddress.Address.Mobile;
				}

				if (!otherAddress.Address.Phone1.IsNullOrWhiteSpace ())
				{
					return otherAddress.Address.Phone1;
				}
			}

			var secondaryAddress = contacts.Where (c => c.AddressType == AddressType.Secondary).FirstOrDefault ();
			if (secondaryAddress != null)
			{
				if (!secondaryAddress.Address.Mobile.IsNullOrWhiteSpace ())
				{
					return secondaryAddress.Address.Mobile;
				}

				if (!secondaryAddress.Address.Phone1.IsNullOrWhiteSpace ())
				{
					return secondaryAddress.Address.Phone1;
				}
			}

			var defaultAddress = contacts.Where (c => c.AddressType == AddressType.Default).FirstOrDefault ();
			if (defaultAddress != null)
			{
				if (!defaultAddress.Address.Mobile.IsNullOrWhiteSpace ())
				{
					return defaultAddress.Address.Mobile;
				}

				if (!defaultAddress.Address.Phone1.IsNullOrWhiteSpace ())
				{
					return defaultAddress.Address.Phone1;
				}
			}

			return "";
		}

		public static string GetMainEmail(IEnumerable<AiderContactEntity> contacts)
		{
			if ((contacts == null) || (contacts.IsEmpty ()))
			{
				return "";
			}

			var professionalAddress = contacts.Where (c => c.AddressType == AddressType.Professional).FirstOrDefault ();
			if (professionalAddress != null)
			{
				if (!professionalAddress.Address.Email.IsNullOrWhiteSpace ())
				{
					return professionalAddress.Address.Email;
				}
			}

			var otherAddress = contacts.Where (c => c.AddressType == AddressType.Other).FirstOrDefault ();
			if (otherAddress != null)
			{
				if (!otherAddress.Address.Email.IsNullOrWhiteSpace ())
				{
					return otherAddress.Address.Email;
				}
			}

			var secondaryAddress = contacts.Where (c => c.AddressType == AddressType.Secondary).FirstOrDefault ();
			if (secondaryAddress != null)
			{
				if (!secondaryAddress.Address.Email.IsNullOrWhiteSpace ())
				{
					return secondaryAddress.Address.Email;
				}
			}

			var defaultAddress = contacts.Where (c => c.AddressType == AddressType.Default).FirstOrDefault ();
			if (defaultAddress != null)
			{
				if (!defaultAddress.Address.Email.IsNullOrWhiteSpace ())
				{
					return defaultAddress.Address.Email;
				}
			}

			return "";
		}


		public static string GetSecondaryPhone(IEnumerable<AiderContactEntity> contacts)
		{
			if ((contacts == null) || (contacts.IsEmpty ()))
			{
				return "";
			}

			var professionalPhones = contacts.Where (c => c.AddressType == AddressType.Professional);
			if (professionalPhones != null)
			{
				//reverse look in professionals phones collection
				for (var e=professionalPhones.Count ()-1; e>=0; e--)
				{
					if (!professionalPhones.ElementAt (e).Address.Mobile.IsNullOrWhiteSpace ())
					{
						if (professionalPhones.ElementAt (e).Address.Mobile != AiderContactsHelpers.GetMainPhone (contacts))
						{
							return professionalPhones.ElementAt (e).Address.Mobile;
						}
					}

					if (!professionalPhones.ElementAt (e).Address.Phone1.IsNullOrWhiteSpace ())
					{
						if (professionalPhones.ElementAt (e).Address.Phone1 != AiderContactsHelpers.GetMainPhone (contacts))
						{
							return professionalPhones.ElementAt (e).Address.Phone1;
						}
					}
				}

				//second pass for phone 2 in normal order
				for (var e=0; e<professionalPhones.Count (); e++)
				{
					if (!professionalPhones.ElementAt (e).Address.Phone2.IsNullOrWhiteSpace ())
					{
						return professionalPhones.ElementAt (e).Address.Phone2;
					}
				}

			}

			var otherAddresses = contacts.Where (c => c.AddressType == AddressType.Other);
			if (otherAddresses.Any ())
			{
				//reverse look in others emails collection
				for (var e=otherAddresses.Count ()-1; e>=0; e--)
				{
					if (!otherAddresses.ElementAt (e).Address.Mobile.IsNullOrWhiteSpace ())
					{
						if (otherAddresses.ElementAt (e).Address.Mobile != AiderContactsHelpers.GetMainPhone (contacts))
						{
							return otherAddresses.ElementAt (e).Address.Mobile;
						}
					}

					if (!otherAddresses.ElementAt (e).Address.Phone1.IsNullOrWhiteSpace ())
					{
						if (otherAddresses.ElementAt (e).Address.Phone1 != AiderContactsHelpers.GetMainPhone (contacts))
						{
							return otherAddresses.ElementAt (e).Address.Phone1;
						}
					}
				}

				//second pass for phone 2 in normal order
				for (var e=0; e<otherAddresses.Count (); e++)
				{
					if (!otherAddresses.ElementAt (e).Address.Phone2.IsNullOrWhiteSpace ())
					{
						return otherAddresses.ElementAt (e).Address.Phone2;
					}
				}
			}

			var secondaryAddress = contacts.Where (c => c.AddressType == AddressType.Secondary);
			if (secondaryAddress.Any ())
			{
				//reverse look in secondary emails collection
				for (var e=secondaryAddress.Count ()-1; e>=0; e--)
				{
					if (!secondaryAddress.ElementAt (e).Address.Mobile.IsNullOrWhiteSpace ())
					{
						if (secondaryAddress.ElementAt (e).Address.Mobile != AiderContactsHelpers.GetMainPhone (contacts))
						{
							return secondaryAddress.ElementAt (e).Address.Mobile;
						}
					}

					if (!secondaryAddress.ElementAt (e).Address.Phone1.IsNullOrWhiteSpace ())
					{
						if (secondaryAddress.ElementAt (e).Address.Phone1 != AiderContactsHelpers.GetMainPhone (contacts))
						{
							return secondaryAddress.ElementAt (e).Address.Phone1;
						}
					}
				}

				//second pass for phone 2 in normal order
				for (var e=0; e<secondaryAddress.Count (); e++)
				{
					if (!secondaryAddress.ElementAt (e).Address.Phone2.IsNullOrWhiteSpace ())
					{
						return secondaryAddress.ElementAt (e).Address.Phone2;
					}
				}
			}

			var defaultAddress = contacts.Where (c => c.AddressType == AddressType.Default).FirstOrDefault ();
			if (defaultAddress != null)
			{
				if (!defaultAddress.Address.Mobile.IsNullOrWhiteSpace ())
				{
					if (defaultAddress.Address.Mobile != AiderContactsHelpers.GetMainPhone (contacts))
					{
						return defaultAddress.Address.Mobile;
					}
				}

				if (!defaultAddress.Address.Phone1.IsNullOrWhiteSpace ())
				{
					if (defaultAddress.Address.Phone1 != AiderContactsHelpers.GetMainPhone (contacts))
					{
						return defaultAddress.Address.Phone1;
					}
				}

				if (!defaultAddress.Address.Phone2.IsNullOrWhiteSpace ())
				{
					if (defaultAddress.Address.Phone2 != AiderContactsHelpers.GetMainPhone (contacts))
					{
						return defaultAddress.Address.Phone2;
					}
				}
			}

			return "";
		}

		public static string GetSecondaryEmail(IEnumerable<AiderContactEntity> contacts)
		{
			if ((contacts == null) || (contacts.IsEmpty ()))
			{
				return "";
			}

			var professionalAddresses = contacts.Where (c => c.AddressType == AddressType.Professional);
			if (professionalAddresses != null)
			{
				//reverse look in professionals emails collection
				for (var e=professionalAddresses.Count ()-1; e>=0; e--)
				{
					if (!professionalAddresses.ElementAt (e).Address.Email.IsNullOrWhiteSpace ())
					{
						if (professionalAddresses.ElementAt (e).Address.Email != AiderContactsHelpers.GetMainEmail (contacts))
						{
							return professionalAddresses.ElementAt (e).Address.Email;
						}
					}
				}
			}

			var otherAddresses = contacts.Where (c => c.AddressType == AddressType.Other);
			if (otherAddresses.Any ())
			{
				//reverse look in others emails collection
				for (var e=otherAddresses.Count ()-1; e>=0; e--)
				{
					if (!otherAddresses.ElementAt (e).Address.Email.IsNullOrWhiteSpace ())
					{
						if (otherAddresses.ElementAt (e).Address.Email != AiderContactsHelpers.GetMainEmail (contacts))
						{
							return otherAddresses.ElementAt (e).Address.Email;
						}
					}
				}
			}

			var secondaryAddress = contacts.Where (c => c.AddressType == AddressType.Secondary);
			if (secondaryAddress.Any ())
			{
				//reverse look in secondary emails collection
				for (var e=secondaryAddress.Count ()-1; e>=0; e--)
				{
					if (!secondaryAddress.ElementAt (e).Address.Email.IsNullOrWhiteSpace ())
					{
						if (secondaryAddress.ElementAt (e).Address.Email != AiderContactsHelpers.GetMainEmail (contacts))
						{
							return secondaryAddress.ElementAt (e).Address.Email;
						}
					}
				}
			}

			var defaultAddress = contacts.Where (c => c.AddressType == AddressType.Default).FirstOrDefault ();
			if (defaultAddress != null)
			{
				if (!defaultAddress.Address.Email.IsNullOrWhiteSpace ())
				{
					if (defaultAddress.Address.Email != AiderContactsHelpers.GetMainEmail (contacts))
					{
						return defaultAddress.Address.Email;
					}
				}
			}

			return "";
		}
	}
}