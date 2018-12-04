using System;
using System.Diagnostics;
using AdventOfCode2018.Solvers;

namespace AdventOfCode2018
{
    class Program
    {
        static void Main(string[] args)
        {
            var dayOne = new DayOneSolver();
            var s = new Stopwatch();
            s.Start();
            dayOne.SolvePartOne();
            s.Stop();
            Console.WriteLine(s.Elapsed.ToString());
            
            s = new Stopwatch();
            s.Start();
            dayOne.SolvePartTwo();
            s.Stop();
            Console.WriteLine(s.Elapsed.ToString());
        }
    }
}