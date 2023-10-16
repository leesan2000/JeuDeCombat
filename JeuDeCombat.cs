using System;
using System.Collections.Generic;

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