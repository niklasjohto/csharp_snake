using System.Collections.Generic;
using System.Drawing;

namespace Snake {
    class Program {
        static void Main() {
            Snake snake = new Snake(20, 50);
            snake.Start();
        }
    }
    class Snake {
        private readonly int[,] _map;

        private string _snakeChar = "\u2588";
        private string _fruitChar = "@";
        private string _tileChar = "\u2591";

        private Point _fruitPos = new Point();

        private List<Point> _tail = new List<Point>();

        private int _x;
        private int _y;

        private int _xSpeed = 1;
        private int _ySpeed = 0;

        private bool _gameOver = false;

        public Snake(int ySize, int xSize) {
            _map = new int[ySize, xSize];

            _y = _map.GetLength(0) / 2;
            _x = _map.GetLength(1) / 2;

            _tail.Add(new Point(_x, _y));
        }

        private void GameOver() {
            _gameOver = true;

            Console.Clear();
            Console.WriteLine("Game Over!");
            Console.ReadKey();
        }

        private void DrawMap() {
            for (int i = 0; i < _map.GetLength(0); i++) {
                for (int j = 0; j < _map.GetLength(1); j++) {
                    Console.SetCursorPosition(j, i);
                    Console.WriteLine(_tileChar);
                }
            }
        }

        private void Fruit() {
            Random rand = new Random();
            int randomY = rand.Next(_map.GetLength(0));
            int randomX = rand.Next(_map.GetLength(1));

            foreach(Point pos in _tail) {
                if (randomY == pos.Y && randomX == pos.X) {
                    randomY = rand.Next(_map.GetLength(0));
                    randomX = rand.Next(_map.GetLength(1));
                }
            }

            _fruitPos = new Point(randomX, randomY);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(_fruitPos.X, _fruitPos.Y);
            Console.Write(_fruitChar);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void CollisionDetection() {
            // Check if snake is outside playable area
            if (_x > _map.GetLength(1) | (_x + 2) <= 0 | _y > _map.GetLength(0) | (_y + 2) <= 0) {
                GameOver();
                return;
            }

            // Check if snake is colliding with itself
            foreach (Point pos in _tail.Skip(1)) {
                if (_tail[0].X == pos.X && _tail[0].Y == pos.Y) {
                    GameOver();
                    return;
                }
            }
        }
        private void Update() {
            // Replace the last tail's position with a tile character
            Console.SetCursorPosition(_tail.Last().X, _tail.Last().Y);
            Console.Write(_tileChar);

            if (_x == _fruitPos.X & _y == _fruitPos.Y) {
                Fruit();
                _tail.Add(new Point(_tail.First().X, _tail.First().Y));
            }

            for (int i = _tail.Count - 1; i > 0; i--) {
                _tail[i] = _tail[i - 1];
            }

            // New head
            _tail[0] = new Point(_x, _y);

            _x += _xSpeed;
            _y += _ySpeed;

            CollisionDetection();

            foreach (Point pos in _tail) {
                Console.ForegroundColor = pos == _tail[0] ? ConsoleColor.Green : ConsoleColor.DarkGreen;
                Console.SetCursorPosition(pos.X, pos.Y);
                Console.Write(_snakeChar);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void KeyPress(ConsoleKeyInfo press) {
            switch (press.Key) {
                case ConsoleKey.UpArrow:
                    if(_ySpeed != 1) {
                        _ySpeed = -1;
                        _xSpeed = 0;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if(_ySpeed != -1) {
                        _ySpeed = 1;
                        _xSpeed = 0;
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    if(_xSpeed != 1) {
                        _xSpeed = -1;
                        _ySpeed = 0;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if(_xSpeed != -1) {
                        _xSpeed = 1;
                        _ySpeed = 0;
                    }
                    break;
                default:
                    throw new Exception();
            }
        }

        private void GameLoop() {
            while (true) {
                if (_gameOver) break;

                if (Console.KeyAvailable) {
                    ConsoleKeyInfo userPress = Console.ReadKey();
                    if (userPress.Key == ConsoleKey.Escape) break;
                    KeyPress(userPress);
                }

                Update();
                Thread.Sleep(100);
            }
        }
        public void Start() {
            Console.CursorVisible = false;

            DrawMap();
            Fruit();

            GameLoop();
        }
    }
}