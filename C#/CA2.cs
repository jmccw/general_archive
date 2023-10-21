using System;

namespace CA2_AM2060
{
    class Program
    {
        static void Main(string[] args)     
        {
            Simpson s10 = new Simpson();    //creating new integral "s10" for 10 intervals - this naming is not important, but does make the following easier to follow
            s10.SetNumIntervals(10);        //setting interval as specified in assignment
            Simpson s20 = new Simpson();
            s20.SetNumIntervals(20);
            Simpson s30 = new Simpson();
            s30.SetNumIntervals(30);
            Simpson s40 = new Simpson();
            s40.SetNumIntervals(40);
            Console.WriteLine("For " + s10.CheckNumInt() + " intervals: the answer is " + s10.CalcIntegral() + "\twith an error of " + s10.GetError());  //prints everyhting asked for in assignment
            Console.WriteLine("For " + s20.CheckNumInt() + " intervals: the answer is " + s20.CalcIntegral() + "\twith an error of " + s20.GetError());  
            Console.WriteLine("For " + s30.CheckNumInt() + " intervals: the answer is " + s30.CalcIntegral() + "\twith an error of " + s30.GetError());
            Console.WriteLine("For " + s40.CheckNumInt() + " intervals: the answer is " + s40.CalcIntegral() + "\twith an error of " + s40.GetError());

        }
    }

    class Simpson
    {
        private double a = 0;
        private double b = 1;
        private double sigma = 1;
        private double mu = 0.5;
        private int numint = 10;

        public void SetParameters(double a, double b, double sigma, double mu)      //needs no explanation
        {

            if (b < a)
            {
                Console.WriteLine("Invalid Parameters 'a' and 'b', 'a' must be less than 'b'. Please try again");
                return;
            }
            else
            {
                this.a = a;
                this.b = b;
            }

            if (sigma == 0)
            {
                Console.WriteLine("Invalid Parameter 'sigma'. Please try again");
                return;
            }
            else
            {
                this.sigma = sigma;

            }
            this.mu = mu;
        }

        public void SetNumIntervals(int numint)         // '%' is modulus and checks for remainders, here its purpose is to tell us if the number is odd
        {
            if (numint < 10 || (numint % 2) != 0)
            {
                Console.WriteLine("invalid parameter, 'numint' must be greater than or equal to 10. Please try again.");
            }

            this.numint = numint;
        }

        public int CheckNumInt()
        {
            return numint;
        }

        private double f(double x)
        {
            return ((1.0 / (sigma * Math.Sqrt(2.0 * Math.PI)))) * Math.Exp(-0.5 * (((x - mu) / sigma) * ((x - mu) / sigma)));       //function provided in assignment
        }

        public double CalcIntegral()
        {
            double h = (b - a) / numint;
            double cmp1 = h / 3;                    //cmp is subscript for "component of the formula" in this case

            double[] x = new double[numint];        //array to store x values easily
            x[0] = a;                               //assigns values to array for x, i.e the values that will be plugged into our function f()
            for (int i = 1; i < numint; i++)
            {
                x[i] = x[i - 1] + h;
            }

            double cmp2 = 0, cmp3 = 0;              //these set initial values to 0 for accuracy of loop in assigning values later (see below)           
            double component2, component3;          //these will act as temporary values for the loop to build on to the final cmp* value
            for (int j = 1; j < numint; j++)
            {
                if (j % 2 == 0)
                {
                    component2 = cmp2 + (f(x[j]));
                    cmp2 = component2;
                }
                else if (j % 2 != 0)
                {
                    component3 = cmp3 + (f(x[j]));      //this whole part loops to calculate values of components, uses cmp* as the previous value and builds on top of it
                    cmp3 = component3;                  //this is why cmp* was set to 0 initially
                }
                //cmp2 represents the sum of odd counts of j, cmp3 is even. (Simpsons composite rule)
            }

            double result = cmp1 * (f(a) + (2 * cmp2) + (4 * cmp3) + f(b));     //finally our cmp* values are plugged into the formula for Simpsons Composite rule.
            return result;
        }

        public double GetError()
        {
            double integral_numints = CalcIntegral();           //retrieves regular interval value
            SetNumIntervals(numint * 2);                        //modifies local value for error calculation
            double integral_2numints = CalcIntegral();          //retrieves modified value

            double result = integral_numints - integral_2numints;       //error based on assignment instructions
            return result;
        }

    }
}
