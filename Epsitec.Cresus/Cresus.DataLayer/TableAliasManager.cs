using Epsitec.Common.Support;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{

	
	internal class TableAliasManager
	{

		public TableAliasManager(Druid entityId)
		{
			this.tableAliasNumber = 0;

			this.currentEntityNode = new EntityTableAliasNode (null, entityId.ToString (), this.CreateNewAlias ());

			this.currentSubtypeNodes = new Stack<SubtypeTableAliasNode> ();
			this.currentSubtypeNodes.Push (null);
		}


		public string CreateEntityAlias(Druid id)
		{
			this.currentEntityNode = this.currentEntityNode.CreateEntityNode (id.ToString (), this.CreateNewAlias ());
			this.currentSubtypeNodes.Push (null);

			return this.GetCurrentEntityAlias ();
		}


		public string GetCurrentEntityAlias()
		{
			return this.currentEntityNode.Alias;
		}


		public string GetPreviousEntityAlias()
		{
			this.currentEntityNode = this.currentEntityNode.GetParentNode ();
			this.currentSubtypeNodes.Pop ();

			return this.GetCurrentEntityAlias ();
		}


		public string GetNextEntityAlias(Druid id)
		{
			this.currentEntityNode = this.currentEntityNode.GetEntityNode (id.ToString ());
			this.currentSubtypeNodes.Push (null);

			return this.GetCurrentEntityAlias ();
		}


		public string CreateSubtypeAlias(Druid id, bool useCurrentEntityAlias)
		{
			string alias = (useCurrentEntityAlias) ? this.GetCurrentEntityAlias () : this.CreateNewAlias ();

			SubtypeTableAliasNode currentSubtypeNode = this.currentEntityNode.CreateSubtypeNode (id.ToString (), alias);

			this.currentSubtypeNodes.Pop ();
			this.currentSubtypeNodes.Push (currentSubtypeNode);

			return this.GetCurrentSubtypeAlias ();
		}


		public string GetCurrentSubtypeAlias()
		{
			return this.currentSubtypeNodes.Peek ().Alias;
		}


		public string GetNextSubtypeAlias(Druid entityId)
		{
			SubtypeTableAliasNode currentSubtypeNode = this.currentEntityNode.GetSubtypeNode (entityId.ToString ());

			this.currentSubtypeNodes.Pop ();
			this.currentSubtypeNodes.Push (currentSubtypeNode);

			return this.GetCurrentSubtypeAlias ();
		}


		private string CreateNewAlias()
		{
			string tableAlias = "tableAlias" + this.tableAliasNumber;

			this.tableAliasNumber++;

			return tableAlias;
		}


		private int tableAliasNumber;


		private EntityTableAliasNode currentEntityNode;


		private Stack<SubtypeTableAliasNode> currentSubtypeNodes;


	}

}
