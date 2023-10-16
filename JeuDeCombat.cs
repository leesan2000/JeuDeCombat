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
        type = type;
        pv = pv;
        force = force;
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
        base.Attack(enemy, damage);
        damageReflected += damage;
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

        while(!endGame){

            while(!deadPlayer){
                Console.WriteLine('Actions possibles:');
                Console.WriteLine('1 - Attaquer');
                Console.WriteLine('2 - Défendre');
                Console.WriteLine('3 - Action spéciale');
                int action = int.Parse(Console.ReadLine());
                if(action == 1){
                    
                }

            }

        }
    }
}







/*
ia :

rdm: 
Random rdm = new Random();
action = rdm.Next(1,3);
1=attack
2=defense
(%+ pour attack)

damager:
if ennemy = tank & vie = 1 :
special
else
rdm


healer:

if vie <= 2 :
heal
else
rdm

tank:
if vie >= 3 :
special
else
rdm

*/
