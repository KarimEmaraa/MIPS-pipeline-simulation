using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MIPS
{
    class CPU
    {
        public Registers registers;

        public Dictionary<int, string> InstrMem;

        public Dictionary<int, string> DataMem;

        private int PCsrcMUX(int ip1, int ip2, bool selector)
        {
            int retval = 0;
            int select = Convert.ToInt32(selector);
            switch (select)
            {
                case 0:
                    retval = ip1;
                    break;
                case 1:
                    retval = ip2;
                    break;
                default:
                    break;
            }
            return retval;
        }

        private int ALUSrcMUX(int ip1, int ip2, bool selector)
        {
            int retval = 0;
            int select = Convert.ToInt32(selector);
            switch (select)
            {
                case 0:
                    retval = ip1;
                    break;
                case 1:
                    retval = ip2;
                    break;
                default:
                    break;
            }
            return retval;
        }

        private int RegDstMUX(int ip1, int ip2, bool selector)
        {
            int retval = 0;
            int select = Convert.ToInt32(selector);
            switch (select)
            {
                case 0:
                    retval = ip1;
                    break;
                case 1:
                    retval = ip2;
                    break;
                default:
                    break;
            }
            return retval;
        }

        private int MEMtoRegMUX(int ip1, int ip2, bool selector)
        {
            int retval = 0;
            int select = Convert.ToInt32(selector);
            switch (select)
            {
                case 0:
                    retval = ip1;
                    break;
                case 1:
                    retval = ip2;
                    break;
                default:
                    break;
            }
            return retval;
        }

        private int ALU(int op1, int op2, Aluop op)
        {
            int result = 0;
            switch (op)
            {
                case Aluop.NOP:
                    result = 0;
                    break;
                case Aluop.ADD:
                    result = op1 + op2;
                    break;
                case Aluop.SUB:
                    result = op1 - op2;
                    break;
                case Aluop.AND:
                    result = op1 & op2;
                    break;
                case Aluop.OR:
                    result = op1 | op2;
                    break;
                default:
                    break;
            }
            return result;
        }

        private int adder(int ip1, int ip2)
        {
            return ip1 + ip2;
        }

        private void NOPClear( PipeRegister r1)
        {
            r1.Rt = 0;
            r1.Rs = 0;
            r1.Rd = 0;
            r1.Opcode = 0;
            r1.Offset = 0;

            r1.MemPCSrc = false;
            //memory control signals
            r1.MemRead = false;
            r1.MemWrite = false;
            //wb control singals
            r1.MemToReg = false;
            r1.RegWrite = false;
            //ALU control signals
            r1.Aluoperation = Aluop.NOP;
            r1.ALUSrc = false;
            r1.ALUREGdst = false;
        }

        public void FetchStage()
        {
            //calculate new PC
            int addtemp = adder(registers.PC, 4);
            int temp = 0; //THIS is the second operand of pcSRC MUX (UNUSED)
            registers.nextPC = PCsrcMUX(addtemp, temp, registers.IFID.MemPCSrc);
            //add instruction to IFID pipeline register
            registers.IFID.Opcode = Convert.ToInt32(InstrMem[registers.PC].Substring(0, 6), 2);
            //if it's R-Format Instruction
            if(registers.IFID.Opcode == 0)
            {
                registers.IFID.Rd = Convert.ToInt32(InstrMem[registers.PC].Substring(32 - 16, 5), 2);
                registers.IFID.Func = Convert.ToInt32(InstrMem[registers.PC].Substring(32 - 6, 6), 2);
            }
            //then it's I-Format Instruction
            else
            {
                registers.IFID.Offset = Convert.ToInt32(InstrMem[registers.PC].Substring(32 - 6, 6), 2);
            }
            //rs and rt are in same position in both instruction types.
            registers.IFID.Rs = Convert.ToInt32(InstrMem[registers.PC].Substring(32 - 26, 5), 2);
            registers.IFID.Rt = Convert.ToInt32(InstrMem[registers.PC].Substring(32 - 21, 5), 2);
            
            registers.PC = registers.nextPC;

            if (registers.IFID.Rd == 0 && registers.IFID.Rs == 0 && registers.IFID.Rt == 0 &&
                 registers.IFID.Func == 0)
            {
                registers.IFID.NOP = true;
                NOPClear(registers.IFID);
                return;
            }
        }

        public void DecodeStage()
        {
            int rsvalue = 0;
            int rtvalue = 0;
            //control signal propagation
            //since there is not branching PC src is always PC+4
            registers.IDEX.MemPCSrc = false;
            //if it's I-Format > Propagate control signals to next pip registers
            if (registers.IFID.Opcode != 0)
            {
                //memory control signals
                registers.IDEX.MemRead = true;
                registers.IDEX.MemWrite = false;
                //wb control singals
                registers.IDEX.MemToReg = true;
                registers.IDEX.RegWrite = true;
                //ALU control signals
                registers.IDEX.Aluoperation = Aluop.ADD;
                registers.IDEX.ALUSrc = true;
                registers.IDEX.ALUREGdst = false;
            }
            else //R-Format
            {
                //no memory access
                registers.IDEX.MemRead = false;
                registers.IDEX.MemWrite = false;

                registers.IDEX.MemToReg = false;
                registers.IDEX.RegWrite = true;

                registers.IDEX.ALUSrc = false;
                registers.IDEX.ALUREGdst = true;

                switch (registers.IFID.Func)
                {
                    case 0x20:
                        registers.IDEX.Aluoperation = Aluop.ADD;
                        break;
                    case 0x24:
                        registers.IDEX.Aluoperation = Aluop.AND;
                        break;
                    case 0x25:
                        registers.IDEX.Aluoperation = Aluop.OR;
                        break;
                    case 0x22:
                        registers.IDEX.Aluoperation = Aluop.SUB;
                        break;
                    default:
                        registers.IDEX.Aluoperation = Aluop.NOP;
                        break;
                }
            }
            //if it's R-Format
            if(registers.IFID.Opcode == 0)
            {
                //read register values
                rsvalue = registers[registers.IFID.Rs];
                rtvalue = registers[registers.IFID.Rt];
                //progpagte registers
                registers.IDEX.ReadData1 = rsvalue;
                registers.IDEX.ReadData2 = rtvalue;
                registers.IDEX.Rd = registers.IFID.Rd;
            }
            else
            {
                registers.IDEX.Offset = registers.IFID.Offset;
                registers.IDEX.Rt = registers.IFID.Rt;
                rsvalue = registers[registers.IFID.Rs];
                registers.IDEX.ReadData1 = rsvalue;
            }

            if (registers.IFID.NOP == true)
            {
                NOPClear(registers.IDEX);
                registers.IDEX.NOP = true;
                return;

            }
        }

        public void ExecStage()
        {
            int aluip2 = ALUSrcMUX(registers.IDEX.ReadData2, registers.IDEX.Offset, registers.IDEX.ALUSrc);
            int result = ALU(registers.IDEX.ReadData1, aluip2, registers.IDEX.Aluoperation);
            int tempRD = RegDstMUX(registers.IDEX.Rt, registers.IDEX.Rd, registers.IDEX.ALUREGdst);

            registers.EXMEM.MemPCSrc = registers.IDEX.MemPCSrc;
            registers.EXMEM.MemRead = registers.IDEX.MemRead;
            registers.EXMEM.MemWrite = registers.IDEX.MemWrite;
            //wb control singals
            registers.EXMEM.MemToReg = registers.IDEX.MemToReg;
            registers.EXMEM.RegWrite = registers.IDEX.RegWrite;
            //ALU control signals
            registers.EXMEM.Aluoperation = registers.IDEX.Aluoperation;
            registers.EXMEM.ALUSrc = registers.IDEX.ALUSrc;
            registers.EXMEM.ALUREGdst = registers.IDEX.ALUREGdst;

            registers.EXMEM.Rd = tempRD;
            registers.EXMEM.Rt = tempRD;
            registers.EXMEM.Result = result;
            registers.EXMEM.WriteData = registers.IDEX.ReadData2;

            if (registers.IDEX.NOP == true)
            {
                NOPClear(registers.EXMEM);
                registers.EXMEM.NOP = true;
                return;

            }

        }

        public void MemStage()
        { 
            int mem = 0;
            if(registers.EXMEM.MemRead == true)
            {
                mem = Convert.ToInt32(DataMem[registers.EXMEM.Result]);
                registers.MEMWB.Muxip1 = mem;
            }
            else
            {
                registers.MEMWB.Muxip2 = registers.EXMEM.Result;
            }

            //wb control singals
            registers.MEMWB.MemToReg = registers.EXMEM.MemToReg;
            registers.MEMWB.RegWrite = registers.EXMEM.RegWrite;
            registers.MEMWB.Rd = registers.EXMEM.Rd;

            if (registers.EXMEM.NOP == true)
            {
                NOPClear(registers.MEMWB);
                registers.EXMEM.Result = 0;
                registers.EXMEM.WriteData = 0;
                registers.MEMWB.NOP = true;
                return;
            }
        }

        public void WBStage()
        {
           
            int regval = MEMtoRegMUX(registers.MEMWB.Muxip2, registers.MEMWB.Muxip1, registers.MEMWB.MemToReg);
            if(registers.MEMWB.RegWrite == true)
            {
                registers[registers.MEMWB.Rd] = regval;
            }
            if (registers.EXMEM.NOP == true)
            {
                NOPClear(registers.MEMWB);
                registers.MEMWB.Muxip1 = 0;
                registers.MEMWB.Muxip2 = 0;
                return;
            }
        }
        public CPU()
        {
            registers = new Registers();
            InstrMem = new Dictionary<int, string>();
            DataMem = new Dictionary<int, string>();
            for(int i = 0; i <= 200; i++)
            {
                DataMem[i] = "99";
            }
        }


    }
}
