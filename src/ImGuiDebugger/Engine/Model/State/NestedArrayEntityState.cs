namespace Divine.Plugin.Engine.Model.State;

using System;
using System.Collections;
using System.Collections.Generic;

using Divine.Plugin.Engine.IO.Source2.Fields;
using Divine.Plugin.Engine.Model.Source2;

internal sealed class NestedArrayEntityState : IEntityState, IArrayEntityState
{
    private static readonly object[] EMPTY_STATE = Array.Empty<object>();

    private readonly SerializerField rootField;
    private readonly List<Entry?> entries;

    private Queue<int>? freeEntries;

    private bool capacityChanged;

    public NestedArrayEntityState(SerializerField field)
    {
        rootField = field;
        entries = new(20)
        {
            new(this)
        };
    }

    private NestedArrayEntityState(NestedArrayEntityState other)
    {
        rootField = other.rootField;
        int otherSize = other.entries.Count;
        entries = new(otherSize + 4);

        for (int i = 0; i < otherSize; i++)
        {
            var e = other.entries[i];
            if (e == null)
            {
                entries.Add(null);
                markFree(i);
            }
            else
            {
                bool modifiable = e.state.Length == 0;
                e.modifiable = modifiable;
                entries.Add(new(this, e.state, modifiable));
            }
        }
    }

    private Entry rootEntry()
    {
        return entries[0]!;
    }

    public int length()
    {
        return rootEntry().length();
    }

    public bool has(int idx)
    {
        return rootEntry().has(idx);
    }

    public object? get(int idx)
    {
        return rootEntry().get(idx);
    }

    public void set(int idx, object value)
    {
        rootEntry().set(idx, value);
    }

    public void clear(int idx)
    {
        rootEntry().clear(idx);
    }

    public bool isSub(int idx)
    {
        return rootEntry().isSub(idx);
    }

    public IArrayEntityState sub(int idx)
    {
        return rootEntry().sub(idx);
    }

    public IArrayEntityState capacity(int wantedSize, bool shrinkIfNeeded)
    {
        return rootEntry().capacity(wantedSize, shrinkIfNeeded);
    }

    public IEntityState copy()
    {
        return new NestedArrayEntityState(this);
    }

    public bool setValueForFieldPath(IFieldPath fp, object value)
    {
        Field field = rootField;
        Entry entry = rootEntry();
        int last = fp.last();

        capacityChanged = false;
        int i = 0;
        while (true)
        {
            int idx = fp.get(i);
            if (!entry.has(idx))
            {
                field.ensureArrayEntityStateCapacity(entry, idx + 1);
            }
            field = field.getChild(idx);
            if (i == last)
            {
                field.setArrayEntityState(entry, idx, value);
                return capacityChanged;
            }
            entry = entry.subEntry(idx);
            i++;
        }
    }

    public T getValueForFieldPath<T>(IFieldPath fp)
    {
        Field field = rootField;
        Entry entry = rootEntry();
        int last = fp.last();

        int i = 0;
        while (true)
        {
            int idx = fp.get(i);
            field = field.getChild(idx);
            if (i == last)
            {
                return (T)field.getArrayEntityState(entry, idx);
            }
            if (!entry.isSub(idx))
            {
                return default;
            }
            entry = entry.subEntry(idx);
            i++;
        }
    }

    public IEnumerator<IFieldPath> fieldPathIterator()
    {
        return new NestedArrayEntityStateIterator(rootEntry());
    }

    private EntryRef createEntryRef(Entry entry)
    {
        int i;
        if (freeEntries == null || freeEntries.Count == 0)
        {
            i = entries.Count;
            entries.Add(entry);
        }
        else
        {
            i = freeEntries.Dequeue();
            entries[i] = entry;
        }
        return new EntryRef(i);
    }

    private void clearEntryRef(EntryRef entryRef)
    {
        entries[entryRef.idx] = null;
        markFree(entryRef.idx);
    }

    private void markFree(int i)
    {
        freeEntries ??= new();
        freeEntries.Enqueue(i);
    }

    private class EntryRef
    {
        public readonly int idx;

