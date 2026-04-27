namespace ImGuiDebugger;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json.Nodes;

internal sealed class EventTable
{
    internal Dictionary<string, dynamic> events;
    internal List<string> names;

    internal bool settings_hide_no_args = false;

    internal dynamic callbacks_funcs;
    internal Dictionary<string, dynamic> callbacks;

    public EventTable()
    {
        events = new();
        names = new();
        callbacks = new ();
        callbacks_funcs = new ExpandoObject();
        callbacks_funcs.New = new Action<dynamic, bool>((dynamic obj, bool wants_args = true) =>
        {
            if (obj.callbacks == null)
                return;

            foreach (var callback in obj.callbacks)
            {
                if (!callbacks.TryGetValue(callback.Key, out dynamic _))
                {
                    dynamic data = new ExpandoObject();

                    data.list = new List<dynamic>();
                    data.want_args = wants_args;
                    data.invoke = new Func<dynamic, bool>((dynamic args) =>
                    {
                        var any_false = false;

                        foreach (var cb in data.list)
                        {
                            var result = cb.invoke(args);

                            if (!result)
                                any_false = true;
                        }
                        return !any_false;
                    });
                    callbacks[callback.Key] = data;
                }

                dynamic data1 = new ExpandoObject();
                data1.object_ = obj;
                data1.callback_ = callback.Value;
                data1.invoke = new Func<dynamic, dynamic>((dynamic args) =>
                {
                    return data1.callback_(args);
                });

                callbacks[callback.Key].list.Add(data1);
            }
        });

        //new_function_only = function(self, name, func)
        //    self:new({
        //        callbacks = {
        //            [name] = func
        //        }
        //    })
        //end,

        //new_function_w_table = function(self, name, tbl, func)
        //    tbl.callbacks = {
        //        [name] = func
        //    }

        //    self:new(tbl)
        //end
    }

    public void Insert(string name)
    {
        events[name] = NewEvent(name);
    }

    private int CountNode(JsonNode node)
    {
        if (node is JsonObject jsonObj)
        {
            var count = 0;

            foreach (var item in jsonObj)
            {
                count++;
            }

            return count;
        }
        return 0;
    }

    private dynamic NewEvent(string name)
    {
        dynamic data = new ExpandoObject();

        data.count = 1;
        data.first_time = true;
        data.last_args = new { };
        data.last_args_count = 0;
        data.settings_wants_args = false;
        data.settings_cb_disabled = false;
        data.set_args = new Action<dynamic>((dynamic args) =>
        {
            data.last_args = args.serialized_data;
            data.last_args_count = CountNode(args.serialized_data);
        });

        return data;
    }
}