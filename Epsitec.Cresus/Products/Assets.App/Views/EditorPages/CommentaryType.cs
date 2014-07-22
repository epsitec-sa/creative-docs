//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public enum CommentaryType
	{
		Unknown,

		Editable,	// blanc
		Defined,	// bleu
		Readonly,	// gris
		Result,		// gris-bleu
		Error,		// orange
	}
}
