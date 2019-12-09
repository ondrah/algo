#!/usr/bin/perl
# A short Perl solution using regular expression.
# First, every group (0|1) is matched. See the /g modifier, match globally.
# For each match a code is executed, see the /e modifier, execute code.
# Is the group match equal to '0', '1' is used as a replacement,
# '10' otherwise.

$count = 11;
$e = '0';
while($count-- > 0) {
	print length $e, " ", $e, "\n";
	$e =~ s/(0|1)/$1 eq '0' ? '1' : '10'/ge;
}
