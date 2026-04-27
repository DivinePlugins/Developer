namespace ImGuiDebugger;

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

using Divine.Numerics;

using ImGuiDebugger.Enum;

using ImGuiNET;

internal sealed partial class Debugger
{
    private Dictionary<CallbackFlag, int> EventCount = new();
    private Dictionary<CallbackFlag, Queue<(int, JsonNode)>> CallbackData = new()
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
        { CallbackFlag.NetMessageSend, new () },
        { CallbackFlag.NetMessageReceived, new () },
        { CallbackFlag.NetworkPropertyChanged, new () },
    };

    private void CheckCountAndEnqueue(CallbackFlag callbackFlag, object e, JsonSerializerOptions op)
    {
        //return;
        if (!initDone)
            return;

        if (((CallbackFlag)EnabledCallbacks & callbackFlag) == 0)
            return;

        if (!EventCount.TryGetValue(callbackFlag, out _))
        {
            EventCount.TryAdd(callbackFlag, 0);
        }
        EventCount[callbackFlag]++;

        if (CallbackData[callbackFlag].Count >= 30)
        {
            CallbackData[callbackFlag].TryDequeue(out _);
        }
        CallbackData[callbackFlag].Enqueue((EventCount[callbackFlag], JsonSerializer.SerializeToNode(e, op)!));
    }

    private void CallbackInspector()
    {
        ImGui.Spacing();

        if (ImGui.BeginTable("###Table_OverallCallbacksEnabled", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX))
        {
            ImGui.TableNextRow();
            ImGui.TableSetColumnIndex(0);
            ImGui.Spacing();

            if (ImGui.CheckboxFlags("Overall Callbacks Enabled###Checkbox_OverallCallbacksEnabled", ref EnabledCallbacks, (int)CallbackFlag.AllCallbacksEnabled))
                SaveCFG();

            ImGui.Spacing();

            ImGui.EndTable();
        }

        ImGui.Spacing();

        foreach (var data in CallbackData)
        {
            var val = EventCount.TryGetValue(data.Key, out var value);

            if (ImGui.CheckboxFlags($"###RadioButton_{data.Key}", ref EnabledCallbacks, (int)data.Key))
                SaveCFG();

            ImGui.SameLine(0, ImGui.GetStyle().ItemInnerSpacing.X);

            if (ImGui.TreeNode($"{data.Key} [{(val ? value : 0)}]###{data.Key}"))
            {
                if (ImGui.TreeNode($"Last callbacks list [{data.Value.Count}]###{data.Key} Last callbacks list"))
                {
                    foreach (var (index, item) in data.Value)
                    {
                        switch (data.Key)
                        {
                            case CallbackFlag.ParticleCreate or CallbackFlag.ParticleAdded or CallbackFlag.ParticleDestroy or CallbackFlag.ParticleRemoved:
                                var nameString = item["Particle"]!["Name"]?.ToString();
                                nameString = nameString?.ToString().Remove(0, nameString.LastIndexOf("/") + 1);
                                ImguiDDDD($"[{nameString}] {index}###{data.Key}{index}", item, 0);
                                break;
                            case CallbackFlag.LinearProjectileCreate or CallbackFlag.LinearProjectileAdded or CallbackFlag.LinearProjectileDestroy or CallbackFlag.LinearProjectileRemoved:
                                nameString = item["Projectile"]!["Particle"]?["Name"]?.ToString();
                                nameString = nameString?.ToString().Remove(0, nameString.LastIndexOf("/") + 1);
                                ImguiDDDD($"[{nameString}] {index}###{data.Key}{index}", item, 0);
                                break;
                            case CallbackFlag.TrackingProjectileCreate or CallbackFlag.TrackingProjectileAdded or CallbackFlag.TrackingProjectileDestroy or CallbackFlag.TrackingProjectileRemoved:
                                nameString = item["Projectile"]!["Particle"]?["Name"]?.ToString();
                                nameString = nameString?.ToString().Remove(0, nameString.LastIndexOf("/") + 1);
                                ImguiDDDD($"[{nameString}] {index}###{data.Key}{index}", item, 0);
                                break;
                            case CallbackFlag.ModifierAdded or CallbackFlag.ModifierRemoved:
                                ImguiDDDD($"[{item["Modifier"]!["Name"]}] {index}###{data.Key}{index}", item, 0);
                                break;
                            case CallbackFlag.AnimationChanged:
                                ImguiDDDD($"[{item["Entity"]!["Name"]} - {item["Args"]!["Name"]}] {index}###{data.Key}{index}", item, 0);
                                break;
                            case CallbackFlag.EntityCreate or CallbackFlag.EntityAdded or CallbackFlag.EntityDestroy or CallbackFlag.EntityRemoved:
                                ImguiDDDD($"[{item["Entity"]!["Name"]}] {index}###{data.Key}{index}", item, 0);
                                break;
                            case CallbackFlag.OrderAdding:
                                ImguiDDDD($"[{item["Order"]?["Type"]}] {index}###{data.Key}{index}", item, 0);
                                break;
                            case CallbackFlag.OrderOverwatchAdding:
                                var val1 = item["OrderOverwatch"];
                                var val2 = val1?["MouseScreenPosition"];
                                var val3 = val1?["CameraPosition"];
                                ImguiDDDD($"[X: {val2!["X"]} Y: {val2!["Y"]} - X: {val3!["X"]} Y: {val3!["Y"]}] {index}###{data.Key}{index}", item, 0);
                                break;
                            case CallbackFlag.GameEvent:
                                ImguiDDDD($"[{item["GameEvent"]?["Name"]}] {index}###{data.Key}{index}", item, 0);
                                break;
                            case CallbackFlag.GCMessageSend or CallbackFlag.GCMessageReceived:
                                ImguiDDDD($"[{item["Protobuf"]?["Name"]}] {index}###{data.Key}{index}", item, 0);
                                break;
                            case CallbackFlag.NetMessageSend or CallbackFlag.NetMessageReceived:
                                ImguiDDDD($"[{item["Protobuf"]?["Name"]}] {index}###{data.Key}{index}", item, 0);
                                break;
                            case CallbackFlag.NetworkPropertyChanged:
                                ImguiDDDD($"[{item["Entity"]!["Name"]} - {item["Args"]!["PropertyName"]}] {index}###{data.Key}{index}", item, 0);
                                break;
                            default:
                                break;
                        }

                    }

                    ImGui.TreePop();
                }

                if (ImGui.TreeNode($"List [{data.Value.Count}]###{data.Key} Name"))

                    //ImGui.SetNextItemOpen(true);
                    ImGui.Spacing();
                ImGui.Separator();
                ImGui.Spacing();
                ImguiDDDD(string.Empty, data.Value.LastOrDefault().Item2, 0, true);
                ImGui.Separator();
                //if (ImGui.TreeNode($"##{data.Key} Last"))
                //{
                //    data.Value.LastOrDefault();
                //}

                ImGui.TreePop();
            }
        }
    }

    private void ImguiDDDD(string? name, JsonNode? node, int index, bool notree = false)
    {
        ImGui.AlignTextToFramePadding();
        if (node is JsonObject jsonObject)
        {
            if (notree || ImGui.TreeNode($"{name}##{index}"))
            {
                foreach (var pair in jsonObject)
                {
                    ImguiDDDD(pair.Key, pair.Value, index++);
                }
                if (!notree)
                    ImGui.TreePop();
            }
        }
        else if (node is JsonArray jsonArray)
        {
            if (ImGui.TreeNode($"{name} [{jsonArray.Count}]##{index}"))
            {
                for (int i = 0; i < jsonArray.Count; i++)
                {
                    ImguiDDDD($"{i}", jsonArray[i], index++);
                }
                ImGui.TreePop();
            }
        }
        else
        {
            ImGui.BeginGroup();
            {
                ImGui.Text($"{name}:");
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(0.660f, 1.000f, 0.702f, 1.000f), $"{node ?? "null"}");
                ImGui.EndGroup();
                CopyConsolePopup(name!, name!, node);
            }
        }
    }
}
