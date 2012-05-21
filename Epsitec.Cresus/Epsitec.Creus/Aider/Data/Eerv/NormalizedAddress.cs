namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class NormalizedAddress
	{


		// TODO Should we be able to handle addresses outside of Switzerland ?


		public string Street
		{
			get;
			set;
		}


		public int? HouseNumber
		{
			get;
			set;
		}


		public int ZipCode
		{
			get;
			set;
		}


		public string Town
		{
			get;
			set;
		}


		public override string ToString()
		{
			return this.Street + " " + this.HouseNumber + " " + this.ZipCode + " " + this.Town;
		}


	}


}

