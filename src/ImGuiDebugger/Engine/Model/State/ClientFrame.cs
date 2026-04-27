namespace Divine.Plugin.Engine.Model.State;

internal sealed class ClientFrame
{
    private readonly NetworkEntity[] entity;

    public ClientFrame(int size)
    {
        this.entity = new NetworkEntity[size];
    }

    public void setEntity(NetworkEntity e)
    {
        int eIdx = e.getIndex();
        this.entity[eIdx] = e;
    }

    public void removeEntity(NetworkEntity e)
    {
        int eIdx = e.getIndex();
        this.entity[eIdx] = null;
    }

    public NetworkEntity getEntity(int eIdx)
    {
        return entity[eIdx];
    }

    public int getSize()
    {
        return entity.Length;
    }

    public Capsule createCapsule()
    {
        return new Capsule(this);
    }

    public sealed class Capsule
    {

        private readonly long[] uid;
        private readonly bool[] active;
        private IEntityState[] state;

        public Capsule(ClientFrame frame)
        {
            int size = frame.getSize();
            uid = new long[size];
            active = new bool[size];
            state = new IEntityState[size];
            for (int i = 0; i < size; i++)
            {
                NetworkEntity e = frame.entity[i];
                uid[i] = e != null ? e.getUid() : -1L;
                active[i] = e != null && e.isActive();
                state[i] = e != null ? e.getState().copy() : null;
            }
        }

        public bool isExistent(int eIdx)
        {
            return uid[eIdx] != -1;
        }

        public bool isActive(int eIdx)
        {
            return active[eIdx];
        }

        public long getUid(int eIdx)
        {
            return uid[eIdx];
        }

        public IEntityState getState(int eIdx)
        {
            return state[eIdx];
        }

    }
}