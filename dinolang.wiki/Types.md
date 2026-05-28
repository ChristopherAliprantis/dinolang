
**num**

a number like 7.89 or 5(turned to a C# 'decimal')

example of declaration: a = 8;

example if turned to string: 8.98

function to get the absolute of a num: abs(num);<br>
example:<br>
in: abs(-88898.98);<br>
out: 88898.98<br>

function to see if the number is negative: isneg(num);<br>
example:<br>
in: isneg(negnum);<br>
out: true<br>

function to see if the number is neutral: isneut(num);<br>
example:<br>
in: isneut(7);<br>
out: false<br>

function to see if number is positive: ispos(num);<br>
example:<br>
in: ispos(negnum)<br>
out: false<br>

**string**

a string like :hello: or :Hello, World!:(turned to a C# 'string')

example of declaration: a = :🧇:;

symbol to represent newline: NL<br>
example:<br>
in: print(+(:I am:, NL, NL, :the best.:));<br>
out(
I am

the best.
)

function to get a character from: charat(string, 0-indexed index);<br>
example:<br>
in: charat(hello, 3);<br>
out: l<br>

function to get string length: Slen(string);<br>
example:<br>
in: Slen(:hello:);<br>
out: 5<br>

function to find if it starts with something: StartsWith((string), (string));<br>
example;<br>
in : StartsWith(:6778:, :6:);<br>
out: true<br>

function to find if it ends with something: EndsWith((string), (string));<br>
example;<br>
in : EndsWith(:6778:, :6:);<br>
out: false<br>


**bool**

a boolean like true or false(turned to C# 'bool')

example of declaration: a = true;

example of being reversed a = !(true);

example if turned to string: TRUE

**null**

it's just null(turned to C# 'null')

example of declaration: a = null;

example if turned to string: NULL

**Tips**

SYMBOLS:

A variable can have two words as a name but be careful because then variable 6num would be mixed with variable 6 num by the interpreter.

use COLON symbol for ":" character because the Dino string would be ::: then and the interpreter wouldn't be able to parse it

use COMMA symbol for commas if you put a comma as an input and Dino thinks you put more parameters then required

use BLANK symbol for empty strings, it's more readable than str = ::; 

use SC symbol for ";"

SHARED FUNCTIONS:

**ToString(type)**

example: ToString(null);<br>
out: NULL<br>

**ToNum(type)**

example: ToNum(false);<br>
out: 0<br>



