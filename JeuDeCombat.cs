using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

/*
Type :

1 : Damager (3 2)
2 : Healer (4 1)
3 : Tank (5 1)

*/

class Personaje{
    public int type;
    public int pv = 10;
    public  int maxpv {get;} = 10;
    public int force = 0;
    public int specialCoolDown = 0;
    public bool dead = false;
    public string nameTag = "";
    public int actionChoice = 0;
    public bool isDefending = false;
    
    
    public Personaje(int type, int pv, int maxpv, int force)
    {
        this.type = type;
        this.pv = pv;
        this.maxpv = maxpv;
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

    public Damager() : base(_type, _pv, _pv, _force)
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

    public Healer() : base(_type, _pv, _pv, _force) {}

    public override void special(Personaje enemy, int d) //Parameters here are not used
    {
        base.getDamaged(-2);
        base.pv = Math.Min(base.pv, 4);
    }
}
class Tank : Personaje
{
    protected static int _type  = 3;
    protected static int _pv = 5;
    protected static int _force = 1;

    public Tank() : base(_type, _pv, _pv, _force) {}

    public override void special(Personaje enemy, int force)
    {
        // Take 1 damage
        base.getDamaged(1);
        // deal 2 damage or only one if defending
        enemy.getDamaged(this.force); // can be defend
        enemy.getDamaged(this.force); // cannot be defend
    }
}

partial class Program
{
    static void Main()
    {
        Program program = new Program();
        program.Interface();
    }

    public List<(int x, int y)> buttonsPositions = new List<(int x, int y)>(); // List of all currently displayed buttons (this is to prevent the cursor from going OOB)
    public (int x, int y) cursorPosition = (0, 2); // Determine the cursor position on the interface

    // Choices
    public int gamemodeChoice = -1; // Gamemode choice in the main menu screen
    public int characterChoice = -1; // Character choice in the character selection screen
    public float difficultyChoice = -1; // Difficulty choice in the difficulty selection screen
    public int enemyChoice = 0;


    // Players
    Personaje? player = null;
    Personaje? enemy = null;


    // Game state
    public bool endGame; // Determine if a game is active
    public int nb_games = 0; // current number of iteration for simulation
    public float dizieme; // 1/10 du nombre de combats pour bar de chargement de la simulation
    public int playerWins = 0;
    public int enemyWins = 0;
    public float iaDifficulty = 50f; // Difficuly of the AI (Easy = 10, Medium = 45, Hard = 70)
    string lastAction = "Ready to fight !"; // Label of the last action of the AI
 

