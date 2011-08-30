//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Workflows
{
	public sealed class ActionVerb
	{
		public ActionVerb(System.Reflection.MemberInfo memberInfo)
		{
			this.name       = memberInfo.Name;
			this.memberInfo = memberInfo;
		}

		public ActionVerb(string name, System.Reflection.MemberInfo memberInfo)
		{
			this.name       = name;
			this.memberInfo = memberInfo;
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


		private readonly string name;
		private readonly System.Reflection.MemberInfo memberInfo;
	}
}
