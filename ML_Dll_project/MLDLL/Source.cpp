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
	std::vector<int> d; //correspond à npl
	int L; // correspond à npl.size() - 1

	std::vector< //[]
		std::vector< //[][]
		std::vector<double> > > w; //[][][] //model

	std::vector< std::vector<double> > x; //valeur effective des neuronnes

	std::vector< std::vector<double> > deltas; //marche d'erreur
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

	__declspec(dllexport) MLP* create_model(int npl[], int layer_counts)
	{
		auto mlp = new MLP;

		mlp->L = layer_counts - 1;

		mlp->d.reserve(mlp->L);
		for (int n = 0; n < layer_counts; ++n)
			mlp->d.emplace_back(npl[n]);
		
		mlp->w.reserve(mlp->L + 1);
		for (int l = 0; l < mlp->L - 1; ++l)
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
				}
			}
			else
			{
				std::vector<double> b;
				b.emplace_back(1.0);
				a.emplace_back(b);
			}

			mlp->w.push_back(a);
		}


		
		return mlp;
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
