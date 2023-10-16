using System;
using System.Collections.Generic;






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










partial class Program
{

    int choix;
    static void Main()
    {


        Console.WriteLine("Best Combat Game Ever Made");
        Console.WriteLine("Bienvenue");
        Console.WriteLine("Veuillez choisir le personnage:");
        Console.WriteLine("1 -  Healer");
        Console.WriteLine("2 - Tank");
        Console.WriteLine("3 - Damager");

        int choix = int.Parse(Console.ReadLine());

        if(choix == 1){
            Damager dam = new Damager(1, 3, 2, 0); 
        }
    }
}







