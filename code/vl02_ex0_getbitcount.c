/*
   Get number of bits that are 1 in n.
   uint8_t = unsigned integer, length 8 bit.
*/
int getbitcount(uint8_t n)
{
	int nr_bits = 0;

	while(n > 0)
	{
		if((n & 1) == 1) {
			// LSB is one!
			nr_bits++;
		}

		n = n >> 1;	// shift 1 bit right, n = n / 2
	}

	return nr_bits;
}

