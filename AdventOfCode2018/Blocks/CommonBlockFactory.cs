using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks.Dataflow;

namespace AdventOfCode2018.Blocks
{
    public static class CommonBlockFactory
    {
        public static readonly ExecutionDataflowBlockOptions UnorderedBlockOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 16
        };
        public static readonly ExecutionDataflowBlockOptions OrderedBlockOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 16,
            EnsureOrdered = true
        };

        public static TransformBlock<string, string[]> GetLinesFromTextFile()
        {
            return new TransformBlock<string, string[]>(async path => await File.ReadAllLinesAsync(path), UnorderedBlockOptions);
        }

        public static TransformManyBlock<IList<T>, T> GetFlattenBlock<T>()
        {
            return new TransformManyBlock<IList<T>, T>(t => t, UnorderedBlockOptions);
        }
    }
}