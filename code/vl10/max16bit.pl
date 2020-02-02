#!/usr/bin/perl

use Data::Dumper;

my @generation;

for ( $i = 0 ; $i < 4 ; $i++ ) {
    push @generation, { code => int( rand 65536 ) };
}

sub fitness {
    $param = shift;

    $result = 0;

    while ( $param > 0 ) {
        if ( $param & 1 ) {
            $result++;
        }

        $param >>= 1;
    }

    return $result;
}

sub recombine {
    my ( $a, $b ) = @_;

    $ac = $a->{code};
    $bc = $b->{code};

    return ( $ac & 0xF0F0 ) | ( $bc & 0x0F0F );
}

sub mutate {
    my $a = shift;

    $code = $a->{code};

    $code ^= 1 << ( int( rand(16) ) );

    return $code;
}

$iteration = 0;

for ( ; ; ) {
    $max = 0;
    foreach $n (@generation) {
        if ( $n->{fitness} > $max ) {
            $max = $n->{fitness};
        }
        $n->{fitness} = fitness( $n->{code} );
    }

    print "iteration = $iteration\n";

    #	print Dumper \@generation;

    if ( $max >= 16 ) {
        printf("all done, maximum found\n");
        exit;
    }

    @generation = sort { $b->{fitness} <=> $a->{fitness} } @generation;
    @generation = splice @generation, 0, 2;

    push @generation, { code => recombine( $generation[0], $generation[1] ) };
    push @generation, { code => mutate( $generation[0] ) };

    print Dumper \@generation;
    $iteration++;
}
