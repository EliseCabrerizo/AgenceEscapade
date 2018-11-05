using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;
using System.Xml.XPath;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.IO; // à ajouter pour lecture/écriture de fichiers 

namespace AgenceEscapade
{
    class Program
    {
        
        static string[] LectureM1()
        {
            string[] client = new string[12];
            string nomDuDocXML = "M1.xml";

            //créer l'arborescence des chemins XPath du document
            //--------------------------------------------------
            XPathDocument doc = new System.Xml.XPath.XPathDocument(nomDuDocXML);
            XPathNavigator nav = doc.CreateNavigator();

            //créer une requete XPath
            //-----------------------
            string maRequeteXPath = "/sejour/Client";
            XPathExpression expr = nav.Compile(maRequeteXPath);

            //exécution de la requete
            //-----------------------
            XPathNodeIterator nodes = nav.Select(expr);// exécution de la requête XPath

            //parcourir le resultat
            //---------------------

            while (nodes.MoveNext()) // pour chaque réponses XPath (on est au niveau d'un noeud sejour)
            {
                Console.Write("Client : "+nodes.Current.SelectSingleNode("genre").InnerXml);
                Console.WriteLine("."+nodes.Current.SelectSingleNode("nom").InnerXml);
                client[0] = nodes.Current.SelectSingleNode("nom").InnerXml;
                client[1] = nodes.Current.SelectSingleNode("prenom").InnerXml;
                client[2] = nodes.Current.SelectSingleNode("age").InnerXml;
                client[3]=nodes.Current.SelectSingleNode("adresseC").InnerXml;
                client[4]= nodes.Current.SelectSingleNode("email").InnerXml;
                client[5]= nodes.Current.SelectSingleNode("telephone").InnerXml;
            }

            //autre requete
            maRequeteXPath = "/sejour/adresse";
            expr = nav.Compile(maRequeteXPath);
            nodes = nav.Select(expr);// exécution de la requête XPath
            while (nodes.MoveNext()) // pour chaque réponses XPath (on est au niveau d'un noeud sejour)
            {
                Console.Write("Adresse : "+nodes.Current.SelectSingleNode("rue").InnerXml);
                Console.Write(","+nodes.Current.SelectSingleNode("ville").InnerXml);
                Console.WriteLine(","+ nodes.Current.SelectSingleNode("codepostal").InnerXml);
                client[6] = nodes.Current.SelectSingleNode("rue").InnerXml;
                client[7] = nodes.Current.SelectSingleNode("ville").InnerXml;
                client[8] = nodes.Current.SelectSingleNode("codepostal").InnerXml;
            }
            //autre requete
            maRequeteXPath = "/sejour/date";
            expr = nav.Compile(maRequeteXPath);
            nodes = nav.Select(expr);// exécution de la requête XPath
            while (nodes.MoveNext()) // pour chaque réponses XPath (on est au niveau d'un noeud sejour)
            {
                Console.Write("Date du séjour (semaine-année) : "+nodes.Current.SelectSingleNode("semaine").InnerXml);
                Console.WriteLine("-"+nodes.Current.SelectSingleNode("annee").InnerXml);
                client[9] = nodes.Current.SelectSingleNode("semaine").InnerXml;
                client[10] = nodes.Current.SelectSingleNode("annee").InnerXml;
            }
             //autre requete
            maRequeteXPath = "/sejour/sjour";
            expr = nav.Compile(maRequeteXPath);
            nodes = nav.Select(expr);// exécution de la requête XPath
            while (nodes.MoveNext()) // pour chaque réponses XPath (on est au niveau d'un noeud sejour)
            {
                Console.WriteLine("Theme : " + nodes.Current.InnerXml);
                client[11] = nodes.Current.InnerXml;
            }
            return client;
        }
        static string E2(string[]client)
        {
            string codeC=null;
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD='';";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "USE AgenceEscapade;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Close();
            
            command.CommandText = "SELECT*FROM client WHERE nom='" + client[0] + "' AND prenom='" + client[1] + "';";
            reader = command.ExecuteReader();

            if (!reader.Read())
            {

                reader.Close();
                command.CommandText = "INSERT INTO `AgenceEscapade`.`client` (`codeC`, `nom`, `prenom`, `age`, `permis`, `adresse`,`email`,`telephone`) VALUES(CONCAT('C',ROUND(RAND()*1000)),'"+client[0]+"','"+client[1]+"',"+client[2]+",null,'"+client[3]+"','"+client[4]+"','"+client[5]+"'); ";
                reader = command.ExecuteReader();
                reader.Close();
                
                command.CommandText = "SELECT codeC FROM client WHERE nom='" + client[0] + "' AND prenom='" + client[1] + "';";
                reader = command.ExecuteReader();
                while (reader.Read())                           // parcours ligne par ligne
                {
                    string valueAsString = "";
                    for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                    {
                        valueAsString = reader.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                    }
                    codeC = valueAsString;    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
                }
                reader.Close();

            }
            else
            {
                reader.Close();
                command = connection.CreateCommand();
                command.CommandText = "SELECT codeC FROM client WHERE nom='" + client[0] + "' AND prenom='" + client[1] + "';";
                reader = command.ExecuteReader();
                while (reader.Read())                           // parcours ligne par ligne
                {
                    string valueAsString = "";
                    for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                    {
                        valueAsString = reader.GetValue(i).ToString();  // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                    }
                    codeC=valueAsString;    // affichage de la ligne (sous forme d'une "grosse" string) sur la sortie standard
                }
                reader.Close();
            }
            connection.Close();
            return codeC;
        }
        static string[] E3()
        {
            string[] infoVoiture = new string[4];
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD='';";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "USE AgenceEscapade;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Close();

            command.CommandText = "Select immat,parking,place,codepostal from voiture where dispo=true and codePostal = 75016;";
            reader = command.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                //Recherche les voitures dispo et choisi la première de la liste
                command.CommandText = "Select immat from voiture where dispo=true;";
                reader = command.ExecuteReader();
                string immatLibre = "";
                while (reader.Read())
                {
                    for (int i = 0; i < 1; i++)    // parcours cellule par cellule
                    {
                        immatLibre = reader.GetValue(i).ToString();
                        // recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                    }
                    break;
                }
                reader.Close();
                //Cherche les places occupées dans le 16ème
                command.CommandText = "Select place from voiture where codepostal=75016;";
                reader = command.ExecuteReader();
                //Si une est prise alors on cherche la première pas prise
                string place = "";
                string parking = "";
                int codePostal = -1;
                if (reader.Read())
                {
                    int i;
                    bool trouver = false;
                    for(i=0;i<10&&i<reader.FieldCount&&trouver!=true;i++)
                    {
                        if (reader.GetValue(i).ToString() != string.Concat('A',i)) 
                        {
                            trouver = true;
                            place = string.Concat('A', i);
                            parking = "Parking Victor Hugo";
                            codePostal = 75016;
                        }

                    }
                    if(!trouver)
                    {
                        place = string.Concat('A', i);
                        parking = "Parking Victor Hugo";
                        codePostal = 75016;
                    }
                }
                //Si aucune est prise alors on prend la première
                else
                {
                    place = "A0";
                    parking = "Parking Victor Hugo";
                    codePostal = 75016;
                }
                reader.Close();
                //Changement de place de la voiture
                command.CommandText = "UPDATE voiture set place = '"+place+"',parking = '"+parking+"',codePostal=75016, dispo=false WHERE immat ='"+immatLibre+"';";
                reader = command.ExecuteReader();
                reader.Close();
                command.CommandText = "INSERT INTO AgenceEscapade.`maintenance`(numMaint,`dateM`,`immatt`,`motif`,`note`) VALUES(CONCAT('M', ROUND(RAND() * 1000)), DATE(NOW()), '"+immatLibre+"', 'Déplacement voiture', null);";
                reader = command.ExecuteReader();
                reader.Close();
                infoVoiture =new string[] {immatLibre, parking, place, Convert.ToString(codePostal)};

            }
            else
            {
                reader.Close();
                reader = command.ExecuteReader();
                while (reader.Read())                           // parcours ligne par ligne
                {
                    string valueAsString = "";
                    for (int i = 0; i < reader.FieldCount; i++)    // parcours cellule par cellule
                    {
                        valueAsString = reader.GetValue(i).ToString();
                        infoVoiture[i] = valueAsString;// recuperation de la valeur de chaque cellule sous forme d'une string (voir cependant les differentes methodes disponibles !!)
                    }   
                }
                reader.Close();
                command.CommandText = "UPDATE voiture set place = '" + infoVoiture[2] + "',parking = '" + infoVoiture[1] + "',codePostal=75016, dispo=false WHERE immat ='" + infoVoiture[0] + "';";
                reader = command.ExecuteReader();
                reader.Close();
            }
            connection.Close();
            return infoVoiture;
        }
        static string[] CreationSejour(string[] client, string codeC, string[] infoVoiture)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD='';";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "USE AgenceEscapade;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Close();

            //On voulait utiliser le random (avec le code si dessous (comme cela devrait vraiment être fait)
            //Cependant pour des raisons pratique (retour client au M3 qui renvoie son numéro de séjour)
            //Nous avons préféré lui attribuer une valeur brut afin que vous n'ayez rien à toucher
            //Random random = new Random();
            //int r=random.Next(100, 999);
            //string numSejour = string.Concat('S', r);

            //Ca permet de lancer plusieurs fois le programme sans problème dans la base de données à cause de clé primaire identique
            string numSejour = string.Concat('S', 660+DateTime.Now.Minute+DateTime.Now.Second);
            command.CommandText = "INSERT INTO AgenceEscapade.`sejour` (annee, codePostal, semaine, immat, note, codeC,`numSejour`,`placeD`,`placeA`,`parkingD`,`parkingA`,`idRoom`,`theme`,`etatDossier`) VALUES(" + client[10] + "," + client[8] + "," + client[9] + ", '" + infoVoiture[0] + "', null, '" + codeC + "','"+numSejour+"', '" + infoVoiture[2] + "', null, '" + infoVoiture[1]+"', null,null,'"+client[11]+"',false); ";
            reader = command.ExecuteReader();
            reader.Close();
            connection.Close();
            string[] sejour = new string[] { numSejour, codeC, client[0], client[1], client[2], client[10], client[9], client[8], infoVoiture[0], infoVoiture[2], infoVoiture[1], client[11] };
            return sejour;
        }
        static List<List<string>> LectureJsonLog()
        {
            List<List<string>> logements = new List<List<string>>();
            StreamReader reader = new StreamReader("ReponseRBNP.json");
            JsonTextReader jreader = new JsonTextReader(reader);
            int compt = 0;
            List<string> tempLog = new List<string>();
            while (jreader.Read() && compt<3)
            {

                //il y a deux sortes de token : avec une valeur associée ou non 6
                if (jreader.Value != null)
                {
                    tempLog.Add(Convert.ToString(jreader.Value));
                }
                else
                {
                    if (jreader.TokenType.ToString() == "EndObject")
                    {
                        string dispo = null;
                        string arrondissement = null;
                        string nbChambre = null;
                        string evalMin = null;
                        for (int i = 0; i < tempLog.Count; i++)
                        {
                            if (tempLog[i] == "availability")
                                dispo = tempLog[i + 1];
                            if (tempLog[i] == "overall_satisfaction")
                                evalMin = tempLog[i + 1];
                            if (tempLog[i] == "borough")
                                arrondissement = tempLog[i + 1];
                            if (tempLog[i] == "bedrooms")
                                nbChambre = tempLog[i + 1];
                        }
                        if (dispo == "yes" && nbChambre == "1" && arrondissement == "16"&&string.Compare(evalMin,"4,5")>0)
                            {
                            logements.Add(new List<string> (tempLog));
                                compt++;
                            }
                            
                        tempLog = new List<string>();
                    }
                

                }
            }

            jreader.Close();
            reader.Close();
            return logements;
        }
        static void M2(string[]sejour, List<List<string>> logement)
        {
            XmlDocument docXml = new XmlDocument();

            // création de l'en-tête XML (no <=> pas de DTD associée)
            docXml.CreateXmlDeclaration("1.0", "UTF-8", "no");

            XmlElement racine = docXml.CreateElement("dossier");
            racine.SetAttribute("etatdossier", "non confirmé");
            racine.SetAttribute("numSejour", sejour[0]);
            docXml.AppendChild(racine);

            XmlElement autreBalise = docXml.CreateElement("client");
            XmlElement baliseEnfant = docXml.CreateElement("codeC");
            baliseEnfant.InnerText = sejour[1];
            autreBalise.AppendChild(baliseEnfant);
            baliseEnfant = docXml.CreateElement("prenom");
            baliseEnfant.InnerText = sejour[3];
            autreBalise.AppendChild(baliseEnfant);
            baliseEnfant = docXml.CreateElement("nom");
            baliseEnfant.InnerText = sejour[2];
            autreBalise.AppendChild(baliseEnfant);
            racine.AppendChild(autreBalise);

            autreBalise = docXml.CreateElement("sejour");
            baliseEnfant = docXml.CreateElement("semaine");
            baliseEnfant.InnerText = sejour[6];
            autreBalise.AppendChild(baliseEnfant);
            baliseEnfant = docXml.CreateElement("annee");
            baliseEnfant.InnerText = sejour[5];
            autreBalise.AppendChild(baliseEnfant);
            baliseEnfant = docXml.CreateElement("theme");
            baliseEnfant.InnerText = sejour[11];
            autreBalise.AppendChild(baliseEnfant);
            racine.AppendChild(autreBalise);
           


            autreBalise = docXml.CreateElement("voiture");
            baliseEnfant = docXml.CreateElement("immat");
            baliseEnfant.InnerText = sejour[8];
            autreBalise.AppendChild(baliseEnfant);
            baliseEnfant = docXml.CreateElement("parking");
            baliseEnfant.InnerText = sejour[10];
            autreBalise.AppendChild(baliseEnfant);
            baliseEnfant = docXml.CreateElement("place");
            baliseEnfant.InnerText = sejour[9];
            autreBalise.AppendChild(baliseEnfant);
            racine.AppendChild(autreBalise);

            int compt = 0;
            while(compt<3)
            {
                string prix = null;
                string arrondissement = null;
                string evalMin = null;
                string idroom = null;
                string idhost = null;
                for (int i = 0; i < logement[compt].Count; i++)
                {
                    if (logement[compt][i] == "host_id")
                        idhost = logement[compt][i + 1];
                    if (logement[compt][i] == "room_id")
                        idroom = logement[compt][i + 1];
                    if (logement[compt][i] == "price")
                        prix = logement[compt][i + 1];
                    if (logement[compt][i] == "overall_satisfaction")
                        evalMin = logement[compt][i + 1];
                    if (logement[compt][i] == "borough")
                        arrondissement = logement[compt][i + 1];
                }

                autreBalise = docXml.CreateElement("logement");
                baliseEnfant = docXml.CreateElement("prix");
                baliseEnfant.InnerText = prix;
                autreBalise.AppendChild(baliseEnfant);
                baliseEnfant = docXml.CreateElement("notemoyenne");
                baliseEnfant.InnerText = evalMin;
                autreBalise.AppendChild(baliseEnfant);
                baliseEnfant = docXml.CreateElement("codepostal");
                baliseEnfant.InnerText = arrondissement;
                autreBalise.AppendChild(baliseEnfant);
                autreBalise.SetAttribute("idroom", idroom);
                racine.AppendChild(autreBalise);
                compt++;
            }
            
            
            // enregistrement du document XML   ==> à retrouver dans le dossier bin\Debug de Visual Studio
            docXml.Save("M2.xml");
        }
       
