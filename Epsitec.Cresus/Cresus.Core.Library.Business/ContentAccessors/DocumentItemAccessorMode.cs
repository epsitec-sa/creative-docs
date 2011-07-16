//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core.Library.Business.ContentAccessors
{
	public enum DocumentItemAccessorMode
	{
		None								= 0,
		SpecialQuantitiesToDistinctLines	= 0x00000001,	// met toutes les quantités dans les mêmes colonnes DocumentItemAccessorColumn.Unique*
		ForceAllLines						= 0x00000002,	// force toutes les lignes, même si elles sont vides
		IncludeTaxes						= 0x00000004,	// 

															// Ces 4 modes ne sont pas cumulables:
		EditArticleName						= 0x00000010,	// édite les descriptions courtes des articles
		EditArticleDescription				= 0x00000020,	// édite les descriptions longues des articles
		UseArticleName						= 0x00000040,	// utilise les descriptions courtes des articles
		UseArticleBoth						= 0x00000080,	// utilise les descriptions courtes + longues des articles
	}
}
