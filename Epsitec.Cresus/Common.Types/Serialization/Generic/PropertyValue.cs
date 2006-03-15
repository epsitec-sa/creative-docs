//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization.Generic
{
	public struct PropertyValue<T>
	{
		public PropertyValue(DependencyProperty property, T value)
		{
			this.property = property;
			this.value = value;
			this.isDataBound = false;
		}
		public PropertyValue(DependencyProperty property, T value, bool isDataBound)
		{
			this.property = property;
			this.value = value;
			this.isDataBound = isDataBound;
		}

		public DependencyProperty				Property
		{
			get
			{
				return this.property;
			}
		}
		public T								Value
		{
			get
			{
				return this.value;
			}
		}
		public bool								IsDataBound
		{
			get
			{
				return this.isDataBound;
			}
			set
			{
				this.isDataBound = value;
			}
		}

		private DependencyProperty property;
		private T value;
		private bool isDataBound;
	}
}
