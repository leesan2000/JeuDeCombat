using System;
using System.Collections.Generic;
using System.Linq;




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
        this.type = type;
        this.pv = pv;
        this.force = force;
    }

    public void getDamaged(int damage){
        pv = pv - damage;
    }

}

class Damager : Personaje
{
    public int specialAttack { get; set; }
    private int damageReflected { get; set; }    

    public Damager(int type, int pv, int force, int special) : base(type, pv, force)
    {
        specialAttack = special;
        damageReflected = 0;

    }


    public new void Attack(Personaje enemy, int damage)
    {
        //base.Attack(enemy, damage);
        //enemy.pv -= damage;
        
        damageReflected += damage;
    }



    public void Defend(Personaje enemy, int pv){

    }


partial class Program
{
    static void Main()
    {
        Program program = new Program();
        program.Interface();
    }

    public List<(int x, int y)> buttonsPositions = new List<(int x, int y)>();
    public (int x, int y) cursorPosition = (0, 2);
    public int characterChoice = -1;

    void Interface()
    {
        // Boucle Menu principale
        while(characterChoice == -1)
        {
            Console.Clear();
            buttonsPositions.Clear();

            Console.WriteLine("################################");
            Console.WriteLine("#         JEU DE COMBAT        #");
            Console.WriteLine("#              v0.1            #");
            Console.WriteLine("################################");
            Console.WriteLine("\nVeuillez choisiez votre classe:");
            Console.WriteLine("         " + Button((0,2), "Damager"));
            Console.WriteLine("         " + Button((0,1), "Healer"));
            Console.WriteLine("         " + Button((0,0), "Tank"));

            if (WaitForInput())
                characterChoice = Math.Abs(cursorPosition.y - 3);
        }

        // characterChoice devrais correspondre a 1, 2 ou 3 (Damager, Healer ou tank)
        Console.WriteLine(characterChoice);
    }

    string Button((int x, int y) position, string label)
    {
        string labelDisplay = "";

        // Check if the button is selected
        bool selected = position == cursorPosition;
        labelDisplay += selected ? ">" : " ";
        labelDisplay += label;
        labelDisplay += selected ? "<" : " ";

        // Register it and return the display string 
        buttonsPositions.Add(position);
        return labelDisplay;
    }

    // Change the position of the cursor on the interface
    // Return true if pressed enter
    public bool WaitForInput()
    {
        (int x, int y) nextPos = cursorPosition;
        string input = Console.ReadKey().Key.ToString();

        switch (input)
        {
            case "RightArrow":
                nextPos.x++;
                break;
            case "LeftArrow":
                nextPos.x--;
                break;
            case "UpArrow":
                nextPos.y++;
                break;
            case "DownArrow":
                nextPos.y--;
                break;
            case "Spacebar":
                return true;
        }
        
        // Check if next cursor position is in bound
        if (buttonsPositions.Contains(nextPos))
            cursorPosition = nextPos;

        return false;
    }
}
}