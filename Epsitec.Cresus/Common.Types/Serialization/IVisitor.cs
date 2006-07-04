//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization
{
	public interface IVisitor
	{
		void VisitNodeBegin(Context context, DependencyObject obj);
		void VisitNodeEnd(Context context, DependencyObject obj);
		void VisitAttached(Context context, PropertyValuePair entry);
		void VisitUnknown(Context context, object obj);
	}
}
