
**Print Functions:**

"print(string);", a method that prints text to terminal, then adds a newline to text 
"printnnl(string);", "print(string;)" but it doesn't add a newline to the text

**Printing variables**

example: print(a);

**Printing Nums**

example: print(7.2);

**Printing Strings**

example: print(:Hello, World!:);

NOTE: \n or other newline ways do not work but print(); does do a newline<br>
NOTE: when printing they turn values to strings, check the Types section to see how types are when turned to string

**External calls:**

**PowershellCall(string);**

example: PowershellCall(:echo h:);

NOTE: it calls Windows Powershell, not the new cross platform one, also the errors get printed last

**Program things:**

**Exit(num)**

exits program with the parameter as exit code

**Derr(string)**

prints your parameter<br>
and does Exit(1);<br>
