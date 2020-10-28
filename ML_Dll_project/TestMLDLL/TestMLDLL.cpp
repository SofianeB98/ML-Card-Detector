#include <iostream>

#include "../MLDLL/Source.cpp"
extern "C"{
	__declspec(dllimport) double my_add(double a, double b);
}

int main()
{
    std::cout << "Hello World!\n " << my_add(42, 51) << std::endl;
	int* npl = new int[3]{2, 3, 1};
	MLP* model = create_model(npl, 5);

	int* X = new int[8]
	{
		0, 0, 0, 1, 1, 0, 1, 1
	};

	int* Y = new int[4]{
		-1, 1, 1, -1
	};


	int sampleSize = 4;

	std::cout << "BEFORE TRAINING !!" << std::endl;
	for(int k = 0; k < sampleSize; k++)
	{
		int sliceSize = (2 * (k + 1)) - (k * 2);
		double* slice = new double[sliceSize];
		for (int i = 0; i < sliceSize; i++) 
		{
			slice[i] = (double)X[k * 2] + i;
		}
		forward_pass(model, slice, false);

		for (int i = 1; i < model->L + 1; i++) {
			std::cout << model->x[model->L][i];
		}

	}

	delete[] npl;
	delete[] X;
	delete[] Y;

}
