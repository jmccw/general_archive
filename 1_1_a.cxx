
#include <iostream>
using namespace std;

double num[4]; //1.1(a) specifes 4 numbers -> using array of 4 numbers
int main(int argc, char **argv)
{
	//USER ENTERS NUMBERS
	cout << "This program prints the maximum of 4 double numbers.\nEnter 4 double numbers (double numbers only!):\n\n";
	for(int i = 0; i <= 3; i++){				   //utilisign a loop+array rather than multiple lines
		cout << "num_" << to_string(i+1) << " > "; //  of the same code with different variables/text.
		cin >> num[i];							   //array assignment by user.
	}
	
	double max = num[0];							//initialise 'max' to num[0] and then checks if there are any other
	for(int i = 0; i <= 3; i++){					//  elements in num array that are bigger than num[0].
		if(num[i] > max) max = num[i];
	}

	cout << "\nMaximum entered number: " << max;	//display 'max'
	return 0;
}
