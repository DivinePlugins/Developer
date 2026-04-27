using ImGuiDebugger.Enum;

namespace ImGuiDebugger;

using System;
using System.Collections.Generic;
using System.Dynamic;

using Divine.Entity.Entities;
using Divine.Entity.Entities.EventArgs;
using Divine.Entity.Entities.Units;
using Divine.Entity.EventArgs;
using Divine.Game.EventArgs;
using Divine.Modifier.EventArgs;
using Divine.Network.EventArgs;
using Divine.Network.GC;
using Divine.Numerics;
using Divine.Order.EventArgs;
using Divine.Particle.EventArgs;
using Divine.Projectile.EventArgs;
using Divine.Protobufs.Dota2;

using ImGuiDebugger.Enum;

using ESOMsg = ESOMsg;

internal sealed partial class Debugger
{
    private CSODOTAParty CSODOTAParty;
    private CSODOTALobby CSODOTALobby;
    private CSODOTAGameAccountClient CSODOTAGameAccountClient;
    private CSODOTAPartyInvite CSODOTAPartyInvite;
    private CSOEconItem CSOEconItem;
    private CSOEconGameAccountClient CSOEconGameAccountClient;
    private CSOEconItemTournamentPassport CSOEconItemTournamentPassport;
    private CSODOTAGameHeroFavorites CSODOTAGameHeroFavorites;
    private CSODOTAMapLocationState CSODOTAMapLocationState;
    private CMsgDOTATournament CMsgDOTATournament;
    private CSODOTAPlayerChallenge CSODOTAPlayerChallenge;
    private CSODOTALobbyInvite CSODOTALobbyInvite;
    private CSODOTAGameAccountPlus CSODOTAGameAccountPlus;
    private CSOEconItemDropRateBonus CSOEconItemDropRateBonus;

