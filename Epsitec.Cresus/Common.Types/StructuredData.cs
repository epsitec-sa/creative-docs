//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class StructuredData : IStructuredTypeProvider, IStructuredData
	{
		public StructuredData() : this (null)
		{
		}
		
		public StructuredData(StructuredType type)
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

		#region IStructuredTypeProvider Members

		IStructuredType IStructuredTypeProvider.GetStructuredType()
		{
			return this.StructuredType;
		}

		#endregion

		#region IStructuredData Members

		public void AttachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		public void DetachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		public string[] GetValueNames()
		{
			if (this.type == null)
			{
				if (this.values == null)
				{
					return new string[0];
				}
				else
				{
					string[] names = new string[this.values.Count];
					this.values.Keys.CopyTo (names, 0);
					System.Array.Sort (names);
					
					return names;
				}
			}
			else
			{
				return this.type.GetFieldNames ();
			}
		}

		public object GetValue(string name)
		{
			object type;
			
			if (! this.CheckNameValidity (name, out type))
			{
				throw new System.Collections.Generic.KeyNotFoundException (string.Format ("The value '{0}' cannot be get; it is not defined by the structure", name));
			}
			
			object value;
			
			if (this.values == null)
			{
				return UndefinedValue.Instance;
			}
			else if (this.values.TryGetValue (name, out value))
			{
				return value;
			}
			else
			{
				return UndefinedValue.Instance;
			}
		}

		public void SetValue(string name, object value)
		{
			object type;
			
			if (!this.CheckNameValidity (name, out type))
			{
				throw new System.Collections.Generic.KeyNotFoundException (string.Format ("The value '{0}' cannot be set; it is not defined by the structure", name));
			}
			if (!this.CheckValueValidity (type, value))
			{
				throw new System.ArgumentException (string.Format ("The value '{0}' has the wrong type", name));
			}
			
			if (this.values == null)
			{
				this.AllocateValues ();
			}

			if (this.values == null)
			{
				throw new System.InvalidOperationException ("Cannot set a value; no storage defined");
			}
			
			this.values[name] = value;
		}

		public bool HasImmutableRoots
		{
			get
			{
				return true;
			}
		}

		#endregion

		protected virtual bool CheckNameValidity(string name, out object type)
		{
			if (this.type == null)
			{
				//	No checking done, as there is no schema.

				type = null;
			}
			else
			{
				type = this.type.GetFieldTypeObject (name);

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
			
			//	TODO: verify compatibility

			return true;
		}
		
		protected virtual void AllocateValues()
		{
			this.values = new HostedDictionary<string, object> (this.NotifyInsertion, this.NotifyRemoval);
		}

		protected virtual void NotifyInsertion(string name, object value)
		{
		}

		protected virtual void NotifyRemoval(string name, object value)
		{
		}

		private StructuredType type;
		private IDictionary<string, object> values;
	}
}
