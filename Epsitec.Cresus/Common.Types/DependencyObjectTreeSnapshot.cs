//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The DependencyObjectTreeSnapshot is used to compute differences between sets of
	/// object properties.
	/// </summary>
	public class DependencyObjectTreeSnapshot
	{
		public DependencyObjectTreeSnapshot()
		{
		}

		public void Record(DependencyObject obj)
		{
			//	Record the state of all inherited properties.

			System.Type type = obj.GetType ();

			foreach (DependencyProperty property in obj.ObjectType.GetProperties ())
			{
				DependencyPropertyMetadata metadata = property.GetMetadata (type);

				if ((metadata != null) &&
					(metadata.InheritsValue))
				{
					this.list.Add (new SnapshotValue (obj, property));
				}
			}
		}
		public void Record(DependencyObject obj, DependencyProperty property)
		{
			System.Diagnostics.Debug.Assert (property != null);

			this.list.Add (new SnapshotValue (obj, property));
		}
		public void Record(DependencyObject obj, DependencyProperty property1, DependencyProperty property2)
		{
			System.Diagnostics.Debug.Assert (property1 != null);
			System.Diagnostics.Debug.Assert (property2 != null);

			this.list.Add (new SnapshotValue (obj, property1));
			this.list.Add (new SnapshotValue (obj, property2));
		}

		public void RecordSubtree(DependencyObject root, DependencyProperty property)
		{
			if (DependencyObjectTree.GetHasChildren (root))
			{
				foreach (DependencyObject child in DependencyObjectTree.GetChildren (root))
				{
					this.Record (child, property);
					this.RecordSubtree (child, property);
				}
			}
		}
		public void RecordSubtree(DependencyObject root, DependencyProperty property1, DependencyProperty property2)
		{
			if (DependencyObjectTree.GetHasChildren (root))
			{
				foreach (DependencyObject child in DependencyObjectTree.GetChildren (root))
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
		
		public ChangeRecord[] GetChanges()
		{
			List<ChangeRecord> records = new List<ChangeRecord> ();
			
			foreach (SnapshotValue snapshot in this.list)
			{
				object oldValue = snapshot.Value;
				object newValue = snapshot.Object.GetValue (snapshot.Property);

				if (oldValue == newValue)
				{
					//	Nothing changed, skip.
				}
				else if ((oldValue == null) ||
					/**/ (!oldValue.Equals (newValue)))
				{
					records.Add (new ChangeRecord (snapshot.Object, snapshot.Property, oldValue, newValue));
				}
			}
			
			return records.ToArray ();
		}

		#region ChangeRecord Structure
		public struct ChangeRecord
		{
			public ChangeRecord(DependencyObject obj, DependencyProperty property, object oldValue, object newValue)
			{
				this.obj = obj;
				this.property = property;
				this.oldValue = oldValue;
				this.newValue = newValue;
			}
			
			public DependencyObject				Object
			{
				get
				{
					return this.obj;
				}
			}
			public DependencyProperty			Property
			{
				get
				{
					return this.property;
				}
			}
			public object						OldValue
			{
				get
				{
					return this.oldValue;
				}
			}
			public object						NewValue
			{
				get
				{
					return this.newValue;
				}
			}
			
			private DependencyObject			obj;
			private DependencyProperty			property;
			private object						oldValue;
			private object						newValue;
		}
		#endregion

		#region Private SnapshotValue Structure
		private struct SnapshotValue
		{
			public SnapshotValue(DependencyObject obj, DependencyProperty property)
			{
				this.obj      = obj;
				this.property = property;
				this.value    = obj.GetValue (property);
			}
			
			public DependencyObject				Object
			{
				get
				{
					return this.obj;
				}
			}
			public DependencyProperty			Property
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

			private DependencyObject			obj;
			private DependencyProperty			property;
			private object						value;
		}
		#endregion

		private List<SnapshotValue>				list = new List<SnapshotValue> ();
	}
}
