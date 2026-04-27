namespace ImGuiDebugger;
using System;
using System.Collections.Generic;
using System.Text.Json;

using Divine.Entity;
using Divine.Entity.Entities.Units;

using ImGuiDebugger.Enum;

internal sealed partial class Debugger
{
    private const string DefaultString = "Osman LOX!!! Osman LOX!!! Osman LOX!!!";
    static string name = "";
    static int num = 0;
    static float float_num = 0.0f;
    //Example callback that keeps track of a changing data structure
    dynamic callback_example = new
    {
        callbacks = new Dictionary<string, dynamic>()
        {
            ["CNETMsg_Tick"] = new Func<dynamic, bool>((args) =>
            {
                var insert_custom = new Action<string, dynamic>((name, args) =>
                {
                    var eventTable = CallbackData2[CallbackFlag.ManuallyCreated];

                    if (eventTable.events.TryGetValue(name, out dynamic table))
                    {
                        table.count = table.count + 1;
                    }
                    else
                    {
                        eventTable.names.Add(name);
                        eventTable.names.Sort();

                        eventTable.Insert(name);
                        table = eventTable.events[name];
                    }

                    table.last_args = args;
                });

                //Not really an event, just a data structure that is changing constantly
                //We insert it into our event list so we can view it in the window
                //This does not need to be done here, it could be done anywhere, CNETMsg_Tick callback is just an example

                //insert_custom("CSODOTALobby", Matchmaking.GetLobbyData());
                //insert_custom("CSODOTAParty", Matchmaking.GetPartyData());
                //insert_custom("CSODOTAGameAccountClient", Matchmaking.GetAccountData());
                //insert_custom("CMsgStartFindingMatch", Matchmaking.GetMatchSearchData());
                var custom_data = new
                {
                    Name = name,
                    Int = num,
                    Float = float_num,
                };

                insert_custom("CustomCallback", JsonSerializer.SerializeToNode(custom_data));
                name = name == DefaultString ? "" : DefaultString.Remove(name.Length + 1);
                num++;
                float_num = float_num + 0.1f;
                return true;
            })
        }
    };

    dynamic game_event = new
    {
        callbacks = new Dictionary<string, dynamic>()
        {
            ["entity_hurt"] = new Func<dynamic, bool>((args) =>
            {
                var game_event = args.raw_data.data.GameEvent;
                var entindex_attacker = game_event.GetInt32("entindex_attacker");
                var entindex_killed = game_event.GetInt32("entindex_killed");
                var entindex_inflictor = game_event.GetInt32("entindex_inflictor");
                var damagebits = game_event.GetInt32("damagebits");
                var damage = game_event.GetSingle("damage");
                var attacker = EntityManager.GetEntityByIndex(entindex_attacker);
                var target = EntityManager.GetEntityByIndex(entindex_killed);
                var inflictor = EntityManager.GetEntityByIndex(entindex_inflictor);

                if (attacker is Unit && target is Unit)
                {
                    Console.WriteLine($"{attacker.Name} -> {target.Name}");
                }
                else if (attacker is Unit && target is null)
                {
                    Console.WriteLine($"{attacker.Name} -> {target}");
                }
                else if (attacker is null && target is Unit)
                {
                    Console.WriteLine($"{attacker} -> {target.Name}");
                }
                Console.WriteLine($"entindex_inflictor: {inflictor ?? "null"}");
                Console.WriteLine($"damagebits: {damagebits}");
                Console.WriteLine($"damage: {damage}");

                return true;
            })
        }
    };

}
