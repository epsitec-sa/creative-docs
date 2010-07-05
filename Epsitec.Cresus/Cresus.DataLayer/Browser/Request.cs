using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.Browser
{
	
	
	public class Request
	{


		public Request()
		{
			this.localConstraints = new Dictionary<AbstractEntity, List<Expression>> ();

			this.RootEntity = null;
			this.RequestedEntity = null;

			this.IsRootEntityReference = false;
			this.LoadFromDatabase = true;
		}


		public AbstractEntity RootEntity
		{
			internal get
			{
				AbstractEntity entity = this.rootEntity ?? this.requestedEntity;

				if (entity == null)
				{
					throw new System.NullReferenceException ("RootEntity is null");
				}

				return entity;
			}
			set
			{
				this.rootEntity = value;
			}
		}


		public AbstractEntity RequestedEntity
		{
			internal get
			{
				AbstractEntity entity = this.requestedEntity ?? this.rootEntity;

				if (entity == null)
				{
					throw new System.NullReferenceException ("RequestedEntity is null");
				}

				return entity;
			}
			set
			{
				this.requestedEntity = value;
			}
		}


		public bool IsRootEntityReference
		{
			internal get;
			set;
		}


		public bool LoadFromDatabase
		{
			internal get;
			set;
		}

		
		public void AddLocalConstraint(AbstractEntity entity, Expression contraint)
		{
			if (!this.CheckLocalConstraint (entity, contraint))
			{
				throw new System.ArgumentException ("A field in 'expression' is not a field or 'entity'.");
			}

			List<Expression> constraints = this.GetWritableLocalConstraints (entity);

			constraints.Add (contraint);
		}


		internal IEnumerable<Expression> GetLocalConstraints(AbstractEntity entity)
		{
			if (this.IsLocalyConstrained (entity))
			{
				return this.localConstraints[entity].Select (c => c);
			}
			else
			{
				return new Expression[0];
			}
		}


		internal bool IsLocalyConstrained(AbstractEntity entity)
		{
			return this.localConstraints.ContainsKey (entity);
		}

		
		private List<Expression> GetWritableLocalConstraints(AbstractEntity entity)
		{
			if (!this.IsLocalyConstrained (entity))
			{
				this.localConstraints[entity] = new List<Expression> ();
			}

			return this.localConstraints[entity];
		}


		private bool CheckLocalConstraint(AbstractEntity entity, Expression contraint)
		{
			HashSet<string> fields1 = new HashSet<string> (entity.GetEntityContext ().GetEntityFieldIds (entity));
			HashSet<string> fields2 = new HashSet<string> (contraint.GetFields ().Select (d => d.ToResourceId ()));

			return fields1.IsSupersetOf (fields2) && fields2.All (id => this.IsValueField (entity, id));
		}


		private bool IsValueField(AbstractEntity entity, string fieldId)
		{
			return entity.GetEntityContext ().GetEntityFieldDefinition (entity.GetEntityStructuredTypeId (), fieldId).Relation == FieldRelation.None;
		}


		private readonly Dictionary<AbstractEntity, List<Expression>> localConstraints;


		private AbstractEntity rootEntity;


		private AbstractEntity requestedEntity;


	}


}
