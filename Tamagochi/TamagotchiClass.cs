using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace Tamagochi
{
    public class TamagotchiClass
    {

        int hunger = 50;
        //int energy = 60;

        private bool isEnergyDepletedNotified = false;
        private bool isHungerDepletedNotified = false;

        public int Happiness { get; set; } = 60;
        public int Energy { get; set; } = 60;

        public int Age { get; set; }
        public string Name { get; set; }

        public int Hunger
        {
            get => hunger;
            set
            {
                hunger = Math.Max(0, value); 
                if (hunger == 0 && !isHungerDepletedNotified)
                {
                    isHungerDepletedNotified = true;
                    OnHungerDepleted();
                }
                else if (hunger > 0)
                {
                    isHungerDepletedNotified = false; 
                }
            }
        }

        public event Action HungerDepleted;

        protected virtual void OnHungerDepleted()
        {
            HungerDepleted?.Invoke();
        }

        //-------------------------TEVÉKENYSÉGEK------------------------
        public void Feed()
        {
            Hunger = Math.Max(0, Hunger - 10);
            Happiness = Math.Min(100, Happiness + 5);
        }

        public void Play()
        {
            Energy = Math.Max(0, Energy - 5);
            Hunger = Math.Min(100, Hunger + 5);
            Happiness = Math.Min(100, Happiness + 10);
        }

        public void Rest()
        {
            Energy = Math.Min(100, Energy + 20);
            Happiness = Math.Min(100, Happiness + 5);
        }
    }
}

