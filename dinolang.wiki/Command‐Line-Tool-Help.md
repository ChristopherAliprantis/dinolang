**--help**

gives a menu of command-line flags.

example: & "C:path\to\dino" --help

**-h**

gives link to the wiki

example: & "C:path\to\dino" -h true

**-c or --code**

a string of Dino code to run if you don't have a file

example: & "C:path\to\dino" -c "print(:Hello, World!:)"

**-f or --file**

the file to run

example: & "C:path\to\dino" -f "file.dno"
example: & "C:path\to\dino" -f "math.dno, file.dno"

TIP: you can make a file called files.txt, put your files in there separated by a comma(with no space after) and you can do -f "$(Get-Content files.txt -Raw)"