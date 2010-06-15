using System.Collections.Generic;
using System.Linq;
using System;


namespace Epsitec.Cresus.DataLayer.TableAlias
{


	internal class EntityTableAliasNode : TableAliasNode
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


		public SubtypeTableAliasNode CreateSubtypeNode(string name, string alias)
		{
			SubtypeTableAliasNode node = new SubtypeTableAliasNode (this, name, alias);

			this.subtypeNodes.Add (node);

			return node;
		}


		public EntityTableAliasNode GetEntityNode(string name)
		{
			return this.GetEntityNode (name, 0);
		}


		public SubtypeTableAliasNode GetSubtypeNode(string name)
		{
			return this.subtypeNodes.Where (n => n.Name == name).FirstOrDefault ();
		}


		public EntityTableAliasNode GetEntityNode(string name, int position)
		{
			return this.entityNodes.Where (n => n.Name == name).ElementAtOrDefault (position);
		}


		private EntityTableAliasNode parentNode;


		private List<EntityTableAliasNode> entityNodes;


		private List<SubtypeTableAliasNode> subtypeNodes;


	}


}
