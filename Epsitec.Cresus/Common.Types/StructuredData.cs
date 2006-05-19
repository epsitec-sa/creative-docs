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
				//	TODO: ...

				throw new System.NotImplementedException ();
			}
			else
			{
				return this.type.GetFieldNames ();
			}
		}

		public object GetValue(string name)
		{
			return null;
		}

		public void SetValue(string name, object value)
		{
		}

		public bool HasImmutableRoots
		{
			get
			{
				return true;
			}
		}

		#endregion

		private StructuredType type;
	}
}
