//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The ObjectTreeSnapshot is used to compute differences between sets of
	/// object properties.
	/// </summary>
	public sealed class ObjectTreeSnapshot
	{
		public ObjectTreeSnapshot()
		{
		}

		public void Record(Object obj)
		{
			//	Record the state of all inherited properties.

			System.Type type = obj.GetType ();

			foreach (Types.Property property in obj.ObjectType.GetProperties ())
			{
				PropertyMetadata metadata = property.GetMetadata (type);

				if ((metadata != null) &&
					(metadata.InheritsValue))
				{
					this.list.Add (new SnapshotValue (obj, property));
				}
			}
		}
		public void Record(Object obj, Types.Property property)
		{
			System.Diagnostics.Debug.Assert (property == null);
			
			this.list.Add (new SnapshotValue (obj, property));
		}

		public void InvalidateDifferentProperties()
		{
			foreach (SnapshotValue snapshot in this.list)
			{
				object oldValue = snapshot.Value;
				object newValue = snapshot.Object.GetValue (snapshot.Property);
				
				if (oldValue == newValue)
				{
					//	Nothing changed, skip.
				}
				else if ((oldValue == null) ||
					/**/ (! oldValue.Equals (newValue)))
				{
					snapshot.Object.InvalidateProperty (snapshot.Property, oldValue, newValue);
				}
			}
		}
		
		#region Private SnapshotValue Structure
		private struct SnapshotValue
		{
			public SnapshotValue(Object obj, Types.Property property)
			{
				this.obj      = obj;
				this.property = property;
				this.value    = obj.GetValue (property);
			}
			
			public Object						Object
			{
				get
				{
					return this.obj;
				}
			}
			public Types.Property				Property
			{
				get
				{
					return this.property;
				}
			}
			public object						Value
			{
				get
				{
					return this.value;
				}
			}

			private Object						obj;
			private Types.Property				property;
			private object						value;
		}
		#endregion

		private List<SnapshotValue>				list = new List<SnapshotValue> ();
	}
}
