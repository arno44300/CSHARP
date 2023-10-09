using System;

namespace Utilitaires;

public class Util
{
    //INT32
    public static int LireInt32(string p_question)
    {
        int entier;

        for (; ; )
        {
            Console.Write(p_question);
            if (Int32.TryParse(Console.ReadLine(), out entier)) break;
            Console.WriteLine("Veuillez entrer un nombre valide.");
        }
        return entier;
    }
    public static int LireInt32Positif(string p_question)
    {
        int entier;

        for (; ; )
        {
            entier = LireInt32(p_question);
            if (entier >= 0) break;
            Console.WriteLine("Veuillez entrer un nombre superieur ou egal a 0.");
        }
        return entier;
    }
    public static int LireInt32AvecMinimum(string p_question, int p_minimum)
    {
        int entier;

        for (; ; )
        {
            entier = LireInt32(p_question);
            if (entier >= p_minimum) break;
            Console.WriteLine($"Veuillez entrer un nombre superieur ou égal à {p_minimum}.");
        }
        return entier;
    }

    public static int LireInt32AvecIntervalle(string p_question, int p_minimum, int p_maximum)
    {
        int entier;

        for (; ; )
        {
            entier = LireInt32(p_question);
            if (entier >= p_minimum && entier <= p_maximum) break;
            Console.WriteLine($"Veuillez entrer un nombre entre {p_minimum} et {p_maximum}.");
        }
        return entier;
    }

    public static int LireInt32AvecIntervalleOuSentinelle(string p_question, int p_minimum, int p_maximum, int p_sentinelle)
    {
        int entier;

        for (; ; )
        {
            entier = LireInt32(p_question);
            if ((entier >= p_minimum && entier <= p_maximum) || entier == p_sentinelle) break;
            Console.WriteLine($"Veuillez entrer un nombre entre {p_minimum} et {p_maximum}.");
        }
        return entier;

    }
    public static int LireInt32AvecMinimumOuSentinelle(string p_question, int p_minimum, int p_sentinelle)
    {
        int entier;

        for (; ; )
        {
            entier = LireInt32(p_question);
            if (entier >= p_minimum || entier == p_sentinelle) break;
            Console.WriteLine($"Veuillez entrer un nombre plus grand ou égal à {p_minimum}.");
        }
        return entier;

    }

    //DOUBLE
    public static double LireDouble(string p_question)
    {
        double reel;

        for (; ; )
        {
            Console.Write(p_question);
            if (Double.TryParse(Console.ReadLine(), out reel)) break;
            Console.WriteLine("Veuillez entrer un nombre valide.");
        }
        return reel;
    }

    public static double LireDoublePositif(string p_question)
    {
        double reel;

        for (; ; )
        {
            reel = LireDouble(p_question);
            if (reel >= 0) break;
            Console.WriteLine("Veuillez entrer un nombre positif.");
        }
        return reel;
    }

    public static double LireDoubleAvecMinimum(string p_question, int p_minimum)
    {
        double reel;

        for (; ; )
        {
            reel = LireDouble(p_question);
            if (reel >= p_minimum) break;
            Console.WriteLine($"Veuillez entrer un nombre plus grand ou égal à {p_minimum}.");
        }
        return reel;
    }

    public static double LireDoubleAvecMinimumOuSentinelle(string p_question, double p_minimum, double p_sentinelle)
    {
        double reel;

        for (; ; )
        {
            reel = LireDouble(p_question);
            if (reel >= p_minimum || reel == p_sentinelle) break;
            Console.WriteLine($"Veuillez entrer un nombre plus grand ou égal à {p_minimum}.");
        }
        return reel;

    }

    public static double LireDoubleAvecIntervalle(string p_question, int p_minimum, int p_maximum)
    {
        double reel;

        for (; ; )
        {
            reel = LireDouble(p_question);
            if (reel >= p_minimum && reel <= p_maximum) break;
            Console.WriteLine($"Veuillez entrer un nombre entre {p_minimum} et {p_maximum} .");
        }
        return reel;
    }

    public static double LireDoubleAvecIntervalleOuSentinelle(string p_question, double p_minimum, double p_maximum, double p_sentinelle)
    {
        double reel;

        for (; ; )
        {
            reel = LireDouble(p_question);
            if ((reel >= p_minimum && reel <= p_maximum) || reel == p_sentinelle) break;
            Console.WriteLine($"Veuillez entrer un nombre entre {p_minimum} et {p_maximum} .");
        }
        return reel;

    }

