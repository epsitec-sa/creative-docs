//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public class EmailManager : CoreAppComponent
	{
		public EmailManager(CoreApp app)
			: base (app)
		{
		}


		

		#region Factory Class

		private sealed class Factory : Epsitec.Cresus.Core.Factories.DefaultCoreAppComponentFactory<EmailManager>
		{
			public override bool ShouldCreate(CoreApp host)
			{
				return true;
			}
		}

		#endregion
	}
}

