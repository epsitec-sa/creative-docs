//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.Settings
{
	/// <summary>
	/// The <c>VectorValueSetting</c> class represents a setting for a single
	/// value in a vector; it is used to name columns in a row, for instance.
	/// See <see cref="VectorSetting"/>.
	/// </summary>
	public class VectorValueSetting
	{
		public VectorValueSetting()
		{
		}

		/// <summary>
		/// Gets or sets the inclusion mode for this value.
		/// </summary>
		/// <value>The inclusion mode.</value>
		public InclusionMode InclusionMode
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the id for this value.
		/// </summary>
		/// <value>The id.</value>
		public string Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the title for this value. The title is represented
		/// using formatted text.
		/// </summary>
		/// <value>The title, as formatted text.</value>
		public string Title
		{
			get;
			set;
		}
	}
}
