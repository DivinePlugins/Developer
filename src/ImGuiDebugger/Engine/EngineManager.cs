namespace Divine.Plugin.Engine;

using System;
using System.Collections.Generic;
using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Network;
using Divine.Network.EventArgs;
using Divine.Network.Net;
using Divine.Plugin.Engine.IO;
using Divine.Plugin.Engine.IO.Source2;
using Divine.Plugin.Engine.Model;
using Divine.Plugin.Engine.Model.Engine;
using Divine.Plugin.Engine.Model.Source2;
using Divine.Plugin.Engine.Model.State;
using Divine.Plugin.Engine.Processor.SendTables;
using Divine.Plugin.Engine.Processor.StringTables;
using Divine.Protobufs.Dota2;
using Divine.Service;

using Google.Protobuf;

using Grpc.Core.Logging;

internal class EngineManager : Bootstrapper
{
    private const int DEFERRED_MESSAGE_MAX = 20;

    private static JsonFormatter JsonFormatter = new(JsonFormatter.Settings.Default.WithIndentation());

    private static bool debug = false;

    private static bool opDebug = false;

    private static bool isDebugEnabled = false;

    private static bool updateEventDebug = false;

    private static bool DumpEntity = true;

    private static int serverTick;

    private static int entitiesServerTick;

    private static bool resetInProgress;

    private static EntityRegistry entityRegistry = new();

    private static readonly List<Action> queuedUpdates = new();

    private static readonly SortedDictionary<int, CSVCMsg_PacketEntities> deferredMessages = new();

    private static Baseline[] classBaselines;
    private static Baseline[][] entityBaselines;

    private static Dictionary<int, ByteString> rawBaselines = new();

    private static int entityCount;

    private static ClientFrame entities;

    private static int[] deletions = null!;

    private static FieldReader fieldReader;

    private static DotaEngineType engineType = new();

    private static DTClasses dtClasses = new();

    private static DTClassEmitter ClassEmitter = null!;

    private static StringTableEmitter StringTableEmitter = new();

    private static event Action? evUpdatesCompleted;

    private static readonly bool Enable = false; //------------------ Enabler

    protected override void OnMainActivate()
    {
        if (!Enable)
        {
            return;
        }

        ClassEmitter = new(dtClasses, Entity.NetworkClassInfos, CSVCMsg_FlattenedSerializer.Parser.ParseFrom(NetworkManager.FlattenedSerializer!.Buffer));

        StringTableEmitter.updateEvent += StringTableEmitter_updateEvent;

        onInit();

        NetworkManager.NetMessageReceived += OnNetMessageReceived;
        NetworkManager.NetMessageSend += NetworkManager_NetMessageSend;
    }

    protected override void OnActivate() // TEST Сделал для StringTable
    {
        if (!Enable)
        {
            return;
        }

        ClassEmitter = new(dtClasses, Entity.NetworkClassInfos, CSVCMsg_FlattenedSerializer.Parser.ParseFrom(NetworkManager.FlattenedSerializer!.Buffer));

        //var ffff = CSVCMsg_FlattenedSerializer.Parser.ParseFrom(NetworkManager.FlattenedSerializer!.Buffer);
        //var format = JsonFormatter.Format(ffff);
        //File.WriteAllText(@"C:\Users\RoccoZero\Desktop\CSVCMsg_FlattenedSerializer.json", format);

        onInit();
    }

    private static void StringTableEmitter_updateEvent(StringTable table, int index, string key, ByteString value)
    {
        if (table.getName() != "instancebaseline")
        {
            return;
        }
        //Console.WriteLine("KEK");
        Console.WriteLine(table);
        var dtClassId = int.Parse(key);
        rawBaselines[dtClassId] = value;
        if (classBaselines != null)
        {
            classBaselines[dtClassId].reset();
        }
    }

    private static void NetworkManager_NetMessageSend(NetMessageSendEventArgs e)
    {
        var protobuf = e.Protobuf;

        switch (protobuf.MessageId)
        {
            case NetMessageId.CSVCMsg_ClassInfo:
                //Console.WriteLine(protobuf);
                break;
        }
    }

