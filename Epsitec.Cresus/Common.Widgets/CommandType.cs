//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandType</c> enumeration lists the different command types.
	/// </summary>
	public enum CommandType
	{
		/// <summary>
		/// The <c>Standard</c> command.
		/// </summary>
		Standard,

		/// <summary>
		/// The <c>Multiple</c> command. It is associated with a list of sub-commands,
		/// one of which can be selected. The selected sub-command will be executed
		/// instead of the multiple command.
		/// </summary>
		Multiple,

		/// <summary>
		/// The <c>Structured</c> command; see <see cref="StructuredCommand"/>.
		/// </summary>
		Structured
	}
}
