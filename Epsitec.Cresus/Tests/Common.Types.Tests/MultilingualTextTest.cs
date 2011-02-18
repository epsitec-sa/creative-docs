//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using NUnit.Framework;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	[TestFixture]
	public class MultilingualTextTest
	{
		[Test]
		public void CheckCreate1()
		{
			var text0 = new MultilingualText ();
			var text1 = new MultilingualText (@"abc");
			var text2 = new MultilingualText (@"<i>abc</i>");

			Assert.AreEqual ("", text0.ToString ());
			Assert.AreEqual ("abc", text1.ToString ());
			Assert.AreEqual ("<i>abc</i>", text2.ToString ());

			Assert.AreEqual (0, text0.Count);
			Assert.AreEqual (1, text1.Count);
			Assert.AreEqual (1, text2.Count);
		}

		[Test]
		public void CheckCreate2()
		{
			var text = new MultilingualText (@"<div lang=""*""><i>abc</i></div>");

			Assert.AreEqual (@"<i>abc</i>", text.ToString ());
			Assert.AreEqual (1, text.Count);
		}

		[Test]
		public void CheckContainsLocalizations()
		{
			var text0 = new MultilingualText ();
			var text1 = new MultilingualText (@"abc");
			var text2 = new MultilingualText (@"<i>abc</i>");
			var text3 = new MultilingualText (@"<div lang=""*""><i>abc</i></div>");
			var text4 = new MultilingualText (@"<div lang=""fr""><i>abc</i></div>");
			var text5 = new MultilingualText (@"<div lang=""*"">Hello</div><div lang=""fr"">Bonjour</div>");

			Assert.IsFalse (text0.ContainsLocalizations);
			Assert.IsFalse (text1.ContainsLocalizations);
			Assert.IsFalse (text2.ContainsLocalizations);
			Assert.IsFalse (text3.ContainsLocalizations);
			Assert.IsTrue (text4.ContainsLocalizations);
			Assert.IsTrue (text5.ContainsLocalizations);
		}

		[Test]
		public void CheckContainsLanguage()
		{
			var text0 = new MultilingualText ();
			var text1 = new MultilingualText (@"abc");
			var text2 = new MultilingualText (@"<i>abc</i>");
			var text3 = new MultilingualText (@"<div lang=""*""><i>abc</i></div>");
			var text4 = new MultilingualText (@"<div lang=""fr""><i>abc</i></div>");
			var text5 = new MultilingualText (@"<div lang=""*"">Hello</div><div lang=""fr"">Bonjour</div>");

			Assert.IsFalse (text0.ContainsLanguage ("fr"));
			Assert.IsFalse (text1.ContainsLanguage ("fr"));
			Assert.IsFalse (text2.ContainsLanguage ("fr"));
			Assert.IsFalse (text3.ContainsLanguage ("fr"));
			Assert.IsTrue (text4.ContainsLanguage ("fr"));
			Assert.IsTrue (text5.ContainsLanguage ("fr"));
		}

		[Test]
		public void CheckSetTextClearText()
		{
			var text = new MultilingualText ();

			Assert.AreEqual (0, text.Count);
			Assert.IsFalse (text.ContainsLocalizations);
			Assert.IsFalse (text.ContainsLanguage ("fr"));

			text.SetText (MultilingualText.DefaultLanguageId, "A");

			Assert.AreEqual (1, text.Count);
			Assert.IsFalse (text.ContainsLocalizations);
			Assert.IsFalse (text.ContainsLanguage ("fr"));

			text.SetText ("de", "B");

			Assert.AreEqual (2, text.Count);
			Assert.IsTrue (text.ContainsLocalizations);
			Assert.IsFalse (text.ContainsLanguage ("fr"));

			text.SetText ("fr", "C");

			Assert.AreEqual (3, text.Count);
			Assert.IsTrue (text.ContainsLocalizations);
			Assert.IsTrue (text.ContainsLanguage ("fr"));


			Assert.AreEqual (new FormattedText ("A"), text.GetTextOrDefault ("en"));
			Assert.AreEqual (new FormattedText ("B"), text.GetTextOrDefault ("de"));
			Assert.AreEqual (new FormattedText ("C"), text.GetTextOrDefault ("fr"));
			Assert.IsFalse (text.GetText ("en").HasValue);
			Assert.IsTrue (text.GetText ("de").HasValue);
			Assert.IsTrue (text.GetText ("fr").HasValue);

			text.ClearText ("fr");

			Assert.AreEqual (2, text.Count);
			Assert.IsTrue (text.ContainsLocalizations);
			Assert.IsFalse (text.ContainsLanguage ("fr"));
		}
	}
}
