#include <cstdlib>
#include <vector>
//Build la dll en debug (et se mettre en debug)
//permet d'attacher la source.cpp à unity et de voir step by step
//ce qu'il se passe

extern "C" {

struct test
{
	int a;
	int b;

	std::vector<double> w;
};

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
	
	//npl ==> [input: 3, hidden: 5,5,5, out: 1], doit etre donnée a chaque fois
	//layer_number = 5, doit etre donnée a chaque fois
	//L = layer number - 1, créer a chaque fois
	//deltas peut etre déclaré et construit dans la fonction train
	//x peut etre construit dans train et passé en paramettre à predict
	//w est toujours return a unity afin de pouvoir le re utiliser
	
	__declspec(dllexport) test* create_test(int a, int b)
	{
		auto w = new test;

		w->a = a;
		w->b = b;

		w->w.emplace_back(33);
	
		return w;
	}
	
	__declspec(dllexport) double* create_linear_model(int input_counts)
	{
		//On crée un tableau de input + 1 pour inclure le biais
		auto weights = new double[input_counts + 1];

		//Initialise le model avec des poids random selon le nombre d'input
		for(auto i = 0; i < input_counts + 1; ++i)
		{
			weights[i] = rand() / static_cast<double>(RAND_MAX) * 2.0 - 1.0;
		}

		return weights;
	}

	__declspec(dllexport) double* predict_linear_model_multiclass(double* model, double inputs[], int input_count, int class_count, bool is_classification)
	{
		// todo

		return new double[3] {1.0, 1.0, -1.0};
	}

	__declspec(dllexport) double predict_linear_model(double* model, double inputs[], int input_count, bool is_classification)
	{
		double result = 0.0;

		//model contient les poids
		int d[2];
		int L = 2;
		
		for(int l = 1; l < L + 1; ++l)
		{
			for(int j = 1; j < d[l] + 1; ++j)
			{
				double sum = 0.0;

				for(int i = 0; i < d[l - 1] + 1; ++i)
				{
					tanh(sum);
				}
			}
		}
		
		return result;
	}
	
	__declspec(dllexport) void train_linear_model_rosenblatt(double* model, double all_inputs[], int input_count, 
		int sample_couts, double all_expected_outputs[], int expected_output_count, int epochs, double learning_rate, bool is_classification)
	{
		// todo

		return;
	}
	
	__declspec(dllexport) void delete_linear_model (double* model)
	{
		delete[] model;
	}


	
	
	__declspec(dllexport) MLP* create_model(int npl[], int layer_counts)
	{
		auto mlp = new MLP;


		mlp->L = layer_counts - 1;


		mlp->d.reserve(mlp->L);
		for (int n = 0; n < layer_counts; ++n)
			mlp->d.emplace_back(npl[n]);


		mlp->w.reserve(layer_counts + 1);
		for (int l = 0; l < layer_counts; ++l)
		{

			std::vector< std::vector<double> > a;

			if (l != 0)
			{
				a.reserve(npl[l - 1] + 2);

				for (int i = 0; i < npl[l - 1] + 1; ++i)
				{
					std::vector<double> b;
					b.reserve(npl[l] + 2);

					for (int j = 0; j < npl[l] + 1; ++j)
					{
						b.emplace_back(rand() / static_cast<double>(RAND_MAX) * 2.0 - 1.0);
					}

					a.emplace_back(b);
					b.clear();
				}
			}
			else
			{
				std::vector<double> b;
				b.emplace_back(1.0);
				a.emplace_back(b);
				b.clear();
			}

			mlp->w.push_back(a);
			a.clear();
		}


		mlp->x.reserve(layer_counts + 1);
		for (int l = 0; l < layer_counts; ++l)
		{
			std::vector<double> a;
			a.reserve(npl[l] + 1 + 1);
			for (int j = 0; j < npl[l] + 1; ++j)
			{
				if (j > 0)
					a.emplace_back(0.0);
				else
					a.emplace_back(1.0);
			}
			mlp->x.emplace_back(a);
			a.clear();
		}


		mlp->deltas.reserve(layer_counts + 1);
		for (int l = 0; l < layer_counts; ++l)
		{
			std::vector<double> a;
			a.reserve(npl[l] + 1 + 1);
			for (int j = 0; j < npl[l] + 1; ++j)
			{
				if (j > 0)
					a.emplace_back(0.0);
				else
					a.emplace_back(1.0);
			}
			mlp->deltas.emplace_back(a);
			a.clear();
		}

		return mlp;
	}

	__declspec(dllexport) void forward_pass(MLP* model, double inputs[], bool isClassification) 
	{
		for(int j = 0; j < model->d[0]; ++j)
			model->x[0][j + 1] = inputs[j];

		for(int l = 1; l < model->L + 1; ++l)
		{
			for(int j = 1; j < model->d[l] + 1; ++j)
			{
				double sum = 0.0;
				
				for (int i = 0; i < model->d[l - 1] + 1; ++i) 
					sum += model->x[l - 1][i] * model->w[l][i][j];
				

				auto th = tanh(sum);
				if (l == model->L && !isClassification) 
					model->x[l][j] = sum;
				else 
					model->x[l][j] = tanh(sum);
			}
		}
	}

	__declspec(dllexport) void train(MLP* model, double allInputs[], double allExpectedOutputs[], 
		int sampleCount, int epochs, double alpha, bool isClassification)
	{
		const int inputsSize = model->d[0];
		const int outputsSize = model->d[model->L];

		for (int it = 0; it < epochs; ++it) 
		{
			int k = rand() % sampleCount - 1; //http://www.cplusplus.com/reference/cstdlib/rand/
			
			// Initialisation de x_k et y_k
			// On récupère une slice des inputs et outputs en fonction de random
			// Correspond à ces lignes dans le code python:
			// x_k = all_inputs[inputs_size * k: inputs_size * (k + 1)]
			// y_k = all_expected_outputs[outputs_size * k:outputs_size * (k + 1)]
			
			const int x_k_length = inputsSize * (k + 1) - (inputsSize * k);
			//double* x_k = new double[x_k_length]; // PAS SUR
			std::vector<double> x_k;
			x_k.reserve(x_k_length);
			for (int i = 0; i < x_k_length; ++i) 
			{
				x_k.emplace_back(allInputs[inputsSize * k + i]);
			}

			const int y_k_length = outputsSize * (k + 1) - (outputsSize * k);
			//double* y_k = new double[y_k_length]; // PAS SUR
			std::vector<double> y_k;
			y_k.reserve(y_k_length);
			for (int i = 0; i < y_k_length; i++)
			{
				y_k.emplace_back(allExpectedOutputs[outputsSize * k + i]);
			}

			forward_pass(model, x_k.data(), isClassification);

			for(int j = 1; j < model->d[model->L] + 1; ++j)
			{
				model->deltas[model->L][j] = model->x[model->L][j] - y_k[j - 1];
				if (isClassification) 
					model->deltas[model->L][j] *= 1 - model->x[model->L][j] * model->x[model->L][j];//a * a plus performant que pow(a, 2)
			}

			for (int l = model->L; l > 1; l--) // correspond à reversed(range(2, self.L + 1))
			{
				for (int i = 0; i < model->d[l - 1] + 1; ++i) 
				{
					double sum = 0.0;
					
					for (int j = 1; j < model->d[l] + 1; ++j) 
						sum += model->w[l][i][j] * model->deltas[l][j];
					
					model->deltas[l - 1][i] = (1.0 - model->x[l - 1][i] * model->x[l - 1][i]) * sum;
				}
			}

			for(int l = 1; l < model->L + 1; ++l)
				for (int i = 0; i < model->d[l - 1] + 1; ++i) 
					for (int j = 1; j < model->d[l] + 1; ++j) 
						model->w[l][i][j] -= alpha * model->x[l - 1][i] * model->deltas[l][j];

			//delete[] x_k;
			//delete[] y_k;
		}
	}

	__declspec(dllexport) void delete_model(MLP* model)
	{
		model->d.clear();
		model->deltas.clear();
		model->w.clear();
		model->x.clear();

		delete model;
	}
	


	//__declspec(dllexport) Permet de spécifier, pour windows, que cette fonction va en dll
	//l'interface d'une fonction doit respecter les code C
	//mais son contenu peut faire appel à du code C++ / C / Other
	__declspec(dllexport) double my_add(double a, double b)
	{
		return a + b;
	}

	__declspec(dllexport) double my_add_ptr(MLP* t)
	{
		return t->L + 0;
	}
}
