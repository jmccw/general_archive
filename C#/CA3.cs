using System;

namespace CA3_AM2060
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Matrix m = new Matrix(4);
                Vector b = new Vector(4);

                m[0, 0] = 9; m[0, 1] = -2; m[0, 2] = 3; m[0, 3] = 2;
                m[1, 0] = 2; m[1, 1] = 8; m[1, 2] = -2; m[1, 3] = 3;
                m[2, 0] = -3; m[2, 1] = 2; m[2, 2] = 11; m[2, 3] = -4;
                m[3, 0] = -2; m[3, 1] = 3; m[3, 2] = 2; m[3, 3] = 10;
                b[0] = 54.5; b[1] = -14; b[2] = 12.5; b[3] = -21;
                Console.WriteLine("The matrix m is: \n\n{0}", m);
                Console.WriteLine("The vector b is: \n\n{0}", b);

                LinSolve l = new LinSolve();
                Vector ans = l.Solve(m, b);

                Console.WriteLine("The solution to mx = b is:\n\n{0}", ans);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error encountered: {0}", e.Message);
            }
        }
    }

    class Vector
    {
        private double[] data = null;
        private int size;

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public Vector()                                 //Constructor initialises all components to 0
        {
            size = 3;                                   //defaults value to 3 as specified
            data = new double[size];
            for (int j = 0; j <= size - 1; j++)
                data[j] = 0;
        }

        public Vector(int size)                         //again, Constructor initialises all components to 0
        {
            if (size > 1)
            {
                this.size = size;
                data = new double[size];

                for (int i = 0; i < size; i++)
                    data[i] = 0;
            }
            else
            {
                ArgumentNullException e = new ArgumentNullException("vector 'size' is not valid.");
                throw e;
            }

        }

        public void SetValue(int index, double val)
        {
            if (index < 0 || index > data.Length - 1)
            {
                Console.WriteLine("Invalid Index: nothing has been changed");   //is this not handled in 'this'? asked about this in class but I dont think you understood what I meant.
            }
            data[index] = val;
        }

        public double this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                if (index < 0 || index >= size)
                {
                    throw new Exception("Invalid index passed into Vector");
                }
                else
                {
                    data[index] = value;
                }
                data[index] = value;
            }
        }

        public static Vector operator +(Vector left, Vector right)
        {
            if (left.Size != right.Size)
            {
                ArgumentNullException e = new ArgumentNullException("Vector 'sizes' do not equal");
                throw e;
            }
            else
            {
                Vector tmp = new Vector(left.Size);

                for (int i = 0; i < left.Size; i++)
                    tmp[i] = left[i] + right[i];            //basic vector algebra, looped for all row(s).
                return tmp;
            }
        }

        public static Vector operator -(Vector left, Vector right)
        {
            if (left.Size != right.Size)
            {
                ArgumentNullException e = new ArgumentNullException("Vector 'sizes' do not equal");
                throw e;
            }
            else
            {
                Vector tmp = new Vector(left.Size);

                for (int i = 0; i < left.Size; i++)
                    tmp[i] = left[i] - right[i];            //basic vector algebra, looped for all row(s).
                return tmp;
            }
        }

        public double Norm(Vector vector)                   //calculates the absolute value of the vector
        {
            double result = 0;                              //initilaised to 0 due to how it used below. cant be null.

            for (int i = 0; i < vector.size; i++)
                result += (vector[i] * vector[i]);

            result = Math.Sqrt(result);
            return result;
        }

        public override string ToString()                   //modified this to be more aesthetic (but it does break with big values, problem out of scope though)
        {
            int i;
            string tmp = "";
            for (i = 0; i < size; i++)
            {
                tmp += "| ";
                tmp += data[i].ToString();
                tmp += "\t|\n";
            };
            return tmp;
        }
    }

    class LinSolve
    {
        private int numiters = 100;

        public Vector Solve(Matrix A, Vector b)
        {
            //required matrices and vectors           
            Matrix D = new Matrix(A.Size);                  //Diagonal
            Matrix Di = new Matrix(A.Size);                 //Di = Inverse of D
            Matrix L = new Matrix(A.Size);                  //Lower
            Matrix U = new Matrix(A.Size);                  //Upper
            Matrix LplusU = new Matrix(A.Size);             //L+U
            Matrix T = new Matrix(A.Size);
            Vector x = new Vector(b.Size);
            Vector c = new Vector(b.Size);
            Vector result = new Vector(A.Size);

            //splitting A into D, L and U
            for (int i = 0; i < A.Size; i++)
            {
                for (int j = 0; j < A.Size; j++)
                {
                    if (i == j)
                    {
                        D[i, j] = A[i, j];
                        Di[i, j] = (1 / A[i, j]);
                    }
                    else if (j > i)
                    {
                        U[i, j] = -(A[i, j]);
                    }
                    else if (j < i)
                    {
                        L[i, j] = -(A[i, j]);
                    }
                }
            }

            Console.WriteLine("Matrix L:\n\n" + L + "\n");
            Console.WriteLine("Matrix U:\n\n" + U + "\n");

            //calculating L+U
            for (int i = 0; i < A.Size; i++)
            {
                for (int j = 0; j < A.Size; j++)
                {
                    LplusU[i, j] = L[i, j] + U[i, j];
                }
            }
            Console.WriteLine("Matrix L+U:\n\n" + LplusU);


            T = Di * LplusU;
            c = Di * b;

            //initialising xArray for iteration
            Vector[] xArray = new Vector[numiters];             //this is an array that stores vectors of x, which simplifies the process for me in the minds eye when thinking about the problem iteratively,     
            xArray[0] = x;                                      //but I can acknowledge that this may not be as efficient memory wise as having just two vectors, especially as depending on the values provided
                                                                //the number of iterations required may not be reached. If I were to ever use this program in a practical setting I would change this (right now I just think its really cool).
                                                                //calculating result iteratively
            int w = 0;
            double e = 1;                                       //this is calculated by finding the .Norm of the vectors, see below. Has to be greater than 0 initially. Otherwise there will be no iterations.
            double E = Math.Pow(10, -16);                       //E = stopping condition. I chose to power of -16 as this produced the optimal, integer, result for the values provided in the example.

            while (e > E)
            {
                xArray[w + 1] = (T * xArray[w]) + c;
                result = xArray[w + 1];

                //stopping conditions
                double e1 = xArray[w + 1].Norm(xArray[w + 1]);
                double e2 = xArray[w].Norm(xArray[w]);
                e = Math.Abs(e1 - e2) / Math.Abs(e2);

                if (w == numiters - 1)
                {
                    Console.WriteLine("Maximum allowed iterations reached.\n");
                    return result;
                }
                w++;
            }

            Console.WriteLine("\nNumber of iterations required: {0}\nwhere E = {1}", w, E);
            return result;
        }

    }

    class Matrix
    {

        private int size;                                  // AKA 'n' in assignment details
        private double[,] data = null;

        public double this[int index, int index2]
        {
            get
            {
                return data[index, index2];
            }
            set
            {
                if (index < 0 || index >= size || index2 < 0 || index2 >= size)
                {
                    ArgumentNullException e = new ArgumentNullException("Invalid index passed into Matrix");
                    throw e;
                }
                else
                {
                    data[index, index2] = value;
                }
            }
        }
        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public Matrix()
        {
            size = 3;
            data = new double[size, size];

            for (int i = 0; i <= size - 1; i++)            //assigning all matrix values to 0           
                for (int j = 0; j <= size - 1; j++)
                    data[i, j] = 0;
        }

        public Matrix(int size)
        {
            if (size > 1)
            {
                this.size = size;
                data = new double[size, size];
                for (int i = 0; i <= size - 1; i++)        //initialising all matrix values to 0
                    for (int j = 0; j <= size - 1; j++)
                        data[i, j] = 0;
            }
            else
            {
                ArgumentNullException e = new ArgumentNullException("matrix 'size' is invalid");
                throw e;
            }
        }

        public void SetValue(int index, int index2, double val)
        {
            if (index < 0 || index > data.Length - 1)
            {
                Console.WriteLine("Invalid Index: nothing has been changed");   //is this not handled in 'this'?              
                return;
            }
            if (index2 < 0 || index2 > data.Length - 1)
            {
                Console.WriteLine("Invalid Index: nothing has been changed");
                return;
            }
            data[index, index2] = val;
        }

        public static Matrix operator *(Matrix lhs, Matrix rhs)
        {
            Matrix result = new Matrix(lhs.size);

            if (lhs.size != rhs.size)
            {
                ArgumentNullException e = new ArgumentNullException("matrix 'sizes' do not equal");
                throw e;
            }
            else
            {
                for (int i = 0; i < lhs.size; i++)                                         //standard loop for i      
                {                                                                          //and j
                    for (int j = 0; j < rhs.size; j++)                                     //third loop makes things easier to follow, w breaks up the process to allow for a semi-constant to be involved. see sum for 'result'
                    {                                                                      //no need to do this for i (add another loop), as following matrix multiplication formula we can simply swap w between first and second co-ordinate in calculating loop.
                        for (int w = 0; w < lhs.size; w++)                                 //in other words w is our 'changing constant' for the formula. While i and j play amore pivotal role in indexing.
                        {
                            result.data[i, j] += lhs[i, w] * rhs[w, j];                    //adds previous value to the next to compute final component of matrix when loop finishes
                        }
                    }
                }
            }

            return result;
        }

        public static Vector operator *(Matrix lhs, Vector rhs)
        {
            int j, i;
            Vector result = new Vector(rhs.Size);

            if (lhs.size != rhs.Size)
            {
                ArgumentNullException e = new ArgumentNullException("matrix and vector 'sizes' do not equal");
                throw e;
            }
            else
            {
                for (i = 0; i < lhs.size; i++)
                {
                    for (j = 0; j < lhs.size; j++)
                    {
                        result[i] += lhs.data[i, j] * rhs[j];                               //should be easy to follow, simple multiplication following formula
                    }
                }
            }

            return result;
        }

        public override string ToString()                       //I edited this method to be slightly more aesthetic, however this can be easily broken with larger values.
        {                                                       //this problem is out of the realistic scope of this programs intention though. Formatting could be used to fix this,                                                               
            int i = 0;                                          //this would require restructuring the program to use Console.WriteLine in all functions opposed to using a string variable.
            string tmp = "";                                    //no point really.
            for (i = 0; i < size; i++)
            {
                tmp += "|\t";
                for (int j = 0; j < size; j++)
                {
                    tmp += data[i, j].ToString();
                    if (j < size - 1)
                    {
                        tmp += "\t";
                    }
                    else if (j == size - 1)
                    {
                        tmp += "\t|\n";
                    }
                }
            }

            return tmp;
        }

    }
}
