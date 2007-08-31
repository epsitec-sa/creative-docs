using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Epsitec.App.Dolphin
{
	[TestFixture] public class ProcessorTest
	{
		[Test] public void CheckInstructions()
		{
			this.memory = new Components.Memory(null);
			this.processor = new Components.TinyProcessor(this.memory);
			this.assembler = new Assembler(this.processor, this.memory);

			foreach (string instruction in ProcessorTest.correct_instructions)
			{
				List<int> codes1 = new List<int>();
				string i1 = this.processor.AssemblyPreprocess(instruction);
				this.processor.AssemblyInstruction(i1, codes1);

				if (codes1.Count == 0)
				{
					System.Console.Out.WriteLine(string.Format("Instruction {0} impossible à assembler", instruction));
					Assert.Fail();
				}
				else
				{
					int address;
					string i2 = this.processor.DessassemblyInstruction(codes1, 0, out address);

					List<int> codes3 = new List<int>();
					string i3 = this.processor.AssemblyPreprocess(i2);
					this.processor.AssemblyInstruction(i3, codes3);

					if (!ProcessorTest.Compare(codes1, codes3))
					{
						System.Console.Out.WriteLine(string.Format("Instruction {0} mal désassemblée", instruction));
						Assert.Fail();
					}
				}
			}
		}

		static protected string[] correct_instructions =
		{
			"move a,h'c00",
			"move b,h'c00+{x}",
			"move x,h'c00+{x}+{y}",
			"move a,{pc}+2",
			"move b,{pc}-3",
			"move x,{sp}+4",
			"move y,{sp}-5",

			"move a,a",
			"move a,b",
			"move a,x",
			"move a,y",
			"move b,a",
			"move b,b",
			"move b,x",
			"move b,y",
			"move x,a",
			"move x,b",
			"move x,x",
			"move x,y",
			"move y,a",
			"move y,b",
			"move y,x",
			"move y,y",

			"move #12,a",
		};

		static protected bool Compare(List<int> l1, List<int> l2)
		{
			if (l1.Count != l2.Count)
			{
				return false;
			}

			for (int i=0; i<l1.Count; i++)
			{
				if (l1[i] != l2[i])
				{
					return false;
				}
			}

			return true;
		}


		protected Components.Memory						memory;
		protected Components.AbstractProcessor			processor;
		protected Assembler								assembler;
	}
}
