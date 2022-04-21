using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebSocketSharp;
using Client.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Windows.Threading;

namespace Client
{
    public partial class MainWindow : Window
    {
        WebSocket socketserver;

        bool Connesso = false;

        int PlayerID;
        Pokemon _jsonPokemon = new Pokemon();//il mio pokemon

        Mossa infoMossabtn1 = new Mossa();
        Mossa infoMossabtn2 = new Mossa();
        Mossa infoMossabtn3 = new Mossa();
        Mossa infoMossabtn4 = new Mossa();

        public MainWindow()
        {
            InitializeComponent();
            Connesso2.Text = "Non Connesso";
            Connesso2.Background = System.Windows.Media.Brushes.Red;
            btnReady.IsEnabled = false;
            DisabilitaChat();
            DisabilitaAttacco();

        }

        public void DisabilitaChat()
        {
            StatoBattaglia.IsEnabled = false;
            Invia.IsEnabled = false;
        }
        public void AbilitaChat()
        {
            StatoBattaglia.IsEnabled = true;
            Invia.IsEnabled = true;
        }
        public void DisabilitaAttacco()
        {
            btnAttack1.IsEnabled = false;
            btnAttack2.IsEnabled = false;
            btnAttack3.IsEnabled = false;
            btnAttack4.IsEnabled = false;
        }
        public void AbilitaAttacco()
        {
            btnAttack1.IsEnabled = true;
            btnAttack2.IsEnabled = true;
            btnAttack3.IsEnabled = true;
            btnAttack4.IsEnabled = true;
        }


        private void Connettiti_clicked(object sender, RoutedEventArgs e)
        {
            Connessione();
        }
        private void Connessione()
        {
            WebSocket wsServer = new WebSocket("ws://127.0.0.1:9000/Game");

            socketserver = wsServer;

            wsServer.OnMessage += Ws_OnMessage;
            wsServer.Connect();
            Connesso = true;
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            AnalizzaJson(e);
        }