        static List<string> LectureM3(string[]sejour,List<List<string>>logement)
        {
            string[] client = new string[6];
            string nomDuDocXML = "M3.xml";

            //créer l'arborescence des chemins XPath du document
            //--------------------------------------------------
            XPathDocument doc = new System.Xml.XPath.XPathDocument(nomDuDocXML);
            XPathNavigator nav = doc.CreateNavigator();


            //créer une requete XPath
            //-----------------------
            string maRequeteXPath = "/sejour/numsejour";
            XPathExpression expr = nav.Compile(maRequeteXPath);

            //exécution de la requete
            //-----------------------
            XPathNodeIterator nodes = nav.Select(expr);// exécution de la requête XPath

            //parcourir le resultat
            //---------------------
            string temp1=null;
            while (nodes.MoveNext()) // pour chaque réponses XPath (on est au niveau d'un noeud sejour)
            {
                temp1 = nodes.Current.InnerXml;
            }
            //autre requete
            maRequeteXPath = "/sejour/refLogement";
            expr = nav.Compile(maRequeteXPath);
            nodes = nav.Select(expr);// exécution de la requête XPath
            string temp2=null;
            while (nodes.MoveNext()) // pour chaque réponses XPath (on est au niveau d'un noeud sejour)
            {
                temp2 = nodes.Current.InnerXml;
            }

            //autre requete
            maRequeteXPath = "/sejour/validation";
            expr = nav.Compile(maRequeteXPath);
            nodes = nav.Select(expr);// exécution de la requête XPath
            string temp3 = null; ;
            while (nodes.MoveNext()) // pour chaque réponses XPath (on est au niveau d'un noeud sejour)
            {
                temp3= nodes.Current.InnerXml;
            }

            List<string> temp = new List<string>();
            if (temp3 == "confirmé")
            {
                string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD='';";
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "USE AgenceEscapade;";
                MySqlDataReader reader;
                reader = command.ExecuteReader();
                reader.Close();
                command.CommandText = "UPDATE sejour set etatDossier = true ,idRoom = '" + temp2 + "' WHERE numSejour ='" + temp1 + "';";
                reader = command.ExecuteReader();
                reader.Close();
                connection.Close();
                if (temp2 == logement[0][3])
                {
                    for (int i = 0; i < logement[0].Count; i++)
                        temp.Add(logement[0][i]);
                }
                else if (temp2 == logement[1][3])
                {
                    for (int i = 0; i < logement[1].Count; i++)
                        temp.Add(logement[1][i]);
                }
                else
                {
                    for (int i = 0; i < logement[2].Count; i++)
                        temp.Add(logement[2][i]);
                }

            }
            else {
                //on dit que la voiture est libre finalement 
                string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD='';";
                MySqlConnection connection = new MySqlConnection(connectionString);
                connection.Open();

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "USE AgenceEscapade;";
                MySqlDataReader reader;
                reader = command.ExecuteReader();
                reader.Close();
                command.CommandText = "UPDATE voiture SET dispo=true WHERE immat ='" + sejour[8] + "';";
                reader = command.ExecuteReader();
                reader.Close();
                command.CommandText = "INSERT INTO AgenceEscapade.`maintenance`(numMaint,`dateM`,`immatt`,`motif`,`note`) VALUES(CONCAT('M', ROUND(RAND() * 1000)), DATE(NOW()), '" + sejour[8] + "', 'Déplacement voiture', null);";
                reader = command.ExecuteReader();
                reader.Close();
                connection.Close();

                //On écrit un message au client pour lui dire que ce n'est pas réalisable
                XmlDocument docXml = new XmlDocument();

                // création de l'en-tête XML (no <=> pas de DTD associée)
                docXml.CreateXmlDeclaration("1.0", "UTF-8", "no");

                XmlElement racine = docXml.CreateElement("dossier");
                racine.SetAttribute("etatdossier", "impossible");
                racine.SetAttribute("numSejour", sejour[0]);
                docXml.AppendChild(racine);

                XmlElement autreBalise = docXml.CreateElement("client");
                XmlElement baliseEnfant = docXml.CreateElement("codeC");
                baliseEnfant.InnerText = sejour[1];
                autreBalise.AppendChild(baliseEnfant);
                baliseEnfant = docXml.CreateElement("prenom");
                baliseEnfant.InnerText = sejour[3];
                autreBalise.AppendChild(baliseEnfant);
                baliseEnfant = docXml.CreateElement("nom");
                baliseEnfant.InnerText = sejour[2];
                autreBalise.AppendChild(baliseEnfant);
                racine.AppendChild(autreBalise);

                autreBalise = docXml.CreateElement("sejour");
                baliseEnfant = docXml.CreateElement("semaine");
                baliseEnfant.InnerText = sejour[6];
                autreBalise.AppendChild(baliseEnfant);
                baliseEnfant = docXml.CreateElement("annee");
                baliseEnfant.InnerText = sejour[5];
                autreBalise.AppendChild(baliseEnfant);
                baliseEnfant = docXml.CreateElement("theme");
                baliseEnfant.InnerText = sejour[11];
                autreBalise.AppendChild(baliseEnfant);
                baliseEnfant = docXml.CreateElement("info");
                baliseEnfant.InnerText = "séjour impossible, veuillez choisir une autre date";
                autreBalise.AppendChild(baliseEnfant);
                racine.AppendChild(autreBalise);
                docXml.Save("M4.xml");

                temp = null;

            }
            return temp;
        }
        static void EcritureFichierJ4(List<string>logement)
        {
            Console.WriteLine("Ecriture de la résevervtion dans un fichier Json pour RBNP");
            string monFichier = "J4.json";
            

            //instanciation des "writer"
            StreamWriter writer = new StreamWriter(monFichier);
            JsonTextWriter jwriter = new JsonTextWriter(writer);

            //debut du fichier Json
            jwriter.WriteStartObject();
            jwriter.WritePropertyName("logement_choisi");

            //ecriture du tableau Json
            jwriter.WriteStartArray();
            jwriter.WriteStartObject();

            for (int i = 0; i < logement.Count; i++)
            {
                if(logement[i]=="host_id")
                {
                    jwriter.WritePropertyName("host_id");
                    jwriter.WriteValue(logement[i+1]);
                }
                if (logement[i] == "room_id")
                {
                    jwriter.WritePropertyName("room_id");
                    jwriter.WriteValue(logement[i+1]);
                }
                if (logement[i] == "week")
                {
                    jwriter.WritePropertyName("week");
                    jwriter.WriteValue(logement[i+1]);
                }
                if (logement[i] == "availability")
                {
                    jwriter.WritePropertyName("availability");
                    jwriter.WriteValue("no");
                }
            }

            jwriter.WriteEndObject();
            jwriter.WriteEndArray();

            //fin du fichier Json
            jwriter.WriteEndObject();

            //fermeture des "writer"
            jwriter.Close();
            writer.Close();
            
        }
        static void Retour(string[] sejour)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD='';";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "USE AgenceEscapade;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Close();

