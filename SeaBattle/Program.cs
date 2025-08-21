using System.Text;
using System.Text.RegularExpressions;

namespace sea_battle
{
    class Cell
    {
        public char Status;
        public bool HasShip;
        public bool WasFired;
        public bool CanPlaceShip;

        public Cell(char status, bool hasShip, bool wasFired, bool canPlaceShip)
        {
            Status = status;
            HasShip = hasShip;
            WasFired = wasFired;
            CanPlaceShip = canPlaceShip;
        }

        public Cell Clone()
        {
            return new Cell(Status, HasShip, WasFired, CanPlaceShip);
        }
    }

    class Board
    {
        public const int BoardSize = 10;
        public const int CellsCount = BoardSize * BoardSize;

        public enum Direction
        {
            Up = 1,
            Left = 2,
            Down = 3,
            Right = 4
        }

        public static List<Cell> MadeField()
        {
            List<Cell> field = new List<Cell>();
            for (int i = 0; i < CellsCount; i++)
            {
                field.Add(new Cell('\u25A1', false, false, true));
            }
            return field;
        }

        public static List<int> ShipCoords(int startidx, List<Cell> Field)
        {
            List<int> coords = new List<int>();
            Queue<int> queue = new Queue<int>();
            bool[] visited = new bool[CellsCount];

            if (!Field[startidx].HasShip)
                return coords;

            queue.Enqueue(startidx);
            visited[startidx] = true;

            int[] direct = { -1, 1, -BoardSize, BoardSize };
            while (queue.Count > 0)
            {
                var cell = queue.Dequeue();
                coords.Add(cell);

                foreach (var dir in direct)
                {
                    int next = cell + dir;

                    if (next >= 0 && next < CellsCount && !visited[next])
                    {
                        if (dir == -1 || dir == 1)
                        {
                            if (next / BoardSize != cell / BoardSize)
                                continue;
                        }
                        if (Field[next].HasShip)
                        {
                            queue.Enqueue(next);
                            visited[next] = true;
                        }
                    }
                }
            }

            return coords;
        }

        public static bool IsShipDestroyed(List<int> coords, List<Cell> field)
        {
            foreach (var idx in coords)
            {
                if (!field[idx].WasFired)
                    return false;
            }

            return true;
        }

        public static List<int> ReturnCellsAroundShipList(int coord)
        {
            List<int> list = new List<int>();

            int row = coord / BoardSize;
            int col = coord % BoardSize;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;

                    if (newRow >= 0 && newRow < BoardSize && newCol >= 0 && newCol < BoardSize)
                        list.Add(newRow * BoardSize + newCol);
                }
            }

