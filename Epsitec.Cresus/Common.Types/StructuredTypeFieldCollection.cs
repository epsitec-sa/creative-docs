//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	internal class StructuredTypeFieldCollection : ICollection<StructuredTypeField>
	{
		public StructuredTypeFieldCollection(StructuredType owner)
		{
			this.owner = owner;
		}

		#region ICollection<StructuredTypeField> Members

		void ICollection<StructuredTypeField>.Add(StructuredTypeField field)
		{
			this.owner.AddField (field.Name, field.Type);
		}

		void ICollection<StructuredTypeField>.Clear()
		{
			this.owner.Fields.Clear ();
		}

		bool ICollection<StructuredTypeField>.Contains(StructuredTypeField item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		void ICollection<StructuredTypeField>.CopyTo(StructuredTypeField[] array, int arrayIndex)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		int ICollection<StructuredTypeField>.Count
		{
			get
			{
				return this.owner.Fields.Count;
			}
		}

		bool ICollection<StructuredTypeField>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool ICollection<StructuredTypeField>.Remove(StructuredTypeField item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<StructuredTypeField> Members

		IEnumerator<StructuredTypeField> IEnumerable<StructuredTypeField>.GetEnumerator()
		{
			if (this.cachedFields == null)
			{
				this.cachedFields = new List<StructuredTypeField> ();
				
				foreach (KeyValuePair<string, INamedType> item in this.owner.Fields)
				{
					this.cachedFields.Add (new StructuredTypeField (item));
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
			if (this.cachedFields == null)
			{
				this.cachedFields = new List<StructuredTypeField> ();

				foreach (KeyValuePair<string, INamedType> item in this.owner.Fields)
				{
					this.cachedFields.Add (new StructuredTypeField (item));
				}
			}

			foreach (StructuredTypeField field in this.cachedFields)
			{
				yield return field;
			}
		}

		#endregion

		private StructuredType					owner;
		private List<StructuredTypeField>		cachedFields;
	}
}
