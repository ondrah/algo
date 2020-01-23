/* A simple RLE compressor that reads input data from standard input
   and prints them to standard output. */

#include <stdio.h>

int main()
{
	/* current character */
	int c;
	/* previous character */
	int previous = -1;
	/* sequential occurences */
	int occurences = 0;

	/* get one byte from the standard input */
	while((c = getchar()) != EOF) {
		if(c == previous) {
			/* if there are too many occurences and
			   their count would not fit in a single byte */
			if(occurences == 255) {
				putchar(255);
				putchar(c);
				occurences = 0;
			} else {
				occurences++;
			}
			continue;
		}
		
		if(previous != -1) {	/* not the first input character */
			putchar(occurences);
			putchar(previous);
			occurences = 0;
		}

		previous = c;
	}

	/* do not forget to print out the last character */
	putchar(occurences);
	putchar(previous);

	return 0;
}
