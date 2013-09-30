//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Roger VUISTINER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec
{
	public class TempFile : FileObject
	{
		public TempFile()
			: base (System.IO.Path.GetTempFileName ())
		{
		}

		public TempFile(string extension)
			: base (System.IO.Path.GetTempPath () + Guid.NewGuid ().ToString () + extension)
		{
			using (System.IO.File.Create (this.Path))
			{
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.Delete ();
			base.Dispose (disposing);
		}
	}
}
