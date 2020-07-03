using GhostNodes.Std.BaseAdapter.Attributes;
using GhostNodes.Std.BaseAdapter.Objects;
using System.ComponentModel;
using System.Composition;

namespace GhostNodes.DestinationAssembly
{
    public enum FileOperationValues : int
    {
        Read,
        ReadAndDelete,
        Write,
        None
    }

    [Export(typeof(INodeParametersBase))]
    public class NodeParams : NodeParametersBase<NodeParams>
    {
        public NodeParams()
        {
            this.Operation = FileOperationValues.Write;
            this.Path = string.Empty;
            this.Filename = "%guid%.txt";
            this.AppendToExisting = false;
            this.PreserveCreationTime = false;
            this.EmptyFileResponse = "";
            this.FileMissingResponse = "";
        }

        public static NodeParams Extract(AdapterMessage message)
        {
            return ExtractParams(message.NodeParamList);
        }

        [IsNodeKey(true)]
        [IsRefreshable(true)]
        [DisplayName("Operation"), Description("Operation to perform."), Category("Mandatory"), DefaultValue(FileOperationValues.Write), AcceptDefaultValue(true)]
        [PropertyOrder(10)]
        public FileOperationValues Operation { get; set; }


        [IsNodeKey(true)]
        [IsRefreshable(true)]
        [DisplayName("Path"), Description("Path to where messages will be sent to."), Category("Mandatory"), DefaultValue(""), AcceptDefaultValue(false)]
        [Editor("GhostNodes.Win.AdapterUIEditors.UIEditors.FolderBrowserUITypeEditor", "System.Drawing.Design.UITypeEditor)")]
        [TypeConverter("GhostNodes.Win.AdapterUIEditors.UIEditors.FolderBrowserTypeConverter")]
        [PropertyOrder(11)]
        public string Path { get; set; }


        [IsNodeKey(true)]
        [IsRefreshable(true)]
        [DisplayName("Filename"), Description("Filename format to use for outgoing messages."), Category("Mandatory"), DefaultValue("%guid%.txt"), AcceptDefaultValue(true)]
        [PropertyOrder(12)]
        public string Filename { get; set; }



        [IsRefreshable(true)]
        [DisplayName("Append To Existing Files"), Description("Specifies if the existing file should be overwritten/created or appended to."), Category("Optional"), DefaultValue(false), AcceptDefaultValue(true)]
        [PropertyOrder(13)]
        public bool AppendToExisting { get; set; }

        [IsRefreshable(true)]
        [DisplayName("Preserve creation time"), Description("If possible preserve the creation time in the written file."), Category("Optional"), DefaultValue(false), AcceptDefaultValue(true)]
        [PropertyOrder(15)]
        public bool PreserveCreationTime { get; set; }

        [IsRefreshable(true)]
        [DisplayName("Empty File Response"), Description("If requested file is empty, set this as response content. If left blank empty files will return empty responses."), Category("Optional"), DefaultValue(""), AcceptDefaultValue(true)]
        public string EmptyFileResponse { get; set; }

        [IsRefreshable(true)]
        [DisplayName("File Missing Response"), Description("If requested file is missing, set this as response content. If left blank missing files will throw exception."), Category("Optional"), DefaultValue(""), AcceptDefaultValue(true)]
        public string FileMissingResponse { get; set; }
    }
}
