//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	internal enum BindingSourceType : byte
	{
		None,
		
		PropertyObject,
		StructuredData,
		SourceItself,
		Resource
	}
}
