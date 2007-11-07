//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>AbstractEntity</c> class is the base class used to store the
	/// data represented by entity instances.
	/// </summary>
	public abstract class AbstractEntity
	{
		protected AbstractEntity()
		{
			this.context = EntityContext.Current;
		}

		/// <summary>
		/// Gets the id of the <see cref="StructuredType"/> which describes
		/// this entity.
		/// </summary>
		/// <returns>The id of the <see cref="StructuredType"/>.</returns>
		public abstract Druid GetStructuredTypeId();


		public EntityDataState DataState
		{
			get
			{
				if (this.modifiedValues != null)
				{
					return EntityDataState.Modified;
				}
				else
				{
					return EntityDataState.Unchanged;
				}
			}
		}

		public bool IsDefiningOriginalValues
		{
			get
			{
				if (this.defineOriginalValuesCount > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public bool ContainsDataVersion(EntityDataVersion version)
		{
			switch (version)
			{
				case EntityDataVersion.Modified:
					return this.modifiedValues != null;
				case EntityDataVersion.Original:
					return this.originalValues != null;
				default:
					throw new System.NotImplementedException ();
			}
		}


		public System.IDisposable DefineOriginalValues()
		{
			return new DefineOriginalValuesHelper (this);
		}
		

		public T GetField<T>(string id)
		{
			StructuredTypeField field = this.context.GetStructuredType (this).GetField (id);

			System.Diagnostics.Debug.Assert (field != null);
			System.Diagnostics.Debug.Assert (field.Relation != FieldRelation.Collection);

			object value = this.InternalGetValue (id);

			if (UnknownValue.IsUnknownValue (value))
			{
				throw new System.NotSupportedException (string.Format ("Field {0} not supported by value store", id));
			}

			if (UndefinedValue.IsUndefinedValue (value))
			{
				return default (T);
			}
			
			return (T) value;
		}

		public IList<T> GetFieldCollection<T>(string id) where T : AbstractEntity
		{
			object  value = this.InternalGetValue (id);
			IList<T> list = value as IList<T>;

			if (list == null)
			{
				if (UndefinedValue.IsUndefinedValue (value))
				{
					//	The value store does not (yet) contain a collection for the
					//	specified items. We have to allocate one :

					using (this.DefineOriginalValues ())
					{
						list = new EntityCollection<T> (id, this, true);
						this.InternalSetValue (id, list);
					}
				}
				else
				{
					throw new System.NotSupportedException (string.Format ("Field {0} uses incompatible collection type", id));
				}
			}

			return list;
		}

		public void SetField<T>(string id, T oldValue, T newValue)
		{
			StructuredTypeField field = this.context.GetStructuredType (this).GetField (id);

			System.Diagnostics.Debug.Assert (field != null);
			System.Diagnostics.Debug.Assert (field.Relation != FieldRelation.Collection);

			if ((field.IsNullable) &&
				(newValue == null))
			{
				//	The value is null and the field is nullable; this operation
				//	is valid and it will clear the field.

				this.InternalSetValue (id, UndefinedValue.Instance);
				return;
			}
			
			IDataConstraint constraint = field.Type as IDataConstraint;

			System.Diagnostics.Debug.Assert (constraint != null);

			if (constraint.IsValidValue (newValue))
			{
				object value;
				
				if (newValue == null)
				{
					value = UndefinedValue.Instance;
				}
				else
				{
					value = (object) newValue;
				}

				this.InternalSetValue (id, value);
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Invalid value '{0}' specified for field {1}", newValue, id));
			}
		}

		
		
		public static TResult GetCalculation<T, TResult>(T entity, string id, System.Func<T, TResult> func)
		{
			return func (entity);
		}

		
		
		private object InternalGetValue(string id)
		{
			object value;

			if ((this.modifiedValues != null) &&
				(this.IsDefiningOriginalValues == false))
			{
				value = this.modifiedValues.GetValue (id);

				if ((this.originalValues != null) &&
					(UndefinedValue.IsUndefinedValue (value)))
				{
					value = this.originalValues.GetValue (id);
				}
			}
			else if (this.originalValues != null)
			{
				value = this.originalValues.GetValue (id);
			}
			else
			{
				value = UndefinedValue.Instance;
			}
			
			return value;
		}
		
		private void InternalSetValue(string id, object value)
		{
			if (this.IsDefiningOriginalValues)
			{
				if (this.originalValues == null)
				{
					this.originalValues = this.context.CreateValueStore (this);
				}

				this.originalValues.SetValue (id, value);
			}
			else
			{
				if (this.modifiedValues == null)
				{
					this.modifiedValues = this.context.CreateValueStore (this);
				}

				this.modifiedValues.SetValue (id, value);
			}
		}

		internal EntityCollection<T> CopyFieldCollection<T>(string id, EntityCollection<T> collection) where T : AbstractEntity
		{
			System.Diagnostics.Debug.Assert (this.IsDefiningOriginalValues == false);

			EntityCollection<T> copy = new EntityCollection<T> (id, this, false);

			copy.AddRange (collection);

			this.InternalSetValue (id, copy);

			return copy;
		}

		internal IStructuredTypeProvider GetStructuredTypeProvider()
		{
			return (this.originalValues ?? this.modifiedValues) as IStructuredTypeProvider;
		}

		private sealed class DefineOriginalValuesHelper : System.IDisposable
		{
			public DefineOriginalValuesHelper(AbstractEntity entity)
			{
				this.entity = entity;
				System.Threading.Interlocked.Increment (ref this.entity.defineOriginalValuesCount);
			}

			#region IDisposable Members

			public void Dispose()
			{
				System.Threading.Interlocked.Decrement (ref this.entity.defineOriginalValuesCount);
			}

			#endregion

			AbstractEntity entity;
		}


		private EntityContext context;
		private IValueStore originalValues;
		private IValueStore modifiedValues;
		private int defineOriginalValuesCount;
	}
}
