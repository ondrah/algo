#!/usr/bin/perl

# A C/Java function prototype would probably look like this
#
# void move_them(int count, int source, int helper, int dest)
#
sub move_them {
	my($count, $source, $helper, $destination) = @_;

	if($count <= 1) {
		print "moving $source => $destination\n";
		return;
	}

	# More than 1 disc remaining...
	# 1. Move all smaller discs on top of it to helper place
	move_them($count - 1, $source, $destination, $helper);
	# 2. The disc is now free, move it to destination
	move_them(1, $source, $helper, $destination);
	# 3. Move all smaller discs from helper place to destination
	move_them($count - 1, $helper, $source, $destination);
}


print "Disc count = ";

# Read n from standard input, assume integer value
$n = <>;

# Move $n discs from place 1 (source) to place 3 (destination),
# using place 2 (helper)
move_them($n, 1, 2, 3);

