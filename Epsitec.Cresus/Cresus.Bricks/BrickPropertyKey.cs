//	Copyright � 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	/// <summary>
	/// The <c>BrickPropertyKey</c> enumeration defines the well known properties, which will
	/// be generated by the various methods executed on the <see cref="Brick"/> classes.
	/// </summary>
	public enum BrickPropertyKey
	{
		Name,
		Icon,
		Title,
		TitleCompact,
		Text,
		TextCompact,

		Attribute,

		Template,
		OfType,

		Input,
		Field,
		Width,
		Height,
		ReadOnly,
		Separator,
		HorizontalGroup,
		
		FromCollection,
		FavoritesCollection,
		DataSetCommandId,

		SpecialController,
		SpecialFieldController,
		GlobalWarning,
		Password,
		Multiline,

		Button,
		SearchPanel,

		Include,

		DefineAction,
		EnableAction,
		Value,
		Type,
	}
}
