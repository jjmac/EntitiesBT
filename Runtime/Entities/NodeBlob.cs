using System;
using EntitiesBT.Core;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

namespace EntitiesBT.Entities
{
    public struct NodeBlob
    {
        public const int VERSION = 2;

#region Serialized Data
        public BlobArray<int> Types;
        public BlobArray<int> EndIndices;
        public BlobArray<int> Offsets; // count = count of nodes + 1
        public BlobArray<byte> DefaultDataBlob;
#endregion
        
#region NonSerialized Data (runtime only)
        public BlobArray<NodeState> States; // states of each nodes
        public BlobArray<byte> RuntimeDataBlob; // initialize from `DefaultDataBlob`
#endregion

        public int Count => Types.Length;

        [Pure]
        public static int CalculateDefaultSize(int count, int dataSize) =>
            UnsafeUtility.SizeOf<NodeBlob>() + dataSize + sizeof(int) * count * 3 /* Types/EndIndices/Offsets */;
        
        [Pure]
        public static int CalculateRuntimeSize(int count, int dataSize) =>
            dataSize/* RuntimeDataBlob */ + sizeof(NodeState) * count;

        [Pure]
        public static int CalculateSize(int count, int dataSize) =>
            CalculateDefaultSize(count, dataSize) + CalculateRuntimeSize(count, dataSize);
    }

    public readonly struct NodeBlobRef : INodeBlob, IEquatable<NodeBlobRef>
    {
        private ref NodeBlob _blob => ref BlobRef.Value;
        public readonly BlobAssetReference<NodeBlob> BlobRef;
        
        public NodeBlobRef(BlobAssetReference<NodeBlob> blobRef) => BlobRef = blobRef;
        
        public int Count => _blob.Count;
        public int GetTypeId(int nodeIndex) => _blob.Types[nodeIndex];
        public int GetEndIndex(int nodeIndex) => _blob.EndIndices[nodeIndex];
        public int GetNodeDataSize(int nodeIndex, int count = 1)
        {
            var currentOffset = _blob.Offsets[nodeIndex];
            var nextOffset = _blob.Offsets[math.min(nodeIndex + count, Count)];
            return nextOffset - currentOffset;
        }

        public unsafe void ResetStates(int index, int count = 1) =>
            UnsafeUtility.MemClear((byte*)_blob.States.GetUnsafePtr() + sizeof(NodeState) * index, sizeof(NodeState) * count);

        public unsafe IntPtr GetDefaultDataPtr(int nodeIndex) =>
            (IntPtr) _blob.DefaultDataBlob.GetUnsafePtr() + _blob.Offsets[nodeIndex];
        
        public unsafe IntPtr GetRuntimeDataPtr(int nodeIndex) =>
            (IntPtr) _blob.RuntimeDataBlob.GetUnsafePtr() + _blob.Offsets[nodeIndex];

        public NodeState GetState(int nodeIndex) => _blob.States[nodeIndex];
        public void SetState(int nodeIndex, NodeState state) => _blob.States[nodeIndex] = state;

        public bool Equals(NodeBlobRef other)
        {
            return BlobRef.Equals(other.BlobRef);
        }

        public override bool Equals(object obj)
        {
            return obj is NodeBlobRef other && Equals(other);
        }

        public override int GetHashCode()
        {
            return BlobRef.GetHashCode();
        }
        
        public static bool operator==(NodeBlobRef lhs, NodeBlobRef rhs)
        {
            return lhs.BlobRef == rhs.BlobRef;
        }

        public static bool operator !=(NodeBlobRef lhs, NodeBlobRef rhs)
        {
            return !(lhs == rhs);
        }
    }
}
