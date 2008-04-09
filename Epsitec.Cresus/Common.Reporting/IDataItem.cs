//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	public interface IDataItem
	{
		/// <summary>
		/// Gets the raw object value.
		/// </summary>
		/// <value>The raw object value.</value>
		object ObjectValue
		{
			get;
		}
		
		string Value
		{
			get;
		}

		int Count
		{
			get;
		}

		DataItemClass ItemClass
		{
			get;
		}

		DataItemType ItemType
		{
			get;
		}

		INamedType DataType
		{
			get;
		}
	}
}
