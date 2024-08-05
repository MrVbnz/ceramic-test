using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Jobs;
using System.Linq;

public static class Solver
{
    public static IEnumerable<Matrix4x4> Solve(CeramicTest test)
    {
        var startTime = Time.realtimeSinceStartupAsDouble;
        
        var spaces = new NativeArray<Matrix4x4>(test.Spaces, Allocator.TempJob);
        var models = new NativeArray<Matrix4x4>(test.Models, Allocator.TempJob);
        var solutions = new NativeArray<Solution>(spaces.Length, Allocator.TempJob);

        var job = new SolverJob()
        {
            TestModelInverse = models.First(m => m.determinant != 0).inverse,
            Models = models,
            Spaces = spaces,
            Solutions = solutions,
        };
        var jobHandle = job.Schedule(solutions.Length, 32);
        jobHandle.Complete();
        var validSolutions = solutions.Where(s => s.IsValid).ToArray();

        models.Dispose();
        spaces.Dispose();
        solutions.Dispose();
        
        var endTime = Time.realtimeSinceStartupAsDouble;

        Debug.Log($"Total time: {endTime - startTime} s.");
        Debug.Log($"Found {validSolutions.Length} solutions");

        return validSolutions.Select(s => s.Offset);
    }
}