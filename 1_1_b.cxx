
#include <iostream>
using namespace std;

int i = 0, num = -1; //num has been initally declared outside of the bounds specified by 1.1(b).
int main(int argc, char **argv)
{
	cout << "Enter an integer number between 0 and 9.\n\n";
	while(num < 0 || num > 9){ 	//because num = -1 initally, the while loop is entered.
		cout << "Attempt " << to_string(i+1) << " > "; 
		cin >> num;				//user updates num with input.   
		i++; 					//declared initally to 0 to track number of attempts
	}
	cout << "\nSuccess, you enetered: " << num; 
	return 0;														  
}