            return list;
        }

        public static void AddShip(List<Cell> field, int coord)
        {
            field[coord].Status = '#';
            field[coord].HasShip = true;
            List<int> cantPlaceList = Board.ReturnCellsAroundShipList(coord);
            foreach (var c in cantPlaceList)
            {
                field[c].CanPlaceShip = false;
            }
        }
    }

    class Program
    {

        public static void PrintField(List<Cell> field)
        {
            StringBuilder header = new StringBuilder("  ");
            for (int n = 1; n <= Board.BoardSize; n++)
            {
                header.Append(n);
                if (n < Board.BoardSize) header.Append(' ');
            }
            Console.WriteLine(header.ToString());

            for (int row = 0; row < Board.BoardSize; row++)
            {
                char rowLabel = (char)('A' + row);
                StringBuilder line = new StringBuilder();
                line.Append(rowLabel);
                line.Append(' ');
                int baseIndex = row * Board.BoardSize;
                for (int col = 0; col < Board.BoardSize; col++)
                {
                    line.Append(field[baseIndex + col].Status);
                    if (col < Board.BoardSize - 1) line.Append(' ');
                }
                Console.WriteLine(line.ToString());
            }
        }

        public static void ConvertCoords(string? inp, out int result)
        {
            result = -1;

            if (string.IsNullOrWhiteSpace(inp))
            {
                return;
            }

            string pattern = @"^([A-Ja-j])\s*(\d+)$";
            var match = Regex.Match(inp.Trim(), pattern);

            if (!match.Success)
            {
                return;
            }

            char letter = char.ToUpper(match.Groups[1].Value[0]);
            if (!int.TryParse(match.Groups[2].Value, out int number))
            {
                return;
            }

            if (number < 1 || number > Board.BoardSize)
            {
                return;
            }

            int letterValue = letter - 'A';

            result = (letterValue * Board.BoardSize) + number - 1;

        }

        public static bool TryGetShipCells(int startIndex, int length, Board.Direction direction, List<Cell> field, out List<int> cells)
        {
            cells = new List<int>();

            int startRow = startIndex / Board.BoardSize;
            int startCol = startIndex % Board.BoardSize;

            int dRow = 0;
            int dCol = 0;
            switch (direction)
            {
                case Board.Direction.Up:
                    dRow = -1; dCol = 0; break;
                case Board.Direction.Left:
                    dRow = 0; dCol = -1; break;
                case Board.Direction.Down:
                    dRow = 1; dCol = 0; break;
                case Board.Direction.Right:
                    dRow = 0; dCol = 1; break;
                default:
                    return false;
            }

            for (int step = 0; step < length; step++)
            {
                int r = startRow + step * dRow;
                int c = startCol + step * dCol;

                if (r < 0 || r >= Board.BoardSize || c < 0 || c >= Board.BoardSize)
                    return false;

                int idx = r * Board.BoardSize + c;
                if (idx < 0 || idx >= Board.CellsCount)
                    return false;

                if (field[idx].HasShip || !field[idx].CanPlaceShip)
                    return false;

                cells.Add(idx);
            }

            return true;
        }

        public static void MakeAreaAroundShip(List<int> ship, List<Cell> field)
        {
            HashSet<int> coords = new HashSet<int>();
            foreach (int idx in ship)
            {
                List<int> around = Board.ReturnCellsAroundShipList(idx);
                foreach (int cell in around)
                {
                    coords.Add(cell);
                }
            }

            foreach (int idx in coords)
            {
                if (!field[idx].HasShip)
                {
                    field[idx].WasFired = true;
                    field[idx].Status = '*';
                }
            }
        }

        public static string ConfirmArrangement(List<Cell> Field, out string? CorrectPlace)
        {
            while (true)
            {
                Console.Clear();
                PrintField(Field);
                Console.WriteLine("Подтвердить расстановку?");
                Console.WriteLine("1. Да");
                Console.WriteLine("2. Расставить заново\n");
                CorrectPlace = Console.ReadLine();
                if (CorrectPlace == "1" || CorrectPlace == "2")
                    break;
                Console.WriteLine("Неверный выбор, попробуйте снова");
            }
            return CorrectPlace;
        }

        public static void ChoiceDirection(out int duoShipLineChoice)
        {
            while (true)
            {
                Console.WriteLine("\nВыберите направление корабля \n");
                Console.WriteLine("1. Вверх");
                Console.WriteLine("2. Влево");
                Console.WriteLine("3. Вниз");
                Console.WriteLine("4. Вправо");

                string? duoShipLine = Console.ReadLine();
                if (int.TryParse(duoShipLine, out duoShipLineChoice) && duoShipLineChoice >= 1 && duoShipLineChoice <= 4)
                {
                    break;
                }
                Console.WriteLine("Неверный выбор, попробуйте еще раз");
            }
        }

        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("Приветствуем вас в морском бое!");

            Console.WriteLine("1. Играть");
            Console.WriteLine("2. Выход");

            int firstChoice;
            while (true)
            {
                Console.Write("\n Выберите действие: ");
                var firstInputLine = Console.ReadLine();
                if (int.TryParse(firstInputLine, out firstChoice) && firstChoice >= 1 && firstChoice <= 2)
                {
                    break;
                }
                Console.WriteLine("Неверный выбор, попробуйте снова!");
            }
            Console.Clear();
            switch (firstChoice)
            {

                case 1:
                    Console.Clear();
                    Console.WriteLine("Приятной игры!\n");

                    List<Cell> Field1 = Board.MadeField();
                    List<Cell> Field2 = Board.MadeField();

                    for (int k = 1; k <= 2; k++)
                    {
                        List<Cell> CurrentField = k == 1 ? Field1 : Field2;


                        Console.WriteLine($"\n Игрок {k}, расставьте корабли:");
                        string? correctOpinion = null;
                        do
                        {
                            //solo ship

                            string? soloCorrectPlace;
                            do
                            {
                                List<Cell> CurrentFieldForSoloShip = CurrentField.Select(c => c.Clone()).ToList();
                                PrintField(CurrentFieldForSoloShip);
                                Console.WriteLine("\nРасставьте однопалубные корабли");

                                for (int i = 0; i < 4; i++)
                                {
                                    bool validInput;
                                    do
                                    {
                                        validInput = false;
                                        Console.WriteLine("\n Введите координаты через пробел\n");
                                        string? coordinate = Console.ReadLine();
                                        ConvertCoords(coordinate, out int res);
                                        if (res != -1 && CurrentFieldForSoloShip[res].CanPlaceShip == true)
                                        {
                                            validInput = true;
                                            Console.Clear();
                                            Board.AddShip(CurrentFieldForSoloShip, res);
                                            PrintField(CurrentFieldForSoloShip);
                                            Console.WriteLine();
                                        }
                                        else if (res == -1)
                                        {
                                            Console.WriteLine("Неверный ввод, введите снова!");
                                        }

                                        else if (CurrentFieldForSoloShip[res].CanPlaceShip == false)
                                        {
                                            Console.WriteLine("Здесь нельзя поставить корабль. Выберите другое место");
                                        }
                                    }

                                    while (!validInput);
                                }

                                ConfirmArrangement(CurrentFieldForSoloShip, out soloCorrectPlace);

                                if (soloCorrectPlace == "1")
                                {
                                    CurrentField = CurrentFieldForSoloShip;
                                }
                            }
                            while (soloCorrectPlace != "1");

                            Console.Clear();

                            //duo ship

                            string? duoCorrectPlace;
                            do
                            {
                                List<Cell> CurrentFieldForDuoShip = CurrentField.Select(c => c.Clone()).ToList();
                                PrintField(CurrentFieldForDuoShip);
                                Console.WriteLine("\nРасставьте двупалубные корабли");

                                for (int i = 0; i < 3; i++)
                                {

                                    bool validInput;
                                    do
                                    {
                                        validInput = false;
                                        Console.WriteLine("\nВведите координаты\n");
                                        string? coordinate = Console.ReadLine();
                                        ConvertCoords(coordinate, out int res);

                                        if (res == -1)
                                        {
                                            Console.WriteLine("Неверный ввод, введите снова!");
                                        }

                                        else if (CurrentFieldForDuoShip[res].CanPlaceShip == false)
                                        {
                                            Console.WriteLine("Здесь нельзя поставить корабль. Выберите другое место");
                                        }

                                        else if (res != -1 && CurrentFieldForDuoShip[res].CanPlaceShip == true)
                                        {


                                            int duoShipLineChoice;
                                            ChoiceDirection(out  duoShipLineChoice);
                                            Board.Direction dir = (Board.Direction)duoShipLineChoice;

                                            if (TryGetShipCells(res, 2, dir, CurrentFieldForDuoShip, out var duoCells))
                                            {
                                                validInput = true;
                                                Console.Clear();
                                                foreach (var cellIndex in duoCells)
                                                {
                                                    Board.AddShip(CurrentFieldForDuoShip, cellIndex);
                                                }
                                                PrintField(CurrentFieldForDuoShip);
                                                Console.WriteLine();
                                            }
                                            else
                                            {
                                                Console.WriteLine("Здесь нельзя поставить корабль, введите заново координаты");
                                            }
                                        }
                                    }

                                    while (!validInput);
                                }

                                ConfirmArrangement(CurrentFieldForDuoShip, out duoCorrectPlace);

                                if (duoCorrectPlace == "1")
                                {
                                    CurrentField = CurrentFieldForDuoShip;
                                }
                            }
                            while (duoCorrectPlace != "1");

                            //trio ship

                            string? trioCorrectPlace;
                            do
                            {
                                Console.Clear();
                                List<Cell> CurrentFieldForTrioShip = CurrentField.Select(c => c.Clone()).ToList();
                                PrintField(CurrentFieldForTrioShip);
                                Console.WriteLine("\nРасставьте трехпалубные корабли");

                                for (int i = 0; i < 2; i++)
                                {

                                    bool validInput;
                                    do
                                    {
                                        validInput = false;
                                        Console.WriteLine("\n Введите координаты через пробел\n");
                                        string? coordinate = Console.ReadLine();
                                        ConvertCoords(coordinate, out int res);

                                        if (res == -1)
                                        {
                                            Console.WriteLine("Неверный ввод, введите снова!");
                                        }

                                        else if (CurrentFieldForTrioShip[res].CanPlaceShip == false)
                                        {
                                            Console.WriteLine("Здесь нельзя поставить корабль. Выберите другое место");
                                        }

                                        else if (res != -1 && CurrentFieldForTrioShip[res].CanPlaceShip == true)
                                        {


                                            int trioShipLineChoice;
                                            ChoiceDirection(out trioShipLineChoice);

                                            Board.Direction dir = (Board.Direction)trioShipLineChoice;
                                            if (TryGetShipCells(res, 3, dir, CurrentFieldForTrioShip, out var trioCells))
                                            {
                                                validInput = true;
                                                Console.Clear();
                                                foreach (var cellIndex in trioCells)
                                                {
                                                    Board.AddShip(CurrentFieldForTrioShip, cellIndex);
                                                }
                                                PrintField(CurrentFieldForTrioShip);
                                                Console.WriteLine();
                                            }
                                            else
                                            {
                                                Console.WriteLine("Здесь нельзя поставить корабль, введите заново координаты");
                                            }
                                        }
                                    }

                                    while (!validInput);
                                }

                                ConfirmArrangement(CurrentFieldForTrioShip, out trioCorrectPlace);

                                if (trioCorrectPlace == "1")
                                {
                                    CurrentField = CurrentFieldForTrioShip;
                                }
                            }
                            while (trioCorrectPlace != "1");

                            // quadro ship

                            Console.Clear();
                            string? quadroCorrectPlace;
                            do
                            {
                                List<Cell> CurrentFieldForQuadroShip = CurrentField.Select(c => c.Clone()).ToList();
                                PrintField(CurrentFieldForQuadroShip);
                                Console.WriteLine("\nПоставьте четырехпалубный корабль");


                                bool validInput;
                                do
                                {
                                    validInput = false;
                                    Console.WriteLine("\nВведите координаты через пробел\n");
                                    string? coordinate = Console.ReadLine();
                                    ConvertCoords(coordinate, out int res);

                                    if (res == -1)
                                    {
                                        Console.WriteLine("Неверный ввод, введите снова!");
                                    }

                                    else if (CurrentFieldForQuadroShip[res].CanPlaceShip == false)
                                    {
                                        Console.WriteLine("Здесь нельзя поставить корабль. Выберите другое место");
                                    }

                                    else if (res != -1 && CurrentFieldForQuadroShip[res].CanPlaceShip == true)
                                    {


                                        int quadroShipLineChoice;
                                        ChoiceDirection(out quadroShipLineChoice);

                                        Board.Direction dir = (Board.Direction)quadroShipLineChoice;
                                        if (TryGetShipCells(res, 4, dir, CurrentFieldForQuadroShip, out var quadCells))
                                        {
                                            validInput = true;
                                            Console.Clear();
                                            foreach (var cellIndex in quadCells)
                                            {
                                                Board.AddShip(CurrentFieldForQuadroShip, cellIndex);
                                            }
                                            PrintField(CurrentFieldForQuadroShip);
                                            Console.WriteLine();
                                        }
                                        else
                                        {
                                            Console.WriteLine("Здесь нельзя поставить корабль, введите заново координаты");
                                        }
                                    }
                                }

                                while (!validInput);

                                ConfirmArrangement(CurrentFieldForQuadroShip, out quadroCorrectPlace);
                                if (quadroCorrectPlace == "1")
                                {
                                    CurrentField = CurrentFieldForQuadroShip;
                                }
                            }
                            while (quadroCorrectPlace != "1");

                            ConfirmArrangement(CurrentField, out correctOpinion);
                            Console.Clear();
                        }

                        while (correctOpinion != "1");

                        if (k == 1)
                        {
                            Field1 = CurrentField;
                        }
                        else if (k == 2)
                        {
                            Field2 = CurrentField;
                        }
                    }


                    for (int j = 0; j < Board.CellsCount; j++)
                    {
                        Field1[j].Status = '\u25A1';
                        Field2[j].Status = '\u25A1';
                    }

                    int firstPlayerShipCount = 10;
                    int secondPlayerShipCount = 10;

                    while (firstPlayerShipCount != 0 && secondPlayerShipCount != 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Console.Clear();
                            int currentPlayerShipCount = i == 0 ? secondPlayerShipCount : firstPlayerShipCount;
                            List<Cell> currentField = i == 0 ? Field2 : Field1;

                            Console.WriteLine($"Игрок {i + 1} делайте выбор\n");
                            while (true)
                            {
                                PrintField(currentField);
                                Console.WriteLine("\nВведите координаты");
                                string? coordinate = Console.ReadLine();
                                ConvertCoords(coordinate, out int res);

                                if (res != -1 && currentField[res].WasFired == false)
                                {

                                    Console.Clear();

                                    if (currentField[res].HasShip)
                                    {

                                        List<int> Ship = Board.ShipCoords(res, currentField);
                                        currentField[res].WasFired = true;
                                        currentField[res].Status = '\u00D7';

                                        if (Board.IsShipDestroyed(Ship, currentField))
                                        {
                                            MakeAreaAroundShip(Ship, currentField);
                                            PrintField(currentField);
                                            Console.WriteLine("Корабль уничтожен");
                                            currentPlayerShipCount--;

                                            if (currentPlayerShipCount == 0)
                                                break;
                                        }

                                        else
                                        {
                                            currentField[res].WasFired = true;
                                            currentField[res].Status = '\u00D7';
                                            PrintField(currentField);
                                            Console.WriteLine("Точно в цель");
                                        }

                                        Console.WriteLine("Нажмите любую клавишу чтобы продолжить");
                                        Console.ReadKey();
                                        Console.Clear();
                                    }

                                    else
                                    {
                                        currentField[res].Status = '*';
                                        currentField[res].WasFired = true;
                                        PrintField(currentField);
                                        Console.WriteLine("Мимо\n");
                                        Console.WriteLine("Нажмите любую клавишу чтобы передать ход");
                                        Console.ReadKey();
                                        break;
                                    }
                                }
                                else if (res == -1)
                                {
                                    Console.WriteLine("Неверный ввод, введите снова!");
                                }

                                else if (currentField[res].WasFired == true)
                                {
                                    Console.WriteLine("Сюда уже стреляли, выберите другое место");
                                }

                                if (currentPlayerShipCount == 0)
                                    break;
                            }

                            if (i == 0)
                            {
                                Field2 = currentField;
                                secondPlayerShipCount = currentPlayerShipCount;
                            }

                            else if (i == 1)
                            {
                                Field1 = currentField;
                                firstPlayerShipCount = currentPlayerShipCount;
                            }

                            if (firstPlayerShipCount == 0 || secondPlayerShipCount == 0)
                                break;

                        }
                    }

                    if (firstPlayerShipCount == 0)
                        Console.WriteLine("Победил игрок 2");

                    else if (secondPlayerShipCount == 0)
                        Console.WriteLine("Победил игрок 1\n");

                    Console.WriteLine("Нажмите любую кнопку чтобы закончить игру");
                    Console.ReadKey();
                    Console.Clear();
                    Console.WriteLine("\nДо скорой встречи!");
                    return;

                case 2:
                    Console.WriteLine("До скорой встречи!");
                    break;

            }
        }
    }
}