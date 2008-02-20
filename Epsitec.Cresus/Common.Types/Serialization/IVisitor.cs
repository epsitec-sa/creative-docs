//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization
{
	/// <summary>
	/// The <c>IVisitor</c> interface is used by <see cref="T:GraphVisitor"/> to
	/// walk through a serializable object graph.
	/// </summary>
	public interface IVisitor
	{
		void VisitNodeBegin(Context context, DependencyObject obj);
		void VisitNodeEnd(Context context, DependencyObject obj);
		void VisitAttached(Context context, System.Type type);
		void VisitUnknown(Context context, object obj);
	}
}
