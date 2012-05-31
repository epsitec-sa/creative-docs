//	Copyright © 2010-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;

using System;


namespace Epsitec.Cresus.DataLayer.Loader
{


	/// <summary>
	/// A <c>Request</c> object represent a high level query that can be executed against the database
	/// via a <see cref="DataContext"/> and <see cref="DataLoader"/>.
	/// </summary>
	/// <remarks>
	/// A Request is basically a search by example query with more power and flexibility. What you
	/// do is to give it a directed acyclic graph of entities where all entities are reachable from
	/// the root of the graphe, which is given by the property RootEntity. This gives you a subset
	/// of the entities in the database.
	/// Moreover, you can reduce this subset furthermore by adding conditions that will allow you to
	/// express things that are not expressable as examples, such a if a value is smaller that
	/// another.
	/// The result of the request is defined by the property RequestedEntity which defaults to the
	/// property RootEntity. It is only the entities in the result set that match this entity that
	/// are retured as the resulf of the request.
	/// Moreover, you can sort the result by adding sort clauses and you can return only a subset
	/// of the result by specifying the number of entities to skip and the number of entities to
	/// take.
	/// </remarks>
	public sealed class Request
	{
		
		
		/// <summary>
		/// Builds a brand new <c>Request</c>.
		/// </summary>
		public Request()
		{
			this.conditions = new List<Expression> ();
			this.sortClauses = new List<SortClause> ();
		}


		/// <summary>
		/// The <see cref="AbstractEntity"/> which is at the root of the <c>Request</c>.
		/// </summary>
		public AbstractEntity RootEntity
		{
			get;
			set;
		}


		/// <summary>
		/// The <see cref="AbstractEntity"/> which is to be returned at the end of the execution of
		/// the <c>Request</c>.
		/// </summary>
		public AbstractEntity RequestedEntity
		{
			get
			{
				return this.requestedEntity ?? this.RootEntity;
			}
			set
			{
				this.requestedEntity = value;
			}
		}


		/// <summary>
		/// The number of entities that the request must skip from the result.
		/// </summary>
		public int? Skip
		{
			get;
			set;
		}


		/// <summary>
		/// The number of entities that the request must take from the result.
		/// </summary>
		public int? Take
		{
			get;
			set;
		}


		public List<Expression> Conditions
		{
			get
			{
				return this.conditions;
			}
		}


		public List<SortClause> SortClauses
		{
			get
			{
				return this.sortClauses;
			}
		}


		public void AddSortClause(EntityField field, SortOrder sortOrder = SortOrder.Ascending)
		{
			this.sortClauses.Add (new SortClause (field, sortOrder));
		}


		public static Request Create(AbstractEntity rootEntity)
		{
			return new Request ()
			{
				RootEntity = rootEntity,
			};
		}


		public static Request Create(AbstractEntity rootEntity, DbKey rootEntityKey)
		{
			var request = Request.Create (rootEntity);

			request.Conditions.Add
			(
				new BinaryComparison
				(
					InternalField.CreateId (rootEntity),
					BinaryComparator.IsEqual,
					new Constant (rootEntityKey.Id.Value)
				)
			);

			return request;
		}


		public static Request Create(AbstractEntity rootEntity, DbKey rootEntityKey, AbstractEntity requestedEntity)
		{
			var request = Request.Create (rootEntity, rootEntityKey);

			request.RequestedEntity = requestedEntity;

			return request;
		}


		public Request Clone()
		{
			var copy = new Request ()
			{
				RootEntity = this.RootEntity,
				RequestedEntity = this.RequestedEntity,
				Skip = this.Skip,
				Take = this.Take,
			};

			copy.Conditions.AddRange (this.Conditions);
			copy.SortClauses.AddRange (this.SortClauses);

			return copy;
		}


