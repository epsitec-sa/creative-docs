//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The ObjectTreeSnapshot is used to compute differences between sets of
	/// object properties.
	/// </summary>
	public class ObjectTreeSnapshot
	{
		public ObjectTreeSnapshot()
		{
		}

		public void Record(Object obj)
		{
			//	Record the state of all inherited properties.

			System.Type type = obj.GetType ();

			foreach (Property property in obj.ObjectType.GetProperties ())
			{
				PropertyMetadata metadata = property.GetMetadata (type);

				if ((metadata != null) &&
					(metadata.InheritsValue))
				{
					this.list.Add (new SnapshotValue (obj, property));
				}
			}
		}
		public void Record(Object obj, Property property)
		{
			System.Diagnostics.Debug.Assert (property != null);

			this.list.Add (new SnapshotValue (obj, property));
		}
		public void Record(Object obj, Property property1, Property property2)
		{
			System.Diagnostics.Debug.Assert (property1 != null);
			System.Diagnostics.Debug.Assert (property2 != null);

			this.list.Add (new SnapshotValue (obj, property1));
			this.list.Add (new SnapshotValue (obj, property2));
		}

		public void RecordSubtree(Object root, Property property)
		{
			if (ObjectTree.GetHasChildren (root))
			{
				foreach (Object child in ObjectTree.GetChildren (root))
				{
					this.Record (child, property);
					this.RecordSubtree (child, property);
				}
			}
		}
		public void RecordSubtree(Object root, Property property1, Property property2)
		{
			if (ObjectTree.GetHasChildren (root))
			{
				foreach (Object child in ObjectTree.GetChildren (root))
				{
					this.Record (child, property1, property2);
					this.RecordSubtree (child, property1, property2);
				}
			}
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
			public SnapshotValue(Object obj, Property property)
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
			public Property						Property
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
			private Property					property;
			private object						value;
		}
		#endregion

		private List<SnapshotValue>				list = new List<SnapshotValue> ();
	}
}
