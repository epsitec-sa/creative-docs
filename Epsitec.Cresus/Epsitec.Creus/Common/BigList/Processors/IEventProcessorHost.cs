//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public interface IEventProcessorHost
	{
		void Add(IEventProcessor processor);
		void Remove(IEventProcessor processor);

		IEnumerable<IEventProcessor> EventProcessors
		{
			get;
		}

		TPolicy GetPolicy<TPolicy>()
			where TPolicy : EventProcessorPolicy, new ();
	}
}
