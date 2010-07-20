//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public class DocumentOption
	{
		public DocumentOption(string title)
		{
			this.IsMargin = false;
			this.IsTitle = true;
			this.Title = title;
		}

		public DocumentOption(double height)
		{
			this.IsMargin = true;
			this.IsTitle = false;
			this.Height = height;
		}

		public DocumentOption(string name, string radioName, string description, bool defautState=false)
		{
			this.IsMargin = false;
			this.IsTitle = false;
			this.Name = name;
			this.RadioName = radioName;
			this.Description = description;
			this.DefautState = defautState;
		}


		public bool IsTitle
		{
			get;
			set;
		}

		public bool IsMargin
		{
			get;
			set;
		}

		public double Height
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string RadioName
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public bool DefautState
		{
			get;
			set;
		}
	}
}
