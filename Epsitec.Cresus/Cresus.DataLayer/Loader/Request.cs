using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections;
using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{
	
	
	/// <summary>
	/// A <c>Request</c> object represent a high level query that can be executed against the database
	/// via a <see cref="DataContext"/> and <see cref="DataLoader"/>.
	/// </summary>
	/// <remarks>
	/// A <c>Request</c> is basically a search by example query with a little more flexibility. What
	/// you do is to give it an acyclic tree of <see cref="AbstractEntity"/>, whose root is
	/// <see cref="RootEntity"/>. Then the <see cref="DataLoader"/> does a search by example in the
	/// database and returns all the <see cref="AbstractEntity"/> corresponding to that example.
	/// The <see cref="AbstractEntity"/> to return is one given by the property <see cref="RequestedEntity"/>,
	/// which defaults to the property <see cref="RootEntity"/>.
	/// If an <see cref="AbstractEntity"/> in the tree is present in the <see cref="DataContext"/>,
	/// then this part of the search related to this <see cref="AbstractEntity"/> is not done by
	/// value, but by reference. In addition, it is possible to specify the <see cref="DbKey"/> of
	/// the root <see cref="AbstractEntity"/> with the property <see cref="RootEntityKey"/>.
	/// Finally, it is possible to associate constraints to the <see cref="AbstractEntity"/>, with
	/// the method <see cref="AddLocalConstraint"/>.
	/// </remarks>
	public sealed class Request
	{


		/// <summary>
		/// Builds a brand new <c>Request</c>.
		/// </summary>
		public Request()
		{
			this.localConstraints = new Dictionary<AbstractEntity, List<Expression>> ();

			this.RootEntity = null;
			this.RequestedEntity = null;
		}


		/// <summary>
		/// The <see cref="AbstractEntity"/> which is at the root of the <c>Request</c>.
		/// </summary>
		public AbstractEntity RootEntity
		{
			internal get;
			set;
		}


		/// <summary>
		/// The <see cref="DbKey"/> of the <see cref="AbstractEntity"/> which is at the root of the
		/// <c>Request</c>.
		/// </summary>
		public DbKey? RootEntityKey
		{
			internal get;
			set;
		}


		/// <summary>
		/// The <see cref="AbstractEntity"/> which is to be returned at the end of the execution of
		/// the <c>Request</c>.
		/// </summary>
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


		/// <summary>
		/// Defines the minimum log id that the entities should have to be returned by the request.
		/// </summary>
		internal long? RequestedEntityMinimumLogId
		{
			get;
			set;
		}

		
		/// <summary>
		/// Adds a constraint to an <see cref="AbstractEntity"/>. Note that the constraint should
		/// only target value fields on the given <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> associated with the constraint.</param>
		/// <param name="constraint">The constraint to add.</param>
		/// <exception cref="System.ArgumentException">If the constraint is not supported.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="constraint"/> is null.</exception>
		public void AddLocalConstraint(AbstractEntity entity, Expression constraint)
		{
			entity.ThrowIfNull ("entity");
			constraint.ThrowIfNull ("constraint");

			if (!this.IsLocalConstraintValid (entity, constraint))
			{
				throw new System.ArgumentException ("A field in 'expression' is not a field or 'entity'.");
			}

			this.GetWritableLocalConstraints (entity).Add (constraint);
		}


		/// <summary>
		/// Gets all the constraints associated with an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose constraints to retrieve.</param>
		/// <returns>The constraints.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is null.</exception>
		internal IEnumerable<Expression> GetLocalConstraints(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");

			if (this.IsLocalyConstrained (entity))
			{
				return this.localConstraints[entity].Select (c => c);
			}
			else
			{
				return new Expression[0];
			}
		}

		/// <summary>
		/// Tells whether there are constraints associated with an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to check if there are constraints associated.</param>
		/// <returns><c>true</c> if there are constraints associated with the <see cref="AbstractEntity"/>, <c>false</c> if there are not.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is null.</exception>
		internal bool IsLocalyConstrained(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");

			return this.localConstraints.ContainsKey (entity);
		}

		
		/// <summary>
		/// Gets the <see cref="IList"/> of constraints associated with an <see cref="AbstractEntity"/>
		/// and creates it if there isn't any.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose constraints to retrieve.</param>
		/// <returns>The <see cref="IList"/> of constraints.</returns>
		private List<Expression> GetWritableLocalConstraints(AbstractEntity entity)
		{
			if (!this.IsLocalyConstrained (entity))
			{
				this.localConstraints[entity] = new List<Expression> ();
			}

			return this.localConstraints[entity];
		}


		/// <summary>
		/// Checks that a constraint is valid for a given <see cref="AbstractEntity"/>, which means
		/// that it targets only value fields which exists in <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> associated with the constraint.</param>
		/// <param name="constraint">The constraint whose validity to check.</param>
		/// <returns><c>true</c> if the constraint is valid, <c>false</c> if it is not.</returns>
		private bool IsLocalConstraintValid(AbstractEntity entity, Expression constraint)
		{
			return constraint.GetFields ().All (field => this.IsEntityValueField (entity, field.ToResourceId ()));
		}


		/// <summary>
		/// Checks that a field is valid for an <see cref="AbstractEntity"/>, which means that it
		/// is a value field of this <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> associated with the field.</param>
		/// <param name="fieldId">The id of the field to check.</param>
		/// <returns><c>true</c> if the field is valid, false if it isn't.</returns>
		private bool IsEntityValueField(AbstractEntity entity, string fieldId)
		{
			StructuredTypeField field = entity.GetEntityContext ().GetStructuredTypeField(entity, fieldId);

			return field != null && field.Relation == FieldRelation.None;
		}


		/// <summary>
		/// The local constraints associated with their <see cref="AbstractEntity"/>.
		/// </summary>
		private readonly Dictionary<AbstractEntity, List<Expression>> localConstraints;


		/// <summary>
		/// The backing variable for the property <see cref="RequestedEntity"/>.
		/// </summary>
		private AbstractEntity requestedEntity;


	}


}
