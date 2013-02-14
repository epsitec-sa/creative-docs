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

				change = !System.Object.Equals (originalValue, modifiedValue);
			}

			return change;
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
				object originalValue = entity.GetOriginalValue (fieldId);

				if (UndefinedValue.IsUndefinedValue (originalValue))
				{
					change = true;
				}
				else
				{
					IList<object> originalValues = (IList<object>) originalValue;
					IList<object> modifiedValues = (IList<object>) modifiedValue;

					change = !originalValues.SequenceEqual (modifiedValues);
				}
			}

			return change;
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
