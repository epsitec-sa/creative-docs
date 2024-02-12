//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGeneration;

using NUnit.Framework;

using System;
using System.Collections.Generic;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture]
	public class CodeFormatterTest
	{
		[Test]
		public void CheckWriteClass()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			CodeFormatter formatter = new CodeFormatter (buffer);

			formatter.WriteBeginNamespace ("Test");

			formatter.WriteBeginClass (new CodeAttributes (CodeVisibility.Public, CodeAccessibility.Default, CodeAttributes.PartialAttribute), "Class1");

			formatter.WriteBeginMethod (new CodeAttributes (CodeVisibility.Public), "Class1(int value)");
			formatter.WriteCodeLine ("this.value = value;");
			formatter.WriteEndMethod ();

			formatter.WriteBeginMethod (new CodeAttributes (CodeVisibility.None, CodeAttributes.PartialDefinitionAttribute), "void OnFooChanged(int value)");
			formatter.WriteEndMethod ();

			formatter.WriteBeginMethod (new CodeAttributes (CodeVisibility.None, CodeAttributes.PartialAttribute), "void OnFooChanged(int value)");
			formatter.WriteEndMethod ();

			formatter.WriteBeginProperty (new CodeAttributes (CodeVisibility.Public), "int Foo");
			formatter.WriteBeginSetter (new CodeAttributes (CodeVisibility.Internal));
			formatter.WriteCodeLine ("if (this.foo != value)");
			formatter.WriteBeginBlock ();
			formatter.WriteCodeLine ("this.foo = value;");
			formatter.WriteCodeLine ("this.OnFooChanged (value);");
			formatter.WriteEndBlock ();
			formatter.WriteEndSetter ();
			formatter.WriteBeginGetter (CodeAttributes.Default);
			formatter.WriteCodeLine ("return this.foo;");
			formatter.WriteEndGetter ();
			formatter.WriteEndProperty ();

			formatter.WriteInstanceVariable (new CodeAttributes (CodeVisibility.Private), "int foo");
			formatter.WriteInstanceVariable (new CodeAttributes (CodeVisibility.Private, CodeAccessibility.Final, CodeAttributes.ReadOnlyAttribute), "int value");

			formatter.WriteEndClass ();

			formatter.WriteEndNamespace ();

			System.Console.Out.WriteLine (buffer);
		}

		[Test]
		public void CheckWriteClassEx1Mismatch()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			CodeFormatter formatter = new CodeFormatter (buffer);

			formatter.WriteBeginNamespace ("Test");
			formatter.WriteBeginClass (CodeAttributes.Default, "Class1");
			// formatter.WriteEndClass ();
			Assert.Throws<InvalidOperationException>(() => formatter.WriteEndNamespace (), "Ending element Namespace, but expected Class");
		}

		[Test]
		public void CheckWriteClassEx2MisplacedGetter()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			CodeFormatter formatter = new CodeFormatter (buffer);

			formatter.WriteBeginNamespace ("Test");
			formatter.WriteBeginClass (CodeAttributes.Default, "Class1");
            Assert.Throws<InvalidOperationException>(() => formatter.WriteBeginGetter (CodeAttributes.Default), "PropertyGetter not defined in a property");
			formatter.WriteCodeLine ("return this.value;");
            Assert.Throws<InvalidOperationException>(() => formatter.WriteEndGetter ());
			Assert.Throws<InvalidOperationException>(() => formatter.WriteEndClass());
            Assert.Throws<InvalidOperationException>(() => formatter.WriteEndNamespace());
		}

		[Test]
		public void CheckWriteClassEx3MisplacedSetter()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			CodeFormatter formatter = new CodeFormatter (buffer);

			formatter.WriteBeginNamespace ("Test");
			formatter.WriteBeginClass (CodeAttributes.Default, "Class1");
			Assert.Throws<InvalidOperationException>(() => formatter.WriteBeginSetter(CodeAttributes.Default), "PropertySetter not defined in a property");
			formatter.WriteCodeLine ("return this.value;");
			Assert.Throws<InvalidOperationException>(() => formatter.WriteEndGetter());
			Assert.Throws<InvalidOperationException>(() => formatter.WriteEndClass());
			Assert.Throws<InvalidOperationException>(() => formatter.WriteEndNamespace());
		}

		[Test]
		public void CheckWriteClassEx4MisplacedMethod()
		{
            Assert.Throws<InvalidOperationException>(() =>
                {
                    System.Text.StringBuilder buffer = new System.Text.StringBuilder();
                    CodeFormatter formatter = new CodeFormatter(buffer);

                    formatter.WriteBeginNamespace("Test");
                    // formatter.WriteBeginClass (CodeAttributes.Default, "Class1");
                    formatter.WriteBeginMethod(CodeAttributes.Default, "Foo");
                    formatter.WriteCodeLine("BlahBlah ();");
                    formatter.WriteEndMethod();
                    // formatter.WriteEndClass ();
                    formatter.WriteEndNamespace();
                },                
                "Method not defined in a class or an interface"
            );
		}

		[Test]
		public void CheckWriteClassEx5AbstractCode()
		{
            Assert.Throws<InvalidOperationException>(() =>
                {
                    System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			        CodeFormatter formatter = new CodeFormatter (buffer);

			        formatter.WriteBeginNamespace ("Test");
			        formatter.WriteBeginClass (CodeAttributes.Default, "Class1");
			        formatter.WriteBeginMethod (new CodeAttributes (CodeAccessibility.Abstract), "void Foo()");
			        formatter.WriteCodeLine ("BlahBlah ();");
			        formatter.WriteEndMethod ();
			        formatter.WriteEndClass ();
			        formatter.WriteEndNamespace();
                },
               "Trying to generate code for an abstract item"
            );
        }

		[Test]
		public void CheckWriteInterface()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			CodeFormatter formatter = new CodeFormatter (buffer);

			formatter.WriteBeginNamespace ("Test");

			formatter.WriteBeginInterface (CodeAttributes.Default, "Interface1");

			formatter.WriteBeginMethod (CodeAttributes.Default, "void Display(int value)");
			formatter.WriteEndMethod ();

			formatter.WriteBeginProperty (CodeAttributes.Default, "int Value");
			formatter.WriteBeginSetter (CodeAttributes.Default);
			formatter.WriteEndSetter ();
			formatter.WriteBeginGetter (CodeAttributes.Default);
			formatter.WriteEndGetter ();
			formatter.WriteEndProperty ();

			formatter.WriteEndInterface ();

			formatter.WriteEndNamespace ();

			System.Console.Out.WriteLine (buffer);
		}

		[Test]
		public void CheckWriteInterfaceEx1CodeInInterface()
		{
            Assert.Throws<InvalidOperationException>(() =>
                {
                    System.Text.StringBuilder buffer = new System.Text.StringBuilder();
                    CodeFormatter formatter = new CodeFormatter(buffer);

                    formatter.WriteBeginNamespace("Test");
                    formatter.WriteBeginInterface(CodeAttributes.Default, "Interface1");
                    formatter.WriteBeginMethod(new CodeAttributes(CodeAccessibility.Final), "void Foo()");
                    formatter.WriteCodeLine("BlahBlah ();");
                    formatter.WriteEndMethod();
                    formatter.WriteEndInterface();
                    formatter.WriteEndNamespace();
                },
                "Trying to generate code for an abstract item"
            );
            
		}

		[Test]
		public void CheckWriteInterfaceEx2InstanceVariable()
		{
            Assert.Throws<InvalidOperationException>(() =>
            {
                System.Text.StringBuilder buffer = new System.Text.StringBuilder();
                CodeFormatter formatter = new CodeFormatter(buffer);

                formatter.WriteBeginNamespace("Test");

                formatter.WriteBeginInterface(CodeAttributes.Default, "Interface1");
                formatter.WriteInstanceVariable(new CodeAttributes(CodeVisibility.Private, CodeAccessibility.Final, CodeAttributes.ReadOnlyAttribute), "int value");
                formatter.WriteEndInterface();

                formatter.WriteEndNamespace();

                System.Console.Out.WriteLine(buffer);
            },
            "Trying to define an instance variable in Interface, not a class"
            );
           
		}

		[Test]
		public void CheckWriteInterfaceEx3EmbeddedInterface()
		{
            Assert.Throws<InvalidOperationException>(() =>
            {
                System.Text.StringBuilder buffer = new System.Text.StringBuilder();
                CodeFormatter formatter = new CodeFormatter(buffer);

                formatter.WriteBeginNamespace("Test");

                formatter.WriteBeginInterface(CodeAttributes.Default, "Interface1");
                formatter.WriteBeginInterface(CodeAttributes.Default, "EmbeddedInterface");
                formatter.WriteEndInterface();
                formatter.WriteEndInterface();

                formatter.WriteEndNamespace();

                System.Console.Out.WriteLine(buffer);
            },
            "Trying to define an interface in Interface, not a class or a namespace"
            );
            
		}

	}
}