		/// <summary>
		/// Checks that the request and all its data is valid and consistent.
		/// </summary>
		internal void Check(DataContext dataContext)
		{
			if (this.RootEntity == null)
			{
				throw new ArgumentException ("RootEntity is null");
			}

			var nonPersistentEntities = this.GetNonPersistentEntities (dataContext);

			this.CheckRequestedEntity (nonPersistentEntities);
			this.CheckForeignEntities (nonPersistentEntities, dataContext);
			this.CheckSkipAndTake ();

			var checker = this.BuildChecker (nonPersistentEntities, dataContext);

			this.CheckConditions (checker);
			this.CheckSortClauses (checker);
		}


		public HashSet<AbstractEntity> GetNonPersistentEntities(DataContext dataContext)
		{
			var typeEngine = dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine;

			var entities = new HashSet<AbstractEntity> ();
			var todo = new Stack<AbstractEntity> ();
			var parents = new Dictionary<AbstractEntity, List<AbstractEntity>> ();

			if (!dataContext.IsPersistent (this.RootEntity))
			{
				todo.Push (this.RootEntity);
			}

			while (todo.Count > 0)
			{
				var entity = todo.Pop ();

				entities.Add (entity);

				foreach (var child in EntityHelper.GetChildren (typeEngine, entity))
				{
					if (!dataContext.IsPersistent (child))
					{
						List<AbstractEntity> parentSet;

						if (!parents.TryGetValue (child, out parentSet))
						{
							parentSet = new List<AbstractEntity> ();

							parents[child] = parentSet;
						}

						parentSet.Add (entity);

						if (entities.Contains (child) || todo.Contains (child))
						{
							this.CheckForCycle (parents, child);
						}
						else
						{
							todo.Push (child);
						}
					}
				}
			}

			return entities;
		}


		private void CheckForCycle(Dictionary<AbstractEntity, List<AbstractEntity>> parents, AbstractEntity entity)
		{
			var todo = new Stack <AbstractEntity> ();

			todo.Push (entity);

			while (todo.Count > 0)
			{
				var child = todo.Pop ();

				List<AbstractEntity> parentSet;

				if (parents.TryGetValue (child, out parentSet))
				{
					foreach (var parent in parentSet)
					{
						if (parent == entity)
						{
							var message = "Cycles are not allowed in entity graph";

							throw new ArgumentException (message);
						}

						todo.Push (parent);
					}
				}			
			}
		}


		private void CheckRequestedEntity(HashSet<AbstractEntity> entities)
		{
			if (this.RequestedEntity != this.RootEntity)
			{
				if (!entities.Contains (requestedEntity))
				{
					throw new ArgumentException ("RequestedEntity is not reachable from RootEntity.");
				}
			}
		}


		private void CheckForeignEntities(HashSet<AbstractEntity> entities, DataContext dataContext)
		{
			foreach (var entity in entities)
			{
				if (dataContext.IsForeignEntity (entity))
				{
					throw new ArgumentException ("Foreign entities are not allowed.");
				}
			}
		}


		private void CheckSkipAndTake()
		{
			if (this.Skip.HasValue && this.Skip < 0)
			{
				throw new ArgumentException ("Skip is lower than zero");
			}

			if (this.Take.HasValue && this.Take < 0)
			{
				throw new ArgumentException ("Take is lower than zero");
			}
		}


		private FieldChecker BuildChecker(HashSet<AbstractEntity> entities, DataContext dataContext)
		{
			var typeEngine = dataContext.DataInfrastructure.EntityEngine.EntityTypeEngine;

			return new FieldChecker (entities, typeEngine);
		}


		private void CheckConditions(FieldChecker checker)
		{
			foreach (var condition in this.Conditions)
			{
				if (condition == null)
				{
					throw new ArgumentException ("A condition is null");
				}

				condition.CheckFields (checker);
			}
		}


		private void CheckSortClauses(FieldChecker checker)
		{
			foreach (var sortClause in this.SortClauses)
			{
				if (sortClause == null)
				{
					throw new ArgumentException ("A sort clause is null");
				}

				sortClause.CheckField (checker);
			}
		}


		private readonly List<Expression> conditions;
		private readonly List<SortClause> sortClauses;
		private AbstractEntity requestedEntity;

	}


}