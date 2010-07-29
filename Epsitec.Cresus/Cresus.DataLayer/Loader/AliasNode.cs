using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{


	/// <summary>
	/// The <c>AliasNode</c> class is used to build some kind of n-ary tree. Each  node has a name
	/// and an alias. Moreover, it may have a single parent and several children. Its children can
	/// be accessed by their name, and it may have several children with the same name.
	/// Moreover, the aliases are automatically generated and have the property that no node in a
	/// tree will have the same alias.
	/// </summary>
	internal sealed class AliasNode
	{


		/// <summary>
		/// Gets the name of the current <c>AliasNode</c>.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the alias of the current <c>AliasNode</c>.
		/// </summary>
		public string Alias
		{
			get;
			private set;
		}


		/// <summary>
		/// Builds a new <c>AliasNode</c> without parent.
		/// </summary>
		/// <param name="name">The name of the <c>AliasNode</c>.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is null or empty.</exception>
		public AliasNode(string name) : this (null, name)
		{
		}


		/// <summary>
		/// Builds a new <c>AliasNode</c> with a parent.
		/// </summary>
		/// <param name="parent">The parent of the new <c>AliasNode</c>.</param>
		/// <param name="name">The name of the new <c>AliasNode</c>.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is null or empty.</exception>
		private AliasNode(AliasNode parent, string name)
		{
			name.ThrowIfNullOrEmpty ("name");
			
			this.parent = parent;
			this.children = new Dictionary<string, List<AliasNode>> ();

			this.Name = name;
			this.Alias = this.CreateNewAlias ();
		}


		/// <summary>
		/// Creates a new child for the current <c>AliasNode</c>.
		/// </summary>
		/// <param name="name">The name of the new child.</param>
		/// <returns>The new child.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is null or empty.</exception>
		public AliasNode CreateChild(string name)
		{
			name.ThrowIfNullOrEmpty ("name");
			
			AliasNode node = new AliasNode (this, name);

			if (!this.children.ContainsKey (name))
			{
				this.children[name] = new List<AliasNode> ();
			}

			this.children[name].Add (node);

			return node;
		}


		/// <summary>
		/// Gets the first child of the current <c>AliasNode</c> that has a given name.
		/// </summary>
		/// <param name="name">The name of the child to get.</param>
		/// <returns>The child <c>AliasNode</c>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is null, empty or if there are no children with that name.</exception>
		public AliasNode GetChild(string name)
		{
			name.ThrowIfNullOrEmpty ("name");
			name.ThrowIf (e => !this.children.ContainsKey (e), "No child with name '" + name + "'.");

			return this.children[name].First ();
		}


		/// <summary>
		/// Get the list of children of the current <c>AliasNode</c> that have a given name.
		/// </summary>
		/// <param name="name">The name of the child to get.</param>
		/// <returns>The list of children <c>AliasNode</c>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is null, empty or if there are no children with that name.</exception>
		public ReadOnlyCollection<AliasNode> GetChildren(string name)
		{
			name.ThrowIfNullOrEmpty ("name");
			name.ThrowIf (e => !this.children.ContainsKey (e), "No child with name '" + name + "'.");

			List<AliasNode> children = this.children[name];

			return new ReadOnlyCollection<AliasNode> (children);
		}


		/// <summary>
		/// Gets the parent of the current <c>AliasNode</c>.
		/// </summary>
		/// <returns>The parent <c>AliasNode</c>.</returns>
		public AliasNode GetParent()
		{
			return this.parent;
		}


		/// <summary>
		/// Creates a new alias which is guaranteed to be unique within the tree. This method is
		/// called recursively until it reaches the ancestor node which will really create the new
		/// alias.
		/// </summary>
		/// <returns>The new alias.</returns>
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


		/// <summary>
		/// Generates a new unique alias.
		/// </summary>
		/// <returns>The new alias.</returns>
		private string GenerateNewAlias()
		{
			string alias = "alias" + this.aliasCounter;

			this.aliasCounter++;

			return alias;
		}

        
		/// <summary>
		/// The parent of the current <c>AliasNode</c>.
		/// </summary>
		private AliasNode parent;
		
		
		/// <summary>
		/// The children of the current <c>AliasNode</c>, organized by their name.
		/// </summary>
		private Dictionary<string, List<AliasNode>> children;


		/// <summary>
		/// Stores the number of aliased generated by the current <c>AliasNode</c>, so that it can
		/// always create new unique ones.
		/// </summary>
		private int aliasCounter;


	}


}
