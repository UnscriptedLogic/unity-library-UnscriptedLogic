using System;
using UnityEngine;
using System.Collections.Generic;
using UnscriptedLogic.MathUtils;

namespace UnscriptedLogic.WaveSystems.Infinite
{
    [Serializable]
    public class SpawnerSettings
    {
        //Point Settings
        public int startPoints;

        //Wave Settings
        public float startInterval;
        public float spawnInterval;
        public float waveInterval;
        public int maximumWaves;
        public bool isInfinite => maximumWaves == -1;

        [Space(5)]

        public List<Spawnable> spawnables;

        public SpawnerSettings(int startPoints, float startInterval, float spawnInterval, float waveInterval, int maximumWaves, List<Spawnable> spawnables)
        {
            this.startPoints = startPoints;
            this.startInterval = startInterval;
            this.spawnInterval = spawnInterval;
            this.waveInterval = waveInterval;
            this.maximumWaves = maximumWaves;
            this.spawnables = spawnables;
        }
    }

    public enum SpawnerState
    {
        Starting,
        Spawning,
        Intermission,
        Completed
    }

    [Serializable]
    public class Spawnable
    {
        [SerializeField] private GameObject spawnable;
        [SerializeField] private int cost;
        [SerializeField] private Vector2 withinWaves;

        public Spawnable(GameObject spawnable, int cost, Vector2 withinWaves)
        {
            this.spawnable = spawnable;
            this.cost = cost;
            this.withinWaves = withinWaves;
        }

        public GameObject? SpawnablePrefab => spawnable;
        public int Cost => cost;
        public Vector2 WithinWaves => withinWaves;
    }

    public class PointBasedWaveSystem
    {
        //Components
        public SpawnerState spawnerState { get; private set; }
        private List<Spawnable> spawnables = new List<Spawnable>();
        private List<Spawnable> spawningSet = new List<Spawnable>();

        //Timers
        private SpawnerSettings spawnerSettings;
        public float interval { get; private set; }

        //Counters
        public int points;
        public int currentWave { get; private set; }
        public int currentSpawnIndex { get; private set; }

        //Conditions
        public bool debugSpawner;
        public bool running { get; private set; }
        public bool isLastWave => currentWave >= spawnerSettings.maximumWaves;
        public bool hasCompletedSpawnSet => currentSpawnIndex >= spawningSet.Count;

        //Displayers
        public string? GetStateName => Enum.GetName(typeof(SpawnerState), spawnerState);

        //Events
        public Action<int, List<Spawnable>>? OnNewWaveStarted;
        public Action<SpawnerState, SpawnerState>? OnStateChanged;
        public Action<List<Spawnable>>? OnSetInitialized;
        public Action<GameObject?>? Spawn;
        public Action OnCompleted;

        public PointBasedWaveSystem(SpawnerSettings spawnerSettings, Action<GameObject?> Spawn, Action OnCompleted, bool start = false)
        {
            points = spawnerSettings.startPoints;
            this.spawnerSettings = spawnerSettings;
            spawnables = spawnerSettings.spawnables;
            this.Spawn = Spawn;
            this.OnCompleted = OnCompleted;

            if (start)
                BeginSpawner();
        }

        public void BeginSpawner()
        {
            spawnerState = SpawnerState.Starting;
            EnterState();

            running = true;
        }

        public void Pause() => running = false;
        public void Continue() => running = true;

        private void EnterState()
        {
            switch (spawnerState)
            {
                case SpawnerState.Starting:
                    interval = spawnerSettings.startInterval;
                    currentSpawnIndex = 0;
                    currentWave = 0;
                    break;
                case SpawnerState.Spawning:
                    currentWave++;
                    interval = 0f;
                    currentSpawnIndex = 0;
                    spawningSet = GetSpawnList(spawnables, points, currentWave);

                    OnNewWaveStarted?.Invoke(currentWave, spawningSet);
                    break;
                case SpawnerState.Intermission:

                    //'isLastWave' should and will never be -1 when doing infinite
                    //but for the sake of clarity I shall add this check too
                    if (!spawnerSettings.isInfinite)
                    {
                        if (isLastWave)
                        {
                            ChangeState(SpawnerState.Completed);
                            return;
                        }
                    }

                    interval = spawnerSettings.waveInterval;
                    break;
                case SpawnerState.Completed:
                    OnCompleted?.Invoke();
                    break;
                default:
                    break;
            }
        }

