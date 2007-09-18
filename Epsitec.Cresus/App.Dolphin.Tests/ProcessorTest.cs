using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Epsitec.App.Dolphin
{
	[TestFixture] public class ProcessorTest
	{
		[Test] public void CheckCorrectInstructions()
		{
			System.Console.Out.WriteLine("CheckCorrectInstructions");

			this.memory = new Components.Memory(null);
			this.processor = new Components.TinyProcessor(this.memory);
			this.assembler = new Assembler(this.processor, this.memory);

			int error = 0;
			int count = 0;
			foreach (string instruction in ProcessorTest.correct_instructions)
			{
				List<int> codes1 = new List<int>();
				string i1 = this.processor.AssemblyPreprocess(instruction);
				this.processor.AssemblyInstruction(i1, codes1);

				if (codes1.Count == 0)
				{
					System.Console.Out.WriteLine(string.Format("Instruction {0} impossible à assembler", instruction));
					error++;
				}
				else
				{
					int length = this.processor.GetInstructionLength(codes1[0]);
					if (length != codes1.Count)
					{
						System.Console.Out.WriteLine(string.Format("Longueur de l'instruction {0} incorrecte", instruction));
						error++;
					}

#if false
					for (int i=0; i<length; i++)
					{
						this.memory.Write(i, codes1[i]);
					}
					this.processor.SetRegisterValue("PC", 0);
					this.processor.Clock();
					int pc = this.processor.GetRegisterValue("PC");

					if (length != pc)
					{
						System.Console.Out.WriteLine(string.Format("Erreur lors de l'exécution de l'instruction {0}", instruction));
						error++;
					}
#endif

					int address;
					string i2 = this.processor.DessassemblyInstruction(codes1, 0, out address);

					List<int> codes3 = new List<int>();
					string i3 = this.processor.AssemblyPreprocess(i2);
					this.processor.AssemblyInstruction(i3, codes3);

					if (!ProcessorTest.Compare(codes1, codes3))
					{
						System.Console.Out.WriteLine(string.Format("Instruction {0} mal désassemblée", instruction));
						error++;
					}
				}

				count++;
			}

			System.Console.Out.WriteLine(string.Format("{0} instructions testées", count.ToString()));
			System.Console.Out.WriteLine(string.Format("{0} erreur(s) rencontrée(s)", error.ToString()));
			System.Console.Out.WriteLine("");

			if (error > 0)
			{
				Assert.Fail();
			}
		}

		[Test] public void CheckWrongInstructions()
		{
			System.Console.Out.WriteLine("CheckWrongInstructions");

			this.memory = new Components.Memory(null);
			this.processor = new Components.TinyProcessor(this.memory);
			this.assembler = new Assembler(this.processor, this.memory);

			int error = 0;
			int count = 0;
			foreach (string instruction in ProcessorTest.wrong_instructions)
			{
				List<int> codes1 = new List<int>();
				string i1 = this.processor.AssemblyPreprocess(instruction);
				this.processor.AssemblyInstruction(i1, codes1);

				if (codes1.Count != 0)
				{
					System.Console.Out.WriteLine(string.Format("Instruction {0} possible à assembler", instruction));
					error++;
				}

				count++;
			}

			System.Console.Out.WriteLine(string.Format("{0} instructions testées", count.ToString()));
			System.Console.Out.WriteLine(string.Format("{0} erreur(s) rencontrée(s)", error.ToString()));
			System.Console.Out.WriteLine("");

			if (error > 0)
			{
				Assert.Fail();
			}
		}

		static protected string[] correct_instructions =
		{
			"move #12,a",
			"move #D'12,a",
			"move #H'DA,a",
			"move #B'10011,a",
			"move #-34,b",
			"move a,h'c00",
			"move b,h'c01+{x}",
			"move b,h'c02+{y}",
			"move x,h'c03+{x}+{y}",
			"move x,h'c04+{y}+{x}",
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
			"move #13,b",
			"move #14,x",
			"move #15,y",

			"move h'111,a",
			"move h'111,b",
			"move h'111,x",
			"move h'111,y",
			"move a,h'222",
			"move b,h'222",
			"move x,h'222",
			"move y,h'222",

			"move #h'da,h'333",

			"add a,a",
			"add a,b",
			"add a,x",
			"add a,y",
			"add b,a",
			"add b,b",
			"add b,x",
			"add b,y",
			"add x,a",
			"add x,b",
			"add x,x",
			"add x,y",
			"add y,a",
			"add y,b",
			"add y,x",
			"add y,y",

			"add #12,a",
			"add #13,b",
			"add #14,x",
			"add #15,y",

			"add h'111,a",
			"add h'111,b",
			"add h'111,x",
			"add h'111,y",
			"add a,h'222",
			"add b,h'222",
			"add x,h'222",
			"add y,h'222",

			"add #h'da,h'333",

			"sub a,a",
			"sub a,b",
			"sub a,x",
			"sub a,y",
			"sub b,a",
			"sub b,b",
			"sub b,x",
			"sub b,y",
			"sub x,a",
			"sub x,b",
			"sub x,x",
			"sub x,y",
			"sub y,a",
			"sub y,b",
			"sub y,x",
			"sub y,y",

			"sub #12,a",
			"sub #13,b",
			"sub #14,x",
			"sub #15,y",

			"sub h'111,a",
			"sub h'111,b",
			"sub h'111,x",
			"sub h'111,y",
			"sub a,h'222",
			"sub b,h'222",
			"sub x,h'222",
			"sub y,h'222",

			"sub #h'da,h'333",

			"and a,b",
			"and b,a",
			"and #h'fd,a",
			"and #h'fc,b",
			"and #h'fb,x",
			"and #h'fa,y",
			"and 1,a",
			"and 1,b",
			"and a,2",
			"and b,2",

			"or a,b",
			"or b,a",
			"or #h'fd,a",
			"or #h'fc,b",
			"or #h'fb,x",
			"or #h'fa,y",
			"or 1,a",
			"or 1,b",
			"or a,2",
			"or b,2",

			"xor a,b",
			"xor b,a",
			"xor #h'fd,a",
			"xor #h'fc,b",
			"xor #h'fb,x",
			"xor #h'fa,y",
			"xor 1,a",
			"xor 1,b",
			"xor a,2",
			"xor b,2",

			"test b:a",
			"test a:b",
			"test a:#1",
			"test b:#1",
			"test h'c08:a",
			"test h'c08:b",
			"test h'c08:#7",

			"tclr b:a",
			"tclr a:b",
			"tclr a:#1",
			"tclr b:#1",
			"tclr h'c08:a",
			"tclr h'c08:b",
			"tclr h'c08:#7",

			"tset b:a",
			"tset a:b",
			"tset a:#1",
			"tset b:#1",
			"tset h'c08:a",
			"tset h'c08:b",
			"tset h'c08:#7",

			"tnot b:a",
			"tnot a:b",
			"tnot a:#1",
			"tnot b:#1",
			"tnot h'c08:a",
			"tnot h'c08:b",
			"tnot h'c08:#7",

			"comp a,a",
			"comp a,b",
			"comp a,x",
			"comp a,y",
			"comp b,a",
			"comp b,b",
			"comp b,x",
			"comp b,y",
			"comp x,a",
			"comp x,b",
			"comp x,x",
			"comp x,y",
			"comp y,a",
			"comp y,b",
			"comp y,x",
			"comp y,y",

			"comp #12,a",
			"comp #13,b",
			"comp #14,x",
			"comp #15,y",

			"comp h'111,a",
			"comp h'111,b",
			"comp h'111,x",
			"comp h'111,y",

			"comp #h'da,h'333",

			"clr a",
			"clr b",
			"clr x",
			"clr y",
			"clr h'100",

			"not a",
			"not b",
			"not x",
			"not y",
			"not h'100",

			"inc a",
			"inc b",
			"inc x",
			"inc y",
			"inc h'100",

			"dec a",
			"dec b",
			"dec x",
			"dec y",
			"dec h'100",

			"rl a",
			"rl b",
			"rl x",
			"rl y",
			"rl h'100",

			"rr a",
			"rr b",
			"rr x",
			"rr y",
			"rr h'100",

			"rlc a",
			"rlc b",
			"rlc x",
			"rlc y",
			"rlc h'100",

			"rrc a",
			"rrc b",
			"rrc x",
			"rrc y",
			"rrc h'100",

			"jump 1",
			"jump,eq 2",
			"jump,ne 3",
			"jump,lo 4",
			"jump,hs 5",
			"jump,ls 6",
			"jump,hi 7",
			"jump,ns 8",
			"jump,nc 9",

			"call 12",
			"ret",

			"push a",
			"push b",
			"push x",
			"push y",
			"push f",

			"pop a",
			"pop b",
			"pop x",
			"pop y",
			"pop f",

			"sub #3,sp",
			"add #3,sp",
			"move {sp}+12,a",
			"move {sp}+12,b",
			"move {sp}+12,x",
			"move {sp}+12,y",
			"move a,{sp}+12",
			"move b,{sp}+12",
			"move x,{sp}+12",
			"move y,{sp}+12",

			"setc",
			"clrc",
			"notc",
			"nop",
			"halt",
			"ex a,b",
			"ex b,a",
			"ex x,y",
			"ex y,x",
			"swap a",
			"swap b",
		};

		static protected string[] wrong_instructions =
		{
			"move",
			"move a",
			"move a,xx",
			"move a,#12",
			"move #D'1A,a",
			"move #B'001200,a",
			"move #H'1G,a",

			"and h'100,x",
			"and h'100,y",
			"and x,h'100",
			"and y,h'100",

			"or h'100,x",
			"or h'100,y",
			"or x,h'100",
			"or y,h'100",

			"xor h'100,x",
			"xor h'100,y",
			"xor x,h'100",
			"xor y,h'100",

			"test x:#2",
			"test y:#2",
			"tclr x:#2",
			"tclr y:#2",
			"tset x:#2",
			"tset y:#2",
			"tnot x:#2",
			"tnot y:#2",

			"clr",
			"not",
			"inc",
			"dec",
			"rl",
			"rr",
			"rlc",
			"rrc",

			"jump",
			"jump a",
			"jump #12",
			"jump,xx 0",

			"call,eq 12",
			"ret,eq",
			"ret a",

			"nop a",
			"halt 12",
			"trap",
			"trap 2",
			"ex a,x",
			"ex a,y",
			"ex b,x",
			"ex b,y",
			"swap x",
			"swap y",
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


		[Test]
		public void CheckCorrectExpressions()
		{
			System.Console.Out.WriteLine("CheckCorrectExpressions");

			this.memory = new Components.Memory(null);
			this.processor = new Components.TinyProcessor(this.memory);
			this.assembler = new Assembler(this.processor, this.memory);

			int error = 0;
			int count = 0;
			for (int i=0; i<correct_expressions.Length; i+=2)
			{
				string exp = correct_expressions[i];
				string err;
				int result = this.assembler.Expression(exp, 0, false, null, out err);
				if (err == null)
				{
					if (result.ToString() != correct_expressions[i+1])
					{
						System.Console.Out.WriteLine(string.Format("L'expressions {0} retourne {1} au lieu de {2}", exp, result.ToString(), correct_expressions[i+1]));
						error++;
					}
				}
				else
				{
					System.Console.Out.WriteLine(string.Format("L'expressions {0} retourne l'erreur {1}", exp, err));
					error++;
				}

				count++;
			}

			System.Console.Out.WriteLine(string.Format("{0} expressions testées", count.ToString()));
			System.Console.Out.WriteLine(string.Format("{0} erreur(s) rencontrée(s)", error.ToString()));
			System.Console.Out.WriteLine("");

			if (error > 0)
			{
				Assert.Fail();
			}
		}

		static protected string[] correct_expressions =
		{
			"123", "123",
			"2+3", "5",
			"111+222", "333",
			"333-111", "222",
			"2*3+4*5", "26",
			"2+3*4+5", "19",
			"(3)", "3",
			"(2*3)+4*5", "26",
			"2*3+(4*5)", "26",
			"(2*3)+(4*5)", "26",
			"2*(3+4)*5", "70",
			"10-2-3", "5",
			"10-2*3", "4",
			"10-2*3-1", "3",
			"10*(4-1)", "30",
			"10*(4-1)-2", "28",
			"10-3-2-1", "4",
			"10-(3-2)-1", "8",
			"+3", "3",
			"-3", "-3",
			"3*(-5)", "-15",
			"3*(+5)", "15",
			"-3*5", "-15",
			"+3*5", "15",
			"-(5-3)*3", "-6",
			"+(5-3)*3", "6",
			"5*(-3+5)", "10",
			"5*(+3+5)", "40",
			"5*(-3+5)-1", "9",
			"5*(+3+5)-1", "39",
			"5*(-3+5)-1*2", "8",
			"5*(+3+5)-1*2", "38",
			"--3", "3",
			"++3", "3",
			"+-3", "-3",
			"-+3", "-3",
			"2+-3", "-1",
			"2*-3", "-6",
			"2*+3", "6",
			"2+(-5)", "-3",
			"2+(+5)", "7",
		};


		[Test]
		public void CheckWrongExpressions()
		{
			System.Console.Out.WriteLine("CheckWrongExpressions");

			this.memory = new Components.Memory(null);
			this.processor = new Components.TinyProcessor(this.memory);
			this.assembler = new Assembler(this.processor, this.memory);

			int error = 0;
			int count = 0;
			for (int i=0; i<wrong_expressions.Length; i++)
			{
				string exp = wrong_expressions[i];
				string err;
				int result = this.assembler.Expression(exp, 0, false, null, out err);
				if (err == null)
				{
					System.Console.Out.WriteLine(string.Format("L'expressions {0} devrait retrourner une erreur", exp));
					error++;
				}

				count++;
			}

			System.Console.Out.WriteLine(string.Format("{0} expressions testées", count.ToString()));
			System.Console.Out.WriteLine(string.Format("{0} erreur(s) rencontrée(s)", error.ToString()));
			System.Console.Out.WriteLine("");

			if (error > 0)
			{
				Assert.Fail();
			}
		}

		static protected string[] wrong_expressions =
		{
			"(2+3",
			"3+2)",
			"3/0",
			"3/(6-3*2)",
		};


		protected Components.Memory						memory;
		protected Components.AbstractProcessor			processor;
		protected Assembler								assembler;
	}
}
