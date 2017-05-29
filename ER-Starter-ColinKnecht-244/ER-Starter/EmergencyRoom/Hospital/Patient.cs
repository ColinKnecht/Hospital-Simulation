using System;
using System.Collections.Generic;
/// <summary>
/// COLIN KNECHT -- FINAL PROGRAM -- CPT 244
/// </summary>
namespace Hospital
{
    public class Patient
    {
        public int TimeToLive { get; private set; }
        public int TimeForProcedure { get; private set; }
        public int LastPossibleMoment { get; private set; }
        private int intakeTime;

        public int IntakeTime {
            get { return intakeTime; }
            set { intakeTime = value; LastPossibleMoment = intakeTime + TimeToLive;  }
        }

        public int TimeEnteringER { get; set; } // time actually on er table


        public Patient(int ttl, int tableTime)
        {
            TimeToLive = ttl;
            TimeForProcedure = tableTime;
            LastPossibleMoment = -1;
            IntakeTime = -1;
        }
    }
}
