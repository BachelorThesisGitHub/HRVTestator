using System;

namespace HRVTestator
{
    public class Measurement
    {
        public enum EnumPhase { Pre, Exp, Post}

        public float HRV { get; set; }

        public int RR { get; set; }

        public bool IsValid { get; set; }

        public DateTime DateTime { get; set; }

        public EnumPhase Phase { get; set; }
    }
}