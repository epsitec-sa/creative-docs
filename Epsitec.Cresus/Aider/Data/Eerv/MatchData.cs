namespace Epsitec.Aider.Data.Eerv
{


	internal class MatchData
	{


		public NameMatch Lastname
		{
			get;
			set;
		}


		public NameMatch Firstname
		{
			get;
			set;
		}


		/// <summary>
		/// True means that the date of birth is the same. False means that they are different. Null
		/// means that at least one of them is undefined.
		/// </summary>
		public bool? DateOfBirth
		{
			get;
			set;
		}


		/// <summary>
		/// True means that the sex is the same. False means that they are different. Null means
		/// that at least one of them is undefined.
		/// </summary>
		public bool? Sex
		{
			get;
			set;
		}


		public AddressMatch Address
		{
			get;
			set;
		}


	}


}

