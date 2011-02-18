//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		private DependencyProperty				property;
		private T								value;
	}
}
