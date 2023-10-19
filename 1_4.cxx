
#include <iostream>
using namespace std;
int n;
const int max_length = 100;

void print_vector(double v[]){			//this function formats the output
	for(int i = 0; i < n; i++){			//  when displaying user entries.
		if(i == 0) cout << "[";	
		cout << v[i];					//print element
		if(i != n-1) cout << ",\n ";	// (decoration*)
		else cout << "]";				// (decoration*)
	}
}

//I have defined a "matrix printer" but passing a multidimensional array into 
//  a function is not possible without defining strict dimensions (that is why 
//  I've defined max_length)
 
void print_matrix(double M[][max_length]){
	for(int i = 0; i < n; i++){	
		for(int j = 0; j < n; j++){ 
			cout << M[i][j];			//print element
			if(j != n-1) cout << ", ";	// (decoration*)
			else if (j == n-1 && i == n-1) cout << "]";
		}
		cout << "\n ";
	}
}

int main(int argc, char **argv)
{
	cout << "This program calculates and prints the matrix/vector product \
of an n component vector with an n x n matrix. \nEnter a positive integer n (max size n=100):\n\nn > ";
	cin >> n;				//user assignment of n.
	
	double v[n], M[n][max_length]; 	//declaration of vector "v" and matrix "M".
	cout << "\nDeclare your vector v and matrix M, where v(i) represents \
the ith component of v and M(i, j) represents the jth component of the ith \
row:\n\n";
	
	//USER INPUT
	for(int i = 0; i < n; i++){		
		cout << "v(" << to_string(i+1) << ") > "; 
		cin >> v[i];				//user assigns vector elements		   
	}
	
	cout << "\n";
	for(int i = 0; i < n; i++){	
		for(int j = 0; j < n; j++){ 
			cout << "M(" << to_string(i+1) << ", " << to_string(j+1) << ") > "; 
			cin >> M[i][j];			//user assigns matrix elemnts
		}
	}
	
	//SHOW USER INPUT
	cout << "\nYou have defined:\nM:\n["; 	
	print_matrix(M);					//see print_matrix() - formatting 

	cout << "\nv:\n";
	print_vector(v);					//see print_vector() - formatting
	cout << "\n\n";
	
	//CALCULATE PRODUCT
	double prod_Mv[n];
	for(int i = 0; i < n; i++){			//This loop calculates the product of
		for(int j = 0; j < n; j++){ 	//  the two defined structures as per
			prod_Mv[i] += M[i][j]*v[j];	//  Linear Algebra definition.
		}
	}

	cout << "\nThe product of these is:\n";
	print_vector(prod_Mv);				//see print_vector() - formatting

	return 0;
}

