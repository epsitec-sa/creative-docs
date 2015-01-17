//	Copyright � 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;


namespace Epsitec.Aider.Enumerations
{
	[DesignerVisible]
	public enum LegalPersonType
	{
		None = 0,

		Institution		= 1,
		Business		= 2,
		Corporation		= 3,
		Association		= 10,
		Foundation		= 11,
		PoliticalBody	= 20,
		Church			= 30,
	}
}