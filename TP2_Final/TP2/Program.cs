/* Cet algorithme simule un combat entre un groupe d'aventurier et des monstres dans un donjon. Le tout se déroule automatiquement.
* J'ai ajouté de l'initiative afin de savoir qui commence et de la fatigue pour pallier les boucles infinies.
* J'ai utilisé des dictionnaires, car j'ai trouvé ce système plus efficace que plusieurs tableaux simples et moins mélangeant.
*
* Cree par Arnaud Poussier en Avril 2023
*/

using Utilitaires;
using System;
using System.Collections.Generic;

namespace TP2
{
    internal class Program
    {
        const int Potion = 500;     //Restaure 500 Pv par aventurier après chaque combat.
        const int Booster = 100;    //Restaure 100 Fatigue par aventurier après chaque combat.

        static void Main(string[] args)
        {
            new Program().Principale();
        }

        void Principale()
        {         
            bool donjon = true; //Permet de déclencher la défaite si la valeur est false.
            bool confirmation;  //Pour regénérer une nouvelle équipe.
            bool mort;          //Permet de savoir s'il y a un ou des morts dans une salle.

            List<string> defunt = new List<string>();   //Liste pour afficher les defunts. (Je l'ai passé en ref car sinon il faut créer une nouvelle liste à chaque fois, comme ca je garde la meme)

            Presentation();
            Dictionary<string, (int, int, int, int, int)> Aventuriers;
            int nbAventurier = Util.LireInt32AvecIntervalle("Veuillez choisir le nombre d'aventurier(s). [min. : 1 et max. : 9] : ", 1, 9);

            //Pour regénérer les aventuriers au besoin.
            do
            {
                CreerAventuriers(nbAventurier, out Aventuriers);
                AfficherAventuriers(Aventuriers, donjon);
                confirmation = Util.ConfirmerOuiNon("Continuer avec ce résultat ? [tapez \"O\" pour oui, \"N\" pour générer une nouvelle équipe] : ");
            }
            while (confirmation == false);

            Dictionary<string, (int, int, int, int)> Monstres;
            int nbMonstre = Util.LireInt32AvecIntervalle("Veuillez choisir le nombre de monstre(s). [min. : 1 et max. : 9] : ", 1, 9);
            CreerMonstres(nbMonstre, out Monstres);

            for (int general = 0; general < nbMonstre; ++general)   //Boucle pour le nb de salle ou nb de monstres.
            {
                if (donjon == false) break;     //Au début de chaque tour on vérifie si il y a défaite ou pas.

                Salle(Monstres, nbMonstre, donjon, general);    //Pour la présentation du monstre.
                Combat(ref Aventuriers, ref Monstres, nbMonstre, ref donjon, general, out mort, ref defunt);    //Le Combat entre les aventuriers et le monstre.
                Recuperation(ref Aventuriers, nbMonstre, donjon, general, mort, defunt);    //La ou les aventuriers récupèrent leur pv et leur fatigue.
                
                if (mort == true)
                    AfficherAventuriers(Aventuriers, donjon);   //S'il y a des morts, on affiche les aventuriers encore en vie avant d'entrée dans la salle suivante afin de connaitre les nouvelles valeurs de groupe.
            }

            Fin(donjon);
        }

        void Presentation()
        {
            Console.WriteLine("   _________________________________________________________\r\n /|     -_-                                             _-  |\\\r\n/ |_-_- _                                         -_- _-   -| \\   \r\n  |                            _-  _--                      | \r\n  |                            ,                            |\r\n  |      .-'````````'.        '(`        .-'```````'-.      |\r\n  |    .` |           `.      `)'      .` |           `.    |          \r\n  |   /   |   ()        \\      U      /   |    ()       \\   |\r\n  |  |    |    ;         | o   T   o |    |    ;         |  |\r\n  |  |    |     ;        |  .  |  .  |    |    ;         |  |\r\n  |  |    |     ;        |   . | .   |    |    ;         |  |\r\n  |  |    |     ;        |    .|.    |    |    ;         |  |\r\n  |  |    |____;_________|     |     |    |____;_________|  |  \r\n  |  |   /  __ ;   -     |     !     |   /     `'() _ -  |  |\r\n  |  |  / __  ()        -|        -  |  /  __--      -   |  |\r\n  |  | /        __-- _   |   _- _ -  | /        __--_    |  |\r\n  |__|/__________________|___________|/__________________|__|\r\n /                                             _ -           \\\r\n/   -_- _ -             _- _---                       -_-  -_ \\");
            Console.WriteLine();
            Console.WriteLine("Vous êtes sur le point d'entrer dans un donjon dont vous ne connaissez pas l'issue." +
                "\nIl est encore temps de faire demi-tour...");
            Console.WriteLine();
        }

