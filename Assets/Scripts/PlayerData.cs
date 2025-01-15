using System;
using Unity.Netcode;

[Serializable]
public struct PlayerData: IEquatable<PlayerData>, INetworkSerializable
{
    public ulong ClientId;
    public int ColorId;

    public bool Equals(PlayerData other)
    {
        return ClientId == other.ClientId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
    }
}
