//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public delegate void SimpleCallback();
	public delegate void SimpleCallback<T>(T item);
	public delegate T TransformCallback<T>(T item);
}
