using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MIPS
{
    class Program
    {
        static void Main(string[] args)
        {
            CPU cpu = new CPU();
            Console.WriteLine(cpu.registers.PC);
            cpu.InstrMem.Add(1000, "10001111101010000000000000000100");
            cpu.InstrMem.Add(1004, "00000000100001010001000000100010");
            cpu.InstrMem.Add(1008, "00000001010010110100100000100100");
            cpu.InstrMem.Add(1012, "00000010001100101000000000100101");
            cpu.InstrMem.Add(1016, "00000001110000000110100000100000");
            cpu.InstrMem.Add(1020, "00000000000000000000000000000000");
            cpu.InstrMem.Add(1024, "00000000000000000000000000000000");
            cpu.InstrMem.Add(1028, "00000000000000000000000000000000");
            cpu.InstrMem.Add(1032, "00000000000000000000000000000000");

            for (int i = 1; i <= cpu.InstrMem.Count; i++)
            {
                if( i == 1)
                {
                    cpu.FetchStage();
                }
                else if(i == 2)
                {
                    cpu.DecodeStage();
                    cpu.FetchStage();
                }
                else if( i == 3)
                {
                    cpu.ExecStage();
                    cpu.DecodeStage();
                    cpu.FetchStage();
                }
                else if(i  == 4)
                {
                    cpu.MemStage();
                    cpu.ExecStage();
                    cpu.DecodeStage();
                    cpu.FetchStage();
                }
                else
                {
                    cpu.WBStage();
                    cpu.MemStage();
                    cpu.ExecStage();
                    cpu.DecodeStage();
                    cpu.FetchStage();
                }
            }
           
            while (true);
        }
    }
}
