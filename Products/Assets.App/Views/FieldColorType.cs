//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Views
{
	public enum FieldColorType
	{
		Unknown,

		Editable,	// blanc
		Automatic,	// bleu clair
		Defined,	// bleu
		Readonly,	// gris
		Result,		// gris-bleu
		Error,		// orange
	}
}
