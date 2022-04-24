using System;
using System.Collections.Generic;

using WebSocketSharp.Server;
using WebSocketSharp;

using Server.Models;

using Newtonsoft.Json;

namespace Server
{
    public class Game : WebSocketBehavior
    {
        //Lista di client connessi
        private static List<WebSocket> _clientSockets = new List<WebSocket>();
        //numero di client connessi
        private static int count;

        //le socket dei 2 Player
        private static WebSocket Player1;
        private static WebSocket Player2;

        private static bool P1ready;
        private static bool P2ready;

        //le info dei 2 pokemon
        private static Pokemon jsonP1 = new Pokemon();
        private static Pokemon jsonP2 = new Pokemon();

        private static int Turno;
        private static Random rnd = new Random();

        private static bool statoPartita;

        /// <summary>
        /// GESTIRE System.IO.IOException per lo scollegamento di un client o crash?
        /// </summary>

        protected override void OnOpen()
        {
            Comunicazione comunicazione = new Comunicazione(); //info comunicazione

            WebSocket clientN = Context.WebSocket;
            count = _clientSockets.Count;
            Console.WriteLine("Richiesta connessione da client: " + (count + 1).ToString());
            //accetto solo due client
            if (count > 1)
            {
                Console.WriteLine("Chiusa connessione con client: " + (count + 1).ToString());
                comunicazione.method = "InfoConnessione";
                comunicazione.info = "Partita al completo 2/2 , riprova più tardi";
                clientN.Send(JsonConvert.SerializeObject(comunicazione));
                clientN.Close();
            }
            else//aggiungo il client e gli passo le informazioni di connessione
            {
                _clientSockets.Add(clientN);

                if(count==0)
                    Player1 = _clientSockets[0];
                else
                    Player2 = _clientSockets[1];

                comunicazione.clientID = count+1;

                if (count == 0)
                {
                    comunicazione.info = "In attesa del 2° Giocatore (1/2)";
                    comunicazione.readyEnabled = false;

                } 
                    
                if (count == 1)
                {
                    comunicazione.info = "Giocatori Collegati (2/2)\nScegliere il proprio pokemon\n";
                    comunicazione.readyEnabled = true;
                    comunicazione.chatEnabled = true;

                }    
                    
                comunicazione.method = "InfoConnessione";

                string JSONoutput = JsonConvert.SerializeObject(comunicazione);
                Sessions.Broadcast(JSONoutput);
            }    
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            AnalizzaJson(e);
        }

        private void ResetPartita()
        {
            statoPartita = false;
            P1ready = false;
            P2ready = false;

            Comunicazione comunicazione = new Comunicazione(); //info comunicazione

            comunicazione.method = "InfoConnessione";
            comunicazione.readyEnabled = true;

            string JSONoutput = JsonConvert.SerializeObject(comunicazione);
            Sessions.Broadcast(JSONoutput);
        }

        private void AnalizzaJson(MessageEventArgs e)
        {
            Comunicazione comunicazione = new Comunicazione(); //info comunicazione
            comunicazione = JsonConvert.DeserializeObject<Comunicazione>(e.Data);

            //scambio il json con le info dei pokemon tra i 2 giocatori
            //e salvo nel server i json

            Console.WriteLine($"client {comunicazione.clientID}: " + e.Data);

            if (comunicazione.method == "SceltaPokemon")
            {
                if(statoPartita==true)// vuol dire che sonon nella partita numero 2
                {
                    ResetPartita();
                }

                if (Context.WebSocket == Player1)
                {
                    _clientSockets[1].Send(e.Data);
                    jsonP1 = comunicazione.mypokemon;
                    P1ready = true;
                }
                else
                {
                    _clientSockets[0].Send(e.Data);
                    jsonP2 = comunicazione.mypokemon;
                    P2ready = true;
                }

                if (P1ready == true && P2ready == true)
                {
                    Comunicazione newcomunicazione = new Comunicazione(); //info comunicazione

                    newcomunicazione.statoPartita = true;
                    statoPartita = true;

                    //Genero random il giocatore che comincia per primo (1 o 2)
                    Turno = rnd.Next(1, 3);
                    newcomunicazione.Turno = Turno;

                    newcomunicazione.method = "InfoConnessione";
                    Sessions.Broadcast(JsonConvert.SerializeObject(newcomunicazione));
                }


            }

            //scambio i messaggi della chat tra i giocatori
            if (comunicazione.method == "Chat")
            {
                //invio ad un client i messaggi dell'altro
                if (Context.WebSocket == Player1)
                {
                    _clientSockets[1].Send(e.Data);
                }
                else
                {
                    _clientSockets[0].Send(e.Data);
                }
            }

            if (comunicazione.method == "Turno")
            {
                Comunicazione Turno = new Comunicazione(); //info comunicazione

                int newHp1;
                int newHp2;
                string JSONoutput;

                //invio ad un client i messaggi dell'altro
                if (Context.WebSocket == _clientSockets[0])
                {
                    newHp2 = CalcolaDanni(1, comunicazione);

                    Turno.method = "UpdateDati";
                    Turno.info = comunicazione.info;

                    Turno.hpP2 = newHp2;
                    Turno.hpP1 = jsonP1.hp;

                    jsonP2.hp = newHp2;//aggiorno la vita del pokemon nel json del server

                    Turno.Turno = 2;
                }
                else
                {
                    newHp1 = CalcolaDanni(2, comunicazione);

                    Turno.method = "UpdateDati";
                    Turno.info = comunicazione.info;

                    Turno.hpP1 = newHp1;
                    Turno.hpP2 = jsonP2.hp;

                    jsonP1.hp = newHp1;//aggiorno la vita del pokemon nel json del server

                    Turno.Turno = 1;
                }

                if (jsonP1.hp<=0)
                {
                    Turno.info += "PLAYER 2 HA VINTOOOOO\nInsersci un nuovo pokemon per una nuova partita\n";
                    Turno.readyEnabled = true;
                }
                if(jsonP2.hp <= 0)
                {
                    Turno.info += "PLAYER 1 HA VINTOOOOO\nInsersci un nuovo pokemon per una nuova partita\n";
                    Turno.readyEnabled = true;
                }

                JSONoutput = JsonConvert.SerializeObject(Turno);
                Sessions.Broadcast(JSONoutput);
            }
        }

        private int CalcolaDanni(int idPlayerAttack,Comunicazione comunicazione)
        {
            //se ad attaccare è il Player1
            //allora hpPlayer2 - Danni da Player1
            if (idPlayerAttack==1)
            {
                int hpP2 = jsonP2.hp - comunicazione.atkdp;
                return hpP2;
            }
            //altrimenti hpPlayer1 - Danni da Player2
            else
            {
                int hpP1 = jsonP1.hp - comunicazione.atkdp;
                return hpP1;
            } 
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Attivo un server con 3 servizi attivi: Echo;EchoAll;PrivateChat
            WebSocketServer wssv = new WebSocketServer("ws://127.0.0.1:9000");

            wssv.AddWebSocketService<Game>("/Game");

            wssv.Start();

            Console.WriteLine("Server started on ws://127.0.0.1:9000/Game");

            //interrompo il server dopo la pressione di un tasto
            Console.ReadKey();
            wssv.Stop();
        }
    }
}
