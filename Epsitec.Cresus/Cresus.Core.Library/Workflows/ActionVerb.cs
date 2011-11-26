//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Workflows
{
	public struct ActionVerb
	{
		public ActionVerb(System.Reflection.MemberInfo memberInfo)
		{
			this.name       = memberInfo.Name;
			this.memberInfo = memberInfo;
			this.attribute  = null;
		}

		public ActionVerb(System.Reflection.MemberInfo memberInfo, WorkflowActionAttribute attribute)
		{
			this.name       = attribute.PublishedName ?? memberInfo.Name;
			this.memberInfo = memberInfo;
			this.attribute  = attribute;
		}


		public bool								IsEmpty
		{
			get
			{
				return this.memberInfo == null;
			}
		}

		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public System.Reflection.MemberInfo		MemberInfo
		{
			get
			{
				return this.memberInfo;
			}
		}

		public WorkflowActionAttribute			Attribute
		{
			get
			{
				return this.attribute;
			}
		}

		private readonly string name;
		private readonly System.Reflection.MemberInfo memberInfo;
		private readonly WorkflowActionAttribute attribute;
	}
}
