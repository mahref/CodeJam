using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeJam
{
    class Node
    {
        public int horizontal;
        public int vertical;
        public int descendingDiagonal;
        public int ascendingDiagonal;
    }

    class Board
    {
        public int Id { get; set; }

        public int N { get; set; }

        public int K { get; set; }

        public Board(int n, int k, int t)
        {
            this.N = n;
            this.K = k;
            this.Id = t;

            this._b = new char[n, n];
        }

        internal void SetPlayer(int i, int j, char player)
        {
            _b[i, j] = player;
        }
        internal char PlayerAt(int i, int j)
        {
            return _b[i, j];
        }

        private char[,] _b;
    }
    class BoardSolver
    {
        static void Main(string[] args)
        {
            List<Board> boards = LoadAllboards("input.txt");

            var answers = new ConcurrentBag<KeyValuePair<int, string>>();

            Parallel.ForEach(boards, b =>
            {
                char[] players = { 'B', 'R' };
                ConcurrentBag<char> winners = new ConcurrentBag<char>();

                Parallel.ForEach(players, player =>
                {
                    if (HasWon(b, player))
                        winners.Add(player);
                });

                if (winners.Count == 0)
                {
                    answers.Add(new KeyValuePair<int, string>(b.Id, "Neither"));
                }
                else if (winners.Count == 2)
                {
                    answers.Add(new KeyValuePair<int, string>(b.Id, "Both"));
                }
                else
                {
                    answers.Add(new KeyValuePair<int, string>(b.Id, winners.First().ToString()));
                }
            });

            var sortedAnswers = answers.ToList();
            sortedAnswers.Sort((a, b) => { return a.Key - b.Key; });

            foreach (var answer in sortedAnswers)
            {
                string winner = string.Empty;
                if (answer.Value == "B")
                    winner = "Blue";
                else if (answer.Value == "R")
                    winner = "Red";
                else
                    winner = answer.Value;
                Console.WriteLine("Case #" + (answer.Key + 1) + ": " + winner);
            }
        }
        static bool HasWon(Board b, char player)
        {
            Node[,] solution = new Node[b.N,b.N];
            for (int i = 0; i < b.N; i++)
            {
                for (int j = 0; j < b.N; j++)
                {
                    Node currentNode = new Node();
                    solution[i, j] = currentNode;
                    Node upper, left, upperLeft, upperRight;
                    GetNodes(solution, i, j, out upper, out left, out upperLeft, out upperRight);
                    if (b.PlayerAt(i, j) == player)
                    {
                        currentNode.horizontal = 1 + (upper != null ? upper.horizontal : 0);
                        currentNode.vertical = 1 + (left != null ? left.vertical : 0);
                        currentNode.ascendingDiagonal = 1 + (upperLeft != null ? upperLeft.ascendingDiagonal : 0);
                        currentNode.descendingDiagonal = 1 + (upperRight != null ? upperRight.descendingDiagonal : 0);
                    }

                    if (IsWinner(currentNode, b))
                        return true;
                }
            }
            return false;
        }
        private static bool IsWinner(Node currentNode, Board b)
        {
            if (currentNode.vertical >= b.K ||
                currentNode.horizontal >= b.K ||
                currentNode.ascendingDiagonal >= b.K ||
                currentNode.descendingDiagonal >= b.K)
                return true;
            return false;
        }
        private static void GetNodes(
            Node[,] solution,
            int i,
            int j,
            out Node upper,
            out Node left,
            out Node upperLeft,
            out Node upperRight
            )
        {
            upper = null; left = null; upperLeft = null; upperRight = null;

            if (i - 1 >= 0 && j - 1 >= 0)
                upperLeft = solution[i - 1, j - 1];
            if (j - 1 >= 0)
                left = solution[i, j - 1];
            if (i - 1 >= 0)
                upper = solution[i - 1, j];
            if (i - 1 >= 0 && j + 1 < solution.GetLength(0))
                upperRight = solution[i - 1, j + 1];
        }
        private static List<Board> LoadAllboards(string input)
        {
            var boards = new List<Board>();
            using(StreamReader stream = new StreamReader(input))
            {
                int testCaseCount = Int32.Parse(stream.ReadLine());
                for(int t = 0 ; t < testCaseCount; t++)
                {
                    int n, k;
                    string nk = stream.ReadLine();
                    n = Int32.Parse(nk.Split(' ')[0]);
                    k = Int32.Parse(nk.Split(' ')[1]);
                    Board currentBoard = new Board(n, k, t);
                    for (int i = 0; i < n; i++)
                    {
                        string currentLine = stream.ReadLine();
                        currentLine = currentLine.Replace(".", "");
                        int j = 0;
                        foreach(char currentPlayer in currentLine.Reverse())
                        {
                            currentBoard.SetPlayer(
                                currentBoard.N - j - 1,
                                currentBoard.N - i - 1,
                                currentPlayer);
                            j++;
                        }
                    }
                    boards.Add(currentBoard);
                }
            }
            return boards;
        }
        static string PrintBoard(Board b)
        {
            string s = string.Empty;
            for (int i = 0; i < b.N; i++)
            {
                for (int j = 0; j < b.N; j++)
                {
                    s += (b.PlayerAt(i, j) == '\0' ? ' ' : b.PlayerAt(i, j));
                }
                s += '\n';
            }
            return s;
        }
    }
}
