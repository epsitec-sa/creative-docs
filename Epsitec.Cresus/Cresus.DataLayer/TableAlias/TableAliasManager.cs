using Epsitec.Common.Support;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer.TableAlias
{

	
	internal class TableAliasManager
	{

		public TableAliasManager(string name)
		{
			this.tableAliasNumber = 0;

			this.currentEntityNode = new EntityTableAliasNode (null, name, this.CreateNewAlias ());

			this.currentSubtypeNodes = new Stack<SubtypeTableAliasNode> ();
			this.currentSubtypeNodes.Push (null);
		}


		public string CreateEntityAlias(string name)
		{
			this.currentEntityNode = this.currentEntityNode.CreateEntityNode (name, this.CreateNewAlias ());
			this.currentSubtypeNodes.Push (null);

			return this.GetCurrentEntityAlias ();
		}


		public string CreateSubtypeAlias(string name, bool useCurrentEntityAlias)
		{
			string alias = (useCurrentEntityAlias) ? this.GetCurrentEntityAlias () : this.CreateNewAlias ();

			SubtypeTableAliasNode currentSubtypeNode = this.currentEntityNode.CreateSubtypeNode (name, alias);

			this.currentSubtypeNodes.Pop ();
			this.currentSubtypeNodes.Push (currentSubtypeNode);

			return this.GetCurrentSubtypeAlias ();
		}


		public string GetCurrentEntityAlias()
		{
			return this.currentEntityNode.Alias;
		}


		public string GetCurrentSubtypeAlias()
		{
			return this.currentSubtypeNodes.Peek ().Alias;
		}


		public string GetNextEntityAlias(string name)
		{
			return this.GetNextEntityAlias (name, 0);
		}


		public string GetNextSubtypeAlias(string name)
		{
			SubtypeTableAliasNode currentSubtypeNode = this.currentEntityNode.GetSubtypeNode (name);

			this.currentSubtypeNodes.Pop ();
			this.currentSubtypeNodes.Push (currentSubtypeNode);

			return this.GetCurrentSubtypeAlias ();
		}


		public string GetNextEntityAlias(string name, int position)
		{
			this.currentEntityNode = this.currentEntityNode.GetEntityNode (name, position);
			this.currentSubtypeNodes.Push (null);

			return this.GetCurrentEntityAlias ();
		}


		private string CreateNewAlias()
		{
			string tableAlias = "tableAlias" + this.tableAliasNumber;

			this.tableAliasNumber++;

			return tableAlias;
		}


		public string GetPreviousEntityAlias()
		{
			this.currentEntityNode = this.currentEntityNode.GetParentNode ();
			this.currentSubtypeNodes.Pop ();

			return this.GetCurrentEntityAlias ();
		}


		private int tableAliasNumber;


		private EntityTableAliasNode currentEntityNode;


		private Stack<SubtypeTableAliasNode> currentSubtypeNodes;


	}

}