    private void NetworkManager_NetMessageReceived(NetMessageReceivedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.NetMessageReceived) == 0)
            return;

        var protobuf = e.Protobuf;

        dynamic Event = new ExpandoObject();
        Event.name = protobuf.Name;
        Event.data = e;
        Event.protobuf_data = protobuf.ToJson();
        RunCallback(CallbackData2[CallbackFlag.NetMessageReceived], Event);
        //CheckCountAndEnqueue(CallbackFlags.NetMessageReceived, e, op);
    }

    private void NetworkManager_NetMessageSend(NetMessageSendEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.NetMessageSend) == 0)
            return;

        var protobuf = e.Protobuf;

        dynamic Event = new ExpandoObject();
        Event.name = protobuf.Name;
        Event.data = e;
        Event.protobuf_data = protobuf.ToJson();
        RunCallback(CallbackData2[CallbackFlag.NetMessageSend], Event);
        //CheckCountAndEnqueue(CallbackFlags.NetMessageSend, e, op);
    }

    private void NetworkManager_GCMessageReceived(GCMessageReceivedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.GCMessageReceived) == 0)
            return;

        var protobuf = e.Protobuf;
        //GCSOMessageReceived(protobuf);

        dynamic Event = new ExpandoObject();
        Event.name = (e.Protobuf.Name == string.Empty
            ? (e.Protobuf.RawMessageId.ToString() == string.Empty
                ? e.Protobuf.MessageId.ToString()
                : e.Protobuf.RawMessageId.ToString())
            : e.Protobuf.Name);
        Event.data = e;
        Event.protobuf_data = e.Protobuf.ToJson();
        RunCallback(CallbackData2[CallbackFlag.GCMessageReceived], Event);
        //CheckCountAndEnqueue(CallbackFlags.GCMessageReceived, e, op);
    }

    private void GCSOMessageReceived(GCProtobuf protobuf)
    {
        if ((int)protobuf.MessageId == (int)ESOMsg.CMsgSOCacheSubscribed
                    || (int)protobuf.MessageId == (int)ESOMsg.CMsgSOCacheUnsubscribed
                    || (int)protobuf.MessageId == (int)ESOMsg.CMsgSOUpdateMultiple)
        {
            switch ((ESOMsg)protobuf.MessageId)
            {
                case ESOMsg.CMsgSOCacheSubscribed:
                    var parsedSOCacheSubscribed = CMsgSOCacheSubscribed.Parser.ParseFrom(protobuf.Buffer);
                    Console.WriteLine();
                    Console.WriteLine($"CMsgSOCacheSubscribed -> OwnerSoidType: {parsedSOCacheSubscribed.OwnerSoid.Type} OwnerSoidId: {parsedSOCacheSubscribed.OwnerSoid.Id}");
                    foreach (var subscribedObject in parsedSOCacheSubscribed.Objects)
                    {
                        Console.WriteLine();
                        switch ((ESOType)subscribedObject.TypeId)
                        {
                            case ESOType.CSOEconItem:
                                var buffer = subscribedObject.ObjectData[0];
                                CSOEconItem = CSOEconItem.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSOEconItem");
                                Console.WriteLine(CSOEconItem);
                                break;
                            case ESOType.CSOItemRecipe:
                                break;
                            case ESOType.CSOEconGameAccountClient:
                                buffer = subscribedObject.ObjectData[0];
                                CSOEconGameAccountClient = CSOEconGameAccountClient.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSOEconGameAccountClient");
                                Console.WriteLine(CSOEconGameAccountClient);
                                break;
                            case ESOType.CSOEconItemDropRateBonus:
                                buffer = subscribedObject.ObjectData[0];
                                CSOEconItemDropRateBonus = CSOEconItemDropRateBonus.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSOEconItemDropRateBonus");
                                Console.WriteLine(CSOEconItemDropRateBonus);
                                break;
                            case ESOType.CSOEconItemEventTicket:
                                break;
                            case ESOType.CSOEconItemTournamentPassport:
                                buffer = subscribedObject.ObjectData[0];
                                CSOEconItemTournamentPassport = CSOEconItemTournamentPassport.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSOEconItemTournamentPassport");
                                Console.WriteLine(CSOEconItemTournamentPassport);
                                break;
                            case ESOType.CSODOTAGameAccountClient:
                                buffer = subscribedObject.ObjectData[0];
                                CSODOTAGameAccountClient = CSODOTAGameAccountClient.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSODOTAGameAccountClient");
                                Console.WriteLine(CSODOTAGameAccountClient);
                                break;
                            case ESOType.CSODOTAParty:
                                buffer = subscribedObject.ObjectData[0];
                                CSODOTAParty = CSODOTAParty.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSODOTAParty");
                                Console.WriteLine(CSODOTAParty);
                                break;
                            case ESOType.CSODOTALobby:
                                buffer = subscribedObject.ObjectData[0];
                                CSODOTALobby = CSODOTALobby.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSODOTALobby");
                                Console.WriteLine(CSODOTALobby);
                                break;
                            case ESOType.CSODOTAPartyInvite:
                                buffer = subscribedObject.ObjectData[0];
                                CSODOTAPartyInvite = CSODOTAPartyInvite.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSODOTAPartyInvite");
                                Console.WriteLine(CSODOTAPartyInvite);
                                break;
                            case ESOType.CSODOTAGameHeroFavorites:
                                buffer = subscribedObject.ObjectData[0];
                                CSODOTAGameHeroFavorites = CSODOTAGameHeroFavorites.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSODOTAGameHeroFavorites");
                                Console.WriteLine(CSODOTAGameHeroFavorites);
                                break;
                            case ESOType.CSODOTAMapLocationState:
                                buffer = subscribedObject.ObjectData[0];
                                CSODOTAMapLocationState = CSODOTAMapLocationState.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSODOTAMapLocationState");
                                Console.WriteLine(CSODOTAMapLocationState);
                                break;
                            case ESOType.CMsgDOTATournament:
                                buffer = subscribedObject.ObjectData[0];
                                CMsgDOTATournament = CMsgDOTATournament.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CMsgDOTATournament");
                                Console.WriteLine(CMsgDOTATournament);
                                break;
                            case ESOType.CSODOTAPlayerChallenge:
                                buffer = subscribedObject.ObjectData[0];
                                CSODOTAPlayerChallenge = CSODOTAPlayerChallenge.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSODOTAPlayerChallenge");
                                Console.WriteLine(CSODOTAPlayerChallenge);
                                break;
                            case ESOType.CSODOTALobbyInvite:
                                buffer = subscribedObject.ObjectData[0];
                                CSODOTALobbyInvite = CSODOTALobbyInvite.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSODOTALobbyInvite");
                                Console.WriteLine(CSODOTALobbyInvite);
                                break;
                            case ESOType.CSODOTAGameAccountPlus:
                                buffer = subscribedObject.ObjectData[0];
                                CSODOTAGameAccountPlus = CSODOTAGameAccountPlus.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOCacheSubscribed -> CSODOTAGameAccountPlus");
                                Console.WriteLine(CSODOTAGameAccountPlus);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case ESOMsg.CMsgSOCacheUnsubscribed:
                    var parsedSOCacheUnsubscribed = CMsgSOCacheUnsubscribed.Parser.ParseFrom(protobuf.Buffer);
                    Console.WriteLine();
                    Console.WriteLine($"CMsgSOCacheUnsubscribed -> OwnerSoidType: {parsedSOCacheUnsubscribed.OwnerSoid.Type} OwnerSoidId: {parsedSOCacheUnsubscribed.OwnerSoid.Id}");
                    break;
                case ESOMsg.CMsgSOUpdateMultiple:
                    var parsedSOMultipleObjects = CMsgSOMultipleObjects.Parser.ParseFrom(protobuf.Buffer);
                    foreach (var subscribedObject in parsedSOMultipleObjects.ObjectsModified)
                    {
                        Console.WriteLine();
                        if (!subscribedObject.HasObjectData)
                            continue;
                        switch ((ESOType)subscribedObject.TypeId)
                        {
                            case ESOType.CSOEconItem:
                                var buffer = subscribedObject.ObjectData;
                                CSOEconItem = CSOEconItem.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSOEconItem");
                                Console.WriteLine(CSOEconItem);
                                break;
                            case ESOType.CSOItemRecipe:
                                break;
                            case ESOType.CSOEconGameAccountClient:
                                buffer = subscribedObject.ObjectData;
                                CSOEconGameAccountClient = CSOEconGameAccountClient.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSOEconGameAccountClient");
                                Console.WriteLine(CSOEconGameAccountClient);
                                break;
                            case ESOType.CSOEconItemDropRateBonus:
                                buffer = subscribedObject.ObjectData;
                                CSOEconItemDropRateBonus = CSOEconItemDropRateBonus.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSOEconItemDropRateBonus");
                                Console.WriteLine(CSOEconItemDropRateBonus);
                                break;
                            case ESOType.CSOEconItemEventTicket:
                                break;
                            case ESOType.CSOEconItemTournamentPassport:
                                buffer = subscribedObject.ObjectData;
                                CSOEconItemTournamentPassport = CSOEconItemTournamentPassport.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSOEconItemTournamentPassport");
                                Console.WriteLine(CSOEconItemTournamentPassport);
                                break;
                            case ESOType.CSODOTAGameAccountClient:
                                buffer = subscribedObject.ObjectData;
                                CSODOTAGameAccountClient = CSODOTAGameAccountClient.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSODOTAGameAccountClient");
                                Console.WriteLine(CSODOTAGameAccountClient);
                                break;
                            case ESOType.CSODOTAParty:
                                buffer = subscribedObject.ObjectData;
                                CSODOTAParty = CSODOTAParty.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSODOTAParty");
                                Console.WriteLine(CSODOTAParty);
                                break;
                            case ESOType.CSODOTALobby:
                                buffer = subscribedObject.ObjectData;
                                CSODOTALobby = CSODOTALobby.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSODOTALobby");
                                Console.WriteLine(CSODOTALobby);
                                break;
                            case ESOType.CSODOTAPartyInvite:
                                buffer = subscribedObject.ObjectData;
                                CSODOTAPartyInvite = CSODOTAPartyInvite.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSODOTAPartyInvite");
                                Console.WriteLine(CSODOTAPartyInvite);
                                break;
                            case ESOType.CSODOTAGameHeroFavorites:
                                buffer = subscribedObject.ObjectData;
                                CSODOTAGameHeroFavorites = CSODOTAGameHeroFavorites.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSODOTAGameHeroFavorites");
                                Console.WriteLine(CSODOTAGameHeroFavorites);
                                break;
                            case ESOType.CSODOTAMapLocationState:
                                buffer = subscribedObject.ObjectData;
                                CSODOTAMapLocationState = CSODOTAMapLocationState.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSODOTAMapLocationState");
                                Console.WriteLine(CSODOTAMapLocationState);
                                break;
                            case ESOType.CMsgDOTATournament:
                                buffer = subscribedObject.ObjectData;
                                CMsgDOTATournament = CMsgDOTATournament.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CMsgDOTATournament");
                                Console.WriteLine(CMsgDOTATournament);
                                break;
                            case ESOType.CSODOTAPlayerChallenge:
                                buffer = subscribedObject.ObjectData;
                                CSODOTAPlayerChallenge = CSODOTAPlayerChallenge.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSODOTAPlayerChallenge");
                                Console.WriteLine(CSODOTAPlayerChallenge);
                                break;
                            case ESOType.CSODOTALobbyInvite:
                                buffer = subscribedObject.ObjectData;
                                CSODOTALobbyInvite = CSODOTALobbyInvite.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSODOTALobbyInvite");
                                Console.WriteLine(CSODOTALobbyInvite);
                                break;
                            case ESOType.CSODOTAGameAccountPlus:
                                buffer = subscribedObject.ObjectData;
                                CSODOTAGameAccountPlus = CSODOTAGameAccountPlus.Parser.ParseFrom(buffer);
                                Console.WriteLine("CMsgSOUpdateMultiple -> CSODOTAGameAccountPlus");
                                Console.WriteLine(CSODOTAGameAccountPlus);
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void NetworkManager_GCMessageSend(GCMessageSendEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.GCMessageSend) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = (e.Protobuf.Name == string.Empty
            ? (e.Protobuf.RawMessageId.ToString() == string.Empty
                ? e.Protobuf.MessageId.ToString()
                : e.Protobuf.RawMessageId.ToString())
            : e.Protobuf.Name);
        Event.data = e;
        Event.protobuf_data = e.Protobuf.ToJson();
        RunCallback(CallbackData2[CallbackFlag.GCMessageSend], Event);
        //CheckCountAndEnqueue(CallbackFlags.GCMessageSend, e, op);
    }

    private void NetworkManager_GCSOMessageUpdate(GCSOMessageUpdateEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.GCSOMessageUpdate) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = (e.Protobuf.Name == string.Empty
            ? e.Protobuf.MessageId.ToString()
            : e.Protobuf.Name);
        Event.data = e;
        Event.protobuf_data = e.Protobuf.ToJson();
        RunCallback(CallbackData2[CallbackFlag.GCSOMessageUpdate], Event);
        //CheckCountAndEnqueue(CallbackFlags.GCMessageSend, e, op);
    }

    private void GameManager_GameEvent(GameEventEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.GameEvent) == 0)
            return;
        //e.GameEvent.GetSingle
        dynamic Event = new ExpandoObject();
        Event.name = e.GameEvent.Name;
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.GameEvent], Event);
        //CheckCountAndEnqueue(CallbackFlags.GameEvent, e, op);
    }

    private void OrderManager_OrderOverwatchAdding(OrderOverwatchAddingEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.OrderOverwatchAdding) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = "order_overwatch_adding";
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.OrderOverwatchAdding], Event);
        //CheckCountAndEnqueue(CallbackFlags.OrderOverwatchAdding, e, op);
    }

    private void OrderManager_OrderAdding(OrderAddingEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.OrderAdding) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Order.Type.ToString();
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.OrderAdding], Event);
        //CheckCountAndEnqueue(CallbackFlags.OrderAdding, e, op);
    }

    private void EntityManager_EntityRemoved(EntityRemovedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.EntityRemoved) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Entity is Unit ? e.Entity.Name : e.Entity.ClassId.ToString();
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.EntityRemoved], Event);
        //CheckCountAndEnqueue(CallbackFlags.EntityRemoved, e, op);
    }

    private void EntityManager_EntityDestroy(EntityDestroyEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.EntityDestroy) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Entity is Unit ? e.Entity.Name : e.Entity.ClassId.ToString();
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.EntityDestroy], Event);
        //CheckCountAndEnqueue(CallbackFlags.EntityDestroy, e, op);
    }

    private void EntityManager_EntityAdded(EntityAddedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.EntityAdded) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Entity is Unit ? e.Entity.Name : e.Entity.ClassId.ToString();
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.EntityAdded], Event);
        //CheckCountAndEnqueue(CallbackFlags.EntityAdded, e, op);
    }

    private void EntityManager_EntityCreate(EntityCreateEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.EntityCreate) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Entity is Unit ? e.Entity.Name : e.Entity.ClassId.ToString();
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.EntityCreate], Event);
        //CheckCountAndEnqueue(CallbackFlags.EntityCreate, e, op);
    }

    private void Entity_NetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.NetworkPropertyChanged) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = sender is Unit ? sender.Name : sender.ClassId.ToString();
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.NetworkPropertyChanged], Event);
        //CheckCountAndEnqueue(CallbackFlags.NetworkPropertyChanged, new { Entity = sender, Args = e }, op);
    }

    private void Entity_AnimationChanged(Entity sender, AnimationChangedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.AnimationChanged) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = sender is Unit ? sender.Name : sender.ClassId.ToString();
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.AnimationChanged], Event);
        //CheckCountAndEnqueue(CallbackFlags.AnimationChanged, new { Entity = sender, Args = e }, op);
    }

    private void ModifierManager_ModifierRemoved(ModifierRemovedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.ModifierRemoved) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Modifier!.Name;
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.ModifierRemoved], Event);
        //CheckCountAndEnqueue(CallbackFlags.ModifierRemoved, e, op);
    }

    private void ModifierManager_ModifierAdded(ModifierAddedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.ModifierAdded) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Modifier!.Name;
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.ModifierAdded], Event);
        //CheckCountAndEnqueue(CallbackFlags.ModifierAdded, e, op);
    }

    private void ProjectileManager_TrackingProjectileRemoved(TrackingProjectileRemovedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.TrackingProjectileRemoved) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Projectile!.Particle?.Name.Remove(0, e.Projectile!.Particle!.Name.LastIndexOf("/") + 1) ?? e.Projectile!.Handle.ToString();
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.TrackingProjectileRemoved], Event);
        //CheckCountAndEnqueue(CallbackFlags.TrackingProjectileRemoved, e, op);
    }

    private void ProjectileManager_TrackingProjectileDestroy(TrackingProjectileDestroyEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.TrackingProjectileDestroy) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Projectile!.Particle?.Name.Remove(0, e.Projectile!.Particle!.Name.LastIndexOf("/") + 1) ?? e.Projectile!.Handle.ToString();
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.TrackingProjectileDestroy], Event);
        //CheckCountAndEnqueue(CallbackFlags.TrackingProjectileDestroy, e, op);
    }

    private void ProjectileManager_TrackingProjectileAdded(TrackingProjectileAddedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.TrackingProjectileAdded) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Projectile!.Particle?.Name.Remove(0, e.Projectile!.Particle!.Name.LastIndexOf("/") + 1);
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.TrackingProjectileAdded], Event);
        //CheckCountAndEnqueue(CallbackFlags.TrackingProjectileAdded, e, op);
    }

    private void ProjectileManager_TrackingProjectileCreate(TrackingProjectileCreateEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.TrackingProjectileCreate) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Projectile!.Particle?.Name.Remove(0, e.Projectile!.Particle!.Name.LastIndexOf("/") + 1);
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.TrackingProjectileCreate], Event);
        //CheckCountAndEnqueue(CallbackFlags.TrackingProjectileCreate, e, op);
    }

    private void ProjectileManager_LinearProjectileRemoved(LinearProjectileRemovedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.LinearProjectileRemoved) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Projectile!.Particle?.Name.Remove(0, e.Projectile!.Particle!.Name.LastIndexOf("/") + 1);
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.LinearProjectileRemoved], Event);
        //CheckCountAndEnqueue(CallbackFlags.LinearProjectileRemoved, e, op);
    }

    private void ProjectileManager_LinearProjectileDestroy(LinearProjectileDestroyEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.LinearProjectileDestroy) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Projectile!.Particle?.Name.Remove(0, e.Projectile!.Particle!.Name.LastIndexOf("/") + 1);
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.LinearProjectileDestroy], Event);
        //CheckCountAndEnqueue(CallbackFlags.LinearProjectileDestroy, e, op);
    }

    private void ProjectileManager_LinearProjectileAdded(LinearProjectileAddedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.LinearProjectileAdded) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Projectile!.Particle?.Name.Remove(0, e.Projectile!.Particle!.Name.LastIndexOf("/") + 1);
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.LinearProjectileAdded], Event);
        //CheckCountAndEnqueue(CallbackFlags.LinearProjectileAdded, e, op);
    }

    private void ProjectileManager_LinearProjectileCreate(LinearProjectileCreateEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.LinearProjectileCreate) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Projectile!.Particle?.Name.Remove(0, e.Projectile!.Particle!.Name.LastIndexOf("/") + 1);
        Event.data = e;
        RunCallback(CallbackData2[CallbackFlag.LinearProjectileCreate], Event);
        //CheckCountAndEnqueue(CallbackFlags.LinearProjectileCreate, e, op);
    }

    private void ParticleManager_ParticleRemoved(ParticleRemovedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.ParticleRemoved) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Particle.Name.Remove(0, e.Particle.Name.LastIndexOf("/") + 1);
        Event.data = new
        {
            ParticleData = e,
            ParticleControlPoints = new Dictionary<int, Vector3>(),
        };
        if (e.Particle is not null)
        {
            for (int i = 0; i <= e.Particle.HighestControlPoint; i++)
            {
                Event.data.ParticleControlPoints.Add(i, e.Particle.GetControlPoint(i));
            }
        }
        RunCallback(CallbackData2[CallbackFlag.ParticleAdded], Event);
        //CheckCountAndEnqueue(CallbackFlags.ParticleRemoved, e, op);
    }

    private void ParticleManager_ParticleDestroy(ParticleDestroyEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.ParticleDestroy) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Particle.Name.Remove(0, e.Particle.Name.LastIndexOf("/") + 1);
        Event.data = new
        {
            ParticleData = e,
            ParticleControlPoints = new Dictionary<int, Vector3>(),
        };
        if (e.Particle is not null)
        {
            for (int i = 0; i <= e.Particle.HighestControlPoint; i++)
            {
                Event.data.ParticleControlPoints.Add(i, e.Particle.GetControlPoint(i));
            }
        }
        RunCallback(CallbackData2[CallbackFlag.ParticleAdded], Event);
        //CheckCountAndEnqueue(CallbackFlags.ParticleDestroy, e, op);
    }

    private void ParticleManager_ParticleAdded(ParticleAddedEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.ParticleAdded) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Particle.Name.Remove(0, e.Particle.Name.LastIndexOf("/") + 1);
        Event.data = new
        {
            ParticleData = e,
            ParticleControlPoints = new Dictionary<int, Vector3>(),
        };
        if (e.Particle is not null)
        {
            for (int i = 0; i <= e.Particle.HighestControlPoint; i++)
            {
                Event.data.ParticleControlPoints.Add(i, e.Particle.GetControlPoint(i));
            }
        }
        RunCallback(CallbackData2[CallbackFlag.ParticleAdded], Event);
        //CheckCountAndEnqueue(CallbackFlags.ParticleAdded, e, op);
    }

    private void ParticleManager_ParticleCreate(ParticleCreateEventArgs e)
    {
        if (((CallbackFlag)EnabledCallbacks & CallbackFlag.ParticleCreate) == 0)
            return;

        dynamic Event = new ExpandoObject();
        Event.name = e.Particle.Name.Remove(0, e.Particle.Name.LastIndexOf("/") + 1);
        Event.data = new
        {
            ParticleData = e,
            ParticleControlPoints = new Dictionary<int, Vector3>(),
        };
        if (e.Particle is not null)
        {
            for (int i = 0; i <= e.Particle.HighestControlPoint; i++)
            {
                Event.data.ParticleControlPoints.Add(i, e.Particle.GetControlPoint(i));
            }
        }
        RunCallback(CallbackData2[CallbackFlag.ParticleCreate], Event);
        //CheckCountAndEnqueue(CallbackFlags.ParticleCreate, e, op);
    }
}
