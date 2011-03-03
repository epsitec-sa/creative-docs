//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Document.Verbose
{
	public enum DocumentOptionValueType
	{
		Undefined,
		Boolean,		// valeur "true" ou "false"
		Enumeration,	// �num�ration libre
		Distance,		// type 'double' correspondant � une distance en mm
	}
}