        /// <summary>
        /// Créer les aventuriers automatiquement avec des valeurs aléatoires.
        /// </summary>
        /// <param name="p_nbAventurier"> nb d'aventurier </param>
        /// <param name="p_Aventuriers"> dictionnaire aventuriers </param>
        void CreerAventuriers(int p_nbAventurier, out Dictionary<string, (int, int, int, int, int)> p_Aventuriers)
        {
            p_Aventuriers = new Dictionary<string, (int, int, int, int, int)>();            
            Random rnd = new Random();

            Console.WriteLine();

            for (int i = 0; i < p_nbAventurier; i++)
            {
                string nom = Util.LireStringTailleControlee($"Veuillez entrer un nom entre 3 et 10 caractères pour l'aventurier no {i + 1} : ", 3, 10);

                int pv = Potion;
                int fatigue = Booster;
                int attaque = rnd.Next(1, 101);
                int defense = rnd.Next(1, 51);
                int initiative = rnd.Next(1, 51);

                p_Aventuriers.Add(nom, (pv, fatigue, attaque, defense, initiative));
            }
        }

        /// <summary>
        /// Affiche les aventuriers en jeu.
        /// Les aventuriers morts sont supprimés et ne sont pas affichés. 
        /// </summary>
        /// <param name="p_donjon"> déclenche la victoire ou la défaite </param>
        /// <param name="p_Aventuriers"> dictionnaire aventuriers </param>
        void AfficherAventuriers(Dictionary<string, (int, int, int, int, int)> p_Aventuriers, bool p_donjon)

        {
            while (p_donjon == true)
            {
                int totalPv = 0;
                int totalFatigue = 0;
                int totalAttaque = 0;
                int totalDefense = 0;
                int totalInitiative = 0;

                Console.WriteLine($" __________________________________________________________________");
                Console.WriteLine($"|{"Nom",7}{"|",7}{"Pv",6}{"|",4}{"Fatigue",8}{"|",2}{"Attaque",8}{"|",2}{"Défense",8}{"|",2}{"Initiative",11}{"|",2}");
                Console.WriteLine($"|-------------|---------|---------|---------|---------|------------|");
                
                foreach (KeyValuePair<string, (int, int, int, int, int)> aventurier in p_Aventuriers)
                {
                    int pv = aventurier.Value.Item1;
                    int fatigue = aventurier.Value.Item2;
                    int attaque = aventurier.Value.Item3;
                    int defense = aventurier.Value.Item4;
                    int initiative = aventurier.Value.Item5;

                    totalPv += pv;
                    totalFatigue += fatigue;
                    totalAttaque += attaque;
                    totalDefense += defense;
                    totalInitiative += initiative;

                    Console.WriteLine($"|{aventurier.Key,12} | {pv,7} | {fatigue,7} | {attaque,7} | {defense,7} | {initiative,10} |");
                }

                Console.WriteLine($"|_____________|_________|_________|_________|_________|____________|");
                
                if (p_Aventuriers.Count > 1)
                {
                    Console.WriteLine($"|{null,7}TOTAL | {totalPv,7} | {totalFatigue,7} | {totalAttaque,7} | {totalDefense,7} | {totalInitiative,10} |");
                    Console.WriteLine($"|_____________|_________|_________|_________|_________|____________|");
                }

                Console.WriteLine();
                break;
            }
        }

