#include <cstdlib>
#include <ctime>
#include <vector>
//Build la dll en debug (et se mettre en debug)
//permet d'attacher la source.cpp à unity et de voir step by step
//ce qu'il se passe

#include <Eigen>
#include <iostream>

extern "C"
{
#pragma region Linear Model

	__declspec(dllexport) double* create_linear_model(int input_counts)
	{
		//On crée un tableau de input + 1 pour inclure le biais
		auto weights = new double[input_counts + 1];

		//Initialise le model avec des poids random selon le nombre d'input
		for (auto i = 0; i < input_counts + 1; ++i)
		{
			weights[i] = rand() / static_cast<double>(RAND_MAX) * 2.0 - 1.0;
		}

		return weights;
	}

	__declspec(dllexport) double* create_linear_model_regression(int input_counts)
	{
		//On crée un tableau de input + 1 pour inclure le biais
		auto weights = new double[input_counts + 1];

		//Initialise le model avec des poids random selon le nombre d'input
		for (auto i = 0; i < input_counts + 1; ++i)
		{
			weights[i] = rand() / static_cast<double>(RAND_MAX) * 2.0 - 1.0;
		}

		return weights;
	}
	

	__declspec(dllexport) double predict_linear_model(double* model, double inputs[], int input_count, bool is_classification)
	{
		double sum = 0.0;
		
		/*
		 * std::vector<double> x_k;
		x_k.reserve(input_count + 1);

		//Puis on remplit notre selection
		for (int i = 0; i < input_count + 1; ++i)
		{
			if (i == 0)
				x_k.emplace_back(1.0);
			else
				x_k.emplace_back(inputs[i - 1]);
		}
		
		for (int i = 0; i < input_count + 1; ++i)
		{
			sum += model[i] * x_k[i];

		}

		x_k.clear();
		 */

		
		for (int i = 0; i < input_count + 1; ++i)
		{
			sum += model[i] * inputs[i];

		}

		//Si on est en classif, on retourne Sign(sum)
		if (is_classification)
			return sum > 0.0 ? 1.0
			: sum < 0.0 ? -1.0
			: 0.0;
		else //sinon on retourne la sum
			return sum;
	}

	__declspec(dllexport) double* predict_linear_model_multiclass(double* model[], double inputs[], int input_count, int class_count, bool is_classification)
	{
		// todo
		double* result = new double[class_count];
		double sum = 0.0;
		
		for (int i = 0; i < class_count; ++i)
		{
			result[i] = predict_linear_model(model[i], inputs, input_count, is_classification);
		}
		//
		return result;
	}

	__declspec(dllexport) void train_linear_model_regression(double* model, double all_inputs[], int input_count,
		int sample_counts, double all_expected_outputs[], int expected_output_count)
	{
		//Pas besoins d'utiliser le model car on va directement calculer les poids optimaux
		//On crée les matrice des inputs et outputs
		Eigen::MatrixXd all_inputs_m = Eigen::MatrixXd(sample_counts, input_count + 1);
		Eigen::MatrixXd all_output_m = Eigen::MatrixXd(sample_counts, expected_output_count);

		Eigen::MatrixXd w = Eigen::MatrixXd(1, input_count + 1);
		
		//on remplit les matrices
		for(int i = 0; i < sample_counts; ++i)
		{
			for(int j = 0; j < input_count + 1; ++j)
			{
				if(j == 0)
					all_inputs_m(i, j) = 1;
				else
					all_inputs_m(i, j) = all_inputs[i * input_count + (j - 1)];
			}
		}
		std::cout << "INPUTS : \n" << all_inputs_m << std::endl;
		for (int i = 0; i < sample_counts; ++i)
		{
			for (int j = 0; j < expected_output_count; ++j)
			{
				//all_output_m(i, j) = all_expected_outputs[i * expected_output_count + j];
				all_output_m(i, j) = all_expected_outputs[i * expected_output_count + j];
			}
		}
		std::cout << "OUTPUTS : \n" << all_output_m << std::endl;

		Eigen::MatrixXd all_inputs_m_transposed = all_inputs_m;
		all_inputs_m_transposed.transposeInPlace();
		
		//on effectue le calcule matriciel
		//TODO : pour le cas du tricky la pseudo inverse merde
		w = (all_inputs_m_transposed * all_inputs_m).inverse() * all_inputs_m_transposed * all_output_m;

		std::cout << "weight = \n" << w << std::endl;
		
		//On assigne les data au model
		for(int i = 0; i < w.size(); ++i)
		{
			model[i] = w(i);
			std::cout << model[i] << " model i" << std::endl;
		}
	}

	/// <summary>
	/// Train linear model with Rosenblatt method. This is for classifier.
	/// </summary>
	/// <param name="model">The model to train</param>
	/// <param name="all_inputs">All inputs we want to train</param>
	/// <param name="input_count">The size of an input, ex : input = Vector2 so input count = 2 </param>
	/// <param name="sample_counts">The number of input/output</param>
	/// <param name="all_expected_outputs">All output we want to have</param>
	/// <param name="expected_output_count">the size of an output</param>
	/// <param name="epochs"></param>
	/// <param name="learning_rate"></param>
	__declspec(dllexport) void train_linear_model_rosenblatt(double* model, double all_inputs[], int input_count,
		int sample_counts, double all_expected_outputs[], int expected_output_count, int epochs, double learning_rate)
	{
		for (int it = 0; it < epochs; ++it)
		{
#pragma region Random Choice
			//On tire au sort entre 0 et sample count
			const int max = sample_counts;
			const int min = 0;
			const int k = rand() % (max - min) + min;

			//Ensuite on prend la lenght de ce tirage
			const int x_k_length = input_count * (k + 1) - (input_count * k);
			std::vector<double> x_k;
			x_k.reserve(x_k_length + 1);

			//Puis on remplit notre selection
			int start = input_count * k;
			int end = input_count * (k + 1);
			for (int i = start; i < end + 1; ++i)
			{
				if(i == start)
					x_k.emplace_back(1.0);
				else
					x_k.emplace_back(all_inputs[i - 1]);
			}

			//On fait de même pour les outputs
			const int y_k_length = expected_output_count * (k + 1) - (expected_output_count * k);
			std::vector<double> y_k;
			y_k.reserve(y_k_length);

			start = expected_output_count * k;
			end = expected_output_count * (k + 1);
			for (int i = start; i < end; ++i)
			{
				y_k.emplace_back(all_expected_outputs[i]);
			}
#pragma endregion

			//On réalise la prediction de l'echantillon en classification car nous somme en resenblatt
			const auto g_Xk = predict_linear_model(model, x_k.data(), input_count, true);

			//Enfin on met a jout les poids
			for (int i = 0; i < input_count + 1; ++i)
				for (int l = 0; l < expected_output_count; ++l)
					model[i] = model[i] + learning_rate * (y_k[l] - g_Xk) * x_k[i];


			x_k.clear();
			y_k.clear();
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


			// Initialisation de x_k et y_k
			// On récupère une slice des inputs et outputs en fonction de random
			// Correspond à ces lignes dans le code python:
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

			for (int l = model->L; l > 1; --l) // correspond à reversed(range(2, self.L + 1))
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

			x_k.clear();
			y_k.clear();
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
#pragma endregion
}
