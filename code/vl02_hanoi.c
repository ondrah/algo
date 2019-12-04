#include <stdio.h>

/* move 'count' disc from 'source' to 'destination' using 'helper' */
void move_them(int count, int source, int helper, int destination)
{
	if(count <= 1) {
		printf("moving %d => %d\n", source, destination);
		return;
	}

	move_them(count - 1, source, destination, helper);
	move_them(1, source, helper, destination);
	move_them(count - 1, helper, source, destination);
}

int main() {
	printf("Disc count = ");

	int n;
	scanf("%d", &n);

	/* move $n discs from rod 1 to rod 3, using rod 2 */
	move_them(n, 1, 2, 3);
}
