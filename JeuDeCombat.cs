using System;
using System.Collections.Generic;




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
        bool endGame = false;
        bool deadPlayer = false;
        bool deadCPU = false;
        int cursorLocation = 0;
        int characterChoice = 0;
        Personaje player = null;
        Personaje enemy = null;

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
            characterChoice = int.Parse(Console.ReadLine());
            switch (characterChoice){
            case 1:
                // Instantiate Healer
                break;

            case 2:
                // Instantiate Tank
                break;

            case 3:
                player = new Damager(2, 3, 2, 0);
                enemy = new Damager(2, 3, 2, 0);
                break;

            }
            Console.WriteLine("Vous avez choisi le personnage: Damager");
            Console.Clear();
        }
        while(!endGame){

            while(!deadPlayer){
                Console.WriteLine("Actions possibles:");
                Console.WriteLine("1 - Attaquer");
                Console.WriteLine("2 - Défendre");
                Console.WriteLine("3 - Action spéciale");
                int action = int.Parse(Console.ReadLine());
                if(action == 1){
                    enemy.getDamaged(player.force);
                    Console.WriteLine("Has realizado {0} pts de daño al enemigo!", player.force);
                    Console.WriteLine("Vida actual del enemigo: {0}", enemy.pv);

                    
                }
            }
        }
    }
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

if (ennemy = tank && vie = 1)
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