using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervGroup : Freezable
	{


		public EervGroup(string id, string name)
		{
			this.Id = id;
			this.Name = name;

			this.activities = new List<EervActivity> ();
			this.subGroups = new List<EervGroup> ();
			this.superGroups = new List<EervGroup> ();
		}


		public IList<EervActivity> Activities
		{
			get
			{
				return this.activities;
			}
		}


		public IList<EervGroup> SubGroups
		{
			get
			{
				return this.subGroups;
			}
		}


		public IList<EervGroup> SuperGroups
		{
			get
			{
				return this.superGroups;
			}
		}


		protected override void HandleFreeze()
		{
			this.activities = this.activities.AsReadOnlyCollection ();
			this.subGroups = this.subGroups.AsReadOnlyCollection ();
			this.superGroups = this.superGroups.AsReadOnlyCollection ();
		}


		public readonly string Id;
		public readonly string Name;


		private IList<EervActivity> activities;
		private IList<EervGroup> subGroups;
		private IList<EervGroup> superGroups;


	}


}
