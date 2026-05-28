**Declaration**

#loop ((amount of times you want it to go(this is 0-indexed and stored as a long))); or #loop ((boolean value(goes until the value is false)));

**Ending**

#endloop;

**NOTES**

a loop cannot go in a loop you can fix this by making a function and putting the loop in it then calling the function from the loop<br>
loops can go in functions<br>
loops can go in ifs<br>
loops can go outside of functions<br>
you manually have to increment and make a variable to track the index of the loop<br>

**Loop-Specific commands(you can now use them in ifs as long if the if is in a loop)**

continue;, goes to the next iteration

break;, stops the loop

**Example**

NOTE: add a tab or 4 spaces in the code of the loop, the example doesn't have this because of github's markdown renderer

CODE(this is a fibonacci program using loops):

count = 10;<br>
now = 0;<br>
last = 1;<br>
temp = 0;<br>
#loop (count);<br>
    printnnl(now);<br>
    print(:,:);<br>
    temp = now;<br>
    now = +(now, last);<br>
    last = temp;<br>
#endloop;<br>

OUTPUT:

0,<br>
1.0,<br>
1.0,<br>
2.0,<br>
3.0,<br>
5.0,<br>
8.0,<br>
13.0,<br>
21.0,<br>
34.0,<br>