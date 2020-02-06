#!/usr/bin/perl

# Estimate the value of Pi using a randomized algorithm.

$n     = 1000000;
$count = 0;
for ( $i = 0 ; $i < $n ; $i++ ) {
    $x = rand(2) - 1;
    $y = rand(2) - 1;
    if ( $x * $x + $y * $y < 1 ) {
        $count++;
    }
}

printf( "pi = %.8f\n", 4 * $count / $n );