        public void UpdateSpawner()
        {
            if (!running)
                return;

            switch (spawnerState)
            {
                case SpawnerState.Starting:

                    interval -= Time.deltaTime;
                    if (interval <= 0f)
                    {
                        ChangeState(SpawnerState.Spawning);
                        return;
                    }

                    break;
                case SpawnerState.Spawning:

                    if (interval <= 0f)
                    {
                        if (hasCompletedSpawnSet)
                        {
                            ChangeState(SpawnerState.Intermission);
                            return;
                        }

                        Spawn?.Invoke(spawningSet[currentSpawnIndex].SpawnablePrefab);

                        currentSpawnIndex++;
                        interval = spawnerSettings.spawnInterval;
                    }
                    else
                    {
                        interval -= Time.deltaTime;
                    }

                    break;
                case SpawnerState.Intermission:

                    if (interval <= 0f)
                    {
                        ChangeState(SpawnerState.Spawning);
                        return;
                    }
                    else
                    {
                        interval -= Time.deltaTime;
                    }

                    break;
                case SpawnerState.Completed:
                    break;
                default:
                    break;
            }
        }

        private void ExitState()
        {
            switch (spawnerState)
            {
                case SpawnerState.Starting:
                    break;
                case SpawnerState.Spawning:
                    break;
                case SpawnerState.Intermission:
                    break;
                case SpawnerState.Completed:
                    break;
                default:
                    break;
            }
        }

        public void ChangeState(SpawnerState newState)
        {
            ExitState();
            spawnerState = newState;
            EnterState();
        }

        private List<Spawnable> GetValidSpawnSet(List<Spawnable> localSpawnables, int pointsToUse, int waveIndex)
        {
            List<Spawnable> newspawnables = new List<Spawnable>();

            for (int i = 0; i < localSpawnables.Count; i++)
            {
                if (IsValidSpawnable(localSpawnables[i], pointsToUse, waveIndex))
                {
                    newspawnables.Add(localSpawnables[i]);
                }
            }

            return newspawnables;
        }

        private List<Spawnable> GetSpawnList(List<Spawnable> spawnableList, int points, int waveindex)
        {
            List<Spawnable> finalSpawnlist = new List<Spawnable>();

            //Changing values
            int availablePoints = points;
            List<Spawnable> temporarySpawnables = spawnableList;

            while (points != 0 && temporarySpawnables.Count > 0)
            {
                List<Spawnable> spawnSet = GetValidSpawnSet(temporarySpawnables, availablePoints, waveindex);

                if (spawnSet.Count == 0)
                    break;

                Spawnable selected = RandomLogic.FromList(spawnSet);
                availablePoints -= selected.Cost;
                finalSpawnlist.Add(selected);

                //On the next iteration, our cost will have been reduced. This will help us filter out the ones which are no longer affordable
                //This also helps us in breaking the loop in case we have left over points and no spawnable to spend it on
                temporarySpawnables = spawnSet;
            }

            OnSetInitialized?.Invoke(finalSpawnlist);
            return finalSpawnlist;
        }

        private bool IsValidSpawnable(Spawnable spawnable, int points, int currentWave)
        {
            //Not enough points
            if (spawnable.Cost > points)
            {
                return false;
            }

            //not greater than wave minimum
            if (spawnable.WithinWaves.x > currentWave)
            {
                return false;
            }

            //not less than wave maximum
            if (spawnable.WithinWaves.y < currentWave)
            {
                return false;
            }

            return true;
        }
    }
}