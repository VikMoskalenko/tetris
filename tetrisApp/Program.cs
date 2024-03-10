using System;
using System.Collections.Generic;
using System.Threading;

class Tetris
{
    static int width = 10;
    static int height = 20;
    static int score = 0;
    static bool[,] board = new bool[width, height];
    static List<int[]> currentPiece;
    static int currentPieceX, currentPieceY;
    static Random random = new Random();
    static bool gameOver = false;

    static void Main()
    {
        Console.Title = "Console Tetris";
        Console.WindowHeight = height + 3;
        Console.WindowWidth = width + 2;
        Console.BufferHeight = height + 3;
        Console.BufferWidth = width + 2;

        while (!gameOver)
        {
            Initialize();
            RunGame();
            GameOver();
        }
    }
    static void Initialize()
    {
        currentPiece = GenerateRandomPiece();
        currentPieceX = width / 2 - 1;
        currentPieceY = 0;
        score = 0;
        gameOver = false;
        Console.Clear();
    }
    static void RunGame()
    {
        while (!gameOver)
        {
            DrawBoard();
            InitializeCurrentPiece();
            DrawPiece();
            ProcessInput();
            MoveDown();
            CheckForCollision();
            RemoveCompletedLines();
            Thread.Sleep(100);
        }
    }
    static void InitializeCurrentPiece()
    {
        if (currentPiece == null)
        {
            currentPiece = GenerateRandomPiece();
            currentPieceX = width / 2 - 1;
            currentPieceY = 0;
        }
    }
    static void DrawBoard()
    {
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Score: {score}");
        Console.WriteLine(new string('-', width * 2));
        for (int y = 0; y < height; y++)
        {
            Console.WriteLine("|");
            for (int x = 0; x < width; x++)
            {
                Console.ForegroundColor = board[x, y] ? ConsoleColor.White : ConsoleColor.Black;
                Console.Write(" ");
            }
            Console.WriteLine("|");
        }
        Console.WriteLine(new string('-', width * 2));
    }
    static void DrawPiece()
    {
        foreach (var point in currentPiece)
        {
            int cursorLeft = (currentPieceX + point[0]) * 2 + 1;
            int cursorTop = currentPieceY + point[1] + 2;
            if (cursorLeft >= 0 && cursorLeft < Console.WindowWidth &&
             cursorTop >= 0 && cursorTop < Console.WindowHeight)
            {
                Console.SetCursorPosition(cursorLeft, cursorTop);
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.Write("■");
            }
        }
    }
    static void ProcessInput()
    {
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    MoveLeft();
                    break;
                case ConsoleKey.RightArrow:
                    MoveRight();
                    break;
                case ConsoleKey.DownArrow:
                    MoveDown();
                    break;
                case ConsoleKey.Spacebar:
                    Rotate();
                    break;
            }
        }
    }
    static void MoveLeft()
    {
        if (!CheckCollision(-1, 0))
        {
            currentPieceX--;
        }
    }
    static void MoveRight()
    {
        if (!CheckCollision(1, 0))
        {
            currentPieceX++;
        }
    }
    static void MoveDown()
    {
        if (!CheckCollision(0, 1))
        {
            currentPieceY++;
        }
    }
    static void Rotate()
    {
        List<int[]> rotatedPiece = new List<int[]>();
        foreach (var point in currentPiece)
        {
            int x = point[1];
            int y = point[0];
            rotatedPiece.Add(new int[] { x, y });
        }
        if (!CheckCollision(0, 0, rotatedPiece))
        {
            currentPiece = rotatedPiece;
        }
    }
    static void CheckForCollision()
    {
        if (CheckCollision(0, 1))
        {
            MergePiece();
            currentPiece = GenerateRandomPiece();
            currentPieceX = width / 2 - 1;
            currentPieceY = 0;
            if (CheckCollision(0, 0))
            {
                gameOver = true;
            }
        }
    }
    static bool CheckCollision(int xOffset, int yOffset)
    {
        return CheckCollision(xOffset, yOffset, currentPiece);
    }
    static bool CheckCollision(int xOffset, int yOffset, List<int[]> piece)
    {
        foreach (var point in piece)
        {
            int x = currentPieceX + point[0] + xOffset;
            int y = currentPieceY + point[1] + yOffset;
            if (x < 0 || x >= width || y >= height || (y >= 0 && board[x, y]))
            {
                return true;
            }
        }
        return false;
    }
    static void MergePiece()
    {
        foreach (var point in currentPiece)
        {
            int x = currentPieceX + point[0];
            int y = currentPieceY + point[1];

            if (y >= 0)
            {
                board[x, y] = true;
            }
        }
    }
    static void RemoveCompletedLines()
    {
        for (int y = height - 1; y >= 0; y--)
        {
            if (IsLineCompleted(y))
            {
                RemoveLine(y);
                score++;
            }
        }
    }
    static bool IsLineCompleted(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (!board[x, y])
            {
                return false;
            }
        }
        return true;
    }
    static void RemoveLine(int y)
    {
        for (int i = y; i > 0; i--)
        {
            for (int x = 0; x < width; x++)
            {
                board[x, i] = board[x, i - 1];
            }
        }
    }
    static List<int[]> GenerateRandomPiece()
    {
        int pieceIndex = random.Next(7);
        switch (pieceIndex)
        {
            case 0: return new List<int[]> { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 0, 2 }, new int[] { 0, -1 } }; // I
            case 1: return new List<int[]> { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 0, -1 }, new int[] { 1, -1 } }; // L
            case 2: return new List<int[]> { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 0, -1 }, new int[] { -1, -1 } }; // J
            case 3: return new List<int[]> { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 1, 1 } };   // O
            case 4: return new List<int[]> { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { -1, 0 }, new int[] { 1, 1 } };  // S
            case 5: return new List<int[]> { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { -1, 1 } };  // Z
            case 6: return new List<int[]> { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { -1, 0 } };  // T
            default: return new List<int[]>();
        }
    }
    static void GameOver()
    {
        Console.Clear();
        int cursorLeft = width / 2 - 5;
        int cursorTop = height / 2;

        if (cursorLeft >= 0 && cursorLeft < Console.WindowWidth &&
            cursorTop >= 0 && cursorTop < Console.WindowHeight)
        {
            Console.SetCursorPosition(cursorLeft, cursorTop);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Game Over");

            cursorTop += 1;

            if (cursorTop >= 0 && cursorTop < Console.WindowHeight)
            {
                Console.SetCursorPosition(cursorLeft, cursorTop);
                Console.WriteLine($"Your Score: {score}");

                cursorTop += 2;

                if (cursorTop >= 0 && cursorTop < Console.WindowHeight)
                {
                    Console.SetCursorPosition(cursorLeft, cursorTop);
                    Console.WriteLine("Press any key to play again or Esc to exit.");

                    ConsoleKeyInfo key = Console.ReadKey();
                    if (key.Key != ConsoleKey.Escape)
                    {
                        gameOver = false;
                    }
                }
            }
        }
    }
}