using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Entities;

namespace EntitiesBT
{
    [UpdateBefore(typeof(VirtualMachineSystem))]
    public class BlackboardTickDeltaTimeSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var deltaTime = TimeSpan.FromSeconds(Time.DeltaTime);
            Entities.ForEach((BlackboardComponent bb, ref TickDeltaTime dt) =>
            {
                dt.Value += deltaTime;
            });
        }
    }
}
