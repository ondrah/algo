#!/usr/bin/perl

$/ = undef;
$input = <>;

# Read the input file
@rows = split("\n", $input);

$nr_cols = -1;
my $nr_rows = 0;
my @maze;

# Check for consistency, is it a rectangular structure?
foreach my $rowdata (@rows) {
	if($nr_cols == -1) {
		$nr_cols = length $rowdata;
	} elsif($nr_cols != length $rowdata) {
		die "inconsistent structure! nr_rows = $nr_rows\n";
	}

	for($i = 0; $i < $nr_cols; $i++) {
		$maze[$i][$nr_rows] = substr $rowdata, $i, 1;
	}

	$nr_rows++;

}

print "maze loaded! cols = $nr_cols  rows = $nr_rows\n";

# Find all openings (2 expected)
# Look on vertical sides
for($y = 0; $y < $nr_rows; $y++) {
	if($maze[0][$y] eq ' ') {
		push @openings,[0,$y];
	}
	if($maze[$nr_cols - 1][$y] eq ' ') {
		push @openings,[$nr_cols - 1,$y];
	}
}

# Look on horizontal sides
for($x = 0; $x < $nr_cols; $x++) {
	if($maze[$x][0] eq ' ') {
		push @openings,[$x,0];
	}
	if($maze[$x][$nr_rows - 1] eq ' ') {
		push @openings,[$x,$nr_rows - 1];
	}
}

# Cannot continue, no openings. Terminate the program.
die "oops, no openings found!" if(!@openings);

$end_x = $openings[1][0];
$end_y = $openings[1][1];

# a Java prototype would look like this:
# void try_walk(int x, int y) {
sub try_walk {
	my($x,$y) = @_;

	# Mark path speculatively
	print "walking on [$x, $y]\n";
	$maze[$x][$y] = '?';

	# Look if end reached
	if($x == $end_x && $y == $end_y) {
		$maze[$x][$y] = '.';
		print "end reached!\n";
		return 1;
	}

	# All possible directions
	@directions = ([1,0],[-1,0],[0,1],[0,-1]);
	my $ret = 0;

	# Go systematically through all directions
	foreach my $d (@directions) {
		$new_x = $x + $d->[0];
		$new_y = $y + $d->[1];

		# Check if not out of boundaries and if the field is a free path (' ')
		if($new_x < $nr_cols
			&& $new_x >= 0
			&& $new_y < $nr_rows
			&& $new_y >= 0
			&& $maze[$new_x][$new_y] eq ' ') {
				# if so, try walking there
				$ret += try_walk($new_x, $new_y);
		}
	}

	if(!$ret) {
		# Mark fields that had no success.
		$maze[$x][$y] = '_';
	} else {
		# Mark fields that lead to finish.
		$maze[$x][$y] = '*';
	}
	return $ret;
}

$result = try_walk(@{$openings[0]});
# Print the result (1 if there is a connection between start and end)
print "result = $result\n";

# Print out the maze with one solution
for($y = 0 ; $y < $nr_rows; $y++) {
	for($x = 0; $x < $nr_cols ; $x++) {
		print $maze[$x][$y];
	}

	print "\n";
}
