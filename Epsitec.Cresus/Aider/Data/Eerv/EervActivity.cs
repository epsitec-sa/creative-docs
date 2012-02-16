using Epsitec.Common.Types;


namespace Epsitec.Aider.Data.Eerv
{
	
	
	internal sealed class EervActivity
	{


		public EervActivity(string personId, string groupId, Date? startDate, Date? endDate, string remarks)
		{
			this.PersonId = personId;
			this.GroupId = groupId;
			this.StartDate = startDate;
			this.EndDate = endDate;
			this.Remarks = remarks;
		}


		public readonly string PersonId;
		public readonly string GroupId;
		public readonly Date? StartDate;
		public readonly Date? EndDate;
		public readonly string Remarks;


	}


}
