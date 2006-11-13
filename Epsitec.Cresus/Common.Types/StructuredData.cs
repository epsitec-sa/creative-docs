//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using PropertyChangedEventHandler = Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs>;
	
	public class StructuredData : IStructuredTypeProvider, IStructuredData
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

		public int InternalGetValueCount()
		{
			return this.values == null ? -1 : this.values.Count;
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
			INamedType type;

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
				record  = new Record (record.Data, (PropertyChangedEventHandler) System.Delegate.Combine (record.Handler, handler));
			}
			else
			{
				record = new Record (UndefinedValue.Instance, handler);
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
				record  = new Record (record.Data, (PropertyChangedEventHandler) System.Delegate.Remove (record.Handler, handler));
				
				if ((record.Data == UndefinedValue.Instance) &&
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
					return Collections.EmptyEnumerable<string>.Instance;
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

		public object GetValue(string id)
		{
			INamedType type;
			
			if (!this.CheckFieldIdValidity (id, out type))
			{
				return UnknownValue.Instance;
			}
			
			Record value;
			
			if (this.values == null)
			{
				return this.GetUndefinedValue (type, id);
			}
			else if (this.values.TryGetValue (id, out value))
			{
				return value.Data;
			}
			else
			{
				return this.GetUndefinedValue (type, id);
			}
		}

		public void SetValue(string id, object value)
		{
			INamedType type;
			
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
						handler = record.Handler;

						if (handler == null)
						{
							this.values.Remove (id);
						}
						else
						{
							record = new Record (UndefinedValue.Instance, handler);
							this.values[id] = record;
						}
					}
				}
			}
			else
			{
				if (!this.CheckValueValidity (type, value))
				{
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
					handler = record.Handler;
					record  = new Record (value, handler);
				}
				else
				{
					record = new Record (value);
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

		protected virtual object GetUndefinedValue(INamedType namedType, string id)
		{
			AbstractType type = namedType as AbstractType;

			if (type != null)
			{
				object value;
				
				switch (this.undefinedValueMode)
				{
					case UndefinedValueMode.Undefined:
						value = UndefinedValue.Instance;
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
					(value == UndefinedValue.Instance))
				{
					IStructuredType structuredType = type as IStructuredType;

					if (structuredType != null)
					{
						StructuredData data = new StructuredData (StructuredType);

						data.UndefinedValueMode = this.UndefinedValueMode;

						value = data;
					}
				}
				
				return value;
			}

			return UndefinedValue.Instance;
		}

		protected virtual bool CheckFieldIdValidity(string id, out INamedType type)
		{
			if (this.type == null)
			{
				//	No checking done, as there is no schema.

				type = null;
			}
			else
			{
				type = this.type.GetField (id).Type;

				if (type == null)
				{
					return false;
				}
			}
			
			return true;
		}

		protected virtual bool CheckValueValidity(object type, object value)
		{
			if (type == null)
			{
				return true;
			}

			return TypeRosetta.IsValidValue (value, type);
		}
		
		protected virtual void AllocateValues()
		{
			this.values = new Collections.HostedDictionary<string, Record> (this.NotifyInsertion, this.NotifyRemoval);
		}

		protected virtual void NotifyInsertion(string id, Record value)
		{
		}

		protected virtual void NotifyRemoval(string id, Record value)
		{
		}

		protected virtual void InvalidateValue(string id, object oldValue, object newValue, PropertyChangedEventHandler handler)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("{0}: {1} --> {2}", id, oldValue, newValue));
			
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
			public Record(object data)
			{
				this.data = data;
				this.handler = null;
			}

			public Record(object data, PropertyChangedEventHandler handler)
			{
				this.data = data;
				this.handler = handler;
			}
			
			public object Data
			{
				get
				{
					return this.data;
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
			private PropertyChangedEventHandler handler;
		}

		#endregion

		public event PropertyChangedEventHandler ValueChanged;

		private IStructuredType type;
		private IDictionary<string, Record> values;
		private UndefinedValueMode undefinedValueMode;
	}
}
