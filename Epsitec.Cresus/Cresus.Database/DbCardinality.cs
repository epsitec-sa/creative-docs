//	Copyright © 2007-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbCardinality</c> enumeration specifies the supported data model
	/// cardinalities. See http://www.datamodel.org/DataModelCardinality.html
	/// </summary>
	public enum DbCardinality : byte
	{
		None = 0,

		Reference = 1,
		Collection = 2,
	}
}
