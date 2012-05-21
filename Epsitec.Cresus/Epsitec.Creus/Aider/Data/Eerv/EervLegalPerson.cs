using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervLegalPerson : Freezable
	{


		public EervLegalPerson(string id, string name, EervAddress address, EervCoordinates coordinates, EervPerson contactPerson)
		{
			this.Id = id;
			this.Name = name;
			this.Address = address;
			this.Coordinates = coordinates;
			this.ContactPerson = contactPerson;

			this.activities = new List<EervActivity> ();
		}


		public IList<EervActivity> Activities
		{
			get
			{
				return this.activities;
			}
		}


		protected override void HandleFreeze()
		{
			base.HandleFreeze ();

			this.activities = this.activities.AsReadOnlyCollection ();
		}


		public readonly string Id;
		public readonly string Name;
		public readonly EervAddress Address;
		public readonly EervCoordinates Coordinates;
		public readonly EervPerson ContactPerson;


		private IList<EervActivity> activities;


	}


}

