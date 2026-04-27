namespace ImGuiDebugger;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

using Divine.Entity;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Divine.Entity.EventArgs;

using ImGuiNET;

internal sealed partial class Debugger
{
    private const int MAX_EVENT_COUNT = 100;
    private Queue<JsonNode> UnitAddedQueue = new Queue<JsonNode>();
    private Queue<JsonNode> UnitRemovedQueue = new Queue<JsonNode>();
    private Queue<JsonNode> AbilityAddedQueue = new Queue<JsonNode>();
    private Queue<JsonNode> AbilityRemovedQueue = new Queue<JsonNode>();

    private void OnAddRemoveActivate()
    {
        EntityManager.EntityAdded += EntityManager_EntityAddedOnAddRemove;
        EntityManager.EntityRemoved += EntityManager_EntityRemovedOnAddRemove;
    }

    private void OnAddRemoveDeactivate()
    {
        EntityManager.EntityAdded -= EntityManager_EntityAddedOnAddRemove;
        EntityManager.EntityRemoved -= EntityManager_EntityRemovedOnAddRemove;
    }

    private void EntityManager_EntityAddedOnAddRemove(EntityAddedEventArgs e)
    {
        if (e.IsCollection)
            return;

        var entity = e.Entity;

        if (!entity.IsValid)
            return;

        if (entity is Unit unit)
        {
            while (UnitAddedQueue.Count >= MAX_EVENT_COUNT)
            {
                UnitAddedQueue.TryDequeue(out _);
            }

            dynamic Event = new ExpandoObject();
            Event.Handle = unit.Owner?.Handle ?? 0;
            Event.Unit = unit;

            UnitAddedQueue.Enqueue(JsonSerializer.SerializeToNode(Event, op));
        }
        else if (entity is Ability ability)
        {
            while (AbilityAddedQueue.Count >= MAX_EVENT_COUNT)
            {
                AbilityAddedQueue.TryDequeue(out _);
            }

            dynamic Event = new ExpandoObject();
            Event.Owner = ability.Owner?.Name ?? "None";
            Event.Handle = ability.Owner?.Handle ?? 0;
            Event.Ability = ability;

            AbilityAddedQueue.Enqueue(JsonSerializer.SerializeToNode(Event, op));
        }
    }

    private void EntityManager_EntityRemovedOnAddRemove(EntityRemovedEventArgs e)
    {
        var entity = e.Entity;

        if (!entity.IsValid)
            return;

        if (entity is Unit unit)
        {
            while (UnitRemovedQueue.Count >= MAX_EVENT_COUNT)
            {
                UnitRemovedQueue.Dequeue();
            }

            dynamic Event = new ExpandoObject();
            Event.Handle = unit.Owner?.Handle ?? 0;
            Event.Unit = unit;

            UnitRemovedQueue.Enqueue(JsonSerializer.SerializeToNode(Event, op));
        }
        else if (entity is Ability ability)
        {
            while (AbilityRemovedQueue.Count >= MAX_EVENT_COUNT)
            {
                AbilityRemovedQueue.Dequeue();
            }

            dynamic Event = new ExpandoObject();
            Event.Owner = ability.Owner?.Name ?? "None";
            Event.Handle = ability.Owner?.Handle ?? 0;
            Event.Ability = ability;

            AbilityRemovedQueue.Enqueue(JsonSerializer.SerializeToNode(Event, op));
        }
    }

    private void OnAddRemove()
    {
        ImGui.AlignTextToFramePadding();
        if (ImGui.TreeNode($"Unit Added {UnitAddedQueue.Count} of {MAX_EVENT_COUNT}###Unit Added"))
        {
            foreach (var unit in UnitAddedQueue.Reverse())
            {
                ImguiDDDD($"{unit["Unit"]!["Name"]} [{unit["Handle"]}]", unit, (int)unit["Handle"]!);
            }
            ImGui.TreePop();
        }

        ImGui.AlignTextToFramePadding();
        if (ImGui.TreeNode($"Unit Removed {UnitRemovedQueue.Count} of {MAX_EVENT_COUNT}###Unit Removed"))
        {
            foreach (var unit in UnitRemovedQueue.Reverse())
            {
                ImguiDDDD($"{unit["Unit"]!["Name"]} [{unit["Handle"]}]", unit, (int)unit["Handle"]!);
            }
            ImGui.TreePop();
        }

        ImGui.AlignTextToFramePadding();
        if (ImGui.TreeNode($"Ability Added {AbilityAddedQueue.Count} of {MAX_EVENT_COUNT}###Ability Added"))
        {
            foreach (var ability in AbilityAddedQueue.Reverse())
            {
                ImguiDDDD($"{ability["Ability"]!["Name"]} [{ability["Owner"]}]", ability, (int)ability["Ability"]!["Handle"]!);
            }
            ImGui.TreePop();
        }

        ImGui.AlignTextToFramePadding();
        if (ImGui.TreeNode($"Ability Removed {AbilityRemovedQueue.Count} of {MAX_EVENT_COUNT}###Ability Removed"))
        {
            foreach (var ability in AbilityRemovedQueue.Reverse())
            {
                ImguiDDDD($"{ability["Ability"]!["Name"]} [{ability["Owner"]}]", ability, (int)ability["Ability"]!["Handle"]!);
            }
            ImGui.TreePop();
        }
    }
}
