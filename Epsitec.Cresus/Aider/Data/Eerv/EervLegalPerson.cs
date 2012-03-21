using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervLegalPerson : Freezable
	{


		public EervLegalPerson(string id, string name, EervAddress address, EervCoordinates coordinates)
		{
			this.Id = id;
			this.Name = name;
			this.Address = address;
			this.Coordinates = coordinates;

			this.activities = new List<EervActivity> ();
		}


		public EervPerson ContactPerson
		{
			get
			{
				return this.contactPerson;
			}
			set
			{
				this.ThrowIfReadOnly ();

				this.contactPerson = value;
			}
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


		private EervPerson contactPerson;
		private IList<EervActivity> activities;


	}


}

