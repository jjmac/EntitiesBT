using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;

namespace EntitiesBT.Nodes
{
    [Serializable]
    [BehaviorNode("46540F67-6145-4433-9A3A-E470992B952E", BehaviorNodeType.Decorate)]
    public struct TimerNode : INodeData
    {
        public float CountdownSeconds;
        public NodeState BreakReturnState;

        [ReadOnly(typeof(BehaviorTreeTickDeltaTime))]
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            var childState = index.TickChildrenReturnFirstOrDefault(ref blob, ref blackboard);
            CountdownSeconds -= blackboard.GetData<BehaviorTreeTickDeltaTime>().Value;
            if (CountdownSeconds <= 0f) return childState.IsCompleted() ? childState : BreakReturnState;
            return NodeState.Running;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
