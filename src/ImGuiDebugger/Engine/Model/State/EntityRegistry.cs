namespace Divine.Plugin.Engine.Model.State;

using System.Collections.Generic;

using Divine.Plugin.Engine.IO.Source2;

internal sealed class EntityRegistry
{
    private readonly Dictionary<long, NetworkEntity> entities = new();

    public NetworkEntity create(int dtClassId, int index, int serial, int handle, DTClass dtClass)
    {
        long uid = NetworkEntity.uid(dtClassId, handle);

        if (!entities.TryGetValue(uid, out var entity))
        {
            entity = new NetworkEntity(index, serial, handle, dtClass);
            entities[uid] = entity;
        }

        entity.setState(null);
        entity.setExistent(false);
        entity.setActive(false);
        return entity;
    }

    public NetworkEntity? get(long uid)
    {
        if (!entities.TryGetValue(uid, out var entity))
        {
            return null;
        }

        return entity;
    }
}