
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
// Constants
const int max_number_steps = 100000;
// Main program (to be changed)
const int max_length_data = 10000;
int main ()
{
	int number_steps = 100;
	double N[number_steps];
	double t[number_steps];
	double delta_t, tf, k1, k2, k3, k4;
	//initial conditions - USER DEFINES HERE
	int nr = 0;
	tf = 10;
	delta_t = tf/number_steps;
	int N_0 = 100;
	N[0] = N_0;
	t[0] = 0;
	double c=0.5;
	double ex[number_steps];
	
	for (int j=0; j<number_steps; j+=1)
	{
	// do Runge-Kutta-Step: j -> j+1
		k1 = delta_t * -c*N[j];
		k2 = delta_t * -c*(N[j] + 0.5 * k1);
		k3 = delta_t * -c*(N[j] + 0.5 * k2);
		k4 = delta_t * -c*(N[j] + k3);
		N[j+1] = N[j] + 1.0/6.0 * k1 + 1.0/3.0 * k2 + 1.0/3.0 * k3 + 1.0/6.0*k4;
		nr += 1;
		t[j] = j*delta_t;
		ex[j] = N_0*exp(-c*t[j]);
	}
	//ONE FUNCTION
		gnuplot_two_functions ("Scattering: square potential: transmission probability", "linespoints", "p", "|T|^2",
			t, N, nr, "numerical", t, ex, nr, "analytical");
		
	return 0;
}