    public static void onInit()
    {
        //entityCount = 1 << engineType.getIndexBits();
        entityCount = ushort.MaxValue;

        entities = new ClientFrame(entityCount);

        fieldReader = engineType.getNewFieldReader();

        deletions = new int[entityCount];

        entityBaselines = new Baseline[entityCount][];
        for (int i = 0; i < entityCount; i++)
        {
            entityBaselines[i] = new Baseline[] { new(), new() };
        }

        classBaselines = new Baseline[dtClasses.Last().getClassId()];
        for (int i = 0; i < classBaselines.Length; i++)
        {
            classBaselines[i] = new Baseline();
            classBaselines[i].dtClassId = i;
        }

        foreach (var entity in EntityManager.Entities)
        {
            var networkEntity = entityRegistry.create(
                entity.ClassNetworkId,
                entity.Index,
                engineType.serialForHandle((int)entity.Handle),
                (int)entity.Handle,
                dtClasses.forClassId(entity.ClassNetworkId));

            networkEntity.setState(dtClasses.forClassId(entity.ClassNetworkId).getEmptyState());
            entities.setEntity(networkEntity);
        }
    }

    private static void OnNetMessageReceived(NetMessageReceivedEventArgs e)
    {
        var protobuf = e.Protobuf;

        switch (protobuf.MessageId)
        {
            case NetMessageId.CNETMsg_Tick:
                onMessage(CNETMsg_Tick.Parser.ParseFrom(protobuf.Buffer));
                break;
            case NetMessageId.CSVCMsg_ClassInfo:
                Console.WriteLine(protobuf);
                break;
            case NetMessageId.CSVCMsg_PacketEntities:
                //var sw = Stopwatch.StartNew();
                //var packetEntities = CSVCMsg_PacketEntities.Parser.ParseFrom(protobuf.Buffer);

                //onPacketEntities(packetEntities);

                //sw.Stop();

                ////Console.WriteLine(sw.Elapsed);
                //var format = JsonFormatter.Format(packetEntities);
                ////Console.WriteLine(format);
                break;
            case NetMessageId.CSVCMsg_CreateStringTable:
                StringTableEmitter.onCreateStringTable(CSVCMsg_CreateStringTable.Parser.ParseFrom(protobuf.Buffer));
                break;
            case NetMessageId.CSVCMsg_UpdateStringTable:
                StringTableEmitter.onUpdateStringTable(CSVCMsg_UpdateStringTable.Parser.ParseFrom(protobuf.Buffer));
                break;
            case NetMessageId.CSVCMsg_ClearAllStringTables:
                StringTableEmitter.clearAllStringTables(CSVCMsg_ClearAllStringTables.Parser.ParseFrom(protobuf.Buffer));
                break;
        }
    }

    public static void onMessage(CNETMsg_Tick message)
    {
        serverTick = (int)message.Tick;
    }

    public static void onPacketEntities(CSVCMsg_PacketEntities message)
    {
        if (message.LegacyIsDelta)
        {
            if (serverTick == message.DeltaFrom)
            {
                throw new Exception($"received self-referential delta update for tick {serverTick}");
            }

            if (entitiesServerTick < message.DeltaFrom)
            {
                Logger.LogDebug($"defer message with delta from {message.DeltaFrom} at {serverTick}, since we are only at {entitiesServerTick}");

                deferredMessages[serverTick] = message;

                if (deferredMessages.Count > DEFERRED_MESSAGE_MAX)
                {
                    Logger.LogWarning($"more than {DEFERRED_MESSAGE_MAX} deferred messages, forcing execution, here be dragons");

                    foreach (var (deferredMessageTick, deferredMessage) in deferredMessages)
                    {
                        Logger.LogWarning($"forcing executing deferred message with delta {deferredMessage.DeltaFrom}, we are now at tick {deferredMessageTick}");
                        processAndRunPacketEntities(deferredMessage, deferredMessageTick);
                    }

                    deferredMessages.Clear();
                }

                return;
            }
        }
        else if (deferredMessages.Count != 0)
        {
            Logger.LogDebug("received full packet, disposing deferred message");
            deferredMessages.Clear();
        }

        if (deferredMessages.Count != 0)
        {
            var deferredToExecute = new SortedDictionary<int, CSVCMsg_PacketEntities>(deferredMessages.Where(x => x.Key < serverTick).ToDictionary(x => x.Key, x => x.Value));
            if (deferredToExecute.Count != 0)
            {
                Logger.LogDebug($"server is now at tick {serverTick}");

                foreach (var (deferredMessageTick, deferredMessage) in deferredToExecute)
                {
                    Logger.LogDebug($"executing deferred message with delta {deferredMessage.DeltaFrom}, we are now at tick {deferredMessageTick}");
                    processAndRunPacketEntities(deferredMessage, deferredMessageTick);
                }

                deferredToExecute.Clear();
            }
        }

        processAndRunPacketEntities(message, serverTick);
    }

