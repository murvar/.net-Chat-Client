
TDDD49 LAB01 förberedelser och tankar.
Följande bör uppfyllas:
MVVM modell
- view skall hantera grässnitt
- viewModel skall hantera data mellan grässnitt och
model
- model skall hantera data mellan clienter samt skicka
tillbaka relevant data till viewModel.

För att hämta information från textfält skall
data-binding användas.
För att exekvera kod vid knapptryck skall ICommand
användas.
För att arbeta mellan nätverk skall System.Net.Sockets
användas.
För att utföra flera arbetsuppgifter samtidigt kommer
multithreading användas. Detta gör bland annat att vi
kan lyssna på anslutningar samtidigt som annat
försegår.(System.Threading.Thread). Threadpools. 
Det finns timer-funktionlitet som kan användas för att
utföra loopade anrop. Kan användas för att lyssna på 
anslutningar.

Att göra: 
Implementera data-binding för textfältet
Implementera ICommand för knappen
Skapa logik via knappen som försöker etablera en
anslutning. Detta bör göras med System.Net.Sockets.
Skapa logik som lyssnar på anslutningar. Detta bör
köra via en thread. 


