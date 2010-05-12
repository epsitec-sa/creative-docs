using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{

	
	internal class TableAliasManager
	{

		public TableAliasManager()
		{
			this.rootTypeTableAlias = new Stack<string> ();
			this.subTypeTableAlias = new Stack<string> ();
		}

		
		public void resetTableAliases()
		{
			this.tableAliasNumber = 0;

			this.rootTypeTableAlias.Clear ();
			this.subTypeTableAlias.Clear ();
		}


		public string PushRootTypeTableAlias()
		{
			string newTableAlias = this.GetNewTableAlias ();

			this.rootTypeTableAlias.Push (newTableAlias);

			return newTableAlias;
		}


		public string PeekRootTypeTableAlias()
		{
			return this.rootTypeTableAlias.Peek ();
		}


		public void PopRootTypeTableAlias()
		{
			this.rootTypeTableAlias.Pop ();
		}


		public string PushSubTypeTableAlias(bool useLastRootTypeAlias)
		{
			string newTableAlias = (useLastRootTypeAlias) ? this.PeekRootTypeTableAlias () : this.GetNewTableAlias ();

			this.subTypeTableAlias.Push (newTableAlias);

			return newTableAlias;
		}


		public string PeekSubTypeTableAlias()
		{
			return this.subTypeTableAlias.Peek ();
		}


		public void PopSubTypeTableAlias()
		{
			this.subTypeTableAlias.Pop ();
		}


		private string GetNewTableAlias()
		{
			string tableAlias = "tableAlias" + this.tableAliasNumber;

			this.tableAliasNumber++;

			return tableAlias;
		}


		private int tableAliasNumber;


		private Stack<string> rootTypeTableAlias;


		private Stack<string> subTypeTableAlias;


	}

}