        /// <summary>
        /// Créer les monstres automatiquement avec des valeurs aléatoires.
        /// </summary>
        /// <param name="p_nbMonstre"> nombre de monstre </param>
        /// <param name="p_Monstres"> dictionnaire monstres </param>
        void CreerMonstres(int p_nbMonstre, out Dictionary<string, (int, int, int, int)> p_Monstres)
        { 
            p_Monstres = new Dictionary<string, (int, int, int, int)>();
            Random rnd = new Random();

            List<string> Monstres = new List<string> { "   Araignée Géante   ", "      Squelette      ", "        Momie        ", "    Goule Enragée    ", "        Liche        ", "     Orc sauvage     ", "       Spectre       ", "        Demon        ", "     Dragon Noir     ", "       Griffon       ", "Cerbère à Trois Têtes", "  Élémentaire d'Eau  ", "  Élémentaire d'Air  ", "       Banshee       ", "       Gobelin       ", "   Dévoreur d'Âmes   ", "       Cyclope       ", " Trolls des Cavernes ", "       Vampire       ", "   Golem de Pierre   ", "  Ogre Sanguinaire   ", "       Basilic       ", "   Harpie Affamée    ", "     Abomination     ", "       Succube       ", "       Gorgone       ", "     Roi Gobelin     ", "   Hydre Venimeuse   ", " Chevalier Squelette ", "      Rat Géant      ", "      Minotaure      " };

            for (int i = 0; i < p_nbMonstre; i++)
            {
                string nom = Monstres[rnd.Next(Monstres.Count)];
                int pv = rnd.Next(1, 500 * (i + 1) + 1);        //Entre 1 inclus et 500 exclu multiplié par niveau (salle) + 1
                int attaque = rnd.Next(1, 100 * (i + 1) + 1);
                int defense = rnd.Next(1, 50 * (i + 1) + 1);
                int initiative = rnd.Next(1, 100 * (i + 1) + 1);

                p_Monstres.Add(nom, (pv, attaque, defense, initiative));
                Monstres.Remove(nom);
            }
        }

