# NIM
Nim.exe is a C# command line version of the Nim game. Nim is a mathematical game of strategy in which two players take turns removing objects from distinct heaps.

Nim and its variants have been played since ancient times and its origins probably lie in China. It is known in Europe since the 16th century and got its name from Charles Leonard Bouton, who also developed and published the complete theory of the game in 1901.

Nim is one of the first computer games and played already by a machine, Nimatron, in New York in 1940[^1]. In 1951, the calculator Nimrod which was built in England defeated the German Economics Minister Ludwig Erhard.

In the Nim game, two players alternately take a number of elements from individual heaps. Elements can be matches in several rows[^2] or liquorice sticks in a number of pots [^3]. The player who takes the last element wins.

The practical strategy to win at the game of Nim is according to Bouton[^4] to convert the number of elements in each heap into its binary representation and then calculate the column sum for each digit. A game state in which all column sums are even is a winning position. If one or more sums are odd this is a losing position. The winning position of a player results in a losing position for the other player automatically on the next move. Therefore, the player who achieved the winning position first and retains up to the end of the game wins always. Whether a column sum is odd or even, can be determined via an XOR operation of element counts.

In the case of the C# command line version of Nim game heaps with their element counts are passed as numbers from the command line. Thus, “nim.exe 1 3 5 7” creates a Nim game with four heaps. By default, the user is the first player and starts the game with a move. A move consists of the heap and the number of elements which should be removed, separated by a colon. The heap is identified by its index starting at one. E.g. the move “3:2” means, that the user takes two elements from the third heap. After the user, the computer executes its move and so forth. This process repeats itself until the user or the computer has taken the last elment:

    C:\>Nim.exe 2 3 5 7
    YOUR MOVE? ([Heap]:[Count]) 1:1
    MY MOVE    ([Heap]:[Count]) 4:7
    YOUR MOVE? ([Heap]:[Count]) 3:3
    MY MOVE    ([Heap]:[Count]) 2:3
    YOUR MOVE? ([Heap]:[Count]) 3:1
    MY MOVE    ([Heap]:[Count]) 1:1
    YOUR MOVE? ([Heap]:[Count]) 3:1
    YOU WIN AFTER 00:00:06

Because the playground is not displayed, the user must keep track of all changes in memory. To win, the user has to determine the binary number of all elements and calculating XOR values or column totals of individual binary digits. Therefore, this command line version is an excellent memory training.

To help the user the verbose switch “-v” shows more and more information about the playground of the game, the binary numbers and the results of XOR operations, depending on how many “v”s are specified. With the switch “-c” the output is colored and can be distinguished more easily.

Various game strategies can be specified with the switches “-l” (use the largest number of elements), “-s” (use the smallest number of elements), “-u M” (take “M” elements at maximum) and “-m “ (misery game in which the one loses, who takes the last element) With the “-f “ switch is specified, that the computer plays the first move.

[^1]:[Nim game in Wikipedia](https://en.wikipedia.org/wiki/Nim) 
[^2]:[Online Nim game (german)](http://www.alraft.de/altenhein/spiele/nim-spiel/)
[^3]:Denken wie ein Computer Das NIM-Spiel und der Trick mit den binären Zahlen, Peter Schmitz, Heise ct c’t 2017, Heft 17, Seite 132-136
[^4]:[Nim game in Wikipedia (german)](https://de.wikipedia.org/wiki/Nim-Spiel)