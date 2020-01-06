#!/usr/bin/perl
#
# Consider a string containing multiple space separated expressions. An
# expression is either a single word or a compound expression consisting of
# multiple words between double quotes.
#
# The (\w+|"[^"]+") regex is used here to match separate
# single or compound expressions.
#

# Example input
$input = q{apple orange banana "honey pie" sun "high noon"};

# Get an array of all expressions
@all_exp = $input =~ /(\w+|"[^"]+")/g;

# Get an array of compound expression, each begins with single "
@compound_exp = grep(/^"/, @all_exp);

print "all expressions: ", scalar @all_exp, "\n";
print "compound expressions: ", scalar @compound_exp, "\n";
