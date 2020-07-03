using GhostNodes.Std.BaseAdapter.Attributes;
using GhostNodes.Std.BaseAdapter.Objects;
using System.ComponentModel;
using System.Composition;

namespace GhostNodes.CustomProcessAssembly
{
    [Export(typeof(INodeParametersBase))]
    public class NodeParams : NodeParametersBase<NodeParams>
    {
        public NodeParams()
        {
            TextToReplace = "";
            ReplaceWithText = "";
            IgnoreCase = true;
        }

        public static NodeParams Extract(AdapterMessage message)
        {
            return ExtractParams(message.NodeParamList);
        }

        [DisplayName("Text To Replace"), Description("The string to be replaced."), Category("Mandatory"), DefaultValue(""), AcceptDefaultValue(false)]
        [PropertyOrder(10)]
        public string TextToReplace { get; set; }

        [DisplayName("Replace With Text"), Description("The string to replace all occurrences of \"old string\"."), Category("Mandatory"), DefaultValue(""), AcceptDefaultValue(true)]
        [PropertyOrder(11)]
        public string ReplaceWithText { get; set; }

        [DisplayName("Ignore Case"), Description("Compare strings using culture-sensitive sort rules, the invariant culture, and ignoring the case of the strings being compared."), Category("Optional"), DefaultValue(true), AcceptDefaultValue(true)]
        [PropertyOrder(12)]
        public bool IgnoreCase { get; set; }
    }
}
