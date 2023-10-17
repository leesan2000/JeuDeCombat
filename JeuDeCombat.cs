using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualBasic.FileIO;




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

    private bool isDefending = false;
    
    public Personaje(int type, int pv, int force)
    {
        this.type = type;
        this.pv = pv;
        this.force = force;
    }

    public void getDamaged(int damage){
        this.pv -= damage;
    }

    public void Attack(Personaje enemy)
    {
        if (!isDefending)
        {
        enemy.pv -= this.force;
        }
        else
        {
            this.isDefending = false;
        }
    }

    public void Defend()
    {
        this.isDefending = true;
    }

}

class Damager : Personaje
{
    public int DamageReflected { get; set; } 

    protected static int _type  = 1;
    protected static int _pv = 3;
    protected static int _force = 2;  

    public Damager() : base(_type, _pv, _force)
    {
        DamageReflected = 0;
    }

    public void special(Personaje enemy, int damageReceive)
    {
        base.getDamaged(damageReceive);
        enemy.getDamaged(damageReceive);
    }
}

class Healer : Personaje
{
    protected static int _type  = 2;
    protected static int _pv = 4;
    protected static int _force = 1;  

    public Healer() : base(_type, _pv, _force) {}

    public void special()
    {
        base.getDamaged(-2);
    }
}
class Tank : Personaje
{
    protected static int _type  = 3;
    protected static int _pv = 5;
    protected static int _force = 1;  

    public Tank() : base(_type, _pv, _force) {}

    public void special(Personaje enemy, int force)
    {
        base.getDamaged(1);
        enemy.getDamaged(1);
        enemy.getDamaged(1);
    }
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

        // Combat
        
        Personaje player;
        switch (characterChoice)
        {
            case 1:
                player = new Damager();
                break;
            case 2:
                player = new Healer();
                break;
            case 3:
                player = new Tank();
                break;
            default:
                player = new Tank();
                break;
        }

        Random rdm = new Random();
        
        int enemyChoice = rdm.Next(1, 4);
        Personaje ia;

        switch (enemyChoice)
        {
            case 1:
                ia = new Damager();
                break;
            case 2:
                ia = new Healer();
                break;
            case 3:
                ia = new Tank();
                break;
            default:
                ia = new Tank();
                break;
        }

        while (ia.pv > 0 && player.pv > 0)
        {
            Console.WriteLine(ia.pv + " / " + player.pv);
            break;
        }
        
       //

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


/*
ia :

rdm:  (((((

Random rdm = new Random();
action = rdm.Next(1,4);

if (action == 3)
{
    defense;
}
else
{
    attack;
}


(%+ pour attack)

)))))

damager:

if (vie = 1)
{
    special;
}
else
{
    rdm;
}


healer:

if (vie <= 2)
{
    heal;
}
else
{
    rdm;
}

tank:

if (vie >= 3)
{
    special
}
else
{
    rdm;
}

*/