    private static void processAndRunPacketEntities(CSVCMsg_PacketEntities message, int serverTick)
    {
        try
        {
            processPacketEntities(message, serverTick);
            entitiesServerTick = serverTick;

            if (isDebugEnabled)
            {
                Logger.LogDebug($"executing {queuedUpdates.Count} changes");
            }

            foreach (var queuedUpdate in queuedUpdates)
            {
                queuedUpdate();
            }

            if (!resetInProgress)
            {
                evUpdatesCompleted?.Invoke();
            }
        }
        finally
        {
            queuedUpdates.Clear();
        }
    }

    private static void processPacketEntities(CSVCMsg_PacketEntities message, int actualTick)
    {
        if (isDebugEnabled) // log.isDebugEnabled()
        {
            Logger.LogDebug($"processing packet entities: now: {actualTick:6}, delta-from: {message.DeltaFrom:6}, update-count: {message.UpdatedEntries:5}, baseline: {message.Baseline}, update-baseline: {message.UpdateBaseline}");
        }

        if (message.UpdateBaseline)
        {
            queueUpdate(() => switchBaselines(message.Baseline));
        }

        var stream = BitStream.createBitStream(message.EntityData);

        int updateCount = message.UpdatedEntries;
        int updateType;
        int eIdx = -1;
        NetworkEntity? eEnt;

        while (updateCount-- != 0)
        {
            eIdx += stream.readUBitVar() + 1;
            eEnt = entities.getEntity(eIdx);

            updateType = stream.readUBitInt(2);
            //Console.WriteLine("updateType: " + updateType + "  " + eIdx + "  " + (eEnt?.ToString() ?? "null"));

            switch (updateType)
            {

                case 2: // CREATE
                    int dtClassId = stream.readUBitInt(dtClasses.getClassBits());
                    DTClass dtClass = dtClasses.forClassId(dtClassId);
                    if (dtClass == null)
                    {
                        throw new Exception($"class for new entity {eIdx} is {dtClassId}, but no dtClass found!.");
                    }

                    int serial = stream.readUBitInt(engineType.getSerialBits());

                    // TODO: there is an extra VarInt encoded here for S2, figure out what it is
                    stream.readVarUInt();

                    if (eEnt != null)
                    {
                        int handle = engineType.handleForIndexAndSerial(eIdx, serial);
                        if (eEnt.getUid() == NetworkEntity.uid(dtClassId, handle))
                        {
                            if (!eEnt.isActive())
                            {
                                queueEntityEnter(eEnt);
                            }

                            queueEntityUpdate(eEnt, stream, false);
                            break;
                        }

                        if (eEnt.isActive())
                        {
                            queueEntityLeave(eEnt);
                        }

                        queueEntityDelete(eEnt);
                    }

                    queueEntityCreate(eIdx, serial, dtClass, message, stream);
                    break;
                case 0: // UPDATE
                    if (eEnt == null)
                    {
                        throw new Exception($"Entity not found for update at index {eIdx}. Entity update cannot be parsed!");
                    }

                    queueEntityUpdate(eEnt, stream, false);
                    break;
                case 1: // LEAVE
                    if (eEnt != null && eEnt.isActive())
                    {
                        queueEntityLeave(eEnt);
                    }
                    break;
                case 3: // DELETE
                    if (eEnt != null)
                    {
                        if (eEnt.isActive())
                        {
                            queueEntityLeave(eEnt);
                        }

                        queueEntityDelete(eEnt);
                    }
                    break;
            }

            if (DumpEntity /*&& eEnt?.getDtClass().getDtName().Contains("CDOTA_Unit_Hero_DrowRanger") == true*/)
            {
                Console.WriteLine(eEnt.ToString());
            }
        }

        if (engineType.handleDeletions() && message.LegacyIsDelta)
        {
            int n = fieldReader.readDeletions(stream, engineType.getIndexBits(), deletions);
            for (int i = 0; i < n; i++)
            {
                eIdx = deletions[i];
                eEnt = entities.getEntity(eIdx);

                if (eEnt != null)
                {
                    if (eEnt.isActive())
                    {
                        queueEntityLeave(eEnt);
                    }

                    queueEntityDelete(eEnt);
                }
            }
        }

        if (isDebugEnabled)
        {
            Logger.LogDebug($"update finished for tick {actualTick}");
        }
    }

