using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer
{


	class EntityTableAliasNode : TableAliasNode
	{

		public EntityTableAliasNode(EntityTableAliasNode parentNode, string name, string alias)
			: base (name, alias)
		{
			this.parentNode = parentNode;

			this.entityNodes = new List<EntityTableAliasNode> ();
			this.subtypeNodes = new List<SubtypeTableAliasNode> ();
		}


		public EntityTableAliasNode GetParentNode()
		{
			return this.parentNode;
		}


		public EntityTableAliasNode CreateEntityNode(string name, string alias)
		{
			EntityTableAliasNode node = new EntityTableAliasNode (this, name, alias);

			this.entityNodes.Add (node);

			return node;
		}


		public EntityTableAliasNode GetEntityNode(string name)
		{
			return this.entityNodes.FirstOrDefault (n => n.Name == name);
		}


		public SubtypeTableAliasNode CreateSubtypeNode(string name, string alias)
		{
			SubtypeTableAliasNode node = new SubtypeTableAliasNode (this, name, alias);

			this.subtypeNodes.Add (node);

			return node;
		}


		public SubtypeTableAliasNode GetSubtypeNode(string name)
		{
			return this.subtypeNodes.FirstOrDefault (n => n.Name == name);
		}


		private EntityTableAliasNode parentNode;


		private List<EntityTableAliasNode> entityNodes;


		private List<SubtypeTableAliasNode> subtypeNodes;


	}


}
