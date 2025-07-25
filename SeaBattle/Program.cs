using System.Security.Authentication;
using System.Text.RegularExpressions;

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
            for (int i = 0; i < 100; i++) {
                field.Add(new Cell('*', false, false, true));
            }
            return field;
        }

        public static void PrintField(List<Cell> field)
        {
            Console.WriteLine("  1 2 3 4 5 6 7 8 9 10");
            int countForPrientField =0;
            for (char i = 'A'; i <= 'J'; i++)
            {
                int j = countForPrientField*10;
                
                Console.WriteLine($"{i} {field[j].Status} {field[j+1].Status} {field[j+2].Status} {field[j+3].Status} {field[j+4].Status} {field[j+5].Status} {field[j+6].Status} {field[j+7].Status} {field[j+8].Status} {field[j+9].Status}");
                // $"{i} * # # * * & * * \u00D7 *
                
                countForPrientField++;
            }
        }

        public static void ConvertCoords(string? inp, out int result) 
        {
            result = -1;

            if (string.IsNullOrWhiteSpace(inp)) {
                Console.WriteLine("Неверный ввод");
                return;
            }

            string pattern = @"^([A-Ja-j])\s+(\d+)$";
            var match = Regex.Match(inp.Trim(), pattern);

            if (!match.Success) {
                return;
            }

            char letter = char.ToUpper(match.Groups[1].Value[0]);
            if (!int.TryParse(match.Groups[2].Value, out int number)) {
                Console.WriteLine("Неверный ввод");
                return;
            }

            if (number < 1 || number > 10) {
                Console.WriteLine("Неверный ввод");
                return;
            }

            int letterValue = letter - 'A';

            result = (letterValue * 10) + number -1;

        }

        public static int LetterToNumber(char letter) 
        {
            return char.ToUpper(letter) - 'A';
        }

        public static void Main()
        {
            Console.WriteLine("Приветствуем вас в морском бое!");

            Console.WriteLine("1. Играть");
            Console.WriteLine("2. Выход");
            
            Console.Write("\n Выберите действие: ");
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
                            
                            List<Cell> Field1 = MadeField();
                            List<Cell> Field2 = MadeField();

                            for (int k = 1; k <= 2; k++)
                            {
                                List<Cell> CurrentField = k == 1 ? Field1 : Field2;
                                

                                Console.WriteLine($"\n Игрок {k}, расставьте корабли:");
                                string? correctOpinion = null;
                                do
                                {
                                    List<Cell> CurrentFieldForSoloShip = new List<Cell>(CurrentField);
                                    PrintField(CurrentFieldForSoloShip);
                                    string? soloCorrectPlace;
                                    //solo ship place
                                    do
                                    {
                                        Console.WriteLine("\n Расставьте однопалубные корабли");
                                        int soloShipCount = 0;
                                        Console.WriteLine("\n Введите координаты через пробел\n");

                                        for (int i = 0; i < 4; i++)
                                        {
                                            string? coordinate = Console.ReadLine();
                                            ConvertCoords(coordinate, out int res);
                                            if (res != -1 && Field1[res].canPlaceShip == true)
                                            {
                                                Console.Clear();
                                                CurrentFieldForSoloShip[res].Status = '#';
                                                soloShipCount++;
                                                PrintField(CurrentFieldForSoloShip);
                                            }
                                            else if (res == -1)
                                            {
                                                Console.WriteLine("Неверный ввод, начните заново!");
                                                break;
                                            }

                                            else if (CurrentFieldForSoloShip[res].canPlaceShip == false)
                                            {
                                                Console.WriteLine("Здесь нельзя поставить корабль. Выберите другое место");
                                                break;
                                            }
                                        }

                                        if (soloShipCount == 4)
                                        {
                                            PrintField(CurrentFieldForSoloShip);
                                            Console.WriteLine("Подтвердить расстановку?");
                                            Console.WriteLine("1. Да");
                                            Console.WriteLine("2. Расставить заново\n");
                                            soloCorrectPlace = Console.ReadLine();
                                            if (soloCorrectPlace == "1")
                                            {
                                                CurrentField = CurrentFieldForSoloShip;
                                            }
                                        }
                                        else
                                        {
                                            soloCorrectPlace = "0";
                                            Console.WriteLine("Неправильная расстановка, попробуйте еще раз");
                                        }
                                    }
                                    while (soloCorrectPlace != "1");
                                    

                                    PrintField(CurrentField);
                                    string? duoCorrectPlace;
//duo ship place
                                    do
                                    {
                                        Console.WriteLine("\n Расставьте двухпалубные корабли");
                                        int duoShipCount = 0;
                                        Console.WriteLine("\n Введите координаты через пробел\n");

                                        for (int i = 0; i < 6; i++)
                                        {
                                            string? coordinate = Console.ReadLine();
                                            ConvertCoords(coordinate, out int res);
                                            if (res != -1)
                                            {
                                                Field1[res].Status = '#';
                                                duoShipCount++;
                                            }
                                            
                                            else
                                            {
                                                Console.WriteLine("Неверный ввод, начните заново!");
                                                break;
                                            }
                                        }

                                        if (duoShipCount == 4)
                                        {
                                            PrintField(CurrentField);
                                            Console.WriteLine("Подтвердить расстановку?");
                                            Console.WriteLine("1. Да");
                                            Console.WriteLine("2. Расставить заново\n");
                                            duoCorrectPlace = Console.ReadLine();
                                        }
                                        else duoCorrectPlace = "0";
                                    }
                                    while (duoCorrectPlace != "1");





                                    Console.WriteLine("\nПодтвердите расстановку кораблей");
                                    Console.WriteLine("1. Подтвердить");
                                    Console.WriteLine("2. Расставить заново");
                                    correctOpinion = Console.ReadLine();
                                }

                                while (correctOpinion != "1");

                            }
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