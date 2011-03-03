//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	[DesignerVisible]
	public enum UnitOfMeasureCategory
	{
		None      = 0,

		Unrelated = 10,
		Unit      = 20,
		
		Mass      = 30,
		
		Length    = 40,
		Surface   = 41,
		Volume    = 42,
		
		Time      = 50,
		
		Energy    = 60,
	}
}
