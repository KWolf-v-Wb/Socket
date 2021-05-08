# Socket
Applicazione di "messaggistica" per provare ad utilizzare i socket per la comunicazione UDP.

Funzionalità:
  -Creazione Thread di ricezione automatico tramite l'identificazione dell'Ip dell'host e l'utilizzo di una porta statica.
  -Controllo sull'inserimento dell'Ip e della Porta per la creazione del socket.
  -Controllo messaggi per evitare che vengano inviati messaggi vuoti.
  -Cronologia dei socket contattati.
  -Possibilità di cancellare i messaggi ricevuti o la cronologia dei contatti tramite il clic destro del mouse sulla lista interessata.
  
Dati importanti:
  -Il progetto Visual Studio ha come porta di ricezione 56000.
  -L'eseguibile esterno ha, come unica differenza, il valore della porta di ricezione impostato a 56002.
