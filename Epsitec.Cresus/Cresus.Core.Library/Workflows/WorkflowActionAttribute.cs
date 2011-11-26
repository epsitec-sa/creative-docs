//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Workflows
{
	/// <summary>
	/// The <c>WorkflowActionAttribute</c> is used to tag a method which implements
	/// an action verb.
	/// </summary>
	
	[System.Serializable]
	[System.AttributeUsage (System.AttributeTargets.Method)]
	
	public class WorkflowActionAttribute : System.Attribute
	{
		public WorkflowActionAttribute()
		{
		}

		public WorkflowActionAttribute(string publishedAs)
		{
			this.PublishedName = publishedAs;
		}

		public string PublishedName
		{
			get;
			set;
		}

		public System.Type CollectionItemType
		{
			get;
			set;
		}
	}
}
