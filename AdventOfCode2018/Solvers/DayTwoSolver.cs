using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AdventOfCode2018.Blocks;
using AdventOfCode2018.Helpers;
using AdventOfCode2018.Interfaces;

namespace AdventOfCode2018.Solvers
{
    public class DayTwoSolver : ISolver
    {
        public void SolvePartOne()
        {
            var readBlock = CommonBlockFactory.GetLinesFromTextFile();
            var flattener = CommonBlockFactory.GetFlattenBlock<string>();
            var lineBuffer = new BufferBlock<string>(CommonBlockFactory.UnorderedBlockOptions);
            var threeCountBuffer = new BufferBlock<char>(CommonBlockFactory.UnorderedBlockOptions);
            var twoCountBuffer = new BufferBlock<char>(CommonBlockFactory.UnorderedBlockOptions);
            var letterCountBlock = new ActionBlock<string>(s =>
            {
                var d = new Dictionary<char, int>();
                foreach (var c in s)
                {
                    if (!d.TryAdd(c, 1))
                    {
                        d[c]++;
                    }
                }

                var threeFound = false;
                var twoFound = false;
                foreach (var kvp in d)
                {
                    if (twoFound && threeFound)
                    {
                        break;
                    }

                    switch (kvp.Value)
                    {
                        case 3 when !threeFound:
                            threeFound = true;
                            threeCountBuffer.Post(kvp.Key);
                            break;
                        case 2 when !twoFound:
                            twoFound = true;
                            twoCountBuffer.Post(kvp.Key);
                            break;
                    }
                }
            }, CommonBlockFactory.UnorderedBlockOptions);
            var twoCount = 0;
            var threeCount = 0;
            var twoCountBlock = new ActionBlock<char>(c => { Interlocked.Increment(ref twoCount); },
                CommonBlockFactory.UnorderedBlockOptions);
            var threeCountBlock = new ActionBlock<char>(c => { Interlocked.Increment(ref threeCount); },
                CommonBlockFactory.UnorderedBlockOptions);

            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
            readBlock.LinkTo(flattener, linkOptions);
            flattener.LinkTo(lineBuffer, linkOptions);
            lineBuffer.LinkTo(letterCountBlock, linkOptions);
            twoCountBuffer.LinkTo(twoCountBlock, linkOptions);
            threeCountBuffer.LinkTo(threeCountBlock, linkOptions);
            readBlock.Post(SharedFunctions.GetCurrentWorkingDirectory("InputFiles\\day2\\partOne.txt"));
            readBlock.Complete();
            letterCountBlock.Completion.Wait();
            twoCountBuffer.Complete();
            threeCountBuffer.Complete();
            Task.WhenAll(twoCountBlock.Completion, threeCountBlock.Completion).Wait();
            Console.WriteLine(twoCount * threeCount);
        }

        public void SolvePartTwo()
        {
            var readBlock = CommonBlockFactory.GetLinesFromTextFile();
            var wordPairBuffer = new BufferBlock<Tuple<string, string>>(CommonBlockFactory.UnorderedBlockOptions);
            var pairProducer = new ActionBlock<string[]>(s =>
            {
                for (var i = 0; i < s.Length / 2; i++)
                {
                    for (var j = s.Length-1; j > s.Length / 2; j--)
                    {
                        wordPairBuffer.Post(new Tuple<string, string>(s[i], s[j]));
                    }
                }
            }, CommonBlockFactory.UnorderedBlockOptions);
            var wordCompareBlock = new ActionBlock<Tuple<string, string>>(tuple =>
            {
                var misses = 0;
                var wrongIndex = 0;
                for (var i = 0; i < tuple.Item1.Length; i++)
                {
                    if (misses > 1)
                    {
                        break;
                    }

                    if (tuple.Item1[i] == tuple.Item2[i]) continue;
                    wrongIndex = i;
                    misses++;
                }

                if (misses == 1)
                {
                    pairProducer.Complete();
                    wordPairBuffer.Complete();
                    Console.WriteLine($"{tuple.Item1} | {tuple.Item2} | {tuple.Item1.Remove(wrongIndex, 1)}");
                }
            }, CommonBlockFactory.UnorderedBlockOptions);
            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
            readBlock.LinkTo(pairProducer, linkOptions);
            wordPairBuffer.LinkTo(wordCompareBlock, linkOptions);
            readBlock.Post(SharedFunctions.GetCurrentWorkingDirectory("InputFiles\\day2\\partOne.txt"));
            readBlock.Complete();
            wordCompareBlock.Completion.Wait();
        }
    }
}