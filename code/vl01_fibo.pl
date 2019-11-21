$a = 1; $b = 2;

print "$a, $b, ";

for($i = 0; $i < 10; $i++) {
	$c = $b;
	$b = $b + $a;
	$a = $c;

	print $b, ", ";
}
