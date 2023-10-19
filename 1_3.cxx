
#include <iostream>
using namespace std;

int n;
int main(int argc, char **argv)
{
	cout << "This program calculates and prints the inital n Fibonacci numbers.\nEnter an integer n:\n\nn > ";
	cin >> n;
	
	int fibonacci[n];
	fibonacci[0] = 0;
	fibonacci[1] = 1;
	
	//CALCULATE FIBONACCI NUMBERS
	for(int i = 0; i < n; i++){
		if(i > 1) fibonacci[i] = fibonacci[i-1] + fibonacci[i-2];	//"sum of two preceeding numbers"

	}
	
	//PRINT CALCULATED NUMBERS
	cout << "\nSet of first " << n << " Fibonacci numbers: ";
	for(int i = 0; i < n; i++){		//
		if(i == 0) cout << "[";		//This is just formatting to keep the output clear. (decoration*)
		cout << fibonacci[i];		//
		if(i != n-1) cout << ", ";	// (decoration*)
		else cout << "]";			// (decoration*)
	}
	
	return 0;
}

//THE PROGRAM CAN BE EASILY SHORTENED BY UTILISING A SINGLE LOOP - THE ASSIGNMENT SPECIFES TO SPLIT UP THESE OPERATIONS:
//  ALTERNATIVE SOLUTION:

//for(int i = 0; i < n; i++){		//utilising a loop that prints as numbers are calculated.
		//if(i == 0) cout << "[";		//This is just to keep the output clean and clear. (decoration*)
		//if(i > 1) fibonacci[i] = fibonacci[i-1] + fibonacci[i-2];	//"sum of two preceeding numbers"
		//cout << fibonacci[i];		//printing numbers as per Q1.3
		//if(i != n-1) cout << ", ";	// (decoration*)
		//else cout << "]";			// (decoration*)
//}
