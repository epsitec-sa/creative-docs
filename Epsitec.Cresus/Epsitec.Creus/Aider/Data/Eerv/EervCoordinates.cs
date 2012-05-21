namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervCoordinates
	{


		public EervCoordinates(string privatePhoneNumber, string professionalPhoneNumber, string mobilePhoneNumber, string faxNumber, string emailAddress)
		{
			this.PrivatePhoneNumber = privatePhoneNumber;
			this.ProfessionalPhoneNumber = professionalPhoneNumber;
			this.MobilePhoneNumber = mobilePhoneNumber;
			this.FaxNumber = faxNumber;
			this.EmailAddress = emailAddress;
		}


		public readonly string PrivatePhoneNumber;
		public readonly string ProfessionalPhoneNumber;
		public readonly string MobilePhoneNumber;
		public readonly string FaxNumber;
		public readonly string EmailAddress;


	}


}

