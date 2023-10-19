
#include <iostream>
using namespace std;

int num[6]; 		 //1.1(c) specifes array of 6 numbers
int product_num = 1; //assignment initially to 1 as to not affect program result.
int main(int argc, char **argv)
{
	cout << "This program prints the product of 6 integer numbers.\nEnter 6 integer numbers:\n\n";
	for(int i = 0; i <= 5; i++){				   	//utilising a loop rather than multiple lines
		cout << "num_" << to_string(i+1) << " > "; 	//  of the same code with different variables/text.
		cin >> num[i];							   	//array assignment by user.
		product_num *= num[i];						//update product_num (initially equal to 1) by
	}												//  multiplying it with all values of num[]

	cout << "\nProduct of entered numbers: " << product_num; //prints product determined above
	return 0;
}
