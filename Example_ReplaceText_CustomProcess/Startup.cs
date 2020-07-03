using GhostNodes.Std.BaseAdapter.Interfaces;
using GhostNodes.Std.BaseAdapter.Objects;
using GhostNodes.Std.SystemExtensions;
using GhostNodes.Std.Tracing;
using System;
using System.Collections.Generic;
using System.Composition;

namespace GhostNodes.CustomProcessAssembly
{
    [Export(typeof(ICustomProcessAdapter))]
    public class Startup : CustomProcessAdapter
    {
        public Startup()
        {
            Id = Guid.Parse("62A1A668-DD3D-48A7-BF98-C2965CC9D58E");
            Description = "Custom process adapter for replacing text in a message.";
        }

        public override List<AdapterMessage> ProcessMessage(AdapterMessage message)
        {
            try
            {
                Tracer.WriteBegin();

                NodeParams p = NodeParams.Extract(message);
                List<AdapterMessage> responseList = new List<AdapterMessage>();

                String s = message.GetDataAsString();
                if (p.IgnoreCase)
                {
                    if (s.Contains(p.TextToReplace, StringComparison.InvariantCultureIgnoreCase))
                    {
                        s = s.Replace(p.TextToReplace, p.ReplaceWithText, StringComparison.InvariantCultureIgnoreCase);
                        Tracer.WriteLine($"Replaced text '{p.TextToReplace}' with '{p.ReplaceWithText}'.");
                        AdapterMessage response = message.CloneWithNewData(s);
                        return new List<AdapterMessage>() { response };
                    }
                    else
                    {
                        Tracer.WriteLine("No text to replace found...");
                        return new List<AdapterMessage>() { message };
                    }
                }
                else
                {
                    if (s.Contains(p.TextToReplace))
                    {
                        s = s.Replace(p.TextToReplace, p.ReplaceWithText);
                        Tracer.WriteLine($"Replaced text '{p.TextToReplace}' with '{p.ReplaceWithText}'.");
                        AdapterMessage response = message.CloneWithNewData(s);
                        return new List<AdapterMessage>() { response };
                    }
                    else
                    {
                        Tracer.WriteLine("No text to replace found...");
                        return new List<AdapterMessage>() { message };
                    }
                }
            }
            finally
            {
                Tracer.WriteEnd();
            }

        }

    }
}
