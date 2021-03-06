using System.Runtime.InteropServices;
using EntitiesBT.Core;
using EntitiesBT.Entities;

namespace EntitiesBT.Nodes
{
    [StructLayout(LayoutKind.Explicit)]
    [BehaviorNode("64E0DAFB-20E2-4DF4-910E-ADFA831DB8A9", BehaviorNodeType.Decorate)]
    public struct RunOnMainThreadNode : INodeData
    {
        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var behaviorTreeElement = ref bb.GetDataRef<BehaviorTreeBufferElement>();
            if (behaviorTreeElement.RuntimeThread == BehaviorTreeRuntimeThread.ForceJobThread
                || behaviorTreeElement.RuntimeThread == BehaviorTreeRuntimeThread.ForceMainThread)
                return index.TickChildrenReturnFirstOrDefault(ref blob, ref bb);
            
            if (behaviorTreeElement.RuntimeThread == BehaviorTreeRuntimeThread.JobThread)
            {
                behaviorTreeElement.RuntimeThread = BehaviorTreeRuntimeThread.MainThread;
                return NodeState.Running;
            }
            var state = index.TickChildrenReturnFirstOrDefault(ref blob, ref bb);
            if (state != NodeState.Running) behaviorTreeElement.RuntimeThread = BehaviorTreeRuntimeThread.JobThread;
            return state;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}
