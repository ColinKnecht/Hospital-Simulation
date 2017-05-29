using System;
using System.Collections.Generic;
using PriorityQueue;
using System.Collections;
/// <summary>
/// COLIN KNECHT -- FINAL PROGRAM -- CPT 244
/// </summary>
namespace Hospital
{
    // DO NOT MODIFY
    public class Hospital
    {
        static public int CurrentTime { get; set; }
        static void Main(string[] args)
        {
            EmergencyRoom er = new EmergencyRoom();
            er.processPatients();
            Console.ReadLine();
        }
    }

    // DO NOT MODIFY
    class ERTable
    {
        // estimated time of completion
        public int ETC { get; set; }
        Patient patient;
        public bool IsFree { get { return (patient == null);}}

        public void AddPatient(Patient p)
        {
            patient = p;
            ETC = p.TimeForProcedure + Hospital.CurrentTime;
        }

        public void Clear()
        {
            patient = null;
            ETC = 0;
        }
    }

    // DO NOT MODIFY
    class TriageUnit
    {
        private readonly int seed = 97;
        private readonly Random rand;

        const int MIN_EXPIRY = 20;     // minimum time until patient will die if not seen
        const int MAX_EXPIRY = 120;    // maximum time until patient will die if not seen
        const int MIN_PATIENTS = 2;    // at least this many new patients every 10 minutes
        const int MAX_PATIENTS = 5;    // no more than this many new patients every 10 minutes
        const int MIN_TABLE_TIME = 10; // shortest time in the ER
        const int MAX_TABLE_TIME = 50; // longest time in the ER

        public TriageUnit()
        {
            rand = new Random(seed);
        }

        public Queue<Patient> getNewPatients()
        {

            Queue<Patient> newPatients = new Queue<Patient>();
            int numPatients = rand.Next(MIN_PATIENTS,MAX_PATIENTS + 1);
            for (int i = 0; i < numPatients; i++)
            {
                int expiryTime = rand.Next(MIN_EXPIRY, MAX_EXPIRY + 1);
                int operationTime = rand.Next(MIN_TABLE_TIME, MAX_TABLE_TIME + 1);

                newPatients.Enqueue(new Patient(expiryTime, operationTime));
            }
            return newPatients;
        }
    }

    // TODO -- your code goes here
    class EmergencyRoom
    {
        const int NUM_TABLES = 12;
        const int SIMULATION_DURATION = 60 * 12; // one shift

