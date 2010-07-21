using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{
	
	
	public sealed class Request
	{


		public Request()
		{
			this.localConstraints = new Dictionary<AbstractEntity, List<Expression>> ();

			this.RootEntity = null;
			this.RequestedEntity = null;

			this.ResolutionMode = ResolutionMode.Database;
		}


		public AbstractEntity RootEntity
		{
			internal get;
			set;
		}


		public DbKey? RootEntityKey
		{
			internal get;
			set;
		}


		public AbstractEntity RequestedEntity
		{
			internal get
			{
				return this.requestedEntity ?? this.RootEntity;
			}
			set
			{
				this.requestedEntity = value;
			}
		}

		
		public ResolutionMode ResolutionMode
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
			return contraint.GetFields ().All (field => this.IsEntityValueField (entity, field.ToResourceId ()));
		}


		private bool IsEntityValueField(AbstractEntity entity, string fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			StructuredTypeField field = entity.GetEntityContext ().GetEntityFieldDefinition (leafEntityId, fieldId);

			return field != null && field.Relation == FieldRelation.None;
		}


		private readonly Dictionary<AbstractEntity, List<Expression>> localConstraints;


		private AbstractEntity requestedEntity;


	}


}
