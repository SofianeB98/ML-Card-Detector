#include <iostream>
#include <vector>

extern "C"{
	struct MLP
	{
		/// <summary>
		/// correspond à npl
		/// </summary>
		std::vector<int> d;

		/// <summary>
		/// correspond à npl.size() - 1
		/// </summary>
		int L;

		/// <summary>
		/// Three Dimensionnal Array weight
		/// [][][] model
		/// </summary>
		std::vector< //[]
			std::vector< //[][]
			std::vector<double> > > w;

		/// <summary>
		/// Valeur effective des neuronnes
		/// </summary>
		std::vector< std::vector<double> > x;

		/// <summary>
		/// marche d'erreur
		/// </summary>
		std::vector< std::vector<double> > deltas;
	};

	struct RBF
	{
		std::vector<std::vector<double>> centroids;
		std::vector<double> w;
		double gamma;
		int k;
	};
	
	__declspec(dllimport) double my_add(double a, double b);
	
	__declspec(dllexport) MLP* create_model(int npl[], int layer_counts);
	__declspec(dllexport) void train(MLP* model, double allInputs[], double allExpectedOutputs[],
		int sampleCount, int epochs, double alpha, bool isClassification);
	__declspec(dllexport) void forward_pass(MLP* model, double inputs[], bool isClassification);
	__declspec(dllexport) void delete_model(MLP* model);
	
	__declspec(dllexport) void train_linear_model_regression(double* model, double all_inputs[], int input_count,
		int sample_counts, double all_expected_outputs[], int expected_output_count);
	__declspec(dllexport) double* create_linear_model_regression(int input_counts);
	__declspec(dllexport) double predict_linear_model(double* model, double inputs[], int input_count, bool is_classification);

	__declspec(dllexport) RBF* create_rbf_model(int k, double gamma);
	__declspec(dllexport) void train_rbf_model(RBF* model, double all_inputs[], int input_count,
		int sample_counts, double all_expected_outputs[], int expected_output_count);
	__declspec(dllexport) double predict_rbf(RBF* model, double inputs[], int input_count);
	__declspec(dllexport) void delete_rbf_model(RBF* model);
}

int main()
{
    std::cout << "Hello World!  " << my_add(42, 51) << std::endl;
	/*int npl[] = {2, 3, 1};
	MLP* model = create_model(npl, 3);

	double X[] = {
		0.0, 0.0,
		0.0, 1.0,
		1.0, 0.0,
		1.0, 1.0
	};

	double Y[] = {
		-1.0,
		1.0,
		1.0,
		-1.0
	};

	const int sampleSize = 4;

	double _00[] = { 0.0, 0.0 };
	double _10[] = { 1.0, 0.0 };
	double _01[] = { 0.0, 1.0 };
	double _11[] = { 1.0, 1.0 };
	
	std::cout << "\n BEFORE TRAINING !!" << std::endl;
	for (int k = 0; k < sampleSize; ++k)
	{

		forward_pass(model, k == 0 ?
			_00 : k == 1 ?
			_10 : k == 2 ? _01 : _11, true);

		for (int i = 1; i < model->x[model->L].size(); i++)
			std::cout << model->x[model->L][i] << std::endl;
	}

	train(model, X, Y, sampleSize, 10000, 0.1, true);

	std::cout << "\n AFTER TRAINING !!" << std::endl;
	for (int k = 0; k < sampleSize; ++k)
	{
		
		forward_pass(model, k == 0 ? 
			_00 : k == 1 ?
			_10 : k == 2 ? _01 :_11, true);

		for (int i = 1; i < model->x[model->L].size(); i++)
			std::cout << model->x[model->L][i] << std::endl;
	}

	delete_model(model);*/

	double X[] = {
		1.0, 1.0,
		2.0, 2.0, 
		0.5, 0.2,
		0.5, 0.6,
		
		3.2, 0.1,
		4.25, 1.5,
		4.9, 1.9,
		3.55, 0.9,

		4.1, 4.1,
		6.0, 6.0,
		5.25, 4.5,
		4.5, 5.6
	};

	double Xk[] = {
		4.5, 1.5
	};
	
	double Y[] = {
		1.0, 0.0, 0.0,
		1.0, 0.0, 0.0,
		1.0, 0.0, 0.0,
		1.0, 0.0, 0.0,

		0.0, 1.0, 0.0,
		0.0, 1.0, 0.0,
		0.0, 1.0, 0.0,
		0.0, 1.0, 0.0,
		
		0.0, 0.0, 1.0,
		0.0, 0.0, 1.0,
		0.0, 0.0, 1.0,
		0.0, 0.0, 1.0,
	};

	int input_count = 2;
	int classCount = 3;
	double gamma = 1.0;
	
	auto model = create_rbf_model(classCount, gamma);
	train_rbf_model(model, X, input_count, 12, Y, classCount);
	auto p = predict_rbf(model, Xk, input_count);
	
	std::cout << p << " prediction" << std::endl;

	delete_rbf_model(model);
	
	return 0;
}
