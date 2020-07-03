using GhostNodes.Std.BaseAdapter.Attributes;
using GhostNodes.Std.BaseAdapter.Objects;
using GhostNodes.Std.BaseObjectModel.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition;

namespace GhostNodes.SourceAssembly
{
    [Export(typeof(INodeParametersBase))]
    public class NodeParams : NodeParametersBase<NodeParams>
    {
        public NodeParams()
        {
            // Mandatory
            Path = string.Empty;
            Filter = "*";

            // Optional
            SortOptions = FileSortOptions.None;
            MinFileAge = 0;
            IncludeSubFolders = false;
            IncludeEmptyFiles = false;

            // Response
            IsTwoWay = false;
            ReplyPath = string.Empty;
            ReplyFilename = string.Empty;
        }

        public static NodeParams Extract(List<Param> cfg)
        {
            return ExtractParams(cfg);
        }
        [IsNodeKey(true)]
        [IsRefreshable(true)]
        [DisplayName("Path")]
        [Description("Path to a local folder or a network share where files will be read from.")]
        [Category("Mandatory"), DefaultValue(""), AcceptDefaultValue(false)]
        [EditorAttribute("GhostNodes.Win.AdapterUIEditors.UIEditors.FolderBrowserUITypeEditor", "System.Drawing.Design.UITypeEditor)")]
        [TypeConverter("GhostNodes.Win.AdapterUIEditors.UIEditors.FolderBrowserTypeConverter")]
        [SampleValues(@"C:\Local\Folder", @"X:\Mapped\Folder", @"\\Server\Shared\Folder")]
        [PropertyOrder(10)]
        public string Path { get; set; }

        [IsNodeKey(true)]
        [IsRefreshable(true)]
        [DisplayName("Filter"), Description("Collect only files that match specified Filter. Multiple filter are separated by any of , ; | characters."), Category("Mandatory"), DefaultValue("*"), AcceptDefaultValue(true)]
        [SampleValues(@"*.txt", @"*.xml|*.json", @"prefix*.txt,*postfix.txt")]
        [PropertyOrder(11)]
        public string Filter { get; set; }

        [IsRefreshable(true)]
        [DisplayName("SortOptions"), Description("The order the files in the Path folder will be picked up."), Category("Mandatory"), DefaultValue("None"), AcceptDefaultValue(true)]
        [PropertyOrder(20)]
        public FileSortOptions SortOptions { get; set; }

        [IsRefreshable(true)]
        [DisplayName("Minimum File Age"), Description("Time in seconds on how old a file need to be before it can be picked up."), Category("Optional"), DefaultValue(0), AcceptDefaultValue(true)]
        [PropertyOrder(23)]
        public int MinFileAge { get; set; }

        [IsRefreshable(true)]
        [DisplayName("Include sub-folders"), Description("Should sub-folders be included."), Category("Optional"), DefaultValue(false), AcceptDefaultValue(true)]
        [PropertyOrder(24)]
        public bool IncludeSubFolders{ get; set; }

        [IsRefreshable(true)]
        [DisplayName("Include Empty Files"), Description("Set to True if empty files should be picked up and processed."), Category("Optional"), DefaultValue(false), AcceptDefaultValue(true)]
        [PropertyOrder(25)]
        public bool IncludeEmptyFiles { get; set; }

        [IsRefreshable(true)]
        [DisplayName("Is Two Way"), Description("True if a response message should be returned."), Category("Response"), DefaultValue(false), AcceptDefaultValue(true)]
        [PropertyOrder(30)]
        [OtherPropertyEnabled("ReplyPath,ReplyFilename")]
        public bool IsTwoWay { get; set; }

        [IsRefreshable(true)]
        [DisplayName("Reply Path"), Description("Path where response messages will be sent to."), Category("Response"), DefaultValue(""), AcceptDefaultValue(true)]
        [Editor("GhostNodes.Win.AdapterUIEditors.UIEditors.FolderBrowserUITypeEditor", "System.Drawing.Design.UITypeEditor)")]
        [TypeConverter("GhostNodes.Win.AdapterUIEditors.UIEditors.FolderBrowserTypeConverter")]
        [PropertyOrder(31)]
        public string ReplyPath { get; set; }

        [IsRefreshable(true)]
        [DisplayName("Reply Filename"), Description("Filename format for response messages (Ex: %guid%.txt)."), Category("Response"), DefaultValue(""), AcceptDefaultValue(true)]
        [PropertyOrder(32)]
        public string ReplyFilename { get; set; }
    }
}
