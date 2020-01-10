#!/usr/bin/perl

# A silly maze generator

use strict;
use warnings;

use List::Util;

# Maze size, command line argument, default 9
my $a = shift // 9;

# Maze map stored as a two-dimensional array $maze[$x][$y]
my @maze;

# x-size / width / columns
my $sizex = $a;
# y-size / height / rows
my $sizey = $a;

# Initialize the maze, wall everywhere
for ( my $y = 0 ; $y < $sizey ; $y++ ) {
    for ( my $x = 0 ; $x < $sizex ; $x++ ) {
        $maze[$x][$y] = '#';
    }
}

# Make fixed openings
$maze[1][0] = ' ';

# Create a maze beginning from the first opening
walk( 1, 1 );

# Find a suitable place for the second opening
for ( my $i = $sizey - 1 ; $i > 0 ; $i-- ) {
    if ( $maze[ $sizex - 2 ][$i] eq ' ' ) {
        last;
    }
    $maze[ $sizex - 2 ][$i] = ' ';
    if ( $maze[ $sizex - 3 ][$i] eq ' ' ) {
        last;
    }
}

# C transcription:
# void walk(int x, int y)
sub walk {
    my ( $x, $y ) = @_;

    # Went to [x,y] so make a path here
    $maze[$x][$y] = ' ';

    # Get a randomized list of possible directions: west, south, east, north
    my @rand_directions =
      List::Util::shuffle( [ -2, 0 ], [ 0, 2 ], [ 2, 0 ], [ 0, -2 ] );

    # Try to walk in each direction, of course only if that is possible!
    foreach my $direction (@rand_directions) {

        # Compute new coordinates
        my $new_x = $x + $direction->[0];
        my $new_y = $y + $direction->[1];

        # Look if this direction is possible, there must be a wall
        if (   $new_x > 0
            && $new_y > 0
            && $new_x < $sizex - 1
            && $new_y < $sizey - 1
            && $maze[$new_x][$new_y] eq '#' )
        {
            $maze[ ( $x + $new_x ) / 2 ][ ( $y + $new_y ) / 2 ] = ' ';
            walk( $new_x, $new_y );
        }
    }
}

# Print out the solution
for ( my $y = 0 ; $y < $sizey ; $y++ ) {
    for ( my $x = 0 ; $x < $sizex ; $x++ ) {
        print $maze[$x][$y];
    }
    print "\n";
}
