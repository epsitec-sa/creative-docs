//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
