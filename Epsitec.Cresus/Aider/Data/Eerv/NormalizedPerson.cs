using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class NormalizedPerson
	{


		public NormalizedPerson()
		{
			this.Households = new List<NormalizedHousehold> ();
			this.PersonalAddresses = new List<NormalizedAddress> ();
		}


		public string[] Firstnames
		{
			get;
			set;
		}


		public string Firstname
		{
			get
			{
				return this.Firstnames.Join (" ");
			}
		}


		public string[] Lastnames
		{
			get;
			set;
		}


		public string Lastname
		{
			get
			{
				return this.Lastnames.Join (" ");
			}
		}


		public Date? DateOfBirth
		{
			get;
			set;
		}


		public PersonSex Sex
		{
			get;
			set;
		}


		public List<NormalizedHousehold> Households
		{
			get;
			set;
		}


		public List<NormalizedAddress> PersonalAddresses
		{
			get;
			set;
		}


		public IEnumerable<NormalizedAddress> GetAddresses()
		{
			return this.Households.Select (h => h.Address).Concat (this.PersonalAddresses);
		}


		public override string ToString()
		{
			return this.Firstnames.Join (" ")
				+ " " + this.Lastnames.Join (" ")
				+ "(" + this.DateOfBirth 
				+ ", " + this.Sex
				+ ", " + string.Join (" | ", this.GetAddresses ())
				+ ")";
		}


	}


}

