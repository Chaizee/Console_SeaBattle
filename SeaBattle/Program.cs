using System.Security.Authentication;

namespace sea_battle
{
    class Cell
    {
        public char Status;
        public bool hasShip;
        public bool wasFired;
        public bool canPlaceShip;

        public Cell(char status, bool hasship, bool wasfired, bool canplaceship)
        {
            Status = status;
            hasShip = hasship;
            wasFired = wasfired;
            canPlaceShip = canplaceship;
        }
    }

    class Program
    {   
        public static List<Cell> MadeField()
        {
            List<Cell> field = new List<Cell>();
            return field;
        }

        public static void PrintField(List<Cell> field)
        {
            Console.WriteLine("  1 2 3 4 5 6 7 8 9 10");
            for (char i = 'A'; i <= 'J'; i++)
            {
                Console.WriteLine($"{i} * # * * * & * * \u00D7 *");
            }
        }


        public static void Main()
        {
            Console.WriteLine("Приветствуем вас в морском бое!");

            Console.WriteLine("1. Играть");
            Console.WriteLine("2. Выход");
            
            Console.Write("\nВыберите действие: ");
            var firstInputLine = Console.ReadLine();
            int firstChoice;
            if (!int.TryParse(firstInputLine, out firstChoice)) {
                Console.WriteLine("Неверный выбор, попробуйте снова!");
            }
            Console.Clear();
            switch(firstChoice)
            {
                case 1:
                    Console.WriteLine("Выберите режим: ");
                    Console.WriteLine("1. Играть с другом");
                    Console.WriteLine("2. Играть с пк");
                    Console.WriteLine("3. Выход");

                    var secondInputLine = Console.ReadLine();
                    int secondChoice;
                    if (!int.TryParse(secondInputLine, out secondChoice))
                    {
                        Console.WriteLine("Неверный выбор, попробуйте снова!");
                    }

                    switch(secondChoice)
                    {
                        case 1:
                            Console.WriteLine("Приятной игры!\n");
                            List<Cell> firstField = MadeField();
                            PrintField(firstField);
                            Console.WriteLine("\nИгрок 1, расставьте корабли:");
                            
                            return;





                            
                        case 2:
                            return;
                        case 3:
                            break;
                        default:
                            Console.WriteLine("Неверный выбор, попробуйте снова!");
                            break;

                    }

                    break;
                case 2:
                    Console.WriteLine("До скорой встречи!");
                    break;
                default:
                    Console.WriteLine("Неверный выбор, попробуйте снова!");
                    break;

            }
        }
    }
}