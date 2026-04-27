namespace ImGuiDebugger;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Entity.Entities.Units.Creeps.Neutrals;
using Divine.Entity.Entities.Units.Heroes;

using ImGuiNET;

internal sealed partial class Debugger
{
    private bool autoUpdate;
    private Dictionary<uint, JsonNode> unitInfos = new();
    private Dictionary<uint, JsonNode> unitSpells = new();
    private Dictionary<uint, JsonNode> unitItems = new();
    private Dictionary<uint, JsonNode> unitModifiers = new();

    private void Information()
    {
        ImGui.Spacing();
        if (ImGui.Button("Get Unit Info") || autoUpdate)
        {
            ImGui.Spacing();
            var queryUnit = EntityManager.LocalPlayer?.QueryUnit;
            if (queryUnit == null)
            {
                var Units = EntityManager.LocalPlayer?.SelectedUnits;
                if (Units == null)
                    return;

                foreach (var unit in Units)
                {
                    var node = JsonSerializer.SerializeToNode(unit, op);
                    unitInfos[unit.Handle] = node!;
                }
            }
            else
            {
                var node = JsonSerializer.SerializeToNode(queryUnit, op);
                unitInfos[queryUnit.Handle] = node!;
            }
        }
        ImGui.SameLine();
        ImGui.Checkbox("Auto Update Unit Info", ref autoUpdate);

        ImGui.Spacing();
        if (ImGui.Button("Get Unit Spells"))
        {
            var queryUnit = EntityManager.LocalPlayer?.QueryUnit;
            if (queryUnit == null)
            {
                var Units = EntityManager.LocalPlayer?.SelectedUnits;
                if (Units == null)
                    return;

                foreach (var unit in Units)
                {
                    if (unit is not Hero && unit is not Courier && unit is not Building && unit is not Creep)
                        continue;

                    var node2 = JsonSerializer.SerializeToNode(
                        new
                        {
                            unit.Name,
                            unit.Handle,
                            Spellbook = new
                            {
                                MainSpells = unit.Spellbook!.MainSpells.ToDictionary((e) => e.Name),
                                ConsumableItems = unit.Spellbook!.ConsumableItems.ToDictionary((e) => e.Name),
                                Talents = unit.Spellbook!.Talents.ToDictionary((e) => e.Name),
                            },
                        }, op1);

                    unitSpells[unit.Handle] = node2!;
                }
            }
            else
            {
                if (queryUnit is not Unit unit || unit is not Tower && unit is not Fort && unit is not Outpost && unit is not Creep && unit is not Neutral)
                    return;

                var node2 = JsonSerializer.SerializeToNode(
                    new
                    {
                        unit.Name,
                        unit.Handle,
                        Spellbook = new
                        {
                            MainSpells = unit.Spellbook?.MainSpells.ToDictionary((e) => e.Name),
                            ConsumableItems = unit.Spellbook?.ConsumableItems.ToDictionary((e) => e.Name),
                            Talents = unit.Spellbook?.Talents.ToDictionary((e) => e.Name),
                        },
                    }, op1);
                unitSpells[unit.Handle] = node2!;
            }
        }

        ImGui.SameLine();
        if (ImGui.Button("Get Unit Items"))
        {
            var queryUnit = EntityManager.LocalPlayer?.QueryUnit;
            if (queryUnit == null)
            {
                var Units = EntityManager.LocalPlayer?.SelectedUnits;
                if (Units == null)
                    return;

                foreach (var unit in Units)
                {
                    if (unit is not Hero && unit is not Courier && unit is not Building && unit is not Creep)
                        continue;

                    var node2 = JsonSerializer.SerializeToNode(
                        new
                        {
                            unit.Name,
                            unit.Handle,
                            Inventory = new
                            {
                                MainItems = unit.Inventory!.MainItems.ToDictionary((e) => e.Name),
                                BackpackItems = unit.Inventory!.BackpackItems.ToDictionary((e) => e.Name),
                                StashItems = unit.Inventory!.StashItems.ToDictionary((e) => e.Name),
                                NeutralItem = unit.Inventory!.NeutralItem,
                            },
                        }, op1);

                    unitItems[unit.Handle] = node2!;
                }
            }
            else
            {
                if (queryUnit is not Unit unit || unit is not Tower && unit is not Fort && unit is not Outpost && unit is not Creep && unit is not Neutral)
                    return;

                var node2 = JsonSerializer.SerializeToNode(
                    new
                    {
                        unit.Name,
                        unit.Handle,
                        Inventory = new
                        {
                            MainItems = unit.Inventory?.MainItems.ToDictionary((e) => e.Name),
                            BackpackItems = unit.Inventory?.BackpackItems.ToDictionary((e) => e.Name),
                            StashItems = unit.Inventory?.StashItems.ToDictionary((e) => e.Name),
                            NeutralItem = unit.Inventory!.NeutralItem,
                        },
                    }, op1);
                unitItems[unit.Handle] = node2!;
            }
        }

        ImGui.SameLine();
        if (ImGui.Button("Get Unit Modifiers"))
        {
            var queryUnit = EntityManager.LocalPlayer?.QueryUnit;
            if (queryUnit == null)
            {
                var Units = EntityManager.LocalPlayer?.SelectedUnits;
                if (Units == null)
                    return;

                foreach (var unit in Units)
                {
                    if (unit is not Hero && unit is not Courier && unit is not Building && unit is not Creep)
                        continue;

                    var node2 = JsonSerializer.SerializeToNode(
                        new
                        {
                            unit.Name,
                            unit.Handle,
                            Modifiers = unit.Modifiers.ToDictionary((e) => e.Name),
                        }, op1);

                    unitModifiers[unit.Handle] = node2!;
                }
            }
            else
            {
                if (queryUnit is not Unit unit || unit is not Tower && unit is not Fort && unit is not Outpost && unit is not Creep && unit is not Neutral)
                    return;

                var node2 = JsonSerializer.SerializeToNode(
                    new
                    {
                        unit.Name,
                        unit.Handle,
                        Modifiers = unit.Modifiers.ToDictionary((e) => e.Name),
                    }, op1);
                unitModifiers[unit.Handle] = node2!;
            }
        }

        ImGui.Spacing();
        if (ImGui.Button("Clear"))
        {
            unitInfos.Clear();
            unitSpells.Clear();
            unitItems.Clear();
            unitModifiers.Clear();
        }

        ImGui.Spacing();
        if (unitInfos.Count != 0)
        {
            foreach (var keyValuePair in unitInfos)
            {
                ImguiDDDD($"Info {keyValuePair.Value["Name"]} [{keyValuePair.Value["Handle"]}]", keyValuePair.Value, (int)keyValuePair.Value["Handle"]!);
            }
        }

        if (unitSpells.Count != 0)
        {
            foreach (KeyValuePair<uint, JsonNode> keyValuePair in unitSpells)
            {
                ImguiDDDD($"Spells {keyValuePair.Value["Name"]} [{keyValuePair.Value["Handle"]}]", keyValuePair.Value, (int)keyValuePair.Value["Handle"]!);
            }
        }

        if (unitItems.Count != 0)
        {
            foreach (KeyValuePair<uint, JsonNode> keyValuePair in unitItems)
            {
                ImguiDDDD($"Items {keyValuePair.Value["Name"]} [{keyValuePair.Value["Handle"]}]", keyValuePair.Value, (int)keyValuePair.Value["Handle"]!);
            }
        }

        if (unitModifiers.Count != 0)
        {
            foreach (KeyValuePair<uint, JsonNode> keyValuePair in unitModifiers)
            {
                ImguiDDDD($"Modifiers {keyValuePair.Value["Name"]} [{keyValuePair.Value["Handle"]}]", keyValuePair.Value, (int)keyValuePair.Value["Handle"]!);
            }
        }
    }
}
