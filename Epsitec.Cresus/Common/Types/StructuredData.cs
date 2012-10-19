//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	public class StructuredData : IStructuredTypeProvider, IStructuredData, System.IEquatable<StructuredData>
	{
		public StructuredData() : this (null)
		{
		}
		
		public StructuredData(IStructuredType type)
		{
			this.type = type;
		}

		public IStructuredType StructuredType
		{
			get
			{
				if (this.type == null)
				{
					return new DynamicStructuredType (this);
				}
				else
				{
					return this.type;
				}
			}
		}

		public UndefinedValueMode UndefinedValueMode
		{
			get
			{
				return this.undefinedValueMode;
			}
			set
			{
				this.undefinedValueMode = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is empty.
		/// </summary>
		/// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
		public bool IsEmpty
		{
			get
			{
				if ((this.values == null) ||
					(this.values.Count == 0))
				{
					return true;
				}

				//	Check that all values are undefined (or empty collections if they
				//	happen to be locked fields)

				foreach (Record record in this.values.Values)
				{
					if (UndefinedValue.IsUndefinedValue (record.Data))
					{
						continue;
					}

					if (!record.IsReadOnly)
					{
						return false;
					}
					
					System.Collections.ICollection collection = record.Data as System.Collections.ICollection;
					
					if ((collection == null) ||
						(collection.Count > 0))
					{
						return false;
					}
				}
				
				return true;
			}
		}

		public int InternalGetValueCount()
		{
			return this.values == null ? -1 : this.values.Count;
		}

		public void FillWithDefaultValues()
		{
			IStructuredType type = this.StructuredType;

			if (type != null)
			{
				foreach (string fieldId in type.GetFieldIds ())
				{
					StructuredTypeField field = type.GetField (fieldId);
					AbstractType fieldType = field.Type as AbstractType;

					if (fieldType != null)
					{
						this.SetValue (fieldId, fieldType.DefaultValue);
					}
				}
			}
		}

		public virtual void CopyContentsFrom(StructuredData data)
		{
			IStructuredType type = this.StructuredType;

			if (type != null)
			{
				foreach (string fieldId in type.GetFieldIds ())
				{
					object value = data.GetValue (fieldId);

					if ((UndefinedValue.IsUndefinedValue (value)) ||
						(UnknownValue.IsUnknownValue (value)))
					{
						//	Skip...
					}
					else
					{
						this.SetValue (fieldId, value);
					}
				}
			}
		}

		public virtual StructuredData CreateEmptyCopy()
		{
			return new StructuredData (this.StructuredType);
		}

		public StructuredData GetShallowCopy()
		{
			StructuredData data = this.CreateEmptyCopy ();

			data.CopyContentsFrom (this);

			return data;
		}

		public void DefineStructuredType(IStructuredType type)
		{
			if ((this.type == null) ||
				(this.type == type))
			{
				this.type = type;
			}
			else
			{
				throw new System.InvalidOperationException ("StructuredType cannot be redefined");
			}
		}

		public IEnumerable<KeyValuePair<string, object>> GetKeyValuePairs()
		{
			if (this.values != null)
			{
				foreach (string id in this.GetValueIds ())
				{
					object value = this.GetValue (id);

					if (UndefinedValue.IsUndefinedValue (value))
					{
						continue;
					}

					yield return new KeyValuePair<string, object> (id, value);
				}
			}
		}

		#region IStructuredTypeProvider Members

		IStructuredType IStructuredTypeProvider.GetStructuredType()
		{
			return this.StructuredType;
		}

		#endregion

		#region IStructuredData Members

		public void AttachListener(string id, PropertyChangedEventHandler handler)
		{
			StructuredTypeField type;

			if (!this.CheckFieldIdValidity (id, out type))
			{
				throw new System.Collections.Generic.KeyNotFoundException (string.Format ("The value '{0}' does not exist; it is not defined by the structure", id));
			}
			
			if (this.values == null)
			{
				this.AllocateValues ();
			}

			if (this.values == null)
			{
				throw new System.InvalidOperationException ("Cannot attach a listener; no storage defined");
			}

			Record record;

			if (this.values.TryGetValue (id, out record))
			{
				handler = (PropertyChangedEventHandler) System.Delegate.Combine (record.Handler, handler);
				record  = new Record (record.Data, record.OriginalData, record.UsesOriginalData, record.IsReadOnly, handler);
			}
			else
			{
				record = new Record (UndefinedValue.Value, false, handler);
			}

			this.values[id] = record;
		}

		public void DetachListener(string id, PropertyChangedEventHandler handler)
		{
			if (this.values == null)
			{
				return;
			}
			
			Record record;

			if (this.values.TryGetValue (id, out record))
			{
				handler = (PropertyChangedEventHandler) System.Delegate.Remove (record.Handler, handler);
				record  = new Record (record.Data, record.OriginalData, record.UsesOriginalData, record.IsReadOnly, handler);
				
				if ((record.Data == UndefinedValue.Value) &&
					(record.OriginalData == UndefinedValue.Value) &&
					(record.Handler == null))
				{
					this.values.Remove (id);
				}
				else
				{
					this.values[id] = record;
				}
			}
		}

		public IEnumerable<string> GetValueIds()
		{
			if (this.type == null)
			{
				if (this.values == null)
				{
					return Enumerable.Empty<string> ();
				}
				else
				{
					string[] ids = new string[this.values.Count];
					this.values.Keys.CopyTo (ids, 0);
					System.Array.Sort (ids);
					
					return ids;
				}
			}
			else
			{
				return this.type.GetFieldIds ();
			}
		}

		public void SetValue(string id, object value)
		{
			StructuredTypeField type;

			if (!this.CheckFieldIdValidity (id, out type))
			{
				throw new System.Collections.Generic.KeyNotFoundException (string.Format ("The value '{0}' cannot be set; it is not defined by the structure", id));
			}

			if (UnknownValue.IsUnknownValue (value))
			{
				throw new System.ArgumentException ("UnknownValue specified");
			}

			object oldValue = this.GetValue (id);
			PropertyChangedEventHandler handler = null;

			if (UndefinedValue.IsUndefinedValue (value))
			{
				if (this.values != null)
				{
					Record record;

					if (this.values.TryGetValue (id, out record))
					{
						if ((record.IsReadOnly) &&
							(!UndefinedValue.IsUndefinedValue (record.Data)))
						{
							throw new System.InvalidOperationException (string.Format ("Field {0} is read only", id));
						}

						handler = record.Handler;

						if ((handler == null) &&
							(record.OriginalData == UndefinedValue.Value))
						{
							this.values.Remove (id);
						}
						else
						{
							record = new Record (UndefinedValue.Value, record.OriginalData, false, false, handler);
							this.values[id] = record;
						}
					}
				}
			}
			else
			{
				if (!this.CheckValueValidity (type, value))
				{
					System.Diagnostics.Debug.Fail (string.Format ("The value '{0}' has the wrong type or is not valid", id));
					throw new System.ArgumentException (string.Format ("The value '{0}' has the wrong type or is not valid", id));
				}

				if (this.values == null)
				{
					this.AllocateValues ();
				}

				if (this.values == null)
				{
					throw new System.InvalidOperationException ("Cannot set a value; no storage defined");
				}

				Record record;

				if (this.values.TryGetValue (id, out record))
				{
					if ((record.IsReadOnly) &&
						(!object.Equals (value, record.Data)))
					{
						throw new System.InvalidOperationException (string.Format ("Field {0} is read only", id));
					}

					handler = record.Handler;
					record  = new Record (value, record.OriginalData, false, record.IsReadOnly, handler);
				}
				else
				{
					record = new Record (value, false);
				}

				this.values[id] = record;
			}

			object newValue = this.GetValue (id);

			if (oldValue == newValue)
			{
			}
			else if ((oldValue == null) || (!oldValue.Equals (newValue)))
			{
				this.InvalidateValue (id, oldValue, newValue, handler);
			}
		}

		#endregion

		#region IValueStore Interface Members

		public object GetValue(string id)
		{
			bool usesOriginalData;
			return this.GetValue (id, out usesOriginalData);
		}

		void IValueStore.SetValue(string id, object value, ValueStoreSetMode mode)
		{
			this.SetValue (id, value);
		}

		#endregion

		#region IEquatable<StructuredData> Members

		public bool Equals(StructuredData other)
		{
			if (other == null)
			{
				return false;
			}

			if (Collection.CompareEqual (this.GetKeyValuePairs (), other.GetKeyValuePairs (),
				delegate (KeyValuePair<string, object> value1, KeyValuePair<string, object> value2)
				{
					if (value1.Key != value2.Key)
					{
						return false;
					}
					if (value1.Value == value2.Value)
					{
						return true;
					}
					if ((value1.Value == null) ||
						(value2.Value == null))
					{
						return false;
					}

					return Comparer.Equal (value1.Value, value2.Value);
				}))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		public override bool Equals(object obj)
		{
			return this.Equals (obj as StructuredData);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public T GetValueOrDefault<T>(Support.Druid id)
		{
			object value = this.GetValue (id);

			if (UndefinedValue.IsUndefinedValue (value))
			{
				return default (T);
			}
			else
			{
				return (T) value;
			}
		}
		
		public object GetValue(Support.Druid id)
		{
			return this.GetValue (id.ToString ());
		}

		public object GetValue(Support.Druid id, out bool usesOriginalData)
		{
			return this.GetValue (id.ToString (), out usesOriginalData);
		}

		public object GetValue(string id, out bool usesOriginalData)
		{
			return this.GetValue (id, DataVersion.Default, out usesOriginalData);
		}

		public object GetOriginalValue(Support.Druid id)
		{
			return this.GetOriginalValue (id.ToString ());
		}

		public object GetOriginalValue(string id)
		{
			bool usesOriginalData;
			return this.GetValue (id, DataVersion.Original, out usesOriginalData);
		}


		private enum DataVersion
		{
			Default,
			Original
		}

		private object GetValue(string id, DataVersion version, out bool usesOriginalData)
		{
			StructuredTypeField type;

			if (!this.CheckFieldIdValidity (id, out type))
			{
				usesOriginalData = true;
				return UnknownValue.Value;
			}

			Record record;

			if (this.values == null)
			{
				usesOriginalData = true;

				if (version == DataVersion.Original)
				{
					return UndefinedValue.Value;
				}
				else
				{
					return this.GetUndefinedValue (type, id);
				}
			}
			else if (this.values.TryGetValue (id, out record))
			{
				usesOriginalData = record.UsesOriginalData;

				if (version == DataVersion.Original)
				{
					return record.OriginalData;
				}
				else
				{
					if (UndefinedValue.IsUndefinedValue (record.Data))
					{
						return this.GetUndefinedValue (type, id);
					}
					else
					{
						return record.Data;
					}
				}
			}
			else
			{
				usesOriginalData = true;

				if (version == DataVersion.Original)
				{
					return UndefinedValue.Value;
				}
				else
				{
					return this.GetUndefinedValue (type, id);
				}
			}
		}

		public void ResetToOriginalValue(string id)
		{
			if (this.values != null)
			{
				Record record;

				if (this.values.TryGetValue (id, out record))
				{
					if ((record.IsReadOnly) &&
						(!UndefinedValue.IsUndefinedValue (record.Data)))
					{
						throw new System.InvalidOperationException (string.Format ("Field {0} is read only", id));
					}

					if (record.UsesOriginalData)
					{
						return;
					}

					object oldValue = record.Data;
					object newValue = record.OriginalData;

					this.values[id] = new Record (UndefinedValue.Value, record.OriginalData, true, record.IsReadOnly, record.Handler);
					
					this.InvalidateValue (id, oldValue, newValue, record.Handler);
				}
			}
		}

		public void ResetToOriginalValue(Support.Druid id)
		{
			this.ResetToOriginalValue (id.ToString ());
		}

		public void ResetToOriginal()
		{
			foreach (string id in this.StructuredType.GetFieldIds ())
			{
				this.ResetToOriginalValue (id);
			}
		}

		public void PromoteToOriginal()
		{
			foreach (string id in this.StructuredType.GetFieldIds ())
			{
				this.PromoteToOriginalValue (id);
			}
		}
		
		public bool PromoteToOriginalValue(string id)
		{
			if (this.values != null)
			{
				Record record;

				if (this.values.TryGetValue (id, out record))
				{
					this.values[id] = new Record (UndefinedValue.Value, record.Data, true, record.IsReadOnly, record.Handler);
					return true;
				}
			}

			return false;
		}

		public bool CopyOriginalToCurrentValue(Support.Druid id)
		{
			return this.CopyOriginalToCurrentValue (id.ToString ());
		}

		public bool CopyOriginalToCurrentValue(string id)
		{
			if (this.values != null)
			{
				Record record;

				if (this.values.TryGetValue (id, out record))
				{
					if ((record.IsReadOnly) &&
						(!UndefinedValue.IsUndefinedValue (record.Data)))
					{
						throw new System.InvalidOperationException (string.Format ("Field {0} is read only", id));
					}

					this.values[id] = new Record (record.OriginalData, record.OriginalData, false, record.IsReadOnly, record.Handler);
					return true;
				}
			}

			return false;
		}

		public void SilentlyCopyValueFrom(Support.Druid id, StructuredData source)
		{
			this.SilentlyCopyValueFrom (id.ToString (), source);
		}

		public void SilentlyCopyValueFrom(string id, StructuredData source)
		{
			if (this.values == null)
			{
				this.AllocateValues ();
			}

			Record srcRecord;
			Record dstRecord;

			if ((source.values != null) &&
				(source.values.TryGetValue (id, out srcRecord)))
			{
				if (this.values.TryGetValue (id, out dstRecord))
				{
					this.values[id] = new Record (srcRecord.Data, srcRecord.OriginalData, srcRecord.UsesOriginalData, srcRecord.IsReadOnly, dstRecord.Handler);
				}
				else
				{
					this.values[id] = new Record (srcRecord.Data, srcRecord.OriginalData, srcRecord.UsesOriginalData, srcRecord.IsReadOnly, null);
				}
			}
			else
			{
				System.Diagnostics.Debug.Fail ("Unexpected empty source");
			}
		}
		
		public void SetValue(Support.Druid id, object value)
		{
			this.SetValue (id.ToString (), value);
		}

		public bool LockValue(Support.Druid id)
		{
			return this.LockValue (id.ToString ());
		}

		public bool LockValue(string id)
		{
			Record record;

			if ((this.values != null) &&
				(this.values.TryGetValue (id, out record)))
			{
				if ((!UndefinedValue.IsUndefinedValue (record.Data)) &&
					(record.IsReadOnly == false))
				{
					this.values[id] = new Record (record.Data, record.OriginalData, record.UsesOriginalData, true, record.Handler);
					return true;
				}
			}

			return false;
		}

		public bool UnlockValue(Support.Druid id)
		{
			return this.UnlockValue (id.ToString ());
		}

		public bool UnlockValue(string id)
		{
			Record record;

			if ((this.values != null) &&
				(this.values.TryGetValue (id, out record)))
			{
				if ((!UndefinedValue.IsUndefinedValue (record.Data)) &&
					(record.IsReadOnly))
				{
					this.values[id] = new Record (record.Data, record.OriginalData, record.UsesOriginalData, false, record.Handler);
					return true;
				}
			}

			return false;
		}

		public bool IsValueLocked(Support.Druid id)
		{
			return this.IsValueLocked (id.ToString ());
		}

		public bool IsValueLocked(string id)
		{
			Record record;

			if ((this.values != null) &&
				(this.values.TryGetValue (id, out record)))
			{
				return record.IsReadOnly;
			}
			else
			{
				return false;
			}
		}

		public static Support.PropertyGetter CreatePropertyGetter(string propertyName)
		{
			return delegate (object obj)
			{
				IStructuredData data = obj as IStructuredData;
				return data.GetValue (propertyName);
			};
		}

		public static Support.PropertySetter CreatePropertySetter(string propertyName)
		{
			return delegate (object obj, object value)
			{
				IStructuredData data = obj as IStructuredData;
				data.SetValue (propertyName, value);
			};
		}

		public static Support.PropertyComparer CreatePropertyComparer(string propertyName)
		{
			return delegate (object objA, object objB)
			{
				IStructuredData dataA = objA as IStructuredData;
				IStructuredData dataB = objB as IStructuredData;

				object a = dataA.GetValue (propertyName);
				object b = dataB.GetValue (propertyName);

				return System.Collections.Comparer.Default.Compare (a, b);
			};
		}

		protected virtual object GetUndefinedValue(StructuredTypeField fieldType, string id)
		{
			if (fieldType == null)
			{
				return UndefinedValue.Value;
			}

			AbstractType type = fieldType.Type as AbstractType;
			FieldRelation relation = fieldType.Relation;

			if (type != null)
			{
				object value;
				
				switch (this.undefinedValueMode)
				{
					case UndefinedValueMode.Undefined:
						value = UndefinedValue.Value;
						break;

					case UndefinedValueMode.Default:
						value = type.DefaultValue;
						break;

					case UndefinedValueMode.Sample:
						value = type.SampleValue;
						break;

					default:
						throw new System.NotSupportedException (string.Format ("Mode {0} not supported", this.undefinedValueMode));
				}

				if ((value == null) ||
					(value == UndefinedValue.Value))
				{
					if (this.undefinedValueMode != UndefinedValueMode.Undefined)
					{
						IStructuredType structuredType = type as IStructuredType;

						if (structuredType != null)
						{
							if (relation == FieldRelation.Collection)
							{
								List<StructuredData> list = new List<StructuredData> ();
								for (int i = 0; i < 3; i++)
								{
									StructuredData data = new StructuredData (structuredType);
									data.UndefinedValueMode = this.UndefinedValueMode;
									list.Add (data);
								}
								value = list;
							}
							else
							{
								StructuredData data = new StructuredData (structuredType);
								data.UndefinedValueMode = this.UndefinedValueMode;
								value = data;
							}
						}
					}
				}
				
				return value;
			}

			return UndefinedValue.Value;
		}

		protected virtual bool CheckFieldIdValidity(string id, out StructuredTypeField type)
		{
			if (this.type == null)
			{
				//	No checking done, as there is no schema.

				type = null;
			}
			else
			{
				type = this.type.GetField (id);

				if (type == null)
				{
					return false;
				}
			}
			
			return true;
		}

		protected virtual bool CheckValueValidity(StructuredTypeField field, object value)
		{
			if (field == null)
			{
				return true;
			}

			return TypeRosetta.IsValidValue (value, field);
		}
		
		protected virtual void AllocateValues()
		{
			this.values = new Collections.HostedDictionary<string, Record> (this.NotifyInsertion, this.NotifyRemoval);
		}

		protected virtual void NotifyInsertion(string id, Record record)
		{
		}

		protected virtual void NotifyRemoval(string id, Record record)
		{
		}

		protected virtual void InvalidateValue(string id, object oldValue, object newValue, PropertyChangedEventHandler handler)
		{
			if (handler != null)
			{
				DependencyPropertyChangedEventArgs e = new DependencyPropertyChangedEventArgs (id, oldValue, newValue);
				handler (this, e);
			}

			this.OnValueChanged (id, oldValue, newValue);
		}

		protected virtual void OnValueChanged(string id, object oldValue, object newValue)
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged (this, new DependencyPropertyChangedEventArgs (id, oldValue, newValue));
			}
		}

		#region Record Structure

		protected struct Record
		{
			public Record(object data, bool readOnly)
			{
				this.data = data;
				this.originalData = UndefinedValue.Value;
				this.handler = null;
				this.readOnly = readOnly;
				this.usesOriginalData = false;
			}

			public Record(object data, bool readOnly, PropertyChangedEventHandler handler)
			{
				this.data = data;
				this.originalData = UndefinedValue.Value;
				this.handler = handler;
				this.readOnly = readOnly;
				this.usesOriginalData = false;
			}

			public Record(object data, object originalData, bool usesOriginalData, bool readOnly, PropertyChangedEventHandler handler)
			{
				this.data = data;
				this.originalData = originalData;
				this.handler = handler;
				this.readOnly = readOnly;
				this.usesOriginalData = usesOriginalData;
			}

			public object Data
			{
				get
				{
					return this.usesOriginalData ? this.originalData : this.data;
				}
			}

			public object OriginalData
			{
				get
				{
					return this.originalData;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return this.readOnly;
				}
			}

			public bool UsesOriginalData
			{
				get
				{
					return this.usesOriginalData;
				}
			}

			public PropertyChangedEventHandler Handler
			{
				get
				{
					return this.handler;
				}
			}
			
			private object data;
			private object originalData;
			private bool readOnly;
			private bool usesOriginalData;
			private PropertyChangedEventHandler handler;
		}

		#endregion

		public event PropertyChangedEventHandler ValueChanged;

		private IStructuredType type;
		private IDictionary<string, Record> values;
		private UndefinedValueMode undefinedValueMode;
	}
}
