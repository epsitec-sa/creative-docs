//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>BusinessRuleAttribute</c> is used to tag a <see cref="GenericBusinessRule"/> derived
	/// class in order to define what type of rule it implements.
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Class)]
	public class BusinessRuleAttribute : System.Attribute
	{
		public BusinessRuleAttribute()
		{
		}
	}
}
