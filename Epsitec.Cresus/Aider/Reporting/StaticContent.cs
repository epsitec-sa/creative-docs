//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Aider.Reporting;

using System.Collections.Generic;
using System.Linq;

[assembly: ContentFormatter (typeof (StaticContent))]

namespace Epsitec.Aider.Reporting
{
	public class StaticContent : AbstractContent<StaticContent>
	{
		public StaticContent()
		{
		}

		public StaticContent(FormattedText text)
		{
			this.text = text.ToString ();
		}


		#region IContentStore Members

		public override byte[] GetContentBlob()
		{
			return System.Text.Encoding.UTF8.GetBytes (this.text);
		}

		public override IContentStore Setup(byte[] blob)
		{
			this.text = System.Text.Encoding.UTF8.GetString (blob);
			return this;
		}

		#endregion

		#region IContentTextProducer Members

		public override FormattedText GetFormattedText(string template)
		{
			return new FormattedText (text);
		}

		#endregion


		private string							text;
	}
}

