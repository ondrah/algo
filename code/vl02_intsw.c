#include <stdio.h>

int a = 100;
int b = 160;

void main()
{
	printf("a = %d ; b = %d\n", a, b);

	a = a ^ b;
	b = a ^ b;
	a = a ^ b;

	printf("a = %d ; b = %d\n", a, b);
}
