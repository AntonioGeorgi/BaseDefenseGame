using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct GameBootstrapSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameBootstrapConfig>();
    }

    public void OnUpdate(ref SystemState state)
    {
        Entity configEntity = SystemAPI.GetSingletonEntity<GameBootstrapConfig>();
        if (state.EntityManager.HasComponent<GameInitialized>(configEntity))
        {
            return;
        }

        GameBootstrapConfig config = SystemAPI.GetSingleton<GameBootstrapConfig>();
        int actorsPerRow = math.max(1, config.ActorsPerRow);
        int actorCount = math.max(0, config.ActorCount);
        float spacing = math.max(0.1f, config.Spacing);

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        for (int index = 0; index < actorCount; index++)
        {
            int column = index % actorsPerRow;
            int row = index / actorsPerRow;
            float3 spawnPosition = config.SpawnOrigin
                + new float3(column * spacing, 0f, row * spacing);

            Entity actor = commandBuffer.Instantiate(config.MovingActorPrefab);
            commandBuffer.SetComponent(actor, LocalTransform.FromPosition(spawnPosition));
            commandBuffer.SetComponent(actor, new MoveTarget
            {
                TargetPosition = config.TargetPosition
            });
        }

        commandBuffer.AddComponent<GameInitialized>(configEntity);
        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}