    void Interface()
    {
        // Boucle menu principal
        cursorPosition = (0, 0);
        while (gamemodeChoice == -1)
        {
            Console.Clear();
            buttonsPositions.Clear();

            Console.WriteLine("+------------------------------+");
            Console.WriteLine("|         JEU DE COMBAT        |");
            Console.WriteLine("+------------------------------+");
            Console.WriteLine("\nUtilisez les flèche directionnelle \npour vous déplacez entre les options.");
            Console.WriteLine("\nAppuyiez sur [Espace] ou [Entrer] \npour selectionner un bouton.");
            Console.WriteLine("\n   " + Button((0,0), "Mode solo") + " " + Button((1,0), "Mode spectateur") + " " + Button((2,0), "Mode simulation"));

            if (WaitForInput())
                gamemodeChoice = cursorPosition.x + 1;
        }

        // Difficulty choice loop
        cursorPosition = (0, 2);
        while(difficultyChoice == -1){

            Console.Clear();
            buttonsPositions.Clear();
            Console.WriteLine("+------------------------------+");
            Console.WriteLine("|           DIFFICULTÉ         |");
            Console.WriteLine("+------------------------------+");
            Console.WriteLine("\nVeuillez choisir la dificulté de l'IA");
            Console.WriteLine("          " + Button((0,2), "Easy"));
            Console.WriteLine("          " + Button((0,1), "Medium"));
            Console.WriteLine("          " + Button((0,0), "Hard"));

            if(WaitForInput()){
                difficultyChoice = Math.Abs(cursorPosition.y - 3);
            }

        }

        switch(difficultyChoice){
            case 1:
                iaDifficulty = 10;
                break;
            case 2:
                iaDifficulty = 45;
                break;
            case 3:
                iaDifficulty = 70;
                break;
        }

        // Boucle choix du personnage
        if (gamemodeChoice == 1 || gamemodeChoice == 3)
        {
            cursorPosition = (0, 2);
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
        }


        // Creation du joueur
        switch (gamemodeChoice == 2 ? IAChoice() : characterChoice)
        {
            case 1:
                player = new Damager();
                player.nameTag = "Damager";
                break;
            case 2:
                player = new Healer();
                player.nameTag = "Healer";
                
                break;
            case 3:
                player = new Tank();
                player.nameTag = "Tank";
                break;
            default:
                player = new Tank();
                player.nameTag = "Tank";
                break;
        }
        
        // Creation de l'IA
        if (gamemodeChoice != 3)
        {
            switch(IAChoice()){
                case 1:
                    enemy = new Damager();
                    enemy.nameTag = "Damager";
                    break;
                case 2:
                    enemy = new Healer();
                    enemy.nameTag = "Healer";
                    break;
                case 3: 
                    enemy = new Tank();
                    enemy.nameTag = "Tank";
                    break;
                default:
                    enemy = new Tank();
                    enemy.nameTag = "Tank";
                    break;
            }
        }
        else
        {
            int enemyChoice = -1;
            cursorPosition = (0, 2);
            while(enemyChoice == -1)
            {
                Console.Clear();
                buttonsPositions.Clear();

                Console.WriteLine("+------------------------------+");
                Console.WriteLine("|          PERSONNAGES         |");
                Console.WriteLine("+------------------------------+");
                Console.WriteLine("\nVeuillez choisir la classe de l'adversaire :");
                Console.WriteLine("          " + Button((0,2), "Damager"));
                Console.WriteLine("          " + Button((0,1), "Healer"));
                Console.WriteLine("          " + Button((0,0), "Tank"));

                if (WaitForInput())
                    enemyChoice = Math.Abs(cursorPosition.y - 3);
            }

            switch(enemyChoice){
                case 1:
                    enemy = new Damager();
                    enemy.nameTag = "Damager";
                    break;
                case 2:
                    enemy = new Healer();
                    enemy.nameTag = "Healer";
                    break;
                case 3: 
                    enemy = new Tank();
                    enemy.nameTag = "Tank";
                    break;
                default:
                    enemy = new Tank();
                    enemy.nameTag = "Tank";
                    break;
            }
        }

        // Choose number of interations for simulation
        int nb_simu = -1;
        if (gamemodeChoice == 3){
            cursorPosition = (0, 2);
            while(nb_simu == -1)
            {
                Console.Clear();
                buttonsPositions.Clear();

                Console.WriteLine("+------------------------------+");
                Console.WriteLine("|          SIMULATION          |");
                Console.WriteLine("+------------------------------+");
                Console.WriteLine("\nVeuillez choisir le nombre de combats :");
                Console.WriteLine("          " + Button((0,3), "10 000"));
                Console.WriteLine("          " + Button((0,2), "1 000 000"));
                Console.WriteLine("          " + Button((0,1), "1"));
                Console.WriteLine("          " + Button((0,0), "Autre : "));

                if (WaitForInput())
                    nb_simu = Math.Abs(cursorPosition.y - 4);

                switch (nb_simu)
                {
                    case 1:
                        nb_simu = 10000;
                        break;
                    case 2:
                        nb_simu = 1000000;
                        break;
                    case 3:
                        nb_simu = 1;
                        break;
                    case 4:
                        while (true)
                        {
                            // Nombre précis de simulation
                            try{
                            nb_simu = Int32.Parse(Console.ReadLine() + "");
                            break;
                            } catch {nb_simu = 4;}
                        }
                        break;
                }
            }
            // 1/10 pour bar de chargement
                dizieme = nb_simu / 10f;
            //
        }

        if ( (gamemodeChoice == 2 || gamemodeChoice == 3) && enemy.nameTag == player.nameTag)
        {
            player.nameTag += " 1";
            enemy.nameTag += " 2";
        }

    gameloop:
        // Game loop
        cursorPosition = (0, 0);
        while(!endGame)
        {
            // Fight loop
            while(!enemy.dead && !player.dead)
            {
                // Update specials cooldown
                if (player.specialCoolDown > 0)
                    player.specialCoolDown--;
                if (enemy.specialCoolDown > 0)
                    enemy.specialCoolDown--;

                // Ask the player his choice if he is not in spectator mode
                if (gamemodeChoice == 1)
                {
                    player.isDefending = false;
                    int action = -1;
                    while (action <= -1)
                    {
                        // Draw the board
                        DrawGame(0);
                        if (action == -2)
                            Console.WriteLine("Special unavailable.");

                        // Player Turn
                        if (WaitForInput())
                            action = cursorPosition.x + 1;


                        // Attack
                        if(action == 1)
                        {
                            lastAction = "You dealt " + (enemy.isDefending ? "0" : player.force) + " damage to the enemy !";
                            enemy.getDamaged(player.force);      
                        }

                        // Defend
                        else if (action == 2)
                        {
                            player.Defend();
                            lastAction = "You defend yourself.";
                        }
                            
                        // Special attack
                        else if (action == 3 && player.specialCoolDown <= 0)
                        {
                            player.special(enemy, player.force);
                            player.specialCoolDown = 2;
                            if (player.type != 2)
                                lastAction = "You make a " + player.nameTag + " special attack on the enemy !";
                            else
                                lastAction = "You make a " + player.nameTag + " special attack on yourself !";
                        }

                        // Illegal move
                        else if (action > -1)
                            action = -2;
                    }
                }

                // Otherwise, let an AI make a move
                else
                {
                    if (gamemodeChoice != 3)
                        DrawGame(0);
                    IATurn(player, enemy);
                }
                if (TestGameOver())
                    break;

                // AI Turn
                if (gamemodeChoice != 3)
                    DrawGame(1);
                enemy.isDefending = false;
                IATurn(enemy, player);
                if (TestGameOver())
                    break;
            }

            if (gamemodeChoice == 3)
            {
                // iterate number of wins
                nb_games++;
                if (player.pv <= 0) enemyWins++;
                if (enemy.pv <= 0) playerWins++;

                // bar de chargement
                if(nb_games == 0) Console.Clear();
                if ((float)nb_games % dizieme == 0)
                    Console.Write("█");

                // loop
                if (nb_games < nb_simu)
                {
                    // character reset
                    player.pv = player.maxpv; player.dead = false;
                    enemy.pv = enemy.maxpv; enemy.dead = false;

                    // relaunch combat
                    endGame = false;
                    goto gameloop;
                }
            }
            
            // Game over loop
            while (true)
            {
                Console.Clear();
                buttonsPositions.Clear();

                Console.WriteLine("+------------------------------+");
                Console.WriteLine("|           GAME OVER          |");
                Console.WriteLine("+------------------------------+");
                if (gamemodeChoice == 1)
                    Console.WriteLine("\n        {0}", player.dead ? "Vous êtes mort !" : "Vous avez gagné !");
                else if (gamemodeChoice == 3)
                    Console.WriteLine("\n     {0}", playerWins >= enemyWins ? "Le joueur 1 ("+player.nameTag+") à gagné avec "+ (playerWins / (float)nb_simu * 100) +"% (" + playerWins + " combats) de victoires sur les "+ nb_simu + " combats!" : "Le joueur 2 ("+enemy.nameTag+") à gagné avec "+ (enemyWins / (float)nb_simu * 100) +"% (" + playerWins + " combats) de victoires sur les "+ nb_simu + " combats!");
                else
                    Console.WriteLine("\n     {0}", player.dead ? "Le joueur 2 ("+enemy.nameTag+") à gagné !" : "Le joueur 1 ("+player.nameTag+") à gagné !");
                Console.WriteLine("\n   " + Button((0,0), "Menu principal") + " " + Button((1,0), "Quitter") );

                if (WaitForInput())
                {
                    // Reset game
                    if (cursorPosition.x == 0)
                    {
                        new Program().Interface();
                    }
                    return;
                }
            }
        }
    }

