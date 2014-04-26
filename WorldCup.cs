using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeJam
{
    class WorldCup
    {
        static int maxP = 11;
        static int[] teamsCanMiss = new int[2 << maxP];
        static int[] matchesCost = new int[2 << maxP];
        static int[,] DP = new int[2 << maxP, maxP];
        static int Main()
        {
            string input = "WorldCup.txt";
            using (StreamReader stream = new StreamReader(input))
            {
                int T = Int32.Parse(stream.ReadLine());
                for (int t = 0; t < T; t++)
                {
                    int P = Int32.Parse(stream.ReadLine());

                    int teamId = 1;
                    foreach (string limit in stream.ReadLine().Split(' '))
                        teamsCanMiss[teamId++] = Int32.Parse(limit);

                    int matchId = 1;
                    string costs = string.Empty;
                    for (int i = 0; i < P; i++)
                        costs += stream.ReadLine() + "\n";

                    foreach (string costline in costs.Trim().Split('\n').Reverse())
                        foreach(string cost in costline.Split(' '))
                            matchesCost[matchId++] = Int32.Parse(cost);

                    for (teamId = 1; teamId < (2 << P); teamId++)
                    {
                        int currentTeam = (2 << (P - 1)) + teamId - 1;
                        int minimumValidTicketsCount = P - teamsCanMiss[teamId];
                        for (int ticketsNeeded = 0; ticketsNeeded <= P; ticketsNeeded++)
                            if (ticketsNeeded < minimumValidTicketsCount)
                                DP[currentTeam, ticketsNeeded] = 1000000;
                            else
                                DP[currentTeam, ticketsNeeded] = 0;

                    }

                    for (matchId = (2 << (P  - 1)) -1; matchId > 0; matchId--)
                    {
                        for (int ticketsNeeded = 0; ticketsNeeded < P; ticketsNeeded++)
                            DP[matchId, ticketsNeeded] = Math.Min(
                                DP[matchId << 1, ticketsNeeded] + DP[(matchId << 1) + 1, ticketsNeeded],
                                DP[matchId << 1, ticketsNeeded + 1] + DP[(matchId << 1) + 1, ticketsNeeded + 1] + matchesCost[matchId]);
                    }

                    Console.WriteLine("Case #" + (t + 1) + ": " + DP[1, 0]);
                }
                return 0;
            }
        }

        static string[] DebugPrintAll(int[,] a, int n, int m)
        {
            var s = new List<string>();
            s.Add("");
            for(int i = 1; i <= n; i++)
            {
                string x = string.Empty;
                for(int j = 0; j < m; j++)
                {
                    x += a[i, j] + " ";
                }
                s.Add(x);
            }
            return s.ToArray();
        }
    }
}
