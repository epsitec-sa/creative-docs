//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Factories
{
	public static class CoreCommandHandlerFactory
	{
		public static IEnumerable<ICommandHandler> CreateCommandHandlers(CoreCommandDispatcher commandDispatcher)
		{
			return InterfaceImplementationResolver<ICommandHandler>.CreateInstances (commandDispatcher);
		}
	}
}
