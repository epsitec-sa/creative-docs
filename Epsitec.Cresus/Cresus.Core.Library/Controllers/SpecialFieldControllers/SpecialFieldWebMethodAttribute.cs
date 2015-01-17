//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;

namespace Epsitec.Cresus.Core.Controllers.SpecialFieldControllers
{
	/// <summary>
	/// This attribute is used by the special field controller to annotate methods that can be
	/// called by the client to obtain data used for the field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class SpecialFieldWebMethodAttribute : Attribute
	{
	}
}
