//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Driver
{
	/// <summary>
	/// La classe CheckPerformance permet de vérifier la performance de
	/// certaines constructions de code avec .NET.
	/// </summary>
	public sealed class CheckPerformance
	{
		public static void RunTests(int size, int runs)
		{
			CheckPerformance.PerfByteArray (size, runs);		//	6.0ns/mot - 21ns/mot - 21ns/mot (*)
			CheckPerformance.PerfByteBuffer (size, runs);		//	          -  8ns/mot -  8ns/mot
			CheckPerformance.PerfLongArray (size, runs);		//	5.5ns/mot -  9ns/mot -  9ns/mot
			CheckPerformance.PerfLongBuffer (size, runs);		//	          -  8ns/mot -  8ns/mot
			
			//	(*) Résultats obtenus avec un bi-Xeon 1.7GHz, size=100'000 et runs=1000.
			//
			//	Avec size=1000, on divise grosso modo par 2 les temps d'exécution grâce
			//	au cache du processeur.
		}
		
		
		static void PerfByteArray(int size, int runs)
		{
			System.Diagnostics.Trace.WriteLine ("Byte Array, direct byte manipulation.");
			
			byte[] data = new byte[size*8];
			
			for (int i = 0; i < size*8; i += 8)
			{
				data[i+0] = 0;
				data[i+1] = 0;
				data[i+2] = 0;
				data[i+3] = 0;
				data[i+4] = 0;
				data[i+5] = 0;
				data[i+6] = (byte)((i >> 8) & 0xff);
				data[i+7] = (byte)((i >> 0) & 0xff);
			}
			
			System.Diagnostics.Trace.WriteLine ("Warming up JIT.");
			CheckPerformance.PerfByteArraySearch (data, size, 1, 0xabc0);
			
			System.Diagnostics.Trace.WriteLine ("Search:");
			int count = CheckPerformance.PerfByteArraySearch (data, size, runs, 0xabc0);
			System.Diagnostics.Trace.WriteLine ("Search done. Count=" + count);
			
			System.Diagnostics.Trace.WriteLine ("Warming up JIT.");
			CheckPerformance.PerfByteArrayInsert (data, size, 1);
			
			System.Diagnostics.Trace.WriteLine ("Insert:");
			CheckPerformance.PerfByteArrayInsert (data, size, runs);
			System.Diagnostics.Trace.WriteLine ("Insert done.");
			
			System.Diagnostics.Trace.WriteLine ("Warming up JIT.");
			CheckPerformance.PerfByteArrayRemove (data, size, 1);
			
			System.Diagnostics.Trace.WriteLine ("Remove:");
			CheckPerformance.PerfByteArrayRemove (data, size, runs);
			System.Diagnostics.Trace.WriteLine ("Remove done.");
		}
		
		static int  PerfByteArraySearch(byte[] data, int size, int runs, int pattern)
		{
			int count = 0;
			
			for (int j = 0; j < runs; j++)
			{
				for (int i = 0; i < size*8; i += 8)
				{
					int x = (data[i+6] << 8) | (data[i+7]);
					if (x == pattern)
					{
						count++;
					}
				}
			}
			
			return count;
		}
		
		static void PerfByteArrayInsert(byte[] data, int size, int runs)
		{
			for (int j = 0; j < runs; j++)
			{
				for (int i = size*8-8; i >= 8; i -= 8)
				{
					data[i+0] = data[i-8+0];
					data[i+1] = data[i-8+1];
					data[i+2] = data[i-8+2];
					data[i+3] = data[i-8+3];
					data[i+4] = data[i-8+4];
					data[i+5] = data[i-8+5];
					data[i+6] = data[i-8+6];
					data[i+7] = data[i-8+7];
				}
			}
		}
		
		static void PerfByteArrayRemove(byte[] data, int size, int runs)
		{
			for (int j = 0; j < runs; j++)
			{
				for (int i = 8; i < size*8; i += 8)
				{
					data[i-8+0] = data[i+0];
					data[i-8+1] = data[i+1];
					data[i-8+2] = data[i+2];
					data[i-8+3] = data[i+3];
					data[i-8+4] = data[i+4];
					data[i-8+5] = data[i+5];
					data[i-8+6] = data[i+6];
					data[i-8+7] = data[i+7];
				}
			}
		}
		
		
		static void PerfByteBuffer(int size, int runs)
		{
			System.Diagnostics.Trace.WriteLine ("Byte Array, byte manipulation through System.Buffer.");
			
			byte[] data = new byte[size*8];
			
			for (int i = 0; i < size*8; i += 8)
			{
				data[i+0] = 0;
				data[i+1] = 0;
				data[i+2] = 0;
				data[i+3] = 0;
				data[i+4] = 0;
				data[i+5] = 0;
				data[i+6] = (byte)((i >> 8) & 0xff);
				data[i+7] = (byte)((i >> 0) & 0xff);
			}
			
			System.Diagnostics.Trace.WriteLine ("Warming up JIT.");
			CheckPerformance.PerfByteArraySearch (data, size, 1, 0xabc0);
			
			System.Diagnostics.Trace.WriteLine ("Search:");
			int count = CheckPerformance.PerfByteArraySearch (data, size, runs, 0xabc0);
			System.Diagnostics.Trace.WriteLine ("Search done. Count=" + count);
			
			System.Diagnostics.Trace.WriteLine ("Warming up JIT.");
			CheckPerformance.PerfByteBufferInsert (data, size, 1);
			
			System.Diagnostics.Trace.WriteLine ("Insert:");
			CheckPerformance.PerfByteBufferInsert (data, size, runs);
			System.Diagnostics.Trace.WriteLine ("Insert done.");
			
			System.Diagnostics.Trace.WriteLine ("Warming up JIT.");
			CheckPerformance.PerfByteBufferRemove (data, size, 1);
			
			System.Diagnostics.Trace.WriteLine ("Remove:");
			CheckPerformance.PerfByteBufferRemove (data, size, runs);
			System.Diagnostics.Trace.WriteLine ("Remove done.");
		}
		
		static void PerfByteBufferInsert(byte[] data, int size, int runs)
		{
			for (int j = 0; j < runs; j++)
			{
				System.Buffer.BlockCopy (data, 0, data, 8, size*8-8);
			}
		}
		
		static void PerfByteBufferRemove(byte[] data, int size, int runs)
		{
			for (int j = 0; j < runs; j++)
			{
				System.Buffer.BlockCopy (data, 8, data, 0, size*8-8);
			}
		}
		
		
		static void PerfLongArray(int size, int runs)
		{
			System.Diagnostics.Trace.WriteLine ("Long Array, direct byte manipulation.");
			
			ulong[] data = new ulong[size];
			
			for (int i = 0; i < size; i++)
			{
				data[i] = (ulong)((i * 8) & 0xffff);
			}
			
			System.Diagnostics.Trace.WriteLine ("Warming up JIT.");
			CheckPerformance.PerfLongArraySearch (data, size, 1, 0xabc0);
			
			System.Diagnostics.Trace.WriteLine ("Search:");
			int count = CheckPerformance.PerfLongArraySearch (data, size, runs, 0xabc0);
			System.Diagnostics.Trace.WriteLine ("Search done. Count=" + count);
			
			System.Diagnostics.Trace.WriteLine ("Warming up JIT.");
			CheckPerformance.PerfLongArrayInsert (data, size, 1);
			
			System.Diagnostics.Trace.WriteLine ("Insert:");
			CheckPerformance.PerfLongArrayInsert (data, size, runs);
			System.Diagnostics.Trace.WriteLine ("Insert done.");
			
			System.Diagnostics.Trace.WriteLine ("Warming up JIT.");
			CheckPerformance.PerfLongArrayRemove (data, size, 1);
			
			System.Diagnostics.Trace.WriteLine ("Remove:");
			CheckPerformance.PerfLongArrayRemove (data, size, runs);
			System.Diagnostics.Trace.WriteLine ("Remove done.");
		}
		
		static int  PerfLongArraySearch(ulong[] data, int size, int runs, int pattern)
		{
			int count = 0;
			
			for (int j = 0; j < runs; j++)
			{
				for (int i = 0; i < size; i++)
				{
					int x = (int)(data[i] & 0xffff);
					if (x == pattern)
					{
						count++;
					}
				}
			}
			
			return count;
		}
		
		static void PerfLongArrayInsert(ulong[] data, int size, int runs)
		{
			for (int j = 0; j < runs; j++)
			{
				for (int i = size-1; i >= 1; i -= 1)
				{
					data[i] = data[i-1];
				}
			}
		}
		
		static void PerfLongArrayRemove(ulong[] data, int size, int runs)
		{
			for (int j = 0; j < runs; j++)
			{
				for (int i = 1; i < size; i += 1)
				{
					data[i-1] = data[i];
				}
			}
		}
		
		
		static void PerfLongBuffer(int size, int runs)
		{
			System.Diagnostics.Trace.WriteLine ("Long Array, byte manipulation through System.Buffer.");
			
			ulong[] data = new ulong[size];
			
			for (int i = 0; i < size; i++)
			{
				data[i] = (ulong)((i * 8) & 0xffff);
			}
			
			System.Diagnostics.Trace.WriteLine ("Warming up JIT.");
			CheckPerformance.PerfLongArraySearch (data, size, 1, 0xabc0);
			
			System.Diagnostics.Trace.WriteLine ("Search:");
			int count = CheckPerformance.PerfLongArraySearch (data, size, runs, 0xabc0);
			System.Diagnostics.Trace.WriteLine ("Search done. Count=" + count);
			
			System.Diagnostics.Trace.WriteLine ("Warming up JIT.");
			CheckPerformance.PerfLongBufferInsert (data, size, 1);
			
			System.Diagnostics.Trace.WriteLine ("Insert:");
			CheckPerformance.PerfLongBufferInsert (data, size, runs);
			System.Diagnostics.Trace.WriteLine ("Insert done.");
			
			System.Diagnostics.Trace.WriteLine ("Warming up JIT.");
			CheckPerformance.PerfLongBufferRemove (data, size, 1);
			
			System.Diagnostics.Trace.WriteLine ("Remove:");
			CheckPerformance.PerfLongBufferRemove (data, size, runs);
			System.Diagnostics.Trace.WriteLine ("Remove done.");
		}
		
		static void PerfLongBufferInsert(ulong[] data, int size, int runs)
		{
			for (int j = 0; j < runs; j++)
			{
				System.Buffer.BlockCopy (data, 0, data, 8, size*8-8);
			}
		}
		
		static void PerfLongBufferRemove(ulong[] data, int size, int runs)
		{
			for (int j = 0; j < runs; j++)
			{
				System.Buffer.BlockCopy (data, 8, data, 0, size*8-8);
			}
		}
		
	}
}
