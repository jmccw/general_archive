using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CA4_MA2060
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                EulerSIR e = new EulerSIR();
                e.Stepsize = 0.1;
                         //S    //I   //R
                e.SetInit(0.999, 0.001, 0);
                e.Solve();
                e.WriteToFile("c://testfile//SIR.csv");         //can't create a file directly in c:/ 
            }
            catch(Exception E)
            {
                Console.WriteLine("An error occured: {0}", E);
            }

            Console.WriteLine("Completed.");
        }
    }
    class EulerSIR
    {
        //manipulatable data - to make the program a bit more practical from main()
        public double Stepsize;             //AKA: h
        public double D;                    //average time a person stays in the infected compartment in days. the fact that this is in days is important.
        public double R0;                   //reproductive number                              //^ this means that the rest of our time units must be in 
        public double X;                    //runtime in Days                                  //  terms of days. Such as 'X'.                  

        private double[,] SIRdata = null;   //double array as specified

        private double Si;      //
        private double Ii;      //initial values
        private double Ri;      //

        private double s;           //proportion of S   //"S dot"
        private double i;           //proportion of I   //"I dot"
        private double r;           //proportion of R   //"R dot"
        private double beta;        //infectious rate
        private double gamma;       //recovery rate

        private int J;

        public EulerSIR()
        {
            Stepsize = 0.1;   //default stepsize - can be modified by user
            D = 14;             //default D - days
            R0 = 2.4;           //default value for R0 - in this case that of COVID-19
            X = 200;            //default value for X if user does not specify - days
        }

        public void SetInit(double Si, double Ii, double Ri)
        {
            if (Si + Ii + Ri == 1)
            {
                this.Si = Si;
                this.Ii = Ii;
                this.Ri = Ri;
            }
            else
            {
                throw new Exception("Invalid parameters: S + I + R must equal 1.");
            }
        }

        public void WriteToFile(string location)
        {
            try
            {
                StreamWriter sw = new StreamWriter(@location, false);
                sw.WriteLine("x, S, I, R");
                for (int j = 0; j <= J; j++)
                {                                              //x          //S             //I             //R         ( See Solve() )
                        sw.WriteLine("{0}, {1}, {2}, {3}", SIRdata[j, 3], SIRdata[j, 0], SIRdata[j, 1], SIRdata[j, 2]);
                                                                                    //I specifically wrote the code like this so that the
                }                                                                   //values can be graphed with ease in excel post execution.
                sw.Close();                                                         //with bigger time steps (0.1) and large runtimes it very obvious 
            }                                                                       //that the algorithm works (seeing it graphically)   
            catch (Exception)
            {   
                throw new Exception("There was an issue with the specified path. Please check your input or if you have the file currently open.");
            }
        }

        public void Solve()
        {
            if (R0 <= 1)
            {
                throw new Exception("Cannot solve the model. R0 number invalid.");
            }

            //from initial conditions
            gamma = 1 / D;
            beta = R0 * gamma;

            //to ensure arrays work as intended and dynamically according to inital conditions set by user:
            double q = X / Stepsize;
            if (q % 1 != 0 || q <= 0)   //q%1 checks if there is a floating point of q, if not then the users preffered parameters are valid
            {
                throw new Exception("Invalid parameters: runtime parameter J will not be an integer or is null/negative");
            }
            J = Convert.ToInt32(q);

            //2d array as specified - array must have J+1 components, as otherwise the euler method will finish 1 timestep early - not completing the model for time specified
            SIRdata = new double[J + 1, 4];         //these were also assigned here in the case that the user may enter custom parameters of X or Stepsize.
            SIRdata[0, 3] = 0;              //x
            SIRdata[0, 0] = Si;             //s
            SIRdata[0, 1] = Ii;             //i
            SIRdata[0, 2] = Ri;             //r

            //iteration of Euler method
            for (int j = 0; j < J; j++)
            {
                s = -(beta * SIRdata[j, 1] * SIRdata[j, 0]);
                i = (beta * SIRdata[j, 1] * SIRdata[j, 0]) - (gamma * SIRdata[j, 1]);
                r = gamma * SIRdata[j, 1];

                SIRdata[j + 1, 0] = SIRdata[j, 0] + Stepsize * s;       //s
                SIRdata[j + 1, 1] = SIRdata[j, 1] + Stepsize * i;       //i
                SIRdata[j + 1, 2] = SIRdata[j, 2] + Stepsize * r;       //r
                SIRdata[j + 1, 3] = SIRdata[j, 3] + Stepsize;           //x
            }
        }
    }
}