        public EntryRef(int idx)
        {
            this.idx = idx;
        }

        public override string ToString()
        {
            return "EntryRef[" + idx + "]";
        }
    }

    public sealed class Entry : IArrayEntityState
    {
        private readonly NestedArrayEntityState parent;
        public object[] state;
        public bool modifiable;

        public Entry(NestedArrayEntityState parent)
            : this(parent, EMPTY_STATE, true)
        {
        }

        public Entry(NestedArrayEntityState parent, object[] state, bool modifiable)
        {
            this.parent = parent;
            this.state = state;
            this.modifiable = modifiable;
        }

        public int length()
        {
            return state.Length;
        }

        public bool has(int idx)
        {
            return state.Length > idx && state[idx] != null;
        }

        public object? get(int idx)
        {
            return state.Length > idx ? state[idx] : null;
        }

        public void set(int idx, object value)
        {
            if (!modifiable)
            {
                object[] newState = new object[state.Length];
                Array.Copy(state, 0, newState, 0, state.Length);
                state = newState;
                modifiable = true;
            }

            if (state[idx] is EntryRef entryRef)
            {
                parent.clearEntryRef(entryRef);
            }

            state[idx] = value;
        }

        public void clear(int idx)
        {
            set(idx, null);
        }

        public bool isSub(int idx)
        {
            return has(idx) && get(idx) is EntryRef;
        }

        public IArrayEntityState sub(int idx)
        {
            return subEntry(idx);
        }

        public Entry subEntry(int idx)
        {
            if (!isSub(idx))
            {
                set(idx, parent.createEntryRef(new Entry(parent)));
            }

            var entryRef = (EntryRef)get(idx)!;
            return parent.entries[entryRef.idx];
        }

        public IArrayEntityState capacity(int wantedSize, bool shrinkIfNeeded)
        {
            int curSize = state.Length;
            if (wantedSize == curSize)
            {
                return this;
            }
            if (wantedSize < 0)
            {
                // TODO: sometimes negative - figure out what this means
                return this;
            }
            if (wantedSize == 0xFFFFFE0)
            {
                // TODO: 7.28 hotfix - figure out what's going on
                return this;
            }

            object[]? newState = null;
            if (wantedSize > curSize)
            {
                newState = new object[wantedSize];
            }
            else if (shrinkIfNeeded)
            {
                newState = wantedSize == 0 ? EMPTY_STATE : new object[wantedSize];
            }

            if (newState != null)
            {
                Array.Copy(state, 0, newState, 0, Math.Min(curSize, wantedSize));
                state = newState;
                modifiable = true;
                parent.capacityChanged = true;
            }
            return this;
        }

        public override string ToString()
        {
            return $"Entry[modifiable={modifiable}, size={state.Length}]";
        }
    }

    internal sealed class NestedArrayEntityStateIterator : IEnumerator<IFieldPath>
    {
        private readonly Entry[] entry = new Entry[LongFieldPathFormat.MAX_FIELDPATH_LENGTH];
        private readonly IModifiableFieldPath fp = IModifiableFieldPath.newInstance();
        private IFieldPath? next;

        public NestedArrayEntityStateIterator(Entry rootEntry)
        {
            entry[0] = rootEntry;
            fp.inc(1);
            next = advance();
        }

        private IFieldPath? advance()
        {
            while (true)
            {
                int last = fp.last();
                Entry e = entry[last];
                int idx = fp.get(last);
                if (e.length() <= idx)
                {
                    if (last == 0)
                    {
                        return null;
                    }
                    fp.up(1);
                    fp.inc(1);
                    continue;
                }

                if (!e.has(idx))
                {
                    fp.inc(1);
                    continue;
                }

                if (e.isSub(idx))
                {
                    entry[last + 1] = (Entry)e.sub(idx);
                    fp.down();
                    continue;
                }

                IFieldPath result = fp.unmodifiable();
                fp.inc(1);
                return result;
            }
        }

        public IFieldPath Current => next!;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            next = advance();
            return next != null;
        }

        public void Reset()
        {
        }
    }
}