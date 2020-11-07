#include <cstdlib>
#include <ctime>
#include <vector>
//Build la dll en debug (et se mettre en debug)
//permet d'attacher la source.cpp � unity et de voir step by step
//ce qu'il se passe

extern "C"
{
#pragma region Linear Model

	__declspec(dllexport) double* create_linear_model(int input_counts)
	{
		//On cr�e un tableau de input + 1 pour inclure le biais
		auto weights = new double[input_counts + 1];

		//Initialise le model avec des poids random selon le nombre d'input
		for (auto i = 0; i < input_counts + 1; ++i)
		{
			weights[i] = rand() / static_cast<double>(RAND_MAX) * 2.0 - 1.0;
		}

		return weights;
	}

	__declspec(dllexport) double* predict_linear_model_multiclass(double* model, double inputs[], int input_count, int class_count, bool is_classification)
	{
		// todo

		return new double[3]{ 1.0, 1.0, -1.0 };
	}

	__declspec(dllexport) double predict_linear_model(double* model, double inputs[], int input_count, int sample_count, bool is_classification)
	{
		double sum = 0.0;

		for(int i = 0; i < input_count; ++i)
		{
			//Somme pond�r� des poinds * inputs
			for(int n = 0; n < sample_count; ++n)
			{
				sum += model[i] * inputs[n];
			}
		}

		if (is_classification)
			return sum > 0.0 ? 1.0
							: sum < 0.0 ? -1.0
							: 0.0;
		else
			return sum;
	}


	__declspec(dllexport) void train_linear_model_regression(double* model, double all_inputs[], int input_count,
		int sample_counts, double all_expected_outputs[], int expected_output_count)
	{

	}
	
	__declspec(dllexport) void train_linear_model_rosenblatt(double* model, double all_inputs[], int input_count,
		int sample_counts, double all_expected_outputs[], int expected_output_count, int epochs, double learning_rate)
	{
		//input count = nombre d'input/output
		//
		//sample count = taille d'un input
		//
		//expected_output_count = taille d'un output
		
		for(int it = 0; it < epochs; ++it)
		{
			const int max = input_count;
			const int min = 0;
			const int k = rand() % (max - min) + min;

			const int x_k_length = sample_counts * (k + 1) - (sample_counts * k);
			std::vector<double> x_k;
			x_k.reserve(x_k_length);

			int start = sample_counts * k;
			int end = sample_counts * (k + 1);
			for (int i = start; i < end; ++i)
			{
				x_k.emplace_back(all_inputs[i]);
			}

			const int y_k_length = expected_output_count * (k + 1) - (expected_output_count * k);
			std::vector<double> y_k;
			y_k.reserve(y_k_length);

			start = expected_output_count * k;
			end = expected_output_count * (k + 1);
			for (int i = start; i < end; ++i)
			{
				y_k.emplace_back(all_expected_outputs[i]);
			}

			auto g_Xk = predict_linear_model(model, x_k.data(), input_count, sample_counts, true);

			for(int i = 0; i < input_count; ++i)
				for(int n = 0; n < sample_counts; ++n)
					for(int l = 0; l < expected_output_count; ++l)
						model[i] = model[i] + learning_rate * (y_k[l] - g_Xk) * x_k[n];
				
		}
	}

	
	__declspec(dllexport) void delete_linear_model(double* model)
	{
		delete[] model;
	}
#pragma endregion 

#pragma region MLP
	struct MLP
	{
		/// <summary>
		/// correspond � npl
		/// </summary>
		std::vector<int> d;

		/// <summary>
		/// correspond � npl.size() - 1
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

	__declspec(dllexport) MLP* create_model(int npl[], int layer_counts)
	{
		auto mlp = new MLP;


		mlp->L = layer_counts - 1;


		mlp->d.reserve(layer_counts);
		for (int n = 0; n < layer_counts; ++n)
			mlp->d.emplace_back(npl[n]);


		mlp->w.reserve(layer_counts + 1);
		for (int l = 0; l < layer_counts; ++l)
		{

			std::vector< std::vector<double> > a;

			if (l == 0)
			{
				std::vector<double> b;
				b.emplace_back(1.0);
				a.emplace_back(b);
				b.clear();
			}
			else
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
		for (int j = 0; j < model->d[0]; ++j)
			model->x[0][j + 1] = inputs[j];

		for (int l = 1; l < model->L + 1; ++l)
		{
			for (int j = 1; j < model->d[l] + 1; ++j)
			{
				double sum = 0.0;

				for (int i = 0; i < model->d[l - 1] + 1; ++i)
					sum += model->x[l - 1][i] * model->w[l][i][j];


				auto th = tanh(sum);
				if (l == model->L && !isClassification)
					model->x[l][j] = sum;
				else
					model->x[l][j] = th;
			}
		}
	}

	__declspec(dllexport) double* predict(MLP* model, double inputs[], bool isClass)
	{
		forward_pass(model, inputs, isClass);

		double* d = new double[model->x[model->L].size()];

		for (int i = 0; i < model->x[model->L].size(); ++i)
			d[i] = model->x[model->L][i];

		return d;
	}

	__declspec(dllexport) void train(MLP* model, double allInputs[], double allExpectedOutputs[],
		int sampleCount, int epochs, double alpha, bool isClassification)
	{
		const int inputsSize = model->d[0];
		const int outputsSize = model->d[model->L];

		for (int it = 0; it < epochs; ++it)
		{
			//srand(time(NULL));
			const int max = sampleCount;
			const int min = 0;
			const int k = rand() % (max - min) + min; //rand() % sampleCount - 1; //http://www.cplusplus.com/reference/cstdlib/rand/

			//for (int k = 0; k < sampleCount; ++k)
			{
				// Initialisation de x_k et y_k
				// On r�cup�re une slice des inputs et outputs en fonction de random
				// Correspond � ces lignes dans le code python:
				// x_k = all_inputs[inputs_size * k: inputs_size * (k + 1)]
				// y_k = all_expected_outputs[outputs_size * k:outputs_size * (k + 1)]

				const int x_k_length = inputsSize * (k + 1) - (inputsSize * k);
				std::vector<double> x_k;
				x_k.reserve(x_k_length);

				int start = inputsSize * k;
				int end = inputsSize * (k + 1);
				for (int i = start; i < end; ++i)
				{
					x_k.emplace_back(allInputs[i]);
				}

				const int y_k_length = outputsSize * (k + 1) - (outputsSize * k);
				std::vector<double> y_k;
				y_k.reserve(y_k_length);

				start = outputsSize * k;
				end = outputsSize * (k + 1);
				for (int i = start; i < end; ++i)
				{
					y_k.emplace_back(allExpectedOutputs[i]);
				}

				forward_pass(model, x_k.data(), isClassification);

				for (int j = 1; j < model->d[model->L] + 1; ++j)
				{
					model->deltas[model->L][j] = model->x[model->L][j] - y_k[j - 1];
					if (isClassification)
						model->deltas[model->L][j] *= 1 - pow(model->x[model->L][j], 2);//a * a plus performant que pow(a, 2)
				}

				for (int l = model->L; l > 1; --l) // correspond � reversed(range(2, self.L + 1))
				{
					for (int i = 0; i < model->d[l - 1] + 1; ++i)
					{
						double sum = 0.0;

						for (int j = 1; j < model->d[l] + 1; ++j)
							sum += model->w[l][i][j] * model->deltas[l][j];

						//(1 - self.x[l - 1][i] ** 2) * sum
						model->deltas[l - 1][i] = (1.0 - pow(model->x[l - 1][i], 2)) * sum;
					}
				}

				for (int l = 1; l < model->L + 1; ++l)
					for (int i = 0; i < model->d[l - 1] + 1; ++i)
						for (int j = 1; j < model->d[l] + 1; ++j)
							model->w[l][i][j] -= alpha * model->x[l - 1][i] * model->deltas[l][j];
			}



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
#pragma endregion 

	__declspec(dllexport) void delete_double_array_ptr(double* ptr)
	{
		delete[] ptr;
	}

#pragma region Fonctions de test
	//__declspec(dllexport) Permet de sp�cifier, pour windows, que cette fonction va en dll
	//l'interface d'une fonction doit respecter les code C
	//mais son contenu peut faire appel � du code C++ / C / Other
	__declspec(dllexport) double my_add(double a, double b)
	{
		return a + b;
	}

	__declspec(dllexport) double my_add_ptr(MLP* t)
	{
		return t->L + 0;
	}
#pragma endregion
}
