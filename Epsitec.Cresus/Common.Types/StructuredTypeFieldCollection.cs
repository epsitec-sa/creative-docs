//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class StructuredTypeFieldCollection : ICollection<DependencyObject>
	{
		public StructuredTypeFieldCollection(StructuredType owner)
		{
			this.owner = owner;
		}

		#region ICollection<DependencyObject> Members

		void ICollection<DependencyObject>.Add(DependencyObject item)
		{
			StructuredTypeField field = item as StructuredTypeField;

			if (!field.IsFullyDefined)
			{
				if (this.pendingFields == null)
				{
					this.pendingFields = new List<StructuredTypeField> ();
				}

				this.pendingFields.Add (field);
				field.DefineContainer (this);
			}
			else
			{
				this.owner.AddField (field.Name, field.Type);
			}
		}

		void ICollection<DependencyObject>.Clear()
		{
			this.owner.Fields.Clear ();
		}

		bool ICollection<DependencyObject>.Contains(DependencyObject item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		void ICollection<DependencyObject>.CopyTo(DependencyObject[] array, int arrayIndex)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		int ICollection<DependencyObject>.Count
		{
			get
			{
				return this.owner.Fields.Count;
			}
		}

		bool ICollection<DependencyObject>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection<DependencyObject>.Remove(DependencyObject item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<DependencyObject> Members

		IEnumerator<DependencyObject> IEnumerable<DependencyObject>.GetEnumerator()
		{
			if (this.cachedFields == null)
			{
				this.cachedFields = new List<StructuredTypeField> ();
				
				foreach (KeyValuePair<string, INamedType> item in this.owner.Fields)
				{
					this.cachedFields.Add (new StructuredTypeField (item.Key, item.Value));
				}
			}
			
			foreach (StructuredTypeField field in this.cachedFields)
			{
				yield return field;
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion

		internal void NotifyFieldFullyDefined(StructuredTypeField field)
		{
			System.Diagnostics.Debug.Assert (this.pendingFields != null);
			System.Diagnostics.Debug.Assert (this.pendingFields.Contains (field));

			this.pendingFields.Remove (field);
			this.owner.AddField (field.Name, field.Type);

			field.DefineContainer (null);
		}

		private StructuredType owner;
		private List<StructuredTypeField> pendingFields;
		private List<StructuredTypeField> cachedFields;
	}
}
