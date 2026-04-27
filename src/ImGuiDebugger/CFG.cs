namespace ImGuiDebugger;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

using Divine.Common.Helpers;
using Divine.Common.Log;

using global::ImGuiDebugger.Enum;

internal sealed partial class Debugger
{

    public void SaveCFG()
    {
        try
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var serializedJsonObject = JsonSerializer.Serialize(
                new
                {
                    WidgetData,
                    EnabledCallbacks,
                },
            jsonSerializerOptions);
            File.WriteAllText(Path.Combine(Directories.Config, @"Plugins\ImGuiDebugger.json"), serializedJsonObject);
        }
        catch (Exception e)
        {
            LogManager.Error(e);
        }
    }

    public void LoadCFG()
    {
        try
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var cfgDir = Path.Combine(Directories.Config, @"Plugins\");
            if (!Directory.Exists(cfgDir))
            {
                Directory.CreateDirectory(cfgDir);
            }
            var cfgFile = Path.Combine(cfgDir, @"ImGuiDebugger.json");

            if (File.Exists(cfgFile))
            {
                var cfgJson = File.ReadAllText(cfgFile);

                var jsonNode = JsonSerializer.Deserialize<JsonNode>(
                    cfgJson,
                    jsonSerializerOptions);

                var deserializedWidgetData = JsonSerializer.Deserialize<Dictionary<string, bool>>(jsonNode!["WidgetData"]);
                WidgetData = deserializedWidgetData == null ? new Dictionary<string, bool>() : deserializedWidgetData;
                var deserializedEnabledCallbacks = JsonSerializer.Deserialize<int?>(jsonNode!["EnabledCallbacks"]);
                EnabledCallbacks = deserializedEnabledCallbacks == null ? 0 : (int)deserializedEnabledCallbacks;
            }
            else
            {
                LoadDefaultCFG();
                SaveCFG();
            }
            initDone = true;
        }
        catch (Exception e)
        {
            LogManager.Error(e);
        }
    }

    private void LoadDefaultCFG()
    {
        WidgetData = new()
        {

        };
        EnabledCallbacks = (int)CallbackFlag.None;
    }
}
