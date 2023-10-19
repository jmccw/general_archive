
#include <iostream>
#include "gnuplot.cxx"
using namespace std;
const double g = 9.81; // acceleration due to gravity (m/s^2)

double v0, x0, tf;
int N;
int main(int argc, char **argv)
{
	cout << "v0 (m/s) > ";			//user enters model parameters
	cin >> v0;
	cout << "x0 (m) > ";
	cin >> x0;
	cout << "tf (s) > ";
	cin >> tf;
	cout << "N [number of plot points] (including inital conditions) > ";
	cin >> N;

	double dt = tf/(N-1);			//for N plot points we require N-1 "gaps"
	double x[N], t[N];				//numerical arrays to store results.
	x[0] = x0;
	t[0] = 0;

	for(int i = 1; i < N; i++){		//starts from i-1 as to fill 10 data points spaced by dt up to tf.
		t[i] =  t[i-1] + dt;  		//t + dt	
		x[i] = -(g*t[i]*t[i])/2 + v0*t[i] + x0;	//x(t) as determined in 1_3.pdf
	}

	cout << "Maximum height x_m: " << (v0*v0)/(2*g) << "m";

	cout << "\n"; //terminal spacing
	gnuplot_one_function("Plot of height Vs. time", "linespoints", "Time (s)", "Trajectory (m)", t, x, N);

	return 0;
}
