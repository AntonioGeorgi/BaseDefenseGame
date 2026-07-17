using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(CommitMovementSystem))]
[UpdateAfter(typeof(SpawnerRandomInitializationSystem))]
public partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer.ParallelWriter commandBuffer = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged)
            .AsParallelWriter();

        new SpawnJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            CommandBuffer = commandBuffer
        }.ScheduleParallel();
    }

    [BurstCompile]
    [WithAll(typeof(SpawnerEnabled))]
    private partial struct SpawnJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter CommandBuffer;

        private void Execute(
            [EntityIndexInQuery] int sortKey,
            ref SpawnerState spawnerState,
            in SpawnerConfig config,
            in LocalTransform transform)
        {
            spawnerState.TimeUntilNextSpawn -= DeltaTime;
            if (spawnerState.TimeUntilNextSpawn > 0f)
            {
                return;
            }

            spawnerState.TimeUntilNextSpawn += config.Interval;

            for (int index = 0; index < config.BatchSize; index++)
            {
                float2 offsetDirection = spawnerState.Random.NextFloat2Direction();
                float radius = math.sqrt(spawnerState.Random.NextFloat())
                    * config.SpawnRadius;
                float3 spawnPosition = transform.Position
                    + new float3(offsetDirection.x * radius, 0f,
                        offsetDirection.y * radius);

                Entity spawnedEntity = CommandBuffer.Instantiate(sortKey, config.Prefab);
                CommandBuffer.SetComponent(sortKey, spawnedEntity,
                    LocalTransform.FromPositionRotationScale(
                        spawnPosition,
                        transform.Rotation,
                        1f));
            }
        }
    }
}
