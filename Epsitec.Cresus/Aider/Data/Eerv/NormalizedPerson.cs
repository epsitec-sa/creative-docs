using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class NormalizedPerson
	{


		public string[] Firstnames
		{
			get;
			set;
		}


		public string[] Lastnames
		{
			get;
			set;
		}


		public Date? DateOfBirth
		{
			get;
			set;
		}


		public Date? DateOfDeath
		{
			get;
			set;
		}


		public PersonSex Sex
		{
			get;
			set;
		}


		public string Origins
		{
			get;
			set;
		}


		public NormalizedAddress Address
		{
			get;
			set;
		}


		public override string ToString()
		{
			return this.Firstnames.Join (" ") + " " + this.Lastnames.Join (" ") + "(" + this.DateOfBirth + "-" + this.DateOfDeath + ", " + this.Sex + ", " + this.Origins + ", " + this.Address + ")";
		}


	}


}

