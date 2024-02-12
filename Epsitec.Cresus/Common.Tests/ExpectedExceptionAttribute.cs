//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;
using NUnit.Framework;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
[Obsolete("Use Assert.Throw")]
public class ExpectedExceptionAttribute : Attribute
{
    private Type expectedException;

    private string expectedExceptionName;

    private string expectedMessage;

    //private MessageMatch matchType;

    private string userMessage;

    private string handler;

    //
    // Summary:
    //     Gets or sets the expected exception type
    public Type ExpectedException
    {
        get { return expectedException; }
        set
        {
            expectedException = value;
            expectedExceptionName = expectedException.FullName;
        }
    }

    //
    // Summary:
    //     Gets or sets the full Type name of the expected exception
    public string ExpectedExceptionName
    {
        get { return expectedExceptionName; }
        set { expectedExceptionName = value; }
    }

    //
    // Summary:
    //     Gets or sets the expected message text
    public string ExpectedMessage
    {
        get { return expectedMessage; }
        set { expectedMessage = value; }
    }

    //
    // Summary:
    //     Gets or sets the user message displayed in case of failure
    public string UserMessage
    {
        get { return userMessage; }
        set { userMessage = value; }
    }

    //
    // Summary:
    //     Gets or sets the type of match to be performed on the expected message
    //public MessageMatch MatchType
    //{
    //    get
    //    {
    //        return matchType;
    //    }
    //    set
    //    {
    //        matchType = value;
    //    }
    //}

    //
    // Summary:
    //     Gets the name of a method to be used as an exception handler
    public string Handler
    {
        get { return handler; }
        set { handler = value; }
    }

    //
    // Summary:
    //     Constructor for a non-specific exception
    public ExpectedExceptionAttribute() { }

    //
    // Summary:
    //     Constructor for a given type of exception
    //
    // Parameters:
    //   exceptionType:
    //     The type of the expected exception
    public ExpectedExceptionAttribute(Type exceptionType)
    {
        expectedException = exceptionType;
        expectedExceptionName = exceptionType.FullName;
    }

    //
    // Summary:
    //     Constructor for a given exception name
    //
    // Parameters:
    //   exceptionName:
    //     The full name of the expected exception
    public ExpectedExceptionAttribute(string exceptionName)
    {
        expectedExceptionName = exceptionName;
    }
}