        /// <summary>
        /// Affiche le monstre à combattre à l'entrée dans chaque salle.
        /// </summary>
        /// <param name="p_nbMonstre"> nombre de monstre </param>
        /// <param name="p_Monstres"> dictionnaire monstres </param>
        /// <param name="p_donjon"> déclenche la victoire ou la défaite </param>
        /// <param name="p_general"> compteur général en rapport avec le nombre de monstre saisie </param>
        void Salle(Dictionary<string, (int, int, int, int)> p_Monstres, int p_nbMonstre, bool p_donjon, int p_general)
        {
            while (p_donjon == true && p_nbMonstre > p_general) //On vérifie si le nb de tour de la boucle général est inferieur au nb de monstre pour entrer dans la méthode.
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"{null,10}Vous entrez dans la salle {p_general + 1} pour combattre :");
                Console.WriteLine($"{null,5} ____________________________________________________");
                Console.WriteLine($"{null,5}|{null,24}|{"Pv",4}{"|",3}{"Att",5}{"|",2}{"Def",5}{"|",2}{"Ini",5}{"|",2}");
                Console.WriteLine($"{null,5}|{null,24}|------|------|------|------|");
                Console.WriteLine($"{null,5}|{null,2}{p_Monstres.Keys.ElementAt(p_general).ToUpper()}{"|",2}{p_Monstres.Values.ElementAt(p_general).Item1,5}{"|",2}{p_Monstres.Values.ElementAt(p_general).Item2,5}{"|",2}{p_Monstres.Values.ElementAt(p_general).Item3,5}{"|",2}{p_Monstres.Values.ElementAt(p_general).Item4,5}{"|",2}");
                Console.WriteLine($"{null,5}|{null,24}|{null,6}|{null,6}|{null,6}|{null,6}|");
                Console.WriteLine($"{null,5}|________________________|______|______|______|______|");
                Console.WriteLine();
                break;
            }
        }

        /// <summary>
        /// Section combat qui comporte des sous sections pour les attaques du monstre et des aventuriers.
        /// </summary>
        /// <param name="p_donjon"> déclenche la victoire ou la défaite </param>
        /// <param name="p_Aventuriers"> dictionnaire aventuriers </param>
        /// <param name="p_Monstres"> dictionnaire monstres </param>
        /// <param name="p_general"> compteur général en rapport avec le nombre de monstre saisie </param>
        /// <param name="p_nbMonstre"> nombre de monstre </param>
        /// <param name="p_mort"> se déclenche si un aventurier tombe au combat </param>
        /// <param name="p_defunt"> liste pour les aventuriers tombé au combat </param>
        void Combat(ref Dictionary<string, (int, int, int, int, int)> p_Aventuriers, ref Dictionary<string, (int, int, int, int)> p_Monstres, int p_nbMonstre, ref bool p_donjon, int p_general, out bool p_mort, ref List<string> p_defunt)
        {
            bool combat;        //Indique la mort du monstre quand la valeur passe a false.
            p_mort = false;     //Il n'y a aucun aventurier mort.
            p_defunt.Clear();   //On efface la liste avant le combat meme si la liste est vide.

            while (p_donjon == true && p_nbMonstre > p_general)
            {
                for (int tour = 0 ; ; ++tour)   //Pas de fin jusqu'a ce que l'un meurt.
                {
                    int attaqueGroupe;
                    int fatigueGroupe;
                    int initiativeGroupe;

                    //Fait la mise a jour avant chaque attaque pour actualiser les valeurs.
                    MiseAJourAventuriers(p_Aventuriers, out attaqueGroupe, out initiativeGroupe); 

                    Console.WriteLine($"Tour {tour + 1} :");

                    //Compare l'initiative du groupe à l'initiative du monstre pour savoir qui attaque en premier.
                    if (initiativeGroupe >= p_Monstres.Values.ElementAt(p_general).Item4)   
                    {
                        AttaqueAventuriers(p_Aventuriers, ref p_Monstres, p_general, attaqueGroupe, out combat, out fatigueGroupe);
                        if (combat == false) break;     //Victoire
                        AttaqueMonstre(ref p_Aventuriers, p_Monstres, ref p_donjon, p_general, ref p_mort, ref p_defunt);
                        if (p_donjon == false) break;   //Défaite
                    }

                    else
                    {
                        AttaqueMonstre(ref p_Aventuriers, p_Monstres, ref p_donjon, p_general, ref p_mort, ref p_defunt);
                        if (p_donjon == false) break;   //Défaite
                        MiseAJourAventuriers(p_Aventuriers, out attaqueGroupe, out initiativeGroupe); //Si le monstre tue un aventurier entre temps, on doit re-actualiser les valeurs du groupe car l'attaque va changer.
                        AttaqueAventuriers(p_Aventuriers, ref p_Monstres, p_general, attaqueGroupe, out combat, out fatigueGroupe);
                        if (combat == false) break;     //Victoire 
                    }

                    //S'arrête au bout de 100 tours pour éviter les combats trop longs et les boucles infinis.
                    if (fatigueGroupe <= 0)
                    {
                        Console.WriteLine();

                        //Deux messages en fonction du nombre d'aventuriers restant
                        if (p_Aventuriers.Count > 1)
                            Console.WriteLine($"Ne parvenant pas à suffisamment affaiblir {p_Monstres.Keys.ElementAt(p_general).Trim()}, les aventuriers ont fini par succomber à la fatigue.");
                        else
                            Console.WriteLine($"Ne parvenant pas à suffisamment affaiblir {p_Monstres.Keys.ElementAt(p_general).Trim()}, {p_Aventuriers.Keys.ElementAt(0)} à fini par succomber à la fatigue.");

                        p_donjon = false;   //Défaite
                        break;
                    }
                    Console.WriteLine();
                }
                break;
            }
        }

        /// <summary>
        /// Sous section pour l'attaque du monstre.
        /// Par defaut le monstre ne se fatigue jamais.
        /// </summary>
        /// <param name="p_donjon"> déclenche la victoire ou la défaite </param>
        /// <param name="p_Aventuriers"> dictionnaire aventuriers </param>
        /// <param name="p_Monstres"> dictionnaire monstres </param>
        /// <param name="p_general"> compteur général en rapport avec le nombre de monstre saisie </param>
        /// <param name="p_mort"> se déclenche si un aventurier tombe au combat </param>
        /// <param name="p_defunt"> liste pour les aventuriers tombés au combat </param>
        void AttaqueMonstre(ref Dictionary<string, (int, int, int, int, int)> p_Aventuriers, Dictionary<string, (int, int, int, int)> p_Monstres, ref bool p_donjon, int p_general, ref bool p_mort, ref List<string> p_defunt)
        {
            Random rnd = new Random();

            int indice = rnd.Next(0, p_Aventuriers.Count);       //Détermine aléatoirement qui le monstre attaque.
            var aventurier = p_Aventuriers.ElementAt(indice);    //Sélection de l'aventurier attaqué.
            int degatMonstre = p_Monstres.Values.ElementAt(p_general).Item2 - aventurier.Value.Item4;   //Calcul des dégâts du monstre.

            if (degatMonstre > 0)   //Si le monstre fait des dégâts
            {
                int nouveauPVAventurier = aventurier.Value.Item1 - degatMonstre;    //Calcul des nouveaux pv de l'aventurier attaqué.
                p_Aventuriers[aventurier.Key] = (nouveauPVAventurier, aventurier.Value.Item2, aventurier.Value.Item3, aventurier.Value.Item4, aventurier.Value.Item5);  //Affectation des nouveaux pv.

                Console.WriteLine($"{p_Monstres.Keys.ElementAt(p_general).Trim()} attaque {aventurier.Key} et fait {degatMonstre} dégâts. (Pv restants : {nouveauPVAventurier})");

                if (nouveauPVAventurier <= 0)   //Si les pv de l'aventurier tombe à 0.
                {
                    p_defunt.Add(aventurier.Key);   //On ajoute l'aventurier décédé à la liste de defunt.
                    p_Aventuriers.Remove(aventurier.Key);   //Suppression de l'aventurier du dictionnaire.
                    Console.WriteLine($"***** {aventurier.Key} est mort ! *****");
                    p_mort = true;  //Activation du booléen pour les aventuriers morts au combat.
                    
                    if (p_Aventuriers.Count == 0)   //Si il n'y a plus d'aventuriers.
                        p_donjon = false;   //Défaite.
                }
            }

            else    //Si le monstre ne fait pas de dégâts.
                Console.WriteLine($"{p_Monstres.Keys.ElementAt(p_general).Trim()} attaque {aventurier.Key} et manque son coup.");
        }

        /// <summary>
        /// Sous section pour l'attaque des aventuriers.
        /// Je n'ai pas affiché la fatigue, car cela n'apporte pas de réel plus-value et gène la lecture plus qu'autre chose.
        /// De plus le parametre fatigue est surtout présent pour pallier aux boucles infinies.
        /// </summary>
        /// <param name="p_Aventuriers"> dictionnaire aventuriers </param>
        /// <param name="p_Monstres"> dictionnaire monstres </param>
        /// <param name="p_general"> compteur général en rapport avec le nombre de monstre saisie </param>
        /// <param name="p_attaqueGroupe"> valeur d'attaque du groupe </param>
        /// <param name="p_combat"> se déclenche le combat est remporté </param>
        /// <param name="p_fatigueGroupe"> valeur de fatigue du groupe </param>
        void AttaqueAventuriers(Dictionary<string, (int, int, int, int, int)> p_Aventuriers, ref Dictionary<string, (int, int, int, int)> p_Monstres, int p_general, int p_attaqueGroupe, out bool p_combat, out int p_fatigueGroupe)
        {
            p_combat = true;    //Combat en cours
            int degatAventuriers = p_attaqueGroupe - p_Monstres.Values.ElementAt(p_general).Item3;  //Calcul des dégâts
                
            foreach (var aventurier in p_Aventuriers)   //- 1 de fatigue par aventurier à chaque tour.
            {
                int nouvelleFatigue = aventurier.Value.Item2 - 1;   //Calcul de la nouvelle fatigue pour chaque aventurier.
                p_Aventuriers[aventurier.Key] = (aventurier.Value.Item1, nouvelleFatigue, aventurier.Value.Item3, aventurier.Value.Item4, aventurier.Value.Item5);  //Actualisation des valeurs dans le dictionnaire.
            }

            p_fatigueGroupe = p_Aventuriers.Values.Sum(aventurier => aventurier.Item2); //Calcul de la nouvelle fatigue du groupe.            

            if (degatAventuriers > 0)   //Si les aventuriers font des dégâts
            {
                int nouveauPVMonstre = p_Monstres.Values.ElementAt(p_general).Item1 - degatAventuriers; //Calcul des nouveaux pv du monstre.
                p_Monstres[p_Monstres.Keys.ElementAt(p_general)] = (nouveauPVMonstre, p_Monstres.Values.ElementAt(p_general).Item2, p_Monstres.Values.ElementAt(p_general).Item3, p_Monstres.Values.ElementAt(p_general).Item4);    //Affectation des nouveaux pv

                //Deux messages en fonction du nombre d'aventuriers restant
                if (p_Aventuriers.Count > 1)    
                    Console.WriteLine($"Les aventuriers attaquent {p_Monstres.Keys.ElementAt(p_general).Trim()} et font {degatAventuriers} dégâts. (Pv restants : {nouveauPVMonstre})");
                else
                    Console.WriteLine($"{p_Aventuriers.Keys.ElementAt(0)} attaque {p_Monstres.Keys.ElementAt(p_general).Trim()} et fait {degatAventuriers} dégâts. (Pv restants : {nouveauPVMonstre})");              

                if (nouveauPVMonstre <= 0)  //Si les pv du monstre tombe à 0
                {
                    Console.WriteLine();
                    Console.WriteLine($"{p_Monstres.Keys.ElementAt(p_general).Trim()} à ete vaincu !");
                    Console.WriteLine();
                    p_combat = false;    //Fin du combat
                }   
            }
            //si les aventuriers ne font pas de dégâts
            else
            {
                //Deux messages en fonction du nombre d'aventuriers restant
                if (p_Aventuriers.Count > 1)    
                    Console.WriteLine($"Les aventuriers tentent d'attaquer {p_Monstres.Keys.ElementAt(p_general).Trim()} et manquent leurs coups.");
                else
                    Console.WriteLine($"{p_Aventuriers.Keys.ElementAt(0)} tente d'attaquer {p_Monstres.Keys.ElementAt(p_general).Trim()} et manque son coup.");
            }
        }

        /// <summary>
        /// Sous section qui actualise les valeurs d'initiative et d'attaque avant chaque attaque d'aventuriers.
        /// </summary>
        /// <param name="p_Aventuriers"> dictionnaire aventuriers </param>
        /// <param name="p_attaqueGroupe"> valeur d'attaque du groupe </param>
        /// <param name="p_initiativeGroupe"> valeur d'initiative du groupe </param>
        void MiseAJourAventuriers(Dictionary<string, (int, int, int, int, int)> p_Aventuriers, out int p_attaqueGroupe, out int p_initiativeGroupe)
        {
            int attaqueGroupe = 0;  
            int initiativeGroupe = 0;

            foreach ((int pv, int fatigue, int attaque, int defense, int initiative) in p_Aventuriers.Values)
            {
                attaqueGroupe += attaque;
                initiativeGroupe += initiative;
            }

            p_attaqueGroupe = attaqueGroupe;
            p_initiativeGroupe = initiativeGroupe;
        }

        /// <summary>
        ///Après chaque victoire les aventuriers se reposent et récupèrent 500 pv et 100 fatigue chacun.
        /// </summary>
        /// <param name="p_donjon"> déclenche la victoire ou la défaite </param>
        /// <param name="p_Aventuriers"> dictionnaire aventuriers </param>
        /// <param name="p_nbMonstre"> nombre de monstre </param>
        /// <param name="p_general"> compteur general en rapport avec le nombre de monstre saisie </param>
        /// <param name="p_mort"> se déclenche si un aventurier tombe au combat </param>
        /// <param name="p_defunt"> liste pour les aventuriers tombés au combat </param>
        void Recuperation(ref Dictionary<string, (int, int, int, int, int)> p_Aventuriers, int p_nbMonstre, bool p_donjon, int p_general, bool p_mort, List<string> p_defunt)
        {
            while (p_donjon == true && p_nbMonstre > (p_general + 1))
            {   
                if (p_Aventuriers.Count > 1)               
                        Console.WriteLine("Après avoir triomphé d'un monstre terrifiant, les aventuriers prennent un moment de repos bien mérité\n" +
                            "pour recouvrer leurs forces tout en partageant leur joie et leur soulagement d'être encore en vie.\n" +
                            "(Tous les points de vie et de fatigue sont restaurés)");               

                else
                        Console.WriteLine($"Après avoir triomphé d'un monstre terrifiant, {p_Aventuriers.Keys.ElementAt(0)} prend un moment de repos bien mérité\n" +
                            $"pour recouvrer ses forces tout en etant soulagé d'être encore en vie.\n" +
                            $"(Tous les points de vie et de fatigue sont restaurés)");
                
                foreach (var aventurier in p_Aventuriers)   //Chaque aventuriers récupèrent 500 pv et 100 points de fatigue.
                    p_Aventuriers[aventurier.Key] = (Potion, Booster, aventurier.Value.Item3, aventurier.Value.Item4, aventurier.Value.Item5);
                
                Console.WriteLine();
                Console.WriteLine("          |\r\n()########|o=======================================================>\r\n          |");

                if (p_mort == true) //Si il y a eu des morts.
                {
                    foreach (string mort in p_defunt)   //On affiche les morts.
                        Console.WriteLine($"{null,14}{mort} nous a quitté... Paix à son âme !");
                   
                    Console.WriteLine();
                    Console.WriteLine($"Il reste {p_Aventuriers.Count} aventurier(s) encore en jeu.");  //On indique combien il reste d'aventuriers en jeu.
                }
                break;
            }
        }

        /// <summary>
        ///Affiche les messages de fin.
        /// </summary>
        /// <param name="p_donjon"> déclenche la victoire ou la défaite </param>
        void Fin(bool p_donjon)
        {
            if (p_donjon == true)
            {
                Console.WriteLine();
                Console.WriteLine("Félicitations, vous avez nettoyé le donjon !");
                Console.WriteLine();
                Console.WriteLine("                           .-.\r\n                          {{@}}\r\n          <>               8@8\r\n        .::::.             888\r\n    @\\\\/W\\/\\/W\\//@         8@8\r\n     \\\\/^\\/\\/^\\//     _    )8(    _\r\n      \\_O_<>_O_/     (@)__/8@8\\__(@)\r\n ____________________ `~\"-=):(=-\"~`\r\n|<><><>  |  |  <><><>|     |.|\r\n|<>      |  |      <>|     |S|\r\n|<>      |  |      <>|     |'|\r\n|<>   .--------.   <>|     |.|\r\n|     |   ()   |     |     |P|\r\n|_____| (O\\/O) |_____|     |'|\r\n|     \\   /\\   /     |     |.|\r\n|------\\  \\/  /------|     |U|\r\n|       '.__.'       |     |'|\r\n|        |  |        |     |.|\r\n:        |  |        :     |N|\r\n \\<>     |  |     <>/      |'|\r\n  \\<>    |  |    <>/       |.|\r\n   \\<>   |  |   <>/        |K|\r\n    `\\<> |  | <>/'         |'|\r\n      `-.|  |.-`           \\ /\r\n         '--'               ^");
                Console.WriteLine();
                Console.WriteLine($"Louange à ces courageux aventuriers qui ont triomphé des dangers les plus redoutables en explorant" +
                    $"\n{null,22}les recoins les plus obscurs d'un donjon hostile." +
                    $"\n{null,8}Leur bravoure, leur habileté et leur détermination ont été les clés de leur succès," +
                    $"\n{null,13}alors qu'ils affrontaient des monstres terrifiants et des pièges mortels." +
                    $"\n{null,3}Le nom de ces héros restera à jamais dans nos souvenirs en tant que symbole de la victoire.");
                Console.WriteLine();
                Console.WriteLine();
            }

            else
            {
                Console.WriteLine();
                Console.WriteLine("Après s'être courageusement battu tous les aventuriers ont péri !");
                Console.WriteLine();
                Console.WriteLine("                  _  /)\r\n                < o / )\r\n                 |/)\\)\r\n                  /\\_\r\n                  \\__|=-\r\n                 (    )\r\n                 __)(__\r\n           _____/      \\\\_____\r\n          |                  ||\r\n          |  _     ___   _   ||\r\n          | | \\     |   | \\  ||\r\n          | |  |    |   |  | ||\r\n          | |_/     |   |_/  ||\r\n          | | \\     |   |    ||\r\n          | |  \\    |   |    ||\r\n          | |   \\. _|_. | .  ||\r\n          |                  ||\r\n  *       | *   **    * **   |**      **\r\n   \\))/\\\\(/.,(//,,..,,\\||(,,.,\\\\,.((//");
                Console.WriteLine();
                Console.WriteLine($"{null,8}Ci-gît les braves aventuriers qui ont pénétré les profondeurs du donjon," +
                    $"\n{null,16}Leur soif de trésors les a menés à leur funeste destin." +
                    $"\n{null,4}Ils ont affronté les monstres les plus redoutables, les pièges les plus perfides," +
                    $"\n{null,7}Mais la mort les a finalement rattrapés, dans l'obscurité des souterrains." +
                    $"\n{null,1}Ils ont été vaincus, mais leur courage et leur détermination resteront à jamais gravés," +
                    $"\n{null,1}Dans les mémoires de ceux qui les ont connus, et dans la légende des héros de la quête.");
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
