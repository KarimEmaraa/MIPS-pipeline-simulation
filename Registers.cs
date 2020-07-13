using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace MIPS
{
    public class Registers
    {
        //register bank
        private int[] r = new int[32];
        //Pipeline registers
        public IFID IFID;
        public IDEX IDEX;
        public EXMEM EXMEM;
        public MEMWB MEMWB;
        public int PC { get; set; }
        public int nextPC { get; set; }

        public int this[int i]
        {
            get
            {
                return r[i];
            }
            set
            {
                if (i == 0)
                {
                    throw new Exception("Can't ser R0 to new value");
                }
                else
                {
                    r[i] = value;
                }
            }
        }
        public Registers()
        {
            for (int i = 1; i < r.Length; i++)
            {
                r[i] = 100 + i;
            }

            this.IFID = new IFID();
            this.IDEX = new IDEX();
            this.EXMEM = new EXMEM();
            this.MEMWB = new MEMWB();
            this.PC = 1000;
            this.nextPC = 0;

        }

    }

    public enum Aluop
    {
        NOP,
        ADD,
        SUB,
        AND,
        OR
    }
    public class PipeRegister
    {
        private int rs;
        private int rt;
        private int rd;
        private int func;
        private int offset;
        private int opcode;

        //CONTROL SIGNALS
        //EXTRA CONTROL SIGNAL TO INDICATE A NOP
        public bool NOP { get; set; }
        //mem
        public bool MemRead { get; set; }
        public bool MemWrite { get; set; }
        public bool MemPCSrc { get; set; }

        //ALU
       
        public Aluop Aluoperation { get; set; }
        public bool ALUSrc { get; set; }

        public bool ALUREGdst { get; set; }

        //WriteBACK
        public bool RegWrite { get; set; }
        public bool MemToReg { get; set; }

        public int Rs
        {
            get
            {
                return rs;
            }
            set
            {
                if (value >= 0 || value <= 31)
                {
                    rs = value;
                }
                else
                    throw new Exception("Invalid Rs value");
            }
        }
        public int Rt
        {
            get
            {
                return rt;
            }
            set
            {
                if (value >= 0 || value <= 31)
                {
                    rt = value;
                }
                else
                    throw new Exception("Invalid Rt value");
            }
        }
        public int Rd
        {
            get
            {
                return rd;
            }
            set
            {
                if (value >= 0 || value <= 31)
                {
                    rd = value;
                }
                else
                    throw new Exception("Invalid Rd value");
            }
        }
        public int Func
        {
            get
            {
                return func;
            }
            set
            {
                func = value;
            }
        }

        public int Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
            }
        }

        public int Opcode
        {
            get { return opcode; }
            set { opcode = value; }
        }

        public PipeRegister()
        {

        }
        public PipeRegister(int rs ,int rt, int rd, int func)
        {
            this.rs = rs;
            this.rt = rt;
            this.rd = rd;
            this.func = func;
            this.Aluoperation = Aluop.NOP;
        }

        public PipeRegister(int rs, int rt, int offset)
        {
            this.rs = rs;
            this.rt = rt;
            this.offset = offset;
            this.Aluoperation = Aluop.NOP;
        }
    }

    public class IFID : PipeRegister
    {
        public IFID() : base()
        {

        }

        public IFID(int rs, int rt, int rd, int func) : base(rs, rt, rd, func)
        {

        }

        public IFID(int rs, int rt, int offset) : base(rs, rt, offset)
        {

        }
    }

    public class IDEX : PipeRegister
    {
        public int ReadData1 { get; set; }
        public int ReadData2 { get; set; }

        public IDEX() : base()
        {

        }

        public IDEX(int rs, int rt, int rd, int func) : base(rs, rt, rd, func)
        {

        }

        public IDEX(int rs, int rt, int offset) : base(rs, rt, offset)
        {

        }
    }

    public class EXMEM : PipeRegister
    {
        public int Result { get; set; }
        public int WriteData { get; set; }
        public EXMEM() : base()
        {

        }

        public EXMEM(int rs, int rt, int rd, int func) : base(rs, rt, rd, func)
        {

        }

        public EXMEM(int rs, int rt, int offset) : base(rs, rt, offset)
        {

        }
    }

    public class MEMWB : PipeRegister
    {
        public int Muxip1 { get; set; }
        public int Muxip2 { get; set; }

        public MEMWB() : base()
        {

        }

        public MEMWB(int rs, int rt, int rd, int func) : base(rs, rt, rd, func)
        {

        }

        public MEMWB(int rs, int rt, int offset) : base(rs, rt, offset)
        {

        }
    }

}
