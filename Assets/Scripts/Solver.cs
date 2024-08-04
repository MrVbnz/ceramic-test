using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Jobs;
using System.Linq;
using UnityEngine.Serialization;

public class Solver : MonoBehaviour
{
    [SerializeField] private TextAsset modelAsset;
    [SerializeField] private TextAsset spaceAsset;
    [SerializeField] private GameObject spacePrefab;
    [SerializeField] private GameObject modelPrefab;

    private enum Stage
    {
        NotLoaded,
        Loaded,
        Solved
    }

    private NativeArray<Matrix4x4> models;
    private NativeArray<Matrix4x4> spaces;
    private NativeArray<Solution> solutions;

    private Stage stage = Stage.NotLoaded;

    private List<Transform> spaceTransforms;
    private List<Transform> modelTransforms;

    private void OnGUI()
    {
        switch (stage)
        {
            case Stage.NotLoaded:
                if (GUILayout.Button("Load"))
                {
                    Load();
                    SpawnSpaces();
                }

                break;
            case Stage.Loaded:
                if (GUILayout.Button("Solve"))
                    Solve();
                break;
            case Stage.Solved: 
                ShowSolutions();
                break;
        }
    }

    private void Load()
    {
        var comparer = new MatrixComparer();

        var modelArray = JsonConvert.DeserializeObject<Matrix4x4[]>(modelAsset.text);
        var spaceArray = JsonConvert.DeserializeObject<Matrix4x4[]>(spaceAsset.text);

        models = new NativeArray<Matrix4x4>(modelArray, Allocator.Persistent);
        spaces = new NativeArray<Matrix4x4>(spaceArray, Allocator.Persistent);
        solutions = new NativeArray<Solution>(spaces.Length, Allocator.Persistent);

        spaceTransforms = new List<Transform>();
        modelTransforms = new List<Transform>();

        stage = Stage.Loaded;
    }

    private void SpawnSpaces()
    {
        foreach (var space in spaceTransforms)
            Destroy(space.gameObject);
        spaceTransforms.Clear();
        
        foreach (var space in spaces)
        {
            var instance = Instantiate(spacePrefab, transform);
            var t = instance.transform;
            t.SetPositionAndRotation(space.GetPosition(), space.rotation);
            t.localScale = space.lossyScale;

            spaceTransforms.Add(t);
        }
    }

    private void Solve()
    {
        var job = new SolverJob()
        {
            TestModelInverse = models.First(m => m.determinant != 0).inverse,
            Models = models,
            Spaces = spaces,
            Solutions = solutions,
        };

        var tStart = Time.realtimeSinceStartupAsDouble;
        var jobHandle = job.Schedule(solutions.Length, 32);
        jobHandle.Complete();
        var dTime = Time.realtimeSinceStartupAsDouble - tStart;

        Debug.Log($"Total time: {dTime} s.");
        Debug.Log($"Found {solutions.Count(s => s.IsValid)} solutions");

        stage = Stage.Solved;
    }

    private void ShowSolutions()
    {
  
        
        var index = 0;
        foreach (var solution in solutions.Where(s => s.IsValid))
        {
            if (GUILayout.Button(index.ToString()))
                SpawnModels(solution.Offset);
            index++;
        }
    }

    private void SpawnModels(Matrix4x4 offset)
    {
        foreach (var model in modelTransforms)
            Destroy(model.gameObject);
        modelTransforms.Clear();
        
        foreach (var model in models)
        {
            var instance = Instantiate(modelPrefab, transform);
            var offsetModel = offset * model;
            var t = instance.transform;
            t.SetPositionAndRotation(offsetModel.GetPosition(), offsetModel.rotation);
            t.localScale = offsetModel.lossyScale;

            modelTransforms.Add(t);
        }
    }

    ~Solver()
    {
        models.Dispose();
        spaces.Dispose();
        solutions.Dispose();
    }
}

public class MatrixComparer : IEqualityComparer<Matrix4x4>
{
    public const float MatrixEqualityThreshold = 0.01f;
    
    public bool Equals(Matrix4x4 a, Matrix4x4 b) =>
        MatrixDifference(a, b) < MatrixEqualityThreshold;

    public int GetHashCode(Matrix4x4 a) => 0;

    public static float MatrixDifference(Matrix4x4 a, Matrix4x4 b)
    {
        var totalDifference = 0f;
        for (var i = 0; i < 9; i++)
            totalDifference += Mathf.Abs(a[i] - b[i]);
        return totalDifference;
    }
}