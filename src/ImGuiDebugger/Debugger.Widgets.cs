namespace ImGuiDebugger;
using System;
using System.Text.Json.Nodes;

using ImGuiNET;

internal sealed partial class Debugger
{
    private void CopyConsolePopup(string name, string str_id, JsonNode? node)
    {
        if (ImGui.BeginPopupContextItem($"Popup_{str_id}", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.AnyPopupLevel))
        {
            ImGui.Text($"Copy:");
            ImGui.SameLine();
            if (ImGui.Button($"Name##Copy_{str_id}"))
            {
                ImGui.SetClipboardText($"{name}");
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine();
            if (ImGui.Button($"Value##Copy_{str_id}"))
            {
                ImGui.SetClipboardText($"{node ?? "null"}");
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine();
            if (ImGui.Button($"Full##Copy_{str_id}"))
            {
                ImGui.SetClipboardText($"{name}: {node ?? "null"}");
                ImGui.CloseCurrentPopup();
            }

            ImGui.Text($"Console:");
            ImGui.SameLine();
            if (ImGui.Button($"Name##Console_{str_id}"))
            {
                Console.WriteLine($"{name}");
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine();
            if (ImGui.Button($"Value##Console_{str_id}"))
            {
                Console.WriteLine($"{node ?? "null"}");
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine();
            if (ImGui.Button($"Full##Console_{str_id}"))
            {
                Console.WriteLine($"{name}: {node ?? "null"}");
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
    }

    private void HelpMarker(string desc)
    {
        ImGui.TextDisabled("(?)");
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.DelayShort))
        {
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(ImGui.GetFontSize() * 35.0f);
            ImGui.TextUnformatted(desc);
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
        }
    }
}
