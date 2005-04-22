using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class TextTest
	{
		[Test] public void CheckInternalCursor()
		{
			Common.Text.Tests.CheckInternalCursor.RunTests ();
		}
		
		[Test] public void CheckInternalCursorIdArray()
		{
			Common.Text.Tests.CheckInternalCursorIdArray.RunTests ();
		}
		
		[Test] public void CheckInternalCursorTable()
		{
			Common.Text.Tests.CheckInternalCursorTable.RunTests ();
		}
		
		[Test] public void CheckLayout()
		{
			Common.Text.Tests.CheckLayout.RunTests ();
		}
		
		[Test] public void CheckProperties()
		{
			Common.Text.Tests.CheckProperties.RunTests ();
		}
		
		[Test] public void CheckSerializerSupport()
		{
			Common.Text.Tests.CheckSerializerSupport.RunTests ();
		}
		
		[Test] public void CheckSimpleHtmlParser()
		{
			Common.Text.Tests.CheckSimpleHtmlParser.RunTests ();
		}
		
		[Test] public void CheckTextConverter()
		{
			Common.Text.Tests.CheckTextConverter.RunTests ();
		}
		
		[Test] public void CheckTextFitter()
		{
			Common.Text.Tests.CheckTextFitter.RunTests ();
		}
		
		[Test] public void CheckTextStory()
		{
			Common.Text.Tests.CheckTextStory.RunTests ();
		}
		
		[Test] public void CheckTextTable()
		{
			Common.Text.Tests.CheckTextTable.RunTests ();
		}
		
		[Test] public void CheckUnicode()
		{
			Common.Text.Tests.CheckUnicode.RunTests ();
		}
	}
}
