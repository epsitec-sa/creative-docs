//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	public sealed class ActionItem
	{
		public ActionItem()
		{

		}

		public string							Name
		{
			get;
			set;
		}

		public FormattedText					Label
		{
			get;
			set;
		}

		public FormattedText					Description
		{
			get;
			set;
		}

		public System.Action					Action
		{
			get;
			set;
		}

		public ActionClass						ActionClass
		{
			get;
			set;
		}
	}
}
