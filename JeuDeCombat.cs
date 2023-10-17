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
    public int pv = 10;
    public int force = 0;

    private bool isDefending = false;
    
    public Personaje(int type, int pv, int force)
    {
        this.type = type;
        this.pv = pv;
        this.force = force;
    }

    public void getDamaged(int damage){
        if (!isDefending)
        {
        this.pv -= damage;
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
        if (base.pv > 4)
        {
            base.pv = 4;
        }
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

    Personaje? player = null;
    Personaje? enemy = null;

    int combatAction = 0;

    int cpuChoice = 0;
    string cpuString = "";
    string playerString = "";
    string iaLastAction = "Ready to fight !";
    public float iaDifficulty = 50f;

    public int playerCooldownSpecial = 0;
    public int enemyCooldownSpecial = 0;

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
            Console.WriteLine("\nAppuyiez sur [Espace] ou [Entrer] \npour selectionner un bouton.");
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
            Console.WriteLine("\nVeuillez choisir votre classe:");
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
            default:
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
            default:
                enemy = new Tank();
                cpuString = "Tank";
                break;
        }

        cursorPosition = (0, 0);

        // Fight loop
        while(!endGame)
        {
            while(!deadCPU && !deadPlayer)
            {
                if (Convert.ToBoolean(playerCooldownSpecial))
                    playerCooldownSpecial--;
                if (Convert.ToBoolean(enemyCooldownSpecial))
                    enemyCooldownSpecial--;

                int action = -1;
                while (action == -1)
                {
                    DrawGame(0);
                    
                    if (WaitForInput())
                        action = cursorPosition.x + 1;

                    // Attack
                    if(action == 1)
                        enemy.getDamaged(player.force);      

                    // Defend
                    else if (action == 2)
                        player.Defend();

                    // Special attack
                    else if (action == 3) // TODO : Add cooldown check in this logic
                    if (!Convert.ToBoolean(playerCooldownSpecial))
                    {
                        player.special(enemy, player.force);
                        playerCooldownSpecial = 2;
                    }
                    else
                    {
                        action = -1;
                        Console.WriteLine("Special unavailable.");
                    }
                }

                if (TestGameOver())
                    break;

                IATurn();
                if (TestGameOver())
                    break;
            }
            
            while (true)
            {
                Console.Clear();
                buttonsPositions.Clear();

                Console.WriteLine("+------------------------------+");
                Console.WriteLine("|           GAME OVER          |");
                Console.WriteLine("+------------------------------+");
                Console.WriteLine("\n        {0}", deadPlayer ? "Vous êtes mort !" : "Vous avez gagné !");
                Console.WriteLine("\n   " + Button((0,0), "Menu principal") + " " + Button((1,0), "Quitter") );

                if (WaitForInput())
                {
                    characterChoice = -1;
                    endGame = true;
                    deadCPU = false;
                    deadPlayer = false;
                    cursorPosition = (0,0);

                    if (cursorPosition.x == 0)
                        Interface();
                    
                    break;
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
    bool WaitForInput()
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

    void DrawGame(int turn)
    {
        // Possiblly null reference check
        if(player == null)
            player = new Tank();
        if (enemy == null)
            enemy = new Tank();
        //

        Console.Clear();
        buttonsPositions.Clear();

        Console.WriteLine(turn == 0 ? ">> Joueur : {0} <<" : "   Joueur : {0}", playerString);
        Console.WriteLine(player.pv + " HP  | " + Gauge(player.pv, 10));
        Console.WriteLine(player.force + " DMG | " + Gauge(player.force, 10));
        Console.WriteLine(turn == 1 ? "\n>> Enemie : {0} <<" : "\n   Enemie : {0}", cpuString);

        Console.WriteLine(enemy.pv + " HP  | " + Gauge(enemy.pv, 10));
        Console.WriteLine(enemy.force + " DMG | " + Gauge(enemy.force, 10));
        Console.WriteLine(iaLastAction);

        if (turn == 0)
        {
            Console.WriteLine("\n          Actions possibles:");
            Console.WriteLine(" " + Button((0, 0), "Attaquer") + " "  + Button((1, 0), "Défendre") + " "  + Button((2, 0), "Action spécial", Convert.ToBoolean(playerCooldownSpecial)));
        }
        else
            Console.WriteLine("\n      ** L'ennemie réfléchie... **");
    }

    bool TestGameOver()
    {
        // Possiblly null reference check
        if(player == null)
            player = new Tank();
        if (enemy == null)
            enemy = new Tank();
        //

        deadPlayer = player.pv <= 0;
        deadCPU = enemy.pv <= 0;
        return deadPlayer || deadCPU;
    }

    int IAChoice(int choice){
        Random rdm = new Random();
        choice = rdm.Next(1,4);
        return choice;
    }

    void IATurn()
    {
        // Possiblly null reference check
        if(player == null)
            player = new Tank();
        if (enemy == null)
            enemy = new Tank();
        //

        Random rdm = new Random();
        DrawGame(1);
        Thread.Sleep(rdm.Next(1000, 3000));

        // Prédisposition pour attaques spéciales ia else : randomAction
        switch (enemy.type)
            {
            case 1:
                if (enemy.pv == 1 && !Convert.ToBoolean(enemyCooldownSpecial))
                {
                    enemy.special(player, enemy.force);
                    iaLastAction = "Special attack from the enemy " + cpuString + " !";
                    enemyCooldownSpecial = 2;
                    return;
                }
                else
                {
                    goto randomAction;
                }

            case 2:
                if (enemy.pv <= 2 && !Convert.ToBoolean(enemyCooldownSpecial))
                {
                    float choice = rdm.Next(0, 100);
                    double mantissa = (rdm.NextDouble() * 2.0f) - 1.0f;
                    if (choice + mantissa <= iaDifficulty)
                    {
                        enemy.special(player, enemy.force);
                        enemyCooldownSpecial = 2;
                        iaLastAction = "Special attack from the enemy " + cpuString + " !";
                        return;
                    }
                    else 
                    {
                        goto randomAction;
                    }
                }
                else
                {
                    goto randomAction;
                }

            case 3:
                if (enemy.pv >= 3 && !Convert.ToBoolean(enemyCooldownSpecial))
                {
                    float choice = rdm.Next(0, 100);
                    double mantissa = (rdm.NextDouble() * 2.0f) - 1.0f;
                    if (choice + mantissa <= iaDifficulty)
                    {
                        enemy.special(player, enemy.force);
                        enemyCooldownSpecial = 2;
                        iaLastAction = "Special attack from the enemy " + cpuString + " !";
                        return;
                    }
                    else 
                    {
                        goto randomAction;
                    }
                }
                else
                {
                    goto randomAction;
                }
            }
        //
        
        // Action aléatoire : 1/3 defend && 2/3 attack
        randomAction:
            int action = rdm.Next(1,4);

            if (action == 3)
            {
                // Console.WriteLine("The enemy defends !");
                enemy.Defend();
                iaLastAction = cpuString + " defended !";
            }
            else
            {
                // Console.WriteLine("The enemy attacks you !");
                player.getDamaged(enemy.force);
                iaLastAction = cpuString + " attacked you!";
            }
            return;
        //

    }
}