        private void AnalizzaJson(MessageEventArgs e)
        {
            Comunicazione comunicazione = new Comunicazione(); //info comunicazione
            comunicazione = JsonConvert.DeserializeObject<Comunicazione>(e.Data);

            if (comunicazione.method == "InfoConnessione")
            {
                PlayerID = comunicazione.clientID;

                Dispatcher.Invoke(() =>
                {
                    StatoBattaglia.Text = comunicazione.info;
                    myPLayerID.Content = $"Player {comunicazione.clientID}";
                    Connesso2.Text = "Connesso";
                    Connesso2.Background = System.Windows.Media.Brushes.Green;
                    btnReady.IsEnabled = true;

                });
            }

            if (comunicazione.method == "Chat")
            {
                Dispatcher.Invoke(() =>
                {
                    StatoBattaglia.Text += $"Player{comunicazione.clientID}:" + comunicazione.info+"\n";
                });

            }

            if (comunicazione.method == "SceltaPokemon")
            {
                string pokemonAvversario = comunicazione.info;
                ThreadStart ts = new ThreadStart(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        CaricaPokemonAvversario(pokemonAvversario);
                    }));
                });

                Thread myThread = new Thread(ts);
                myThread.Start();
            }


            //gestisco il refresh dei dati dopo aver subito i danni
            if (comunicazione.method == "UpdateDati")
            {
                //aggiorno la vita del mio pokemon nel json

                ThreadStart tshp = new ThreadStart(() =>
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        StatoBattaglia.Text += $"{comunicazione.info}";
                        if (PlayerID == 1)
                        {
                            HpP1.Content = comunicazione.hpP1;
                            HpP2.Content = comunicazione.hpP2;
                        }
                        else
                        {
                            HpP1.Content = comunicazione.hpP2;
                            HpP2.Content = comunicazione.hpP1;
                        }
                        
                    }));
                });
                Thread myThread = new Thread(tshp);
                myThread.Start();
            }
        }

        private void btnAttack1_Click(object sender, RoutedEventArgs e)
        {
            Comunicazione Turno = new Comunicazione();
            Turno.method = "Turno";
            Turno.clientID = PlayerID;
            Turno.info = $"Player{PlayerID} usa {infoMossabtn1.name} !\n";

            Turno.atkname = infoMossabtn1.name;
            Turno.atkdp = infoMossabtn1.dp;
            Turno.atktype = infoMossabtn1.type;

            string JSONoutput = JsonConvert.SerializeObject(Turno);

            socketserver.Send(JSONoutput);
        }

        private void btnAttack2_Click(object sender, RoutedEventArgs e)
        {
            Comunicazione Turno = new Comunicazione();
            Turno.method = "Turno";
            Turno.clientID = PlayerID;
            Turno.info = $"Player{PlayerID} usa {infoMossabtn2.name} !\n";


            Turno.atkname = infoMossabtn2.name;
            Turno.atkdp = infoMossabtn2.dp;
            Turno.atktype = infoMossabtn2.type;

            string JSONoutput = JsonConvert.SerializeObject(Turno);

            socketserver.Send(JSONoutput);
        }

        private void btnAttack3_Click(object sender, RoutedEventArgs e)
        {
            Comunicazione Turno = new Comunicazione();
            Turno.method = "Turno";
            Turno.clientID = PlayerID;
            Turno.info = $"Player{PlayerID} usa {infoMossabtn3.name} !\n";

            Turno.atkname = infoMossabtn3.name;
            Turno.atkdp = infoMossabtn3.dp;
            Turno.atktype = infoMossabtn3.type;

            string JSONoutput = JsonConvert.SerializeObject(Turno);

            socketserver.Send(JSONoutput);
        }

        private void btnAttack4_Click(object sender, RoutedEventArgs e)
        {
            Comunicazione Turno = new Comunicazione();
            Turno.method = "Turno";
            Turno.clientID = PlayerID;
            Turno.info = $"Player{PlayerID} usa {infoMossabtn4.name} !\n";

            Turno.atkname = infoMossabtn4.name;
            Turno.atkdp = infoMossabtn4.dp;
            Turno.atktype = infoMossabtn4.type;

            string JSONoutput = JsonConvert.SerializeObject(Turno);

            socketserver.Send(JSONoutput);
        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            CaricaPokemon();

            Comunicazione comunicazione = new Comunicazione(); //info comunicazioneù
            comunicazione.method = "SceltaPokemon";
            comunicazione.clientID = PlayerID;
            comunicazione.info = TextBoxNomePokemon.Text;
            comunicazione.mypokemon = _jsonPokemon;

            string JSONoutput = JsonConvert.SerializeObject(comunicazione);
            //creo un ciclo per l'invio di più messaggi consecutivi al Server

            socketserver.Send(JSONoutput);
        }

        private void CaricaPokemon()
        {
            using var client = new HttpClient();

            var endpoint = new Uri($"https://courses.cs.washington.edu/courses/cse154/webservices/pokedex/pokedex.php?pokemon=" + TextBoxNomePokemon.Text.ToLower());

            var getResult = client.GetAsync(endpoint).Result;

            var getResultJson = getResult.Content.ReadAsStringAsync().Result;

            Pokemon jsonObjectInfo = JsonConvert.DeserializeObject<Pokemon>(getResultJson);

            _jsonPokemon = jsonObjectInfo;

            //Caricamento delle immagini
            BitmapImage bitmap = new BitmapImage();
            BitmapImage bitmap2 = new BitmapImage();
            BitmapImage bitmap3 = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri($"https://courses.cs.washington.edu/courses/cse154/webservices/pokedex/" + _jsonPokemon.images.photo);
            bitmap.EndInit();
            
            bitmap2.BeginInit();
            bitmap2.UriSource = new Uri($"https://courses.cs.washington.edu/courses/cse154/webservices/pokedex/" + _jsonPokemon.images.typeIcon);
            bitmap2.EndInit();

            bitmap3.BeginInit();
            bitmap3.UriSource = new Uri($"https://courses.cs.washington.edu/courses/cse154/webservices/pokedex/" + _jsonPokemon.images.weaknessIcon);
            bitmap3.EndInit();

            Dispatcher.Invoke(() =>
            {
                imgPokemonP1.Source = bitmap;
                ImgTypeP1.Source = bitmap2;
                ImgWeaknessP1.Source = bitmap3;
                HpP1.Content = _jsonPokemon.hp;
            });

            if (_jsonPokemon.moves.Count() == 1)
            {
                Dispatcher.Invoke(() =>
                {
                    btnAttack1.Content = _jsonPokemon.moves[0].name;
                    btnAttack2.IsEnabled = false;
                    btnAttack3.IsEnabled = false;
                    btnAttack4.IsEnabled = false;
                });
                CaricaMossaPulsante(1,_jsonPokemon);
            }

            if (_jsonPokemon.moves.Count() == 2)
            {
                Dispatcher.Invoke(() =>
                {
                    btnAttack1.Content = _jsonPokemon.moves[0].name;
                    btnAttack2.Content = _jsonPokemon.moves[1].name;
                    btnAttack3.IsEnabled = false;
                    btnAttack4.IsEnabled = false;
                });
                CaricaMossaPulsante(2, _jsonPokemon);
            }
            if (_jsonPokemon.moves.Count() == 3)
            {
                Dispatcher.Invoke(() =>
                {
                    btnAttack1.Content = _jsonPokemon.moves[0].name;
                    btnAttack2.Content = _jsonPokemon.moves[1].name;
                    btnAttack3.Content = _jsonPokemon.moves[2].name;
                    btnAttack4.IsEnabled = false;
                });
                CaricaMossaPulsante(3, _jsonPokemon);
            }

            if (_jsonPokemon.moves.Count() == 4)
            {
                Dispatcher.Invoke(() =>
                {
                    btnAttack1.Content = _jsonPokemon.moves[0].name;
                    btnAttack2.Content = _jsonPokemon.moves[1].name;
                    btnAttack3.Content = _jsonPokemon.moves[2].name;
                    btnAttack4.Content = _jsonPokemon.moves[3].name;
                });
                CaricaMossaPulsante(4, _jsonPokemon);
            }

        }

        //carico le info delle mosse nei rispettivi pulsanti
        private void CaricaMossaPulsante(int nPulsante, Pokemon json)
        {
            if (nPulsante >= 1)
            {
                infoMossabtn1.name = json.moves[0].name;
                infoMossabtn1.dp = json.moves[0].dp;
                infoMossabtn1.type = json.moves[0].type;
            }
            if (nPulsante >= 2)
            {
                infoMossabtn2.name = json.moves[1].name;
                infoMossabtn2.dp = json.moves[1].dp;
                infoMossabtn2.type = json.moves[1].type;
            }
            if (nPulsante >= 3)
            {
                infoMossabtn3.name = json.moves[2].name;
                infoMossabtn3.dp = json.moves[2].dp;
                infoMossabtn3.type = json.moves[2].type;
            }
            if (nPulsante == 4)
            {
                infoMossabtn4.name = json.moves[3].name;
                infoMossabtn4.dp = json.moves[3].dp;
                infoMossabtn4.type = json.moves[3].type;
            }
        }

        private void CaricaPokemonAvversario(string pokemon)//passo il nome del pokemon
        {
            using var client = new HttpClient();

            var endpoint = new Uri($"https://courses.cs.washington.edu/courses/cse154/webservices/pokedex/pokedex.php?pokemon=" + pokemon.ToLower());

            var getResult = client.GetAsync(endpoint).Result;

            var getResultJson = getResult.Content.ReadAsStringAsync().Result;

            Pokemon jsonObjectInfo = JsonConvert.DeserializeObject<Pokemon>(getResultJson);

            _jsonPokemon = jsonObjectInfo;

            //Caricamento delle immagini
            BitmapImage bitmap = new BitmapImage();
            BitmapImage bitmap2 = new BitmapImage();
            BitmapImage bitmap3 = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri($"https://courses.cs.washington.edu/courses/cse154/webservices/pokedex/" + _jsonPokemon.images.photo);
            bitmap.EndInit();

            bitmap2.BeginInit();
            bitmap2.UriSource = new Uri($"https://courses.cs.washington.edu/courses/cse154/webservices/pokedex/" + _jsonPokemon.images.typeIcon);
            bitmap2.EndInit();

            bitmap3.BeginInit();
            bitmap3.UriSource = new Uri($"https://courses.cs.washington.edu/courses/cse154/webservices/pokedex/" + _jsonPokemon.images.weaknessIcon);
            bitmap3.EndInit();

            Dispatcher.Invoke(() =>
            {
                imgPokemonP2.Source = bitmap;
                ImgTypeP2.Source = bitmap2;
                ImgWeaknessP2.Source = bitmap3;
                HpP2.Content = _jsonPokemon.hp;
            });
        }

        private void Connesso2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Chat_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Invia_Click(object sender, RoutedEventArgs e)
        {
            Comunicazione comunicazione = new Comunicazione(); //info comunicazione

            comunicazione.method = "Chat";
            comunicazione.clientID = PlayerID;
            comunicazione.info = Chat.Text;

            string JSONoutput = JsonConvert.SerializeObject(comunicazione);
            //creo un ciclo per l'invio di più messaggi consecutivi al Server

            socketserver.Send(JSONoutput);

        }

        private void StatoBattaglia_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
