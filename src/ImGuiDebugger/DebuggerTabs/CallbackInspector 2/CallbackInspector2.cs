namespace ImGuiDebugger;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Game;
using Divine.Modifier;
using Divine.Network;
using Divine.Numerics;
using Divine.Order;
using Divine.Particle;
using Divine.Projectile;

using ImGuiNET;

using ImGuiDebugger.Enum;

internal sealed partial class Debugger
{
    private int EnabledCallbacks;
    private Dictionary<string, bool>? filtered_names = new();

    private static Dictionary<CallbackFlag, EventTable> CallbackData2 = new()
    {
        { CallbackFlag.ParticleCreate, new () },
        { CallbackFlag.ParticleAdded, new () },
        { CallbackFlag.ParticleDestroy, new () },
        { CallbackFlag.ParticleRemoved, new () },
        { CallbackFlag.LinearProjectileCreate, new () },
        { CallbackFlag.LinearProjectileAdded, new () },
        { CallbackFlag.LinearProjectileDestroy, new () },
        { CallbackFlag.LinearProjectileRemoved, new () },
        { CallbackFlag.TrackingProjectileCreate, new () },
        { CallbackFlag.TrackingProjectileAdded, new () },
        { CallbackFlag.TrackingProjectileDestroy, new () },
        { CallbackFlag.TrackingProjectileRemoved, new () },
        { CallbackFlag.ModifierAdded, new () },
        { CallbackFlag.ModifierRemoved, new () },
        { CallbackFlag.AnimationChanged, new () },
        { CallbackFlag.EntityCreate, new () },
        { CallbackFlag.EntityAdded, new () },
        { CallbackFlag.EntityDestroy, new () },
        { CallbackFlag.EntityRemoved, new () },
        { CallbackFlag.OrderAdding, new () },
        { CallbackFlag.OrderOverwatchAdding, new () },
        { CallbackFlag.GameEvent, new () },
        { CallbackFlag.GCMessageSend, new () },
        { CallbackFlag.GCMessageReceived, new () },
        { CallbackFlag.GCSOMessageUpdate, new () },
        { CallbackFlag.NetMessageSend, new () },
        { CallbackFlag.NetMessageReceived, new () },
        { CallbackFlag.NetworkPropertyChanged, new () },
        { CallbackFlag.ManuallyCreated, new () },
    };

    private void CallbackInspector2Activate()
    {
        //===========================================================================================================================================
        ParticleManager.ParticleCreate += ParticleManager_ParticleCreate;
        ParticleManager.ParticleAdded += ParticleManager_ParticleAdded;
        ParticleManager.ParticleDestroy += ParticleManager_ParticleDestroy;
        ParticleManager.ParticleRemoved += ParticleManager_ParticleRemoved;
        //===========================================================================================================================================
        ProjectileManager.LinearProjectileCreate += ProjectileManager_LinearProjectileCreate;
        ProjectileManager.LinearProjectileAdded += ProjectileManager_LinearProjectileAdded;
        ProjectileManager.LinearProjectileDestroy += ProjectileManager_LinearProjectileDestroy;
        ProjectileManager.LinearProjectileRemoved += ProjectileManager_LinearProjectileRemoved;
        //===========================================================================================================================================
        ProjectileManager.TrackingProjectileCreate += ProjectileManager_TrackingProjectileCreate;
        ProjectileManager.TrackingProjectileAdded += ProjectileManager_TrackingProjectileAdded;
        ProjectileManager.TrackingProjectileDestroy += ProjectileManager_TrackingProjectileDestroy;
        ProjectileManager.TrackingProjectileRemoved += ProjectileManager_TrackingProjectileRemoved;
        //===========================================================================================================================================
        ModifierManager.ModifierAdded += ModifierManager_ModifierAdded;
        ModifierManager.ModifierRemoved += ModifierManager_ModifierRemoved;
        //===========================================================================================================================================
        Entity.AnimationChanged += Entity_AnimationChanged;
        Entity.NetworkPropertyChanged += Entity_NetworkPropertyChanged;
        //===========================================================================================================================================
        EntityManager.EntityCreate += EntityManager_EntityCreate;
        EntityManager.EntityAdded += EntityManager_EntityAdded;
        EntityManager.EntityDestroy += EntityManager_EntityDestroy;
        EntityManager.EntityRemoved += EntityManager_EntityRemoved;
        //===========================================================================================================================================
        OrderManager.OrderAdding += OrderManager_OrderAdding;
        OrderManager.OrderOverwatchAdding += OrderManager_OrderOverwatchAdding;
        //===========================================================================================================================================
        GameManager.GameEvent += GameManager_GameEvent;
        //===========================================================================================================================================
        NetworkManager.GCMessageSend += NetworkManager_GCMessageSend;
        NetworkManager.GCMessageReceived += NetworkManager_GCMessageReceived;
        NetworkManager.NetMessageSend += NetworkManager_NetMessageSend;
        NetworkManager.NetMessageReceived += NetworkManager_NetMessageReceived;
        NetworkManager.GCSOMessageUpdate += NetworkManager_GCSOMessageUpdate;
        //===========================================================================================================================================
    }

