/*
	Crank Nicholson Method - 1D Heat Eq. / Diffusion
	Jordan Walsh - 120387836
*/


// Header
#include <iostream>
#include <string>
#include <stdio.h>
#include <unistd.h>
#include <math.h>
using namespace std;
#include <stdlib.h>
#include <stdarg.h>
#include <assert.h>
#include <complex.h>
#define complex _Complex
#include "gnuplot.cxx"

const double pi = M_PI;

// Constants (max. size of matrix)
const int max_matrix = 10000;

// Input (will not be distroyed)
double trimatrix_diag [max_matrix];

// we assume the value 1 on the subdiagonal here
double vector_b [max_matrix];

// Output
double solution_psi [max_matrix];

// only used during solution_tridiagonal_matrixequation
double value_d [max_matrix], value_bx [max_matrix];

//Some of the above have been changed so that they are not complex valued


// Solution of a Matrix Equation with a Tridiagonal Matrix (do not change)
void solution_tridiagonal_matrixequation (int nn)
{
	int j;
	
	// First forward loop
	value_d [0] = trimatrix_diag [0];
	value_bx [0] = vector_b [0];
	for (j=1; j<nn; j=j+1)
	{
		value_d [j] = trimatrix_diag[j] - 1.0/value_d[j-1];
		value_bx [j] = vector_b [j] - value_bx [j-1]/value_d[j-1];
	}
	
	// Second backward loop
	solution_psi [nn-1] = value_bx[nn-1]/value_d[nn-1];
	for (j=nn-2; j>=0; j=j-1)
	{
		solution_psi [j] = (value_bx[j]-solution_psi [j+1])/value_d [j];
	}
}


// constants
const double length = 10.0;
double final_time = 0.2;

double x0 = 5.0;  // center of initial wave packet
double sigma = 0.1; // width parameter of initial distribution

const int number_x_steps = 1000;
const int number_t_steps = 1000;
double heat_distribution [number_t_steps][number_x_steps];


// main function (to be changed)
int main ()
{
	int nt, nx;
	double delta_x, delta_t, alpha, x;
	double beta, betax;
	double data_x [number_x_steps], data_y1 [number_x_steps], data_y2 [number_x_steps];
	
	delta_x = length/(number_x_steps + 1.0);
	delta_t = final_time/(1.0*number_t_steps);

	double a = 1; //Diffusion coefficient
	alpha = delta_t*a/(2*delta_x*delta_x);
	//Betas in form seen in notes such that algorithm will solve without changes to code
	beta = -1.0/alpha - 2.0;
	betax = -1.0/alpha + 2.0; 

	// set trimatrix for free Schroedinger evolution - applies to 1D heat Eq.
	for (nx=0; nx<number_x_steps; nx=nx+1)
		trimatrix_diag [nx] = beta;

	// set initial distribution
	for (nx=0; nx < number_x_steps; nx=nx+1)
	{
		x = (nx+1) * delta_x;
		//GAUSSIAN DISTRIBUTION
		heat_distribution [0][nx] = 1.0/sqrt(sqrt(pi)*sigma)*exp(-(x-x0)*(x-x0)/(2.0*sigma*sigma));
	}	
	
	// time steps
	for (nt=0; nt < number_t_steps-1; nt=nt+1)
	{
		// calculate vector b
		vector_b [0] = betax * heat_distribution [nt][0] - heat_distribution [nt][1];
		for (nx=1; nx < number_x_steps-1; nx=nx+1)
		{
			vector_b [nx] = -heat_distribution [nt][nx-1] + betax * heat_distribution [nt][nx] - heat_distribution [nt][nx+1];
		}
		vector_b [number_x_steps-1] = -heat_distribution [nt][number_x_steps-2] + betax * heat_distribution [nt][number_x_steps-1];

		// solve matrixequation
		solution_tridiagonal_matrixequation (number_x_steps);
		
		// copy solution in wavefunction
		for (nx=0; nx<number_x_steps; nx=nx+1)
			heat_distribution [nt+1][nx] = solution_psi [nx];
	}
	
	// set plotting data
	for (nx=0; nx < number_x_steps; nx=nx+1)
	{
		data_x [nx] = (nx+1) * delta_x;
		data_y1 [nx] = sqrt(heat_distribution [0][nx]*heat_distribution [0][nx]);
		data_y2 [nx] = sqrt(heat_distribution [number_t_steps-1][nx]*heat_distribution [number_t_steps-1][nx]);
	}

	gnuplot_two_functions ("1D Diffusion / Heat Distribution", "linespoints", "x", "|psi|^2",
		data_x, data_y1, number_x_steps, "inital time",
		data_x, data_y2, number_x_steps, "final time");
		
	return 0;
}
