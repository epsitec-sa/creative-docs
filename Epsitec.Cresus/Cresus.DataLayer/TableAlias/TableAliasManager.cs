using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.TableAlias
{


	/// <summary>
	/// The <b>TableAliasManager</b> class is used by the <see cref="DataBrowser"/> class to store
	/// the graph of SQL table aliases that is uses to generate its query.
	/// </summary>
	/// <remarks>
	/// When generating a SQL query with the <see cref="DataBrowser"/>, a tree of SQL table alias is
	/// generated and need to be reused in some cases. This class manages the creation of this tree,
	/// stores it, and let the <see cref="DataBrowser"/> explore it again.
	/// 
	/// The tree of aliases, is composed of <see cref="AbstractNode"/>. Every <see cref="AbstractNode"/>
	/// contains the name of the table it refers to and the value of its alias. In addition, it
	/// contains a reference to its parent <see cref="AbstractNode"/> in the tree.
	/// 
	/// There are two kind of <see cref="AbstractNode"/>:
	/// 
	///	- <see cref="TopLevelNode"/> for the aliases of the top level tables (which are the root
	///	entity SQL tables and the relation SQL tables). They contain two list of references, one for
	///	its <see cref="TopLevelNode"/> children and one for its <see cref="SubLevelNode"/> children.
	///	
	///	- <see cref="SubLevelNode"/> for the aliases of the sub level tables (which are the derived
	///	entity SQL tables).
	///	
	/// The tree stored by the <c>TableAliasManager</c> is thus a tree of <see cref="TopLevelNode"/>
	/// which each can have their list of <see cref="SubLevelNode"/> children. Therefore, the tree of
	/// the <see cref="TopLevelNode"/> can be navigated back and forth. The <see cref="TopLevelNode"/>
	/// children of a given <see cref="TopLevelNode"/> can be accessed only when the current
	/// <see cref="TopLevelNode"/> is this <see cref="TopLevelNode"/>.
	/// 
	/// Moreover, the current <see cref="SubLevelNode"/> is stored when navigating to a child
	/// <see cref="TopLevelNode"/>, which means that when the tree is navigated backwards, from
	/// children to parents, the current <see cref="SubLevelNode"/> are restored to what they where
	/// before.
	/// </remarks>
	internal class TableAliasManager
	{


		/// <summary>
		/// Creates a new instance of <c>TableAliasManager</c> with a <see cref="TopLevelNode"/> of
		/// name <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name of the top level <see cref="TopLevelNode"/>.</param>
		public TableAliasManager(string name)
		{
			this.aliasCounter = 0;

			this.currentTopLevelNode = new TopLevelNode (null, name, this.CreateNewAlias ());

			this.currentSubLevelNodes = new Stack<SubLevelNode> ();
			this.currentSubLevelNodes.Push (null);
		}


		/// <summary>
		/// Adds a new <see cref="TopLevelNode"/> to the children of the current
		/// <see cref="TopLevelNode"/>.
		/// </summary>
		/// <param name="name">The name of the table.</param>
		/// <returns>The alias of the newly created <see cref="TopLevelNode"/>.</returns>
		public string CreateTopLevelAlias(string name)
		{
			this.currentTopLevelNode = this.currentTopLevelNode.CreateTopLevelNode (name, this.CreateNewAlias ());
			this.currentSubLevelNodes.Push (null);

			return this.GetCurrentTopLevelAlias ();
		}



		/// <summary>
		/// Adds a new <see cref="SubLevelNode"/> to the children of the current
		/// <see cref="TopLevelNode"/>.
		/// </summary>
		/// <param name="name">The name of the table.</param>
		/// <param name="aliasCreationMode">Tells how the new alias is created.</param>
		/// <returns>The alias of the newly created <see cref="SubLevelNode"/>.</returns>
		public string CreateSubLevelAlias(string name, AliasCreationMode aliasCreationMode)
		{
			string alias;

			switch (aliasCreationMode)
			{
				case AliasCreationMode.UseCurrentTopLevelAlias:
					alias =  this.GetCurrentTopLevelAlias ();
					break;

				case AliasCreationMode.UseNewAlias:
					alias =  this.CreateNewAlias ();
					break;

				default:
					throw new System.NotImplementedException ();
			}

			SubLevelNode currentSubtypeNode = this.currentTopLevelNode.CreateSubLevelNode (name, alias);

			this.currentSubLevelNodes.Pop ();
			this.currentSubLevelNodes.Push (currentSubtypeNode);

			return this.GetCurrentSubLevelAlias ();
		}


		/// <summary>
		/// This enum is used as an argument in the <see cref="CreateSubLevelAlias"/> method and let
		/// the user specify how to create the new alias.
		/// </summary>
		public enum AliasCreationMode
		{


			/// <summary>
			/// The alias of the new <see cref="SubLevelNode"/> will be the same as the one of the
			/// current <see cref="TopLevelNode"/>.
			/// </summary>
			UseCurrentTopLevelAlias,


			/// <summary>
			/// The alias of the new <see cref="SubLevelNode"/> will be created from scratch.
			/// </summary>
			UseNewAlias,


		}


		/// <summary>
		/// Gets the alias of the current <see cref="TopLevelNode"/>.
		/// </summary>
		/// <returns>The alias of the current <see cref="TopLevelNode"/>.</returns>
		public string GetCurrentTopLevelAlias()
		{
			return this.currentTopLevelNode.Alias;
		}


		/// <summary>
		/// Gets the alias of the current <see cref="SubLevelNode"/>.
		/// </summary>
		/// <returns>The alias of the current <see cref="SubLevelNode"/>.</returns>
		public string GetCurrentSubLevelAlias()
		{
			return this.currentSubLevelNodes.Peek ().Alias;
		}


		/// <summary>
		/// Sets the <see cref="TopLevelNode"/> child of the current <see cref="TopLevelNode"/>
		/// whose SQL table name is <paramref name="name"/> as the current <see cref="TopLevelNode"/>.
		/// </summary>
		/// <param name="name">The name of the SQL table.</param>
		/// <returns>The alias of the child <see cref="TopLevelNode"/>.</returns>
		public string NavigateToChildTopLevelAlias(string name)
		{
			return this.NavigateToChildTopLevelAlias (name, 0);
		}


		/// <summary>
		/// Sets the <see cref="SubLevelNode"/> child of the current <see cref="TopLevelNode"/>
		/// whose SQL table name is <paramref name="name"/> as the current <see cref="SubLevelNode"/>.
		/// </summary>
		/// <param name="name">The name of the SQL table.</param>
		/// <returns>The alias of the child <see cref="SubLevelNode"/>.</returns>
		public string NavigateToChildLevelAlias(string name)
		{
			SubLevelNode currentSubtypeNode = this.currentTopLevelNode.GetSubLevelNode (name);

			this.currentSubLevelNodes.Pop ();
			this.currentSubLevelNodes.Push (currentSubtypeNode);

			return this.GetCurrentSubLevelAlias ();
		}


		/// <summary>
		/// Sets the <see cref="TopLevelNode"/> child of the current <see cref="TopLevelNode"/>
		/// whose SQL table name is <paramref name="name"/> and whose index is among its siblings
		/// is <paramref name="index"/> as the current <see cref="TopLevelNode"/>.
		/// </summary>
		/// <param name="name">The name of the SQL table.</param>
		/// <param name="index">
		/// The index of the <see cref="TopLevelNode"/> within the <see cref="TopLevelNode"/> of the
		/// current <see cref="TopLevelNode"/> that have the name <paramref name="name"/>. 
		/// </param>
		/// <returns>The alias of the child <see cref="TopLevelNode"/>.</returns>
		public string NavigateToChildTopLevelAlias(string name, int index)
		{
			this.currentTopLevelNode = this.currentTopLevelNode.GetTopLevelNode (name, index);
			this.currentSubLevelNodes.Push (null);

			return this.GetCurrentTopLevelAlias ();
		}


		/// <summary>
		/// Sets the parent <see cref="TopLevelNode"/> of the current <see cref="TopLevelNode"/> as
		/// the current <see cref="TopLevelNode"/>.
		/// </summary>
		/// <returns>The alias of the parent <see cref="TopLevelNode"/>.</returns>
		public string NavigateToParentTopLevelAlias()
		{
			this.currentTopLevelNode = this.currentTopLevelNode.GetParentNode ();
			this.currentSubLevelNodes.Pop ();

			return this.GetCurrentTopLevelAlias ();
		}


		/// <summary>
		/// Creates a new alias which is unique to this instance of <c>TableAliasManager</c>.
		/// </summary>
		/// <returns>A new unique alias.</returns>
		private string CreateNewAlias()
		{
			string tableAlias = "tableAlias" + this.aliasCounter;

			this.aliasCounter++;

			return tableAlias;
		}


		/// <summary>
		/// Counts the number of alias generated by this instance, and is used to generate new unique
		/// aliases.
		/// </summary>
		private int aliasCounter;


		/// <summary>
		/// The reference to the current <see cref="TopLevelNode"/>.
		/// </summary>
		private TopLevelNode currentTopLevelNode;


		/// <summary>
		/// The references to the current <see cref="SubLevelNode"/>. They are stores in a stack, so
		/// when we navigate back to a previous <see cref="TopLevelNode"/>, we can find back the
		/// <see cref="SubLevelNode"/> which was the current <see cref="SubLevelNode"/> at that time.
		/// </summary>
		private Stack<SubLevelNode> currentSubLevelNodes;


	}

}
