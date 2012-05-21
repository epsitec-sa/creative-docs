//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Widgets
{
	public class TabItem
	{
		public string Icon
		{
			get;
			set;
		}

		public FormattedText FormattedText
		{
			get;
			set;
		}


		public bool RenameVisibility
		{
			get;
			set;
		}

		public bool RenameEnable
		{
			get;
			set;
		}


		public bool DeleteVisibility
		{
			get;
			set;
		}

		public bool DeleteEnable
		{
			get;
			set;
		}


		public bool MoveVisibility
		{
			get;
			set;
		}

		public bool MoveBeginEnable
		{
			get;
			set;
		}

		public bool MoveEndEnable
		{
			get;
			set;
		}
	}
}
