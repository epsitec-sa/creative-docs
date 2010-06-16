using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer
{
	
	
	public class EntityConstrainer
	{


		public EntityConstrainer()
		{
			this.localConstraints = new Dictionary<AbstractEntity, List<Expression>> ();
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


	}


}
