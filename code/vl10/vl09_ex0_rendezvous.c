#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>

bool rendezvous(int timea, int timeb)
{
	return abs(timea - timeb) < 20 * 60;
}

void perform_estimation(int nr_iterations)
{
	int nr_true = 0;

	int j = nr_iterations;
	while(j--)
	{
		if(rendezvous(random() * 60 * 60 / RAND_MAX, random() * 60 * 60 / RAND_MAX))
		{
			nr_true++;
		}
	}

	printf("%d %.4f\n", nr_iterations, (double)nr_true / nr_iterations);
}

int main()
{
	for(int i = 1 ; i < 32; i++) {
		perform_estimation(1 << i);
	}
}
