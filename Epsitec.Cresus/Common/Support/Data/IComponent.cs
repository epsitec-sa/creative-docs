//	Copyright © 2003-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support.Data
{
	public interface IComponent : IDisposed
	{
		event Support.EventHandler	Disposed;
	}
}