    private static void switchBaselines(int iFrom)
    {
        int iTo = 1 - iFrom;
        foreach (var baseline in entityBaselines)
        {
            baseline[iTo].copyFrom(baseline[iFrom]);
        }
    }

    private static void queueEntityCreate(int eIdx, int serial, DTClass dtClass, CSVCMsg_PacketEntities message, BitStream stream)
    {
        FieldChanges changes = fieldReader.readFields(stream, dtClass, debug, opDebug);
        queueUpdate(() => executeEntityCreate(eIdx, serial, dtClass, message, changes));
    }

    private static void executeEntityCreate(int eIdx, int serial, DTClass dtClass, CSVCMsg_PacketEntities message, FieldChanges changes)
    {
        Baseline baseline = getBaseline(dtClass.getClassId(), message.Baseline, eIdx, message.LegacyIsDelta);
        IEntityState newState = baseline.state.copy();
        changes.applyTo(newState);
        NetworkEntity entity = entityRegistry.create(
                dtClass.getClassId(),
                eIdx, serial,
                engineType.handleForIndexAndSerial(eIdx, serial),
                dtClass);
        entity.setExistent(true);
        entity.setState(newState);
        entities.setEntity(entity);
        logModification("CREATE", entity);
        emitCreatedEvent(entity);
        executeEntityEnter(entity);
        if (message.UpdateBaseline)
        {
            Baseline updatedBaseline = entityBaselines[eIdx][1 - message.Baseline];
            updatedBaseline.dtClassId = dtClass.getClassId();
            updatedBaseline.state = newState.copy();
        }
    }

    private static void queueEntityUpdate(NetworkEntity entity, BitStream stream, bool silent)
    {
        FieldChanges changes = fieldReader.readFields(stream, entity.getDtClass(), debug, opDebug);
        queueUpdate(() => executeEntityUpdate(entity, changes, silent));
    }

    private static void executeEntityUpdate(NetworkEntity entity, FieldChanges changes, bool silent)
    {
        bool capacityChanged = changes.applyTo(entity.getState());
        logModification("UPDATE", entity);
        if (!silent)
        {
            if (capacityChanged)
            {
                emitPropertyCountChangedEvent(entity);
            }

            emitUpdatedEvent(entity, changes.getFieldPaths());
        }
    }

    private static void queueEntityEnter(NetworkEntity entity)
    {
        queueUpdate(() => executeEntityEnter(entity));
    }

    private static void executeEntityEnter(NetworkEntity entity)
    {
        entity.setActive(true);
        logModification("ENTER", entity);
        emitEnteredEvent(entity);
    }

    private static void queueEntityLeave(NetworkEntity entity)
    {
        queueUpdate(() => executeEntityLeave(entity));
    }

    private static void executeEntityLeave(NetworkEntity entity)
    {
        entity.setActive(false);
        logModification("LEAVE", entity);
        emitLeftEvent(entity);
    }

    private static void queueEntityDelete(NetworkEntity entity)
    {
        queueUpdate(() => executeEntityDelete(entity));
    }

