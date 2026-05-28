**GetN()**

returns a num with the amount of microseconds<br>
that have passed since 12:00 AM January, 1st, 1(AKA the DinoTime epoch)<br>

**GetT(num, num, num)**

parameter1: seconds

parameter2: minutes

parameter3: hours

takes those parameters converts them to microseconds and adds them up

**GetD(num, num, num)**

parameter 1: days

parameter 2: months(1 = 31 days)

parameter 3: years(1 = 365 days)

takes those parameters converts them to microseconds and adds them up

**GetDT(num, num, num, num, num, num);**

the parameters of GetT(num, num, num); and GetD(num, num, num); combined<br>
and it takes those parameters converts them to microseconds and adds them up<br>

**addminute(num);**

multiplies the parameter by the amount of microseconds in a minute<br>
and returns that value

NOTES:

Time functions are just an amount of microseconds that return a num(which is a 128 bit, base 10, floating point number)

you can add the functions because each returns a num, so it add the microseconds

