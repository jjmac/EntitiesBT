using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using NodeBlobRef = EntitiesBT.Entities.NodeBlobRef;

namespace EntitiesBT.Sample
{
    public class JobVirtualMachineSystem : JobComponentSystem
    {
        protected override unsafe JobHandle OnUpdate(JobHandle inputDeps)
        {
            var deltaTime = TimeSpan.FromSeconds(Time.DeltaTime);
            var delatTimeIndex = TypeManager.GetTypeIndex<TickDeltaTime>();
            var translationIndex = TypeManager.GetTypeIndex<Translation>();
            Entities.WithoutBurst().ForEach((ref CustomBlackboard bb) =>
            {
                bb.DataMap = new NativeHashMap<int,int>(4, Allocator.TempJob);
            }).Run();
            
            return Entities.ForEach((ref CustomBlackboard bb, ref NodeBlobRef blob, ref Translation translation, ref TickDeltaTime tickDeltaTime) =>
            {
                tickDeltaTime.Value = deltaTime;
                bb.DataMap[delatTimeIndex] = (int)UnsafeUtility.AddressOf(ref tickDeltaTime);
                bb.DataMap[translationIndex] = (int)UnsafeUtility.AddressOf(ref translation);
                // bb.TickDeltaTime = (TickDeltaTime*) UnsafeUtility.AddressOf(ref tickDeltaTime);
                // bb.Translation = (Translation*) UnsafeUtility.AddressOf(ref translation);
                VirtualMachine.Tick(ref blob, ref bb);
            }).Schedule(inputDeps);
        }
    }
}
