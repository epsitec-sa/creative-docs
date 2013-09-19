//	Copyright © 2006-2013, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
