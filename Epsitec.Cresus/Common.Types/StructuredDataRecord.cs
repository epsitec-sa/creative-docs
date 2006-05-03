//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class StructuredDataRecord : StructuredDataField, IListHost<StructuredDataField>, IStructuredData
	{
		public StructuredDataRecord(string name) : base (name, null)
		{
			this.fields = new HostedList<StructuredDataField> (this);
		}



		#region IListHost<StructuredDataField> Members

		public HostedList<StructuredDataField> Items
		{
			get
			{
				return this.fields;
			}
		}

		void IListHost<StructuredDataField>.NotifyListInsertion(StructuredDataField item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		void IListHost<StructuredDataField>.NotifyListRemoval(StructuredDataField item)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion

		#region IStructuredData Members

		void IStructuredData.AttachListener(DependencyPropertyPath path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		void IStructuredData.DetachListener(DependencyPropertyPath path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		object IStructuredData.GetValue(DependencyPropertyPath path)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		void IStructuredData.SetValue(DependencyPropertyPath path, object value)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion

		private HostedList<StructuredDataField> fields;
	}
}
