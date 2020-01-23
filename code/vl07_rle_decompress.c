/* A simple RLE decompressor that reads input data from standard input
   and prints them to standard output. */

#include <stdio.h>

int main()
{
	int c;
	int occurences;

	while((occurences = getchar()) != EOF) {
		/* Obviously, the length of file should be even. */
		if((c = getchar()) == EOF)
			return -1;	/* exit, file format broken (incomplete file) */

		do {
			putchar(c);
		} while(occurences--);
	}

	return 0;
}
