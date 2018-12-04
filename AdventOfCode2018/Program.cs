using System;
using System.Diagnostics;
using AdventOfCode2018.Solvers;

namespace AdventOfCode2018
{
    class Program
    {
        static void Main(string[] args)
        {
            var solver = new DayTwoSolver();
            var s = new Stopwatch();
            s.Start();
            solver.SolvePartOne();
            s.Stop();
            Console.WriteLine(s.Elapsed.ToString());
            
            s = new Stopwatch();
            s.Start();
            solver.SolvePartTwo();
            s.Stop();
            Console.WriteLine(s.Elapsed.ToString());
        }
    }
}