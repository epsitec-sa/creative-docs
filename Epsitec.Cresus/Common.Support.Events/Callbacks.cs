//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public delegate void SimpleCallback();
	public delegate void SimpleCallback<T>(T item);
	public delegate T TransformCallback<T>(T item);
}