    //STRING
    public static string LireStringTailleControlee(string p_question, int p_lgMin, int p_lgMax)
    {
        string texte;  
        for (;;)    
        {     
        Console.Write(p_question);
        texte = Console.ReadLine().Trim();
        if (p_lgMin <= texte.Length && texte.Length <= p_lgMax) break;
        Console.WriteLine($"Veuillez entrer un texte entre {p_lgMin} et {p_lgMax} caractères.");    
        }
        return texte;
    }

    public static string LireStringTailleDesiree(string p_question, int p_lg)
    {
        string texte;
        for (; ; )
        {
            Console.Write(p_question);
            texte = Console.ReadLine().Trim();
            if (p_lg == texte.Length) break;
            Console.WriteLine($"Veuillez entrer un texte de {p_lg} caractères.");
        }
        return texte;
    }
    public static string LireStringNonVide(string p_question)
    {
        string texte;
        for (; ; )
        {
            Console.Write(p_question);
            texte = Console.ReadLine().Trim();
            if (texte.Length > 0) break;
            Console.WriteLine("Veuillez entrer un texte.");
        }
        return texte;
    }

    //CHAR
        public static char LireChar(string p_question)
    {
        char caractere;

        for (; ; )
        {
            Console.Write(p_question);

            if (Char.TryParse(Console.ReadLine(), out caractere) && !Char.IsControl(caractere)) break;
            Console.WriteLine("Veuillez entrer un seul caractère.");
        }
        return caractere;
    }

    public static char LireCharOuiNon(string p_question)
    {
        char caractere;

        for (; ; )
        {
            Console.Write(p_question);

            if (Char.TryParse(Console.ReadLine().ToUpper(), out caractere) && !Char.IsControl(caractere) && (caractere == 'N' || caractere == 'O')) break;
            Console.WriteLine("Veuillez entrer seulement \"O\" pour oui ou \"N\" pour non.");
        }
        return caractere;
    }
    //BOOL
    public static bool ConfirmerOuiNon(string p_question)
    {
        bool binaire;
        char reponse = LireCharOuiNon(p_question);

        if (reponse == 'O')
            binaire = true;
        else
            binaire = false;

        return binaire;

    }

    public static bool BoolMinimum(string p_question, int p_minimum)
    {
        bool binaire = false;
        int reponse = LireInt32AvecMinimum(p_question,0);
        if (reponse >= p_minimum)
            binaire = true;
        
        return binaire;
    }

    public static bool ValiderHeure(string p_heureAppel)
    {
        if (p_heureAppel.Length == 4)
        {
            try 
            { 
            int p_heureAppelValide = Convert.ToInt32(p_heureAppel);
            
            int heure = p_heureAppelValide / 100;
            int minute = p_heureAppelValide % 100;

            if (heure >= 0 && heure <= 23 && minute >= 0 && minute <= 59)
                return true;
            else Console.WriteLine("Veuillez entrer une heure valide. HH pour l'heure et MM pour les minutes.");
            }
            catch
            {
                Console.WriteLine("Veuillez entrer uniquement des chiffres.");
                return false; 
            }
        }
        else Console.WriteLine("L'heure doit comprendre 4 chiffres au format HHMM.");
        return false;
    }

    public static bool ValiderDate(string p_date)
    {
        DateTime today = DateTime.Now;
        if (p_date.Length == 8)
        {
            try
            {
                int p_dateValide = Convert.ToInt32(p_date);

                int annee = p_dateValide / 10000;
                int obtenirMois = p_dateValide % 10000;
                int mois = obtenirMois / 100;
                int jour = p_dateValide % 100;

                if (annee <= today.Year)
                {
                    if (mois >= 1 && mois <= 12)
                    {
                        switch (mois)
                        {
                            case 1:
                            case 3:
                            case 5:
                            case 7:
                            case 8:
                            case 10:
                            case 12:
                                if (jour >= 1 && jour <= 31) return true;
                                else Console.WriteLine("Le jour doit etre compris entre 1 et 31"); return false;

                            case 2:
                                if (jour >= 1 && jour <= 28) return true;
                                else Console.WriteLine("Le jour doit etre compris entre 1 et 28"); return false;

                            case 4:
                            case 6:
                            case 9:
                            case 11:
                                if (jour >= 1 && jour <= 30) return true;
                                else Console.WriteLine("Le jour doit etre compris entre 1 et 30"); return false;

                            default: return false;
                        }
                    }
                    else Console.WriteLine("Le mois doit etre compris entre 1 et 12."); return false;
                }
                else Console.WriteLine("L'annee ne doit pas etre superieure a l'annee en cours"); return false;
            }
            catch
            {
                Console.WriteLine("Veuillez entrer uniquement des chiffres.");
                return false;
            }
        }
        else Console.WriteLine("La date doit comprendre 8 caracteres. AAAA pour l'annee, MM pour le mois et JJ pour le jour."); return false;       
    }
}
