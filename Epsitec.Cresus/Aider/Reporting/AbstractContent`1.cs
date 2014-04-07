//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Reporting
{
	public abstract class AbstractContent<T> : IContent
		where T : AbstractContent<T>
	{
		#region IContentStore Members

		public string Format
		{
			get
			{
				return typeof (T).Name;
			}
		}

		public abstract byte[] GetContentBlob();

		public abstract IContentStore Setup(byte[] blob);

		#endregion

		#region IContentTextProducer Members

		public abstract FormattedText GetFormattedText(string template);

		#endregion
	}
}

