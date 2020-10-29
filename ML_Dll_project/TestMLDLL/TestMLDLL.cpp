#include <iostream>

#include "../MLDLL/Source.cpp"
extern "C"{
	__declspec(dllimport) double my_add(double a, double b);
}

int main()
{
    std::cout << "Hello World!  " << my_add(42, 51) << std::endl;
	int npl[] = {2, 3, 1};
	MLP* model = create_model(npl, 2);

	double X[8] = {
		1.0, 1.0,
		2.0, 3.0,
		3.0, 3.0,
	};

	double Y[4] = {
		1.0,
		-1.0,
		-1.0,
	};

	const int sampleSize = 3;

	//std::cout << "BEFORE TRAINING !!" << std::endl;
	//for(int k = 0; k < sampleSize; ++k)
	//{
	//	int sliceSize = (2 * (k + 1)) - (k * 2);
	//	double* slice = new double[sliceSize];
	//	for (int i = 0; i < sliceSize; i++) 
	//		slice[i] = (double)X[k * 2] + i;
	//	
	//	forward_pass(model, slice, true);

	//	for (int i = 1; i < model->x[model->L].size(); i++)
	//		std::cout << model->x[model->L][i] << std::endl;

	//	delete[] slice;
	//}

	//delete_model(model);

	//model = create_model(npl, 3);
	
	train(model, X, Y, sampleSize, 100, 0.01, true);

	std::cout << "AFTER TRAINING !!" << std::endl;
	for (int k = 0; k < sampleSize; ++k)
	{
		int sliceSize = (2 * (k + 1)) - (k * 2);
		double* slice = new double[sliceSize];
		for (int i = 0; i < sliceSize; i++)
			slice[i] = (double)X[k * 2] + i;

		forward_pass(model, slice, true);

		for (int i = 1; i < model->x[model->L].size(); i++)
			std::cout << model->x[model->L][i] << std::endl;

		delete[] slice;
	}

	delete_model(model);

	return 0;
}
