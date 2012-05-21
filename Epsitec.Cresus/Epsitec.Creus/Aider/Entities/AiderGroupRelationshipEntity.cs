using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupRelationshipEntity
	{
		public static AiderGroupRelationshipEntity Create(BusinessContext businessContext, AiderGroupEntity group, AiderGroupEntity subGroup, GroupRelationshipType type)
		{
			var relationship = businessContext.CreateEntity<AiderGroupRelationshipEntity> ();

			relationship.Group1 = group;
			relationship.Group2 = subGroup;
			relationship.Type = type;

			return relationship;
		}
	}
}