    private void CallbackInspector2Deactivate()
    {
        //===========================================================================================================================================
        ParticleManager.ParticleCreate -= ParticleManager_ParticleCreate;
        ParticleManager.ParticleAdded -= ParticleManager_ParticleAdded;
        ParticleManager.ParticleDestroy -= ParticleManager_ParticleDestroy;
        ParticleManager.ParticleRemoved -= ParticleManager_ParticleRemoved;
        //===========================================================================================================================================
        ProjectileManager.LinearProjectileCreate -= ProjectileManager_LinearProjectileCreate;
        ProjectileManager.LinearProjectileAdded -= ProjectileManager_LinearProjectileAdded;
        ProjectileManager.LinearProjectileDestroy -= ProjectileManager_LinearProjectileDestroy;
        ProjectileManager.LinearProjectileRemoved -= ProjectileManager_LinearProjectileRemoved;
        //===========================================================================================================================================
        ProjectileManager.TrackingProjectileCreate -= ProjectileManager_TrackingProjectileCreate;
        ProjectileManager.TrackingProjectileAdded -= ProjectileManager_TrackingProjectileAdded;
        ProjectileManager.TrackingProjectileDestroy -= ProjectileManager_TrackingProjectileDestroy;
        ProjectileManager.TrackingProjectileRemoved -= ProjectileManager_TrackingProjectileRemoved;
        //===========================================================================================================================================
        ModifierManager.ModifierAdded -= ModifierManager_ModifierAdded;
        ModifierManager.ModifierRemoved -= ModifierManager_ModifierRemoved;
        //===========================================================================================================================================
        Entity.AnimationChanged -= Entity_AnimationChanged;
        Entity.NetworkPropertyChanged -= Entity_NetworkPropertyChanged;
        //===========================================================================================================================================
        EntityManager.EntityCreate -= EntityManager_EntityCreate;
        EntityManager.EntityAdded -= EntityManager_EntityAdded;
        EntityManager.EntityDestroy -= EntityManager_EntityDestroy;
        EntityManager.EntityRemoved -= EntityManager_EntityRemoved;
        //===========================================================================================================================================
        OrderManager.OrderAdding -= OrderManager_OrderAdding;
        OrderManager.OrderOverwatchAdding -= OrderManager_OrderOverwatchAdding;
        //===========================================================================================================================================
        GameManager.GameEvent -= GameManager_GameEvent;
        //===========================================================================================================================================
        NetworkManager.GCMessageSend -= NetworkManager_GCMessageSend;
        NetworkManager.GCMessageReceived -= NetworkManager_GCMessageReceived;
        NetworkManager.NetMessageSend -= NetworkManager_NetMessageSend;
        NetworkManager.NetMessageReceived -= NetworkManager_NetMessageReceived;
        NetworkManager.GCSOMessageUpdate -= NetworkManager_GCSOMessageUpdate;
        //===========================================================================================================================================
    }

    private void CallbackInspector2()
    {
        ImGui.Spacing();

        if (ImGui.BeginTable("###Table_OverallCallbacks2Enabled", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX))
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Spacing();

            if (ImGui.CheckboxFlags("Overall Callbacks Enabled###Checkbox_OverallCallbacks2Enabled", ref EnabledCallbacks, (int)CallbackFlag.AllCallbacksEnabled))
                SaveCFG();

            ImGui.Spacing();

            ImGui.EndTable();
        }

