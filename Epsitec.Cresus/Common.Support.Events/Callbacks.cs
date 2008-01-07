//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public delegate void SimpleCallback();
	public delegate void SimpleCallback<T>(T item);
	public delegate T TransformCallback<T>(T item);
}
