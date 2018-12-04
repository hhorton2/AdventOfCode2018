using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AdventOfCode2018.Blocks;
using AdventOfCode2018.Interfaces;

namespace AdventOfCode2018.Solvers
{
    public class DayOneSolver : ISolver
    {
        public void SolvePartOne()
        {
            var readBlock = CommonBlockFactory.GetLinesFromTextFile();
            var flattenBlock =
                new TransformManyBlock<string[], string>(lines => lines, CommonBlockFactory.UnorderedBlockOptions);
            var parseBuffer = new BufferBlock<string>(CommonBlockFactory.UnorderedBlockOptions);
            var parseBlock =
                new TransformBlock<string, int>(async stringValue => await Task.Run(() => int.Parse(stringValue)),
                    CommonBlockFactory.UnorderedBlockOptions);
            var sum = 0;
            var sumBlock = new ActionBlock<int>(async num => await Task.Run(() => Interlocked.Add(ref sum, num)),
                CommonBlockFactory.UnorderedBlockOptions);

            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
            readBlock.LinkTo(flattenBlock, linkOptions);
            flattenBlock.LinkTo(parseBuffer, linkOptions);
            parseBuffer.LinkTo(parseBlock, linkOptions);
            parseBlock.LinkTo(sumBlock, linkOptions);
            readBlock.Post(
                $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\InputFiles\\day1\\partOne.txt");
            readBlock.Complete();
            sumBlock.Completion.Wait();
            Console.WriteLine(sum);
        }

        public void SolvePartTwo()
        {
            var readBlock = CommonBlockFactory.GetLinesFromTextFile();
            var flattenBlock =
                new TransformManyBlock<string[], string>(lines => lines, CommonBlockFactory.OrderedBlockOptions);
            var parseBuffer = new BufferBlock<string>(CommonBlockFactory.OrderedBlockOptions);
            var parseBlock =
                new TransformBlock<string, int>(stringValue => int.Parse(stringValue),
                    CommonBlockFactory.OrderedBlockOptions);
            var sum = 0;
            var dictionary = new Dictionary<int, int>();
            dictionary.TryAdd(0, 0);
            var sumBuffer = new BufferBlock<int>(CommonBlockFactory.OrderedBlockOptions);
            bool found;
            var sumBlock = new ActionBlock<int>(num =>
                {
                    sum += num;
                    found = !dictionary.TryAdd(sum, sum);
                    if (!found)
                    {
                        sumBuffer.Post(num);
                    }
                    else
                    {
                        sumBuffer.Complete();
                    }
                },
                new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1});

            var linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
            readBlock.LinkTo(flattenBlock, linkOptions);
            flattenBlock.LinkTo(parseBuffer, linkOptions);
            parseBuffer.LinkTo(parseBlock, linkOptions);
            parseBlock.LinkTo(sumBlock);
            sumBuffer.LinkTo(sumBlock, linkOptions);
            readBlock.Post(
                $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\InputFiles\\day1\\partOne.txt");
            readBlock.Complete();
            sumBlock.Completion.Wait();
            Console.WriteLine(sum);
        }
    }
}