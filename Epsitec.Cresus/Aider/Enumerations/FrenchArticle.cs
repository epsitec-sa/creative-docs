//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum FrenchArticle
	{
		Undefined = 0,

		SingularMasculineBeforeConsonant		= 1,		//	le, du
		SingularFeminineBeforeConsonant			= 2,		//	la, de la
		SingularMasculineBeforeVowelOrMute		= 3,		//	l', de l'
		SingularFeminineBeforeVowelOrMute		= 4,		//	l', de l'
		PluralMasculine							= 5,		//	les, des
		PluralFeminine							= 6,		//	les, des
	}
}