    private static void executeEntityDelete(NetworkEntity entity)
    {
        entity.setExistent(false);
        entities.removeEntity(entity);
        logModification("DELETE", entity);
        emitDeletedEvent(entity);
    }

    private static void emitCreatedEvent(NetworkEntity entity)
    {
        if (resetInProgress /*|| !evCreated.isListenedTo()*/)
        {
            return;
        }

        debugUpdateEvent("CREATE", entity);
        //evCreated.raise(entity);
    }

    private static void emitEnteredEvent(NetworkEntity entity)
    {
        if (resetInProgress /*|| !evEntered.isListenedTo()*/)
        {
            return;
        }

        debugUpdateEvent("ENTER", entity);
        //evEntered.raise(entity);
    }

    private static void emitUpdatedEvent(NetworkEntity entity, IFieldPath[] updatedFieldPaths)
    {
        if (resetInProgress /*|| !evUpdated.isListenedTo()*/)
        {
            return;
        }

        debugUpdateEvent("UPDATE", entity);
        //evUpdated.raise(entity, updatedFieldPaths, updatedFieldPaths.Length);
    }

    private static void emitPropertyCountChangedEvent(NetworkEntity entity)
    {
        if (resetInProgress /*|| !evPropertyCountChanged.isListenedTo()*/)
        {
            return;
        }

        debugUpdateEvent("PROPERTYCOUNTCHANGE", entity);
        //evPropertyCountChanged.raise(entity);
    }

    private static void queueUpdate(Action update)
    {
        queuedUpdates.Add(update);
    }

    private static void emitLeftEvent(NetworkEntity entity)
    {
        if (resetInProgress /*|| !evLeft.isListenedTo()*/)
        {
            return;
        }

        debugUpdateEvent("LEAVE", entity);
        //evLeft.raise(entity);
    }

    private static void emitDeletedEvent(NetworkEntity entity)
    {
        if (resetInProgress /*|| !evDeleted.isListenedTo()*/)
        {
            return;
        }

        debugUpdateEvent("DELETE", entity);
        //evDeleted.raise(entity);
    }

    private static void logModification(string which, NetworkEntity entity)
    {
        if (!isDebugEnabled) // !log.isDebugEnabled()
        {
            return;
        }

        Logger.LogDebug($"\t{which:6}: index: {entity.getIndex():4}, serial: {entity.getSerial():X03}, handle: {entity.getHandle():7}, class: {entity.getDtClass().getDtName()}");
    }

    private static void debugUpdateEvent(string which, NetworkEntity entity)
    {
        if (!updateEventDebug)
        {
            return;
        }

        Logger.LogInformation($"\t{which:6}: index: {entity.getIndex():4}, serial: {entity.getSerial():X03}, handle: {entity.getHandle():7}, class: {entity.getDtClass().getDtName()}");
    }

    private static Baseline getBaseline(int clsId, int baseline, int entityIdx, bool delta)
    {
        Baseline b;
        if (delta)
        {
            b = entityBaselines[entityIdx][baseline];
            if (b.dtClassId == clsId && b.state != null)
            {
                return b;
            }
        }
        b = classBaselines[clsId];
        if (b.state != null)
        {
            return b;
        }
        DTClass cls = dtClasses.forClassId(clsId);
        if (cls == null)
        {
            throw new Exception($"DTClass for id {clsId} not found.");
        }

        b.state = cls.getEmptyState();

        if (!rawBaselines.TryGetValue(clsId, out var raw) || raw.Length == 0)
        {
            Logger.LogError($"Baseline for class {cls.getDtName()} ({clsId}) not found. Continuing anyway, but data might be missing!");
        }
        else
        {
            BitStream stream = BitStream.createBitStream(raw);
            FieldChanges changes = fieldReader.readFields(stream, cls, debug, opDebug);
            changes.applyTo(b.state);
        }

        return b;
    }

    private class Baseline
    {
        public int dtClassId = -1;
        public IEntityState state;
        public void reset()
        {
            state = null;
        }

        public void copyFrom(Baseline other)
        {
            this.dtClassId = other.dtClassId;
            this.state = other.state;
        }
    }
}