    // Return selectable button that can be placed in a WriteLine
    string Button((int x, int y) position, string label, bool disabled=false, string cursorDisplay = "X")
    {
        string labelDisplay = "";

        // Check if the button is selected
        bool selected = position == cursorPosition;
        labelDisplay += selected ? disabled ? cursorDisplay : ">" : " ";
        labelDisplay += label;
        labelDisplay += selected ? disabled ? cursorDisplay : "<" : " ";

        // Register it and return the display string 
        buttonsPositions.Add(position);
        return labelDisplay;
    }

    // Return a level gauge that can be placed in a WriteLine
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

        // Check wich key was pressed
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

    // Draw the game depending on the player turn
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

        // Player info
        Console.WriteLine(turn == 0 ? ">> Joueur : {0} <<" : "   Joueur : {0}", player.nameTag);
        Console.WriteLine(player.pv + " HP  | " + Gauge(player.pv, 10));
        Console.WriteLine(player.force + " DMG | " + Gauge(player.force, 10));

        // Enemy info
        Console.WriteLine(turn == 1 ? "\n>> Enemie : {0} <<" : "\n   Enemie : {0}", enemy.nameTag);
        Console.WriteLine(enemy.pv + " HP  | " + Gauge(enemy.pv, 10));
        Console.WriteLine(enemy.force + " DMG | " + Gauge(enemy.force, 10));
        Console.WriteLine("\n" + lastAction);

