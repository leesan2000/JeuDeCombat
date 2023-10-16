using System;
using System.Collections.Generic;

<<<<<<< HEAD
=======



/*
Type :

1 : Damager (3 2)
2 : Healer (4 1)
3 : Tank (5 1)

*/

class Personaje{
    public int type;
    public int pv;
    public int force;
    
    public Personaje(int type, int pv, int force)
    {
        type = type;
        pv = pv;
        force = force;
    }
}

class Damager : Personaje
{
    public int specialAttack { get; set; }
    private int degatsEnRetour { get; set; }    

    public Damager(int type, int pv, int force, int special) : base(type, pv, force)
    {
        specialAttack = special;
        degatsEnRetour = 0;

    }
}










>>>>>>> 465a80bd6bb456e2d3669d387467bfcce1678001
partial class Program
{
    int characterChoice = 0;

    static void Main()
    {

        int cursorLocation = 0;

        // Boucle Menu principale
        while(characterChoice == -1)
        {
            Console.WriteLine("################################");
            Console.WriteLine("#         JEU DE COMBAT        #");
            Console.WriteLine("#              v0.1            #");
            Console.WriteLine("################################");
            Console.WriteLine("\nVeuillez choisiez votre classe:");
            Console.WriteLine("1 - Healer");
            Console.WriteLine("2 - Tank");
            Console.WriteLine("3 - Damager");
            Console.Clear();
        }
    }
}




public class Damager{

    int vie = 2;
    int att = 3;

    int special;


    public void DamagerA()
    {







    }








}