        ImGui.Spacing();

        foreach (var (callback_name, event_table) in CallbackData2)
        {
            if (ImGui.CheckboxFlags($"###RadioButton_{callback_name}2", ref EnabledCallbacks, (int)callback_name))
                SaveCFG();

            ImGui.SameLine(0, ImGui.GetStyle().ItemInnerSpacing.X);

            if (ImGui.TreeNode($"{callback_name}###{callback_name}2"))
            {
                //event_table.settings:draw_bools()
                if (ImGui.Checkbox($"Hide No Argument Events##{callback_name}2", ref event_table.settings_hide_no_args))
                {

                }

                //if UI.Button("Filter Seen Events") then
                //    for i, name in ipairs(event_table.names) do
                //        filtered_names[name] = true
                //    end
                //end
                if (ImGui.Button($"Filter Seen Events##{callback_name}2"))
                {
                    foreach (var name in event_table.names)
                    {
                        filtered_names![name] = true;
                    }
                }

                //UI.SameLine()
                //if UI.Button("Clear Filter") then
                //    filtered_names = {}
                //end
                ImGui.SameLine();
                if (ImGui.Button($"Clear Filter##{callback_name}2"))
                {
                    filtered_names!.Clear();
                }

                //if UI.Button("Clear Events") then
                //    event_table.events = {}
                //    event_table.names = {}
                //end
                if (ImGui.Button($"Clear Events##{callback_name}2"))
                {
                    //Console.WriteLine($"{callback_name} Clear Events");
                    event_table.events.Clear();
                    event_table.names.Clear();
                }

                //if UI.Button("Update ALL Args") then
                //    for k, tbl in pairs(event_table.events) do
                //        tbl.settings.bools.wants_args.val = true
                //    end
                //end
                if (ImGui.Button($"Update ALL Args##{callback_name}2"))
                {
                    foreach (var (name, table) in event_table.events)
                    {
                        table.settings_wants_args = true;
                    }
                }

                //local hide_no_args = event_table.settings:get_bool("hide_no_args")
                var hide_no_args = event_table.settings_hide_no_args;

                foreach (var name in event_table.names)
                {
                    var table = event_table.events[name];
                    if (!hide_no_args || (hide_no_args && table.last_args_count > 0))
                    {

                        draw_tree(name, table);
                    }
                }

                ImGui.TreePop();
            }
        }
    }

    private bool RunCallback(EventTable eventTable, dynamic Event)
    {
        //Console.WriteLine(Event.name);
        if (eventTable.events.TryGetValue(Event.name, out dynamic table))
        {
            table.count = table.count + 1;
        }
        else
        {
            eventTable.names.Add(Event.name);
            eventTable.names.Sort();

            eventTable.Insert(Event.name);
            table = eventTable.events[Event.name];
        }

        var get_params = (dynamic args) =>
        {
            var node_args = args as JsonNode ?? JsonSerializer.SerializeToNode(args, op);
            return new {
                serialized_data = node_args,
                raw_data = args,
            };
        };

        bool ok = eventTable.callbacks.TryGetValue(Event.name, out dynamic callback);
        var test = (ok && callback.want_args) || table.settings_wants_args || table.first_time;
        var paramss = test ? get_params(Event) : null;
        var invoke_result = true;

        if (ok)
        {
            if (paramss is null)
                paramss = new { };
            invoke_result = callback.invoke(paramss);
        }

        if (paramss is not null)
        {
            if (table.first_time)
                table.first_time = false;

            table.set_args(paramss);
        }

        return invoke_result && !table.settings_cb_disabled;
    }

    private void draw_tree(string name, dynamic table)
    {
        ImGui.AlignTextToFramePadding();
        var tree_pushed = ImGui.TreeNode(name);
        ImGui.SameLine(0, ImGui.GetStyle().ItemInnerSpacing.X);
        ImGui.Text($": {table.count}");

        if (tree_pushed)
        {
            label_table($"{name}", $"{name}", table.last_args);
            ImGui.TreePop();
        }
    }

    private void label_table(string name, string str_id, dynamic last_args, Dictionary<dynamic, bool>? known = null, dynamic? prefix = null)
    {
        if (last_args is null)
            return;
        if (prefix is null)
            prefix = "";

        if (known is null)
            known = new();

        ImGui.AlignTextToFramePadding();
        if (last_args is JsonObject jsonObj)
        {
            if (!known.TryGetValue(last_args, out bool _))
            {
                known[last_args] = true;
            }

            List<string> sorted_t = new();

            var k = 0;
            foreach (var obj in last_args)
            {
                sorted_t.Add(obj.Key ?? k.ToString());
                k++;
            }

            sorted_t.Sort((str1, str2) =>
            {
                if (last_args[str1] is JsonObject)
                {
                    if (last_args[str2] is JsonArray)
                    {
                        return -1;
                    }
                    else if (last_args[str2] is JsonValue)
                    {
                        return -1;
                    }
                    else if (last_args[str2] is JsonObject)
                    {
                        return str1.CompareTo(str2);
                    }
                    else
                        return 0;
                }
                else if (last_args[str1] is JsonArray)
                {
                    if (last_args[str2] is JsonObject)
                    {
                        return 1;
                    }
                    if (last_args[str2] is JsonValue)
                    {
                        return -1;
                    }
                    else if (last_args[str2] is JsonArray)
                    {
                        return str1.CompareTo(str2);
                    }
                    else
                        return 0;
                }
                else if (last_args[str1] is JsonValue)
                {
                    if (last_args[str2] is JsonObject)
                    {
                        return 1;
                    }
                    else if (last_args[str2] is JsonArray)
                    {
                        return 1;
                    }
                    else if (last_args[str2] is JsonValue)
                    {
                        return str1.CompareTo(str2);
                    }
                    else
                        return 0;
                }
                else
                    return 0;
            });

            foreach (var name1 in sorted_t)
            {
                var arg = last_args[name1];

                ImGui.AlignTextToFramePadding();
                if (arg is JsonObject jsonObject)
                {
                    if (!known.TryGetValue(arg, out bool _))
                    {
                        ImGui.AlignTextToFramePadding();
                        if (ImGui.TreeNode($"{prefix}{name1} [{name}]###jsonObject_{str_id}_{name1}"))
                        {
                            label_table($"{name} [{name1}]", $"{name} [{name1}]", jsonObject, known, $"{prefix}");
                            ImGui.TreePop();
                        }
                    }
                }
                else if (arg is JsonArray jsonArray)
                {
                    if (!known.TryGetValue(arg, out bool _))
                    {
                        ImGui.AlignTextToFramePadding();
                        if (ImGui.TreeNode($"{prefix}{name1} [{jsonArray.Count}] [{name}]###jsonArray_{str_id}_{name1}"))
                        {
                            label_table($"{name} [{name1}]", $"{name} [{name1}]", jsonArray, known, $"{prefix}");
                            ImGui.TreePop();
                        }
                    }
                }
                else
                {
                    ImGui.BeginGroup();
                    {
                        ImGui.Text($"{prefix}{name1}:");
                        ImGui.SameLine();
                        ImGui.TextColored(new Vector4(0.660f, 1.000f, 0.702f, 1.000f), $"{arg ?? "null"}");
                        ImGui.EndGroup();
                        CopyConsolePopup(name1, $"{prefix}{name1}", arg);
                    }
                }
            }
        }
        else if (last_args is JsonArray jsonArray)
        {
            for (int i = 0; i < jsonArray.Count; i++)
            {
                var value = jsonArray[i]!;
                if (!known.TryGetValue(value, out bool _))
                {
                    ImGui.AlignTextToFramePadding();
                    if (ImGui.TreeNode($"{prefix}{i} [{name}]###jsonArray_{str_id}_{i}"))
                    {
                        label_table($"[{i}]", $"{name} [{i}]", value, known, $"{prefix}");
                        ImGui.TreePop();
                    }
                }
            }
        }
        else if (last_args is JsonValue jsonValue)
        {
            ImGui.BeginGroup();
            {
                ImGui.Text($"{prefix}{name}:");
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(0.660f, 1.000f, 0.702f, 1.000f), $"{jsonValue}");
                ImGui.EndGroup();
                CopyConsolePopup(name, $"{prefix}{name}", jsonValue);
            }
        }
    }
}
