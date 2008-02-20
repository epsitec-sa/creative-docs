//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// Describes the action that caused a <see cref="INotifyCollectionChanged.CollectionChanged"/> event.
	/// </summary>
	public enum CollectionChangedAction
	{
		Add,
		Move,
		Remove,
		Replace,
		Reset
	}
}
