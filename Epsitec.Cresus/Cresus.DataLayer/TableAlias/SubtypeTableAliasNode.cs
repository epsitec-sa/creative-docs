namespace Epsitec.Cresus.DataLayer.TableAlias
{


	internal class SubtypeTableAliasNode : TableAliasNode
	{


		public SubtypeTableAliasNode(EntityTableAliasNode supertypeNode, string name, string alias)
			: base (name, alias)
		{
			this.supertypeNode = supertypeNode;
		}


		public EntityTableAliasNode GetSupertypeNode()
		{
			return this.supertypeNode;
		}


		EntityTableAliasNode supertypeNode;


	}


}
