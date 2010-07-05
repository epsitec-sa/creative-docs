using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Epsitec.Cresus.DataLayer.Browser
{


	internal class AliasNode
	{


		public string Name
		{
			get;
			private set;
		}


		public string Alias
		{
			get;
			private set;
		}


		public AliasNode(string name) : this (null, name)
		{
		}


		public AliasNode(AliasNode parent, string name)
		{
			this.parent = parent;
			this.children = new Dictionary<string, List<AliasNode>> ();

			this.Name = name;
			this.Alias = this.CreateNewAlias ();
		}


		public AliasNode CreateChild(string name)
		{
			AliasNode node = new AliasNode (this, name);

			if (!this.children.ContainsKey (name))
			{
				this.children[name] = new List<AliasNode> ();
			}

			this.children[name].Add (node);

			return node;
		}


		public AliasNode GetChild(string name)
		{
			return this.children[name][0];
		}


		public ReadOnlyCollection<AliasNode> GetChildren(string name)
		{
			List<AliasNode> children = this.children[name];

			return new ReadOnlyCollection<AliasNode> (children);
		}


		public AliasNode GetParent()
		{
			return this.parent;
		}


		private string CreateNewAlias()
		{
			if (this.parent != null)
			{
				return this.parent.CreateNewAlias ();
			}
			else
			{
				return this.GenerateNewAlias ();
			}
		}


		private string GenerateNewAlias()
		{
			string alias = "alias" + this.aliasCounter;

			this.aliasCounter++;

			return alias;
		}

        
		private AliasNode parent;
		
		
		private Dictionary<string, List<AliasNode>> children;


		private int aliasCounter;


	}


}
