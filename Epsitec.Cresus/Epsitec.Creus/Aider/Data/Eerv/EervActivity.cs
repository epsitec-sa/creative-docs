using Epsitec.Common.Types;


namespace Epsitec.Aider.Data.Eerv
{
	
	
	internal sealed class EervActivity : Freezable
	{


		public EervActivity(Date? startDate, Date? endDate, string remarks)
		{
			this.StartDate = startDate;
			this.EndDate = endDate;
			this.Remarks = remarks;
		}


		public EervPerson Person
		{
			get
			{
				return this.person;
			}
			set
			{
				this.ThrowIfReadOnly ();

				this.person = value;
			}
		}


		public EervLegalPerson LegalPerson
		{
			get
			{
				return this.legalPerson;
			}
			set
			{
				this.ThrowIfReadOnly ();

				this.legalPerson = value;
			}
		}


		public EervGroup Group
		{
			get
			{
				return this.group;
			}
			set
			{
				this.ThrowIfReadOnly ();

				this.group = value;
			}
		}


		public readonly Date? StartDate;
		public readonly Date? EndDate;
		public readonly string Remarks;


		private EervGroup group;
		private EervPerson person;
		private EervLegalPerson legalPerson;


	}


}
