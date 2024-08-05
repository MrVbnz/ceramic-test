using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct SolverJob : IJobParallelFor
{
    [ReadOnly] public Matrix4x4 TestModelInverse;
    [ReadOnly] public NativeArray<Matrix4x4> Models;
    [ReadOnly] public NativeArray<Matrix4x4> Spaces;
    [WriteOnly] public NativeArray<Solution> Solutions;

    public void Execute(int spaceIndex)
    {
        var offset = Spaces[spaceIndex] * TestModelInverse;
        Solutions[spaceIndex] = new Solution
        {
            Offset = offset,
            IsValid = IsValidOffset(offset)
        };
    }

    private bool IsValidOffset(Matrix4x4 offset)
    {
        var modelsLength = Models.Length;
        var spacesLength = Spaces.Length;
        var isValidModel = true;
        for (var modelIndex = 0; modelIndex < modelsLength; modelIndex++)
        {
            var isValidSpace = false;
            for (var spaceIndex = 0; spaceIndex < spacesLength; spaceIndex++)
            {
                if (MatrixComparer.Equals(offset * Models[modelIndex], Spaces[spaceIndex]))
                {
                    isValidSpace = true;
                    break;
                }
            }

            if (!isValidSpace)
            {
                isValidModel = false;
                break;
            }
        }

        return isValidModel;
    }

   
}

public struct Solution
{
    public Matrix4x4 Offset;
    public bool IsValid;
}