        // Buttons
        if (gamemodeChoice == 2) return;
        if (turn == 0)
        {
            Console.WriteLine("\n          Actions possibles:");
            Console.WriteLine(" " + Button((0, 0), "Attaquer", enemy.isDefending, "!") + " "  + Button((1, 0), "Défendre") + " "  + Button((2, 0), "Action spécial", Convert.ToBoolean(player.specialCoolDown)));
        }
        else
            Console.WriteLine("\n      ** L'ennemie réfléchie... **");
    }

    // Check if the game should end
    bool TestGameOver()
    {
        // Possiblly null reference check
        if (player == null || enemy == null)
            return false;

        player.dead = player.pv <= 0;
        enemy.dead = enemy.pv <= 0;
        return player.dead || enemy.dead;
    }

    // Called to init the AI character choice
    int IAChoice(){
        Random rdm = new Random();
        int choice = rdm.Next(1,4);
        return choice;
    }

    // Called when a AI make a move
    void IATurn(Personaje ia, Personaje opponent)
    {
        Random rdm = new Random();
        if (gamemodeChoice != 3)
            Thread.Sleep(rdm.Next(1000, 3000));
        ia.isDefending = false;

        // Prédisposition pour attaques spéciales ia else : randomAction
        switch (ia.type)
            {
            case 1: // Damager
        
                if(iaDifficulty == 10){ //Difficulty set to 10 (Easy) --> AI does random action and no special attacks
                    goto randomAction;
                }
                
                if (ia.pv == 1 && ia.specialCoolDown <= 0)
                {
                    ia.special(opponent, ia.force);
                    lastAction = "Special attack from " + ia.nameTag + " !";
                    ia.specialCoolDown = 2;
                    return;
                }
                else
                {
                    goto randomAction;
                }

            case 2: // Healer
            
                if(iaDifficulty == 10){ //IA Difficulty = 10 (Easy) --> AI does any action randomly, doesn't take anything into account
                    goto randomAction;
                } 
                
                if (ia.pv <= 2 && ia.specialCoolDown <= 0)
                {
                    float choice = rdm.Next(0, 100);
                    double mantissa = (rdm.NextDouble() * 2.0f) - 1.0f;
                    // Console.WriteLine(choice + mantissa);
                    if (choice + mantissa <= iaDifficulty) //If iaDifficulty is set to Medium (45) then there will be about 45% chances that the IA uses the special attack based on his HP
                                                            //The same thing goes for Hard difficulty
                    {
                        ia.special(opponent, ia.force);
                        ia.specialCoolDown = 2;
                        lastAction = "The enemy " + ia.nameTag + " healed himself !";
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

            case 3: // Tank
        
                if(iaDifficulty == 10){
                    goto randomAction;
                }

                if (ia.pv >= 3 && ia.specialCoolDown <= 0)
                {
                    float choice = rdm.Next(0, 100);
                    double mantissa = (rdm.NextDouble() * 2.0f) - 1.0f;
                    // Console.WriteLine(choice + mantissa);
                    if (choice + mantissa <= iaDifficulty)
                    {
                        ia.special(opponent, ia.force);
                        ia.specialCoolDown = 2;
                        lastAction = "Special attack from " + ia.nameTag + " !";
                        return;
                    }
                    else 
                    {
                        goto randomAction;
                    }
                }

                else if (ia.force > 1)
                {
                    opponent.getDamaged(ia.force);
                    lastAction = ia.nameTag + " attacked !";
                    return;
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
                // Console.WriteLine("The opponent defends !");
                ia.Defend();
                lastAction = ia.nameTag + " defended !";
            }
            else
            {
                // Console.WriteLine("The opponent attacks you !");
                opponent.getDamaged(ia.force);
                lastAction = ia.nameTag + " attacked !";
            }
            return;
        //

    }
}

/*
	Damger 	Healer 	Tank  1
Damager 90	92	99		
Healer	92	50	18
Tank	5.5	94	83
2
*/