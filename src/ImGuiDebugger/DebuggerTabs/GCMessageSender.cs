namespace ImGuiDebugger;

using System.Collections.Generic;
using System.Reflection;

using Divine.Protobufs.Dota2;

using Google.Protobuf.Reflection;

internal sealed partial class Debugger
{
    private int item_current_idx = 0;
    private bool is_selected = false;
    private MessageDescriptor selectedGCMessageDescriptor;
    private object? selectedGCMessage;
    private List<MessageDescriptor> sendableReceivableMessageDescriptors = new List<MessageDescriptor>();
    private Dictionary<string, string> propertyValues = new Dictionary<string, string>();

    private void GCMessageSender()
    {
        //Console.WriteLine("    " + messageType.Name + " - " + Activator.CreateInstance(messageType.ClrType));
        //var sendableGCMessages = DotaGcmessagesMsgidReflection.Descriptor.EnumTypes[0].ToProto().Value;
        //ImGui.BeginListBox("Selected GC Message");

        //for (int n = 0; n < sendableReceivableMessageDescriptors.Count; n++)
        //{
        //    is_selected = (item_current_idx == n);
        //    if (ImGui.Selectable(sendableReceivableMessageDescriptors[n].Name, is_selected))
        //    {
        //        item_current_idx = n;
        //        selectedGCMessageDescriptor = sendableReceivableMessageDescriptors[n];
        //        selectedGCMessage = Activator.CreateInstance(selectedGCMessageDescriptor.ClrType);
        //    }

        //    // Set the initial focus when opening the combo (scrolling + keyboard navigation focus)
        //    if (is_selected)
        //        ImGui.SetItemDefaultFocus();
        //}
        //ImGui.EndListBox();

        //if (selectedGCMessage is null)
        //    return;
        ////CMsgClientToGCAddGuildRole
        //var msgType = selectedGCMessage.GetType();
        //var properties = msgType.GetProperties();
        //foreach (var property in properties)
        //{
        //    if (property.Name.Contains("Has") || property.Name.Contains("CSODOTA") || property.Name.Contains("GCStore")
        //        || property.Name.Contains("Parser") || property.Name.Contains("Descriptor"))
        //        continue;

        //    if (!propertyValues.TryGetValue(property.Name, out _))
        //        propertyValues.Add(property.Name, string.Empty);



        //    var str = propertyValues[property.Name];

        //    if (ImGui.InputText($"{property.Name} ({property.PropertyType.FullName})", ref str, uint.MaxValue))
        //    {
        //        propertyValues[property.Name] = str;
        //    }
        //}
        //NetworkManager.SendGCMessage()
    }

    private void GetMessageDesciptors()
    {
        foreach (var type in Assembly.GetAssembly(typeof(BaseGcmessagesReflection))!.GetTypes())
        {
            if (!type.IsSealed || !type.IsAbstract)
            {
                continue;
            }

            if (type.Namespace?.StartsWith("Divine.Protobufs.Dota2") is false || !type.Name.EndsWith("Reflection"))
            {
                continue;
            }

            var descriptorProperty = type.GetProperty("Descriptor");
            if (descriptorProperty is null || descriptorProperty.PropertyType != typeof(FileDescriptor))
            {
                continue;
            }

            var descriptor = (FileDescriptor)descriptorProperty.GetValue(null)!;

            //Console.WriteLine(descriptor.Name);
            //if (!descriptor.Name.Contains("gc"))
            //    continue;

            foreach (var messageType in descriptor.MessageTypes)
            {
                //Console.WriteLine("    " + messageType.Name);
                sendableReceivableMessageDescriptors.Add(messageType);
            }
        }
    }
}
