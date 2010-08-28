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
			this.isMargin = false;
			this.isTitle  = true;
			this.title    = title;
		}

		public DocumentOption(double height)
		{
			this.isMargin = true;
			this.isTitle  = false;
			this.height   = height;
		}

		public DocumentOption(string name, string radioName, string description, bool defautState=false)
		{
			this.isMargin    = false;
			this.isTitle     = false;
			this.name        = name;
			this.radioName   = radioName;
			this.description = description;
			this.defautState = defautState;
		}


		public bool IsTitle
		{
			get
			{
				return this.isTitle;
			}
		}

		public bool IsMargin
		{
			get
			{
				return this.isMargin;
			}
		}

		public double Height
		{
			get
			{
				return this.height;
			}
		}

		public string Title
		{
			get
			{
				return this.title;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string RadioName
		{
			get
			{
				return this.radioName;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public bool DefautState
		{
			get
			{
				return this.defautState;
			}
		}


		private readonly bool		isTitle;
		private readonly bool		isMargin;
		private readonly double		height;
		private readonly string		title;
		private readonly string		name;
		private readonly string		radioName;
		private readonly string		description;
		private readonly bool		defautState;
	}
}
