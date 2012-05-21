using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Collections.ObjectModel;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervMainData
	{


		public EervMainData(IEnumerable<EervGroupDefinition> groupDefinitions)
		{
			this.GroupDefinitions = groupDefinitions.AsReadOnlyCollection ();
		}


		public readonly ReadOnlyCollection<EervGroupDefinition> GroupDefinitions;



	}



}
