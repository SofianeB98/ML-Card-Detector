#include <cstdlib>

//Build la dll en debug (et se mettre en debug)
//permet d'attacher la source.cpp à unity et de voir step by step
//ce qu'il se passe
extern "C" {

	__declspec(dllexport) double* create_linear_model(int input_counts)
	{
		auto weights = new double[input_counts + 1];

		for(auto i = 0; i < input_counts + 1; ++i)
		{
			weights[i] = rand() / static_cast<double>(RAND_MAX) * 2.0 - 1.0;
		}

		return weights;
	}

	__declspec(dllexport) double* predict_linear_model_multiclass_classification(double* model, double inputs[], int input_count, int class_count)
	{
		// todo

		return new double[3] {1.0, 1.0, -1.0};
	}

	__declspec(dllexport) double predict_linear_model_classification(double* model, double inputs[], int input_count)
	{
		// todo

		return 1;
	}
	
	__declspec(dllexport) void train_linear_model_rosenblatt(double* model, double all_inputs[], int input_count, int sample_couts, double all_expected_outputs[], int expected_output_count, int epochs, double learning_rate)
	{
		// todo

		return;
	}
	
	__declspec(dllexport) void delete_linear_model (double* model)
	{
		delete[] model;
	}
	//__declspec(dllexport) Permet de spécifier, pour windows, que cette fonction va en dll
	//l'interface d'une fonction doit respecter les code C
	//mais son contenu peut faire appel à du code C++ / C / Other
	__declspec(dllexport) double my_add(double a, double b)
	{
		return a + b;
	}
	
}
