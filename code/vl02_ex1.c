/*
This example shows rather complicated conversion of a Basic function.  Some
constructs are simply not supported in C and a suitable workaround must be
found.

A deeper reconsideration and redesign would be appreciated.
That could probably prevent the use of the goto instruction.

But as a quick solution, this would probably work.
*/

/*
' Original code (Basic):

While row < lastrow
    row = row + 1   ' moved to top to prevent deadlock ;)

    For i As Integer = 0 To lastcol
        If i = 5 Then
            Continue While
        End If
    Next i

End While
*/

void a() {
    int row = 0;
    int lastrow = 10;
    int lastcol = 10;

CONT_WHILE: /* a label */

    while(row < lastrow) {
        row++;

        for( int i = 0 ; i <= lastcol; i++ ) {
            if(i == 5) {
                /* C just cannot continue while from here,
                   this is ugly but working */
                goto CONT_WHILE;
            }
        }
    }
}
