//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Workflows
{
	/// <summary>
	/// The <c>ActionAttribute</c> is used to tag a method which implements
	/// an action verb.
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Method)]
	public class ActionAttribute : System.Attribute
	{
		public ActionAttribute()
		{
		}

		public ActionAttribute(string publishedAs)
		{
			this.PublishedName = publishedAs;
		}

		public string PublishedName
		{
			get;
			set;
		}
	}
}
