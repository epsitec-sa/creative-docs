using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;


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


		protected override void HandleFreeze()
		{
			this.activities = this.activities.AsReadOnlyCollection ();
			this.subGroups = this.subGroups.AsReadOnlyCollection ();
		}


		public readonly string Id;
		public readonly string Name;


		private IList<EervActivity> activities;
		private IList<EervGroup> subGroups;


	}


}
