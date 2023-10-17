using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public virtual void special(Personaje e, int d){}


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

    public override void special(Personaje enemy, int damageReceive)
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

    public override void special(Personaje enemy, int d) //Parameters here are not used
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

    public override void special(Personaje enemy, int force)
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

    public bool endGame;
    public bool deadPlayer;
    public bool deadCPU;

    Personaje player = null;
    Personaje enemy = null;

    int combatAction = 0;

    int cpuChoice = 0;
    string cpuString = "";
    string playerString = "";

    void Interface()
    {
        while (true)
        {
            Console.Clear();
            buttonsPositions.Clear();

            Console.WriteLine("+------------------------------+");
            Console.WriteLine("|         JEU DE COMBAT        |");
            Console.WriteLine("+------------------------------+");
            Console.WriteLine("\nUtilisez les flèche directionnelle \npour vous déplacez entre les options.");
            Console.WriteLine("Appuyiez sur [Espace] ou [Entrer] \npour selectionner un bouton.");
            Console.WriteLine("\n             >OK<");

            if (WaitForInput())
                break;
        }

        // Boucle Menu principale
        IAChoice(cpuChoice);
        while(characterChoice == -1)
        {
            Console.Clear();
            buttonsPositions.Clear();

            Console.WriteLine("+------------------------------+");
            Console.WriteLine("|          PERSONNAGES         |");
            Console.WriteLine("+------------------------------+");
            Console.WriteLine("\nVeuillez choisiez votre classe:");
            Console.WriteLine("          " + Button((0,2), "Damager"));
            Console.WriteLine("          " + Button((0,1), "Healer"));
            Console.WriteLine("          " + Button((0,0), "Tank"));

            if (WaitForInput())
                characterChoice = Math.Abs(cursorPosition.y - 3);
        }



        // Combat
        
        switch (characterChoice)
        {
        case 1:
            player = new Damager();
            playerString = "Damager";
            break;
        case 2:
            player = new Healer();
            playerString = "Healer";
            
            break;
        case 3:
            player = new Tank();
            playerString = "Tank";
            break;
        }
        
        switch(IAChoice(cpuChoice)){
            case 1:
                enemy = new Damager();
                cpuString = "Damager";
                break;
            case 2:
                enemy = new Healer();
                cpuString = "Healer";
                break;
            case 3: 
                enemy = new Tank();
                cpuString = "Tank";
                break;
        }

        cursorPosition = (0, 0);
        while(!endGame){

            while(!deadCPU || !deadPlayer)
            {
                int action = -1;
                while (action == -1)
                {

                    Console.Clear();
                    buttonsPositions.Clear();

                    Console.WriteLine("Joueur : {0}", playerString);
                    Console.WriteLine(player.pv + " HP  | " + Gauge(player.pv, 10));
                    Console.WriteLine(player.force + " DMG | " + Gauge(player.force, 10));
                    Console.WriteLine("\nEnemie : {0}", cpuString);
                    if(enemy.pv > 0){ //If the enemy isn't dead
                        Console.WriteLine(enemy.pv + " HP  | " + Gauge(enemy.pv, 10));
                        Console.WriteLine(enemy.force + " DMG | " + Gauge(enemy.force, 10));
                    }else{
                        if(enemy.pv <= 0){ //If the enemy dies (HP = 0)
                            Console.WriteLine("Enemy is dead!");
                            deadCPU = true;
                            endGame = true;
                        }
                    }
                    Console.WriteLine("\n          Actions possibles:");
                    Console.WriteLine(" " + Button((0, 0), "Attaquer") + " "  + Button((1, 0), "Défendre") + " "  + Button((2, 0), "Action spécial", true));
                    
                    if (WaitForInput())
                        action = cursorPosition.x + 1;


                    // Attack
                    if(action == 1)
                    {
                        enemy.getDamaged(player.force);      
                    }

                    // Defend
                    else if (action == 2)
                    {
                        player.Defend();
                    }

                    // Special attack
                    else if (action == 3)
                    {
                        player.special(enemy, player.force);
                    }

                    IATurn();
                }
            }
        }
    }

            
        
    


    string Button((int x, int y) position, string label, bool disabled=false)
    {
        string labelDisplay = "";

        // Check if the button is selected
        bool selected = position == cursorPosition;
        labelDisplay += selected ? disabled ? "X" : ">" : " ";
        labelDisplay += label;
        labelDisplay += selected ? disabled ? "X" : "<" : " ";

        // Register it and return the display string 
        buttonsPositions.Add(position);
        return labelDisplay;
    }

    string Gauge(int value, int maxSize)
    {
        maxSize *= 2;
        string gaugeDisplay = "";

        // Add blocs
        for (int i = 1; i < maxSize; i++)
        {
            // Overflow
            if (i <= value && i == maxSize - 2)
            {
                gaugeDisplay += "+" + (value - (maxSize - 2));
                break;
            }

            // Normal bloc
            else if (i < value)
                gaugeDisplay += "█";

            // Even number
            else if (i == value)
                gaugeDisplay += value % 2 == 0 ? "▒" : "█";
            
            // Empty
            else
                gaugeDisplay += " ";
        }
        
        return gaugeDisplay;
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
            case "Enter":
                return true;
        }
        
        // Check if next cursor position is in bound
        if (buttonsPositions.Contains(nextPos))
            cursorPosition = nextPos;

        return false;
    }

    public int IAChoice(int choice){
        Random rdm = new Random();
        choice = rdm.Next(1,4);
        return choice;
        
    }

    public void IATurn()
    {

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