        public void processPatients()
        {
            IQueue<Patient> waitingRoom = null;
            bool usePriority = false;//orig false

            // create an array or list of ERTables
            ERTable[] tables = new ERTable[NUM_TABLES];

            // TODO -- populate the array with tables
            for (int i = 0; i < NUM_TABLES; i++)/////////////colin
            {
                tables[i] = new ERTable();
            }

            // this for loop will run twice, once with a SimpleQueue and once with the PriorityQueue
            for (int i = 0; i < 2; i++) {
                Console.WriteLine("\nUsing Priority Queue: {0}", usePriority);
                int totalExpired = 0;
                int totalPatients = 0;
                int maxWait = 0;
                int tmpMaxWait = 0;
                int totalWait = 0;
                int totalStay = 0;
                int maxWaitingInLine = 0;
                int tmpWaitingInLine = 0;
                bool stillWorking = false;


                // TODO -- create triage unit
                TriageUnit triageUnit = new TriageUnit();
                Queue<Patient> frontDoor = triageUnit.getNewPatients();

                // TODO set the waitingQueue to one or the other type Queue, depending on the value of usePriority
                // you will need to instantiate the appropriate Queue in the if/else statement.
                if (usePriority == true)
                {
                    waitingRoom = new PriorityQueue<Patient>();
                    while (frontDoor.Count > 0)
                    {
                        Patient p = frontDoor.Dequeue();
                        waitingRoom.Add(p.LastPossibleMoment,p);
                    }  
                }
                else if (usePriority == false)
                {
                    waitingRoom = new SimpleQueue<Patient>();
                    while (frontDoor.Count > 0)
                    {
                        Patient p = frontDoor.Dequeue();
                        waitingRoom.Add(p.LastPossibleMoment, p);
                    }
                }
                // Reset Hospital clock
                Hospital.CurrentTime = 0;

                while (Hospital.CurrentTime < SIMULATION_DURATION || waitingRoom.Count > 0 || stillWorking) {
                    // TODO empty tables that are free
                    // look for table where ETC <= currentTime
                    // do not look for expired patients; if they made it to an ER table, they lived
                    for (int j = 0; j < NUM_TABLES; j++)
                    {
                        if (tables[j].ETC <= Hospital.CurrentTime)
                        {
                            tables[j].Clear();
                        }
                    }
                        // NOTE: do the following *ONLY* if currentTime < simulation duration, otherwise you'll never finish
                        // TODO: get list of new patients from triage unit
                    if (Hospital.CurrentTime < SIMULATION_DURATION)
                    {
                        // for each patient in the triage queue
                        // set IntakeTime for each patient to 'currentTime'
                        // place new patients into waiting room
                        // when placing in waiting room, priority is the patient's last possible moment
                        frontDoor = triageUnit.getNewPatients();
                        foreach (Patient p in frontDoor)
                        {
                            p.IntakeTime = Hospital.CurrentTime;
                        }
                        while (frontDoor.Count > 0)
                        {
                            Patient p = frontDoor.Dequeue();
                            waitingRoom.Add(p.LastPossibleMoment, p);
                        }
                    }
                    // TODO: check for maximum number in waiting room
                    if (waitingRoom.Count > 0)
                    {
                        tmpWaitingInLine = waitingRoom.Count;
                        if (tmpWaitingInLine > maxWaitingInLine)
                        {
                            maxWaitingInLine = tmpWaitingInLine;
                        }
                    }
                    // TODO: for every EMPTY tables, be careful here
                        // remove next patient from waiting room
                        // check for expired patients (count them)
                        // (if the patient has expired, you'll need to get another one)
                        // placing living patients on empty ER table
                        // update any accumulators, maximums, etc.
                    for (int d = 0; d < NUM_TABLES; d++)
                    {
                        if (tables[d].IsFree && waitingRoom.Count > 0)
                        {
                            Patient p = waitingRoom.Remove();
                            //currenttime > intaketime + timetolive
                            if (Hospital.CurrentTime > p.IntakeTime + p.TimeToLive)
                            {
                                //died
                                totalExpired++;
                                p = null;
                            }
                            else if (Hospital.CurrentTime < p.IntakeTime + p.TimeToLive)
                            {
                                tmpMaxWait = Hospital.CurrentTime - p.IntakeTime;
                                if (tmpMaxWait > maxWait)
                                {
                                    maxWait = tmpMaxWait;
                                }
                                totalWait += Hospital.CurrentTime - p.IntakeTime;
                                //totalStay = 
                                p.TimeEnteringER = Hospital.CurrentTime;
                                totalStay += p.TimeEnteringER + p.TimeForProcedure - p.IntakeTime;
                                tables[d].AddPatient(p);
                                totalPatients++;
                            }
                        }
                    }

                    stillWorking = false;
                    // TODO: Make certain ALL of the tables are free
                    // if any table is not free, set stillWorking to true
                    for (int d = 0; d < NUM_TABLES; d++)
                    {
                        if (!tables[d].IsFree)
                        {
                            stillWorking = true;
                        }
                    }

                    // set add 10 minutes to hospital time
                    Hospital.CurrentTime += 10;
                }

                // print parameters (num tables, duration, using priority queue)
                // print time, total patients, max waiting, average waiting, expired patients
                Console.WriteLine("Total Elapsed Time: {0,7}", Hospital.CurrentTime);
                Console.WriteLine("Total patients:     {0,7}", totalPatients);
                Console.WriteLine("Total Expired:      {0,7}", totalExpired);
                Console.WriteLine("Longest Wait:       {0,7}", maxWait);
                Console.WriteLine("Average Wait:       {0,7:N2}", (double)totalWait/totalPatients);
                Console.WriteLine("Average Stay:       {0,7:N2}", (double)totalStay/totalPatients);
                Console.WriteLine("Maximum waiting:    {0,7:N2}", maxWaitingInLine);

                usePriority = true;
            } // end for
        } // end processPatients
    }
}
