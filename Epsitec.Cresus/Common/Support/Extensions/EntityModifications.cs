using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Common.Support.Extensions
{


	/// <summary>
	/// The <c>EntityModifications</c> class provides Extension methods used to check if
	/// the values of an <see cref="AbstractEntity"/> have changed.
	/// </summary>
	public static class EntityModifications
	{


		/// <summary>
		/// Checks if a value field of an <see cref="AbstracEntity"/> has changed.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to check.</param>
		/// <param name="fieldId">The id of the field to check.</param>
		/// <returns><c>true</c> if the value of the field has changed, false if it has not.</returns>
		public static bool HasValueChanged(this AbstractEntity entity, Druid fieldId)
		{
			entity.ThrowIfNull ("entity");

			object modifiedValue = entity.GetModifiedValue (fieldId);

			bool change = false;

			if (!UndefinedValue.IsUndefinedValue (modifiedValue))
			{
				object originalValue = entity.GetOriginalValue (fieldId);

				change = !System.Object.Equals (originalValue, modifiedValue);
			}

			return change;
		}


		/// <summary>
		/// Checks if a reference field of an <see cref="AbstracEntity"/> has changed.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to check.</param>
		/// <param name="fieldId">The id of the field to check.</param>
		/// <returns><c>true</c> if the reference of the field has changed, false if it has not.</returns>
		public static bool HasReferenceChanged(this AbstractEntity entity, Druid fieldId)
		{
			entity.ThrowIfNull ("entity");

			object modifiedValue = entity.GetModifiedValue (fieldId);

			bool change = false;

			if (!UndefinedValue.IsUndefinedValue (modifiedValue))
			{
				object originalValue = entity.GetOriginalValue (fieldId);

				originalValue = EntityModifications.GetValueForReferenceComparison (originalValue);
				modifiedValue = EntityModifications.GetValueForReferenceComparison (modifiedValue);

				change = !System.Object.Equals (originalValue, modifiedValue);
			}

			return change;
		}


		/// <summary>
		/// Gets the value that will be used for the comparison of two references.
		/// </summary>
		/// <param name="value">The value that must be compared to another</param>
		/// <returns>The processed value.</returns>
		private static object GetValueForReferenceComparison(object value)
		{
			// For the comparison, we consider null entities equal to null virtualized entities,
			// so we unwrap the values if they are entities (they might be undefined values for
			// instance).

			return value is AbstractEntity
				? EntityNullReferenceVirtualizer.UnwrapNullEntity ((AbstractEntity) value)
				: value;
		}


		/// <summary>
		/// Checks if a collection field of an <see cref="AbstracEntity"/> has changed.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to check.</param>
		/// <param name="fieldId">The id of the field to check.</param>
		/// <returns><c>true</c> if the collection of the field has changed, false if it has not.</returns>
		public static bool HasCollectionChanged(this AbstractEntity entity, Druid fieldId)
		{
			entity.ThrowIfNull ("entity");

			object modifiedValue = entity.GetModifiedValue (fieldId);

			bool change = false;

			if (!UndefinedValue.IsUndefinedValue (modifiedValue))
			{
				var modifiedValues = EntityModifications.GetValueForCollectionComparison (modifiedValue);

				object originalValue = entity.GetOriginalValue (fieldId);

				if (UndefinedValue.IsUndefinedValue (originalValue))
				{
					// The collection has changed only if it contain at least an element that is not
					// null and not null virtualized.

					change = modifiedValues.Any ();
				}
				else
				{
					var originalValues = EntityModifications.GetValueForCollectionComparison (originalValue);

					change = !originalValues.SequenceEqual (modifiedValues);
				}
			}

			return change;
		}


		/// <summary>
		/// Gets the value that will be used for the comparison of two collections.
		/// </summary>
		/// <param name="value">The value that must be compared to another</param>
		/// <returns>The processed value.</returns>
		private static IEnumerable<AbstractEntity> GetValueForCollectionComparison(object value)
		{
			// For the comparison, we don't consider null entities or null virtualized entities
			// because these won't be saved to the database anyway. So we strip the collections
			// of these before comparing them.

			var collection = (IEnumerable<AbstractEntity>) value;

			return collection.Where (e => !EntityNullReferenceVirtualizer.IsNullEntity (e));
		}


		/// <summary>
		/// Gets the original value of the field of an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose field value to get.</param>
		/// <param name="fieldId">The id of the field to get.</param>
		/// <returns>The original value of the field.</returns>
		private static object GetOriginalValue(this AbstractEntity entity, Druid fieldId)
		{
			IValueStore originalValues = entity.GetOriginalValues ();

			return originalValues.GetValue (fieldId.ToResourceId ());
		}


		/// <summary>
		/// Gets the modified value of the field of an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose field value to get.</param>
		/// <param name="fieldId">The id of the field to get.</param>
		/// <returns>The modified value of the field.</returns>
		private static object GetModifiedValue(this AbstractEntity entity, Druid fieldId)
		{
			IValueStore modifiedValues = entity.GetModifiedValues ();

			return modifiedValues.GetValue (fieldId.ToResourceId ());
		}


	}


}