            command.CommandText = "UPDATE sejour set placeA = 'A8', parkingA = 'Parking Rivoli', codePostal = 75001, note = 4 where numSejour='"+sejour[0] +"';";
            reader = command.ExecuteReader();
            reader.Close();
            command.CommandText = "UPDATE voiture set place = 'A8',parking= 'Parking Rivoli', codePostal = 75001 WHERE immat = (select immat from sejour where numSejour='" + sejour[0]+"');";
            reader = command.ExecuteReader();
            reader.Close();
            command.CommandText = "INSERT INTO AgenceEscapade.`maintenance`(numMaint,`dateM`,`immatt`,`motif`,`note`) VALUE (CONCAT('M', ROUND(RAND() * 1000)), DATE(NOW()), '"+sejour[8]+"', 'Nettoyage', 'tout ok'); ";
            reader = command.ExecuteReader();
            reader.Close();
            command.CommandText = "UPDATE voiture set  dispo = true WHERE immat = (select immat from sejour where numSejour='" + sejour[0] + "');";
            reader = command.ExecuteReader();
            reader.Close();
            connection.Close();
        }
        static void Vehicule(string immat)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD='';";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "USE AgenceEscapade;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Close();

            command.CommandText = "Select COUNT(*) from maintenance where immatt = '"+immat+"';";
            reader = command.ExecuteReader();
            reader.Read();
            Console.WriteLine("Le vehicule dont l'imamtriculation est : " + immat + " a eu " + reader.GetValue(0).ToString() + " interventions.");
            reader.Close();
            command.CommandText = "Select COUNT(*) from sejour where immat = '" + immat + "';";
            reader = command.ExecuteReader();
            reader.Read();
            Console.WriteLine("Il a été loué " + reader.GetValue(0).ToString() + " fois.");
            reader.Close();
            connection.Close();
        }
        static void SejourCLient(string codeC)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueur;UID=root;PASSWORD='';";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "USE AgenceEscapade;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            reader.Close();

            command.CommandText = "Select COUNT(*) from sejour where codeC = '" + codeC + "';";
            reader = command.ExecuteReader();
            reader.Read();
            Console.WriteLine("Le client dont le code client est : " + codeC + " a fait " + reader.GetValue(0).ToString() + " sejours.");
            reader.Close();
            connection.Close();
        }
        static void Main(string[] args)
        {
            //E1
            Console.WriteLine("Etape1 : \n");
            string[] client = LectureM1();
            Console.WriteLine("\n\nAppuyer sur une touche pour passer à l'étape suivante");
            Console.ReadKey();

            //E2
            Console.WriteLine("\nEtape2 : \n");
            string codeC = E2(client);
            Console.WriteLine("Le client a été trouvé ou créé via les informations de XML. Voici son codeC : " + codeC);
            Console.WriteLine("\n\nAppuyer sur une touche pour passer à l'étape suivante");
            Console.ReadKey();

            //E3
            Console.WriteLine("\nEtape3 : \n");
            string[] infoVoiture = E3();
            Console.WriteLine("Voici les informations de la voiture qui lui a été attrbuée (immat, parking, place, code postal) : ");
            for (int i = 0; i < infoVoiture.Length; i++)
                Console.Write(infoVoiture[i] + ", ");
            string[] sejour = CreationSejour(client, codeC, infoVoiture);
            Console.WriteLine("Son numéro de séjour est "+sejour[0]);
            Console.WriteLine("\n\nAppuyer sur une touche pour passer à l'étape suivante");
            Console.ReadKey();

            //E4
            Console.WriteLine("\nEtape4: \n");
            Console.WriteLine("Simulation de RBNP");
            Console.WriteLine("\n\nAppuyer sur une touche pour passer à l'étape suivante");
            Console.ReadKey();

            //E5
            Console.WriteLine("\nEtape5 : \n");
            List<List<string>> logement=LectureJsonLog();
            Console.WriteLine("Les 3 logements sélectionnées sont :");
            for (int i = 0; i < 3; i++)
                Console.WriteLine("Ref : "+logement[i][3]);
            Console.WriteLine("\n\nAppuyer sur une touche pour passer à l'étape suivante");
            Console.ReadKey();

            //E6
            Console.WriteLine("\nEtape6 : \n");
            M2(sejour,logement);
            Console.WriteLine("Fichier M2 créé\n");
            Console.WriteLine("\n\nAppuyer sur une touche pour passer à l'étape suivante");
            Console.ReadKey();


            //E7
            Console.WriteLine("\nEtape7: \n");
            List<string> logementFinal=LectureM3(sejour,logement);
            if (logementFinal != null)
            {
                EcritureFichierJ4(logementFinal);
                Console.WriteLine("Le logement finale sélectionné est : " + logementFinal[3]);
            }
            else Console.WriteLine("Le séjour est impossible");   
            Console.WriteLine("\n\nAppuyer sur une touche pour passer à l'étape suivante");
            Console.ReadKey();

            //Retour 
            Console.WriteLine("\nCheck-out");
            Retour(sejour);
            Console.WriteLine("\n\nAppuyer sur une touche pour passer à l'étape suivante");
            Console.ReadKey();

            //Tableau de bord
            Console.WriteLine("\nTableau de bord \n");
            Vehicule(sejour[8]);
            SejourCLient(sejour[1]);
            Console.WriteLine("\n\nLa démonstration du logiciel est finie.\nAppuyer sur une touche pour fermer la fenêtre");
            Console.ReadKey();
        }
